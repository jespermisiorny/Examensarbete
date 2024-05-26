using ClosedXML.Excel;
using Examensarbete.Data;
using Examensarbete.DTO;
using Examensarbete.Models;
using Examensarbete.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using ProductModel = Examensarbete.Models.Product;

namespace Examensarbete.Pages.Uploads
{
    public class UploadOrderDataModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IOrderDataService _orderDataService;

        public UploadOrderDataModel(ApplicationDbContext context, IWebHostEnvironment environment, IOrderDataService orderDataService)
        {
            _context = context;
            _environment = environment;
            _orderDataService = orderDataService;
        }

        [BindProperty]
        public List<OrderData> UploadedData { get; set; } = new List<OrderData>();
        public bool Success { get; set; }
        public bool ShowUploadForm { get; set; } = true;

        public void OnGet()
        {
            Success = false;
        }

        public async Task<IActionResult> OnPostUploadOrderDataAsync(IFormFile fileUpload)
        {
            //try
            //{
            //    var result = await _orderDataService.ImportOrderDataAsync(fileUpload);
            //    // Hantera success
            //    SetMessage(true, $"{result.RecordsAdded} lades till.");
            //}
            //catch (Exception ex)
            //{
            //    // Hantera error
            //    SetMessage(false, $"Error: {ex.Message}");
            //}
            //return Page();


            if (fileUpload == null || fileUpload.Length == 0)
            {
                SetMessage(false, "Ingen fil valdes för uppladdning.");
                ShowUploadForm = false;
                return Page();
            }

            var validFileExtensions = new List<string> { ".xls", ".xlsx", ".xlsm", ".xlt", ".xltx", ".xltm" };
            if (!validFileExtensions.Any(ext => fileUpload.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                SetMessage(false, "Fel filformat.<br/>Endast Excel-filer är tillåtna.");
                ShowUploadForm = false;
                return Page();
            }

            try
            {
                var filePath = await SaveUploadedFile(fileUpload);
                var extractionResult = await ExtractOrderDataFromExcelAndSave(filePath);
                System.IO.File.Delete(filePath);
                ProcessExtractionResult(extractionResult);
                var simpleUploadedData = CreateSimpleOrderDataDTOs(extractionResult.uploadedData);
                var jsonData = JsonSerializer.Serialize(simpleUploadedData);
                HttpContext.Session.SetString("UploadedData", jsonData);
                Success = true;
            }

            catch (Exception ex)
            {
                SetMessage(false, $"Ett fel uppstod vid uppladdningen av filen:<br/>{ex.Message}");
                Success = false;
                ShowUploadForm = false;

                // Ta bort filer ifall uppladdningen misslyckas.
                var uploadedFilePath = Path.Combine(_environment.WebRootPath, "uploads", fileUpload.FileName);
                if (System.IO.File.Exists(uploadedFilePath))
                {
                    System.IO.File.Delete(uploadedFilePath);
                }
                var folderPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (Directory.Exists(folderPath) && !Directory.EnumerateFileSystemEntries(folderPath).Any())
                {
                    Directory.Delete(folderPath);
                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostCreateIncompleteProductAsync(int id)
        {
            var jsonData = HttpContext.Session.GetString("UploadedData");
            if (string.IsNullOrEmpty(jsonData))
            {
                return new JsonResult(new { success = false, message = "Sessionen har g ått ut eller data finns inte." });
            }

            var simpleUploadedData = JsonSerializer.Deserialize<List<SimpleOrderDataDTO>>(jsonData);
            var orderData = simpleUploadedData.FirstOrDefault(od => od.Id == id);

            if (orderData != null)
            {
                var newProduct = new ProductModel
                {
                    Name = orderData.ItemDescription,
                    ArticleNumber = orderData.ArticleNumber,
                    IsIncomplete = true
                };

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                await MatchAndSaveOrdersToProducts();

                return new JsonResult(new { success = true, message = $"Produkt med artikelnummer {orderData.ArticleNumber} har skapats.", id });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Kunde inte skapa produkt, ingen matchande orderdata hittades." });
            }

        }
        public async Task<IActionResult> OnPostCreateAllIncompleteProductsAsync()
        {
            var jsonData = HttpContext.Session.GetString("UploadedData");
            if (string.IsNullOrEmpty(jsonData))
            {
                return new JsonResult(new { success = false, message = "Sessionen har gått ut eller data finns inte." });
            }

            var simpleUploadedData = JsonSerializer.Deserialize<List<SimpleOrderDataDTO>>(jsonData);
            var unmatchedOrderData = simpleUploadedData.Where(od => !_context.Products.Any(p => p.ArticleNumber == od.ArticleNumber)).ToList();

            if (!unmatchedOrderData.Any())
            {
                return new JsonResult(new { success = false, message = "Inga omatchade orderdata hittades." });
            }

            foreach (var orderData in unmatchedOrderData)
            {
                var newProduct = new ProductModel
                {
                    Name = orderData.ItemDescription,
                    ArticleNumber = orderData.ArticleNumber,
                    IsIncomplete = true
                };
                _context.Products.Add(newProduct);
            }
            await _context.SaveChangesAsync();
            await MatchAndSaveOrdersToProducts();
            return new JsonResult(new { success = true, message = $"{unmatchedOrderData.Count} omatchade produkter har skapats." });
        }
        private async Task<(int duplicatesRemoved, int newRecordsAdded, List<OrderData> uploadedData)> ExtractOrderDataFromExcelAndSave(string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);
            var orderDataList = worksheet.RowsUsed().Skip(1).Select(row => CreateOrderDataFromRow(row)).ToList();
            var existingData = await _context.OrderData.ToListAsync();
            var newData = orderDataList.Where(od => !existingData.Any(ed => IsDuplicate(ed, od))).ToList();

            foreach (var order in newData)
            {
                MatchAndAssignProductToOrderData(order);
            }

            await _context.OrderData.AddRangeAsync(newData);
            await _context.SaveChangesAsync();

            return (orderDataList.Count - newData.Count, newData.Count, newData);

        }
        private void ProcessExtractionResult((int duplicatesRemoved, int newRecordsAdded, List<OrderData> uploadedData) result)
        {
            var (duplicatesRemoved, newRecordsAdded, uploadedData) = result;

            var dupesMessage = duplicatesRemoved == 0 ? "Inga dubletter hittades." : $"{duplicatesRemoved} dubletter togs bort.";

            if (newRecordsAdded == 0)
            {
                SetMessage(false, $"Inga nya rader att lägga till!<br/>{dupesMessage}");
                ShowUploadForm = false;
            }
            else
            {
                SetMessage(true, $"{newRecordsAdded} nya rader lades till.<br/>{dupesMessage}");
                UploadedData = uploadedData;
                Success = true;
            }

        }
        private async Task<string> SaveUploadedFile(IFormFile fileUpload)
        {
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileUpload.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileUpload.CopyToAsync(stream);
            }
            return filePath;
        }

        private OrderData CreateOrderDataFromRow(IXLRow row)
        {
            var dateString = row.Cell(1).GetValue<string>();
            if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime orderDate))
                return null;

            return new OrderData
            {
                OrderDate = orderDate,
                OrderGroup = row.Cell(2).GetValue<string>(),
                SubArea = row.Cell(3).GetValue<string>(),
                ItemDescription = row.Cell(4).GetValue<string>(),
                ArticleNumber = row.Cell(5).GetValue<string>(),
                SupplierName = row.Cell(6).GetValue<string>(),
                UnitType = row.Cell(7).GetValue<string>(),
                ConfirmedQuantity = row.Cell(8).GetValue<int>(),
                Price = row.Cell(9).GetValue<decimal>(),
                ConfirmedNetAmount = row.Cell(10).GetValue<decimal>(),
                Account = row.Cell(11).GetValue<string>(),
                CostCenter = row.Cell(12).GetValue<string>(),
                Organization = row.Cell(13).GetValue<string>(),
            };
        }
        private void MatchAndAssignProductToOrderData(OrderData orderData)
        {
            var matchedProduct = _context.Products
                .FirstOrDefault(p => p.ArticleNumber == orderData.ArticleNumber);
            if (matchedProduct != null)
            {
                orderData.ProductId = matchedProduct.Id;
            }
        }
        private async Task MatchAndSaveOrdersToProducts()
        {
            var unmatchedOrders = _context.OrderData.Where(od => od.ProductId == null);
            var products = await _context.Products.ToDictionaryAsync(p => p.ArticleNumber, p => p.Id);

            foreach (var order in unmatchedOrders)
            {
                if (products.TryGetValue(order.ArticleNumber, out var productId))
                {
                    order.ProductId = productId;
                }
            }

            await _context.SaveChangesAsync();
        }
        private List<SimpleOrderDataDTO> CreateSimpleOrderDataDTOs(List<OrderData> orderData)
        {
            return orderData.Select(od => new SimpleOrderDataDTO
            {
                Id = od.Id,
                ItemDescription = od.ItemDescription,
                ArticleNumber = od.ArticleNumber
            }).ToList();
        }
        private bool IsDuplicate(OrderData existingData, OrderData newData)
        {
            return
                existingData.OrderDate == newData.OrderDate &&
                existingData.ArticleNumber == newData.ArticleNumber &&
                existingData.ConfirmedQuantity == newData.ConfirmedQuantity;
        }
        private void SetMessage(bool isSuccess, string message)
        {
            var key = isSuccess ? "SuccessMessage" : "ErrorMessage";
            TempData[key] = message;
        }
    }
}
