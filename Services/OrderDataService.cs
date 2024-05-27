using ClosedXML.Excel;
using Examensarbete.Data;
using Examensarbete.Models;
using System.Globalization;
using Examensarbete.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Examensarbete.Services
{
    public class OrderDataService : IOrderDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IProductService _productService;

        public OrderDataService(ApplicationDbContext context, IWebHostEnvironment environment, IProductService productService)
        {
            _context = context;
            _environment = environment;
            _productService = productService;
        }

        public async Task<ImportResultDTO> ProcessUploadedFileAsync(IFormFile fileUpload)
        {
            if (fileUpload == null || fileUpload.Length == 0)
                return new ImportResultDTO { ErrorMessage = "Ingen fil valdes för uppladdning." };

            if (!IsValidFile(fileUpload.FileName))
                return new ImportResultDTO { ErrorMessage = "Fel filformat. Ladda upp en fil med något av följande format: .xls, .xlsx, .xlsm, .xlt, .xltx, eller .xltm." };

            var filePath = string.Empty;
            try
            {
                filePath = await SaveUploadedFile(fileUpload);
                var orders = await Task.Run(() => ReadOrdersFromFile(filePath));
                var nonDuplicateOrders = RemoveDuplicates(orders);
                await MatchOrdersToProducts(nonDuplicateOrders);

                _context.OrderData.AddRange(nonDuplicateOrders);
                var savedCount = await _context.SaveChangesAsync();

                return new ImportResultDTO { RecordsAdded = savedCount };
            }
            catch (Exception ex)
            {
                return new ImportResultDTO { ErrorMessage = $"Ett fel uppstod vid bearbetning av filen: {ex.Message}" };
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        _ = new ImportResultDTO { ErrorMessage = $"Kunde inte hantera den uppladdade filen på servern: {ex.Message}" };
                    }
                }
            }
        }

        private bool IsValidFile(string fileName)
        {
            var validFileExtensions = new List<string> { ".xls", ".xlsx", ".xlsm", ".xlt", ".xltx", ".xltm" };
            return validFileExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<List<OrderData>> ReadOrdersFromFile(string filePath)
        {
            var orders = new List<OrderData>();
            using (var stream = File.OpenRead(filePath))
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var order = CreateOrderDataFromRow(row);
                    if (order != null)
                        orders.Add(order);
                }
            }
            return orders;
        }

        private List<OrderData> RemoveDuplicates(List<OrderData> orders)
        {
            return orders
                .GroupBy(o => new { o.OrderDate, o.ArticleNumber, o.ConfirmedQuantity })
                .Select(g => g.First())
                .ToList();
        }

        private async Task MatchOrdersToProducts(List<OrderData> orders)
        {
            var productDictionary = await _context.Products.ToDictionaryAsync(p => p.ArticleNumber, p => p);
            foreach (var order in orders)
            {
                if (productDictionary.TryGetValue(order.ArticleNumber, out var product))
                    order.ProductId = product.Id;
            }
        }

        public async Task<IEnumerable<OrderData>> RetrieveUnmatchedOrders()
        {
            var unmatchedOrders = await _context.OrderData
                .Where(od => od.ProductId == null)
                .ToListAsync();

            return unmatchedOrders;
        }

        public async Task<CreateIncompleteProductsResultDTO> CreateAllIncompleteProducts(string jsonData)
        {
            try
            {
                var simpleUploadedData = JsonSerializer.Deserialize<List<SimpleOrderDataDTO>>(jsonData);
                if (simpleUploadedData == null)
                {
                    return new CreateIncompleteProductsResultDTO { ErrorMessage = "Kunde inte tolka data från JSON-formatet." };
                }

                var unmatchedOrderData = simpleUploadedData
                    .Where(od => !_context.Products.Any(p => p.ArticleNumber == od.ArticleNumber))
                    .Select(od => new Product
                    {
                        Name = od.ItemDescription,
                        ArticleNumber = od.ArticleNumber,
                        IsIncomplete = true
                    })
                    .ToList();

                if (!unmatchedOrderData.Any())
                {
                    return new CreateIncompleteProductsResultDTO { ErrorMessage = "Inga omatchade orderdata hittades för skapande av nya produkter." };
                }

                await _productService.CreateProductsAsync(unmatchedOrderData);

                return new CreateIncompleteProductsResultDTO { ProductsCreated = unmatchedOrderData.Count };
            }
            catch (Exception ex)
            {
                return new CreateIncompleteProductsResultDTO { ErrorMessage = $"Ett fel uppstod vid skapandet av ofullständiga produkter: {ex.Message}" };
            }
        }

        public async Task<ImportResultDTO> ImportOrderDataAsync(IFormFile fileUpload)
        {
            if (fileUpload == null || fileUpload.Length == 0)
                throw new ArgumentException("Ingen fil valdes för uppladdningen, eller så är den tom.");

            var filePath = await SaveUploadedFile(fileUpload);
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet(1);
                var orders = new List<OrderData>();

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    var order = CreateOrderDataFromRow(row);
                    if (order != null)
                        orders.Add(order);
                }

                if (!orders.Any())
                    return new ImportResultDTO { RecordsAdded = 0, ErrorMessage = "Inga giltiga rader hittades i filen." };

                _context.OrderData.AddRange(orders);
                int saved = await _context.SaveChangesAsync();
                return new ImportResultDTO { RecordsAdded = saved };
            }
            catch (Exception ex)
            {
                return new ImportResultDTO { ErrorMessage = ex.Message };
            }
            finally
            {
                File.Delete(filePath);
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

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }
                return filePath;
            }

            catch (Exception ex)
            {
                throw new ApplicationException("Ett fel uppstod vid sparandet av den uppladdade filen.", ex);
            }
        }


        private OrderData? CreateOrderDataFromRow(IXLRow row)
        {
            try
            {
                DateTime orderDate;
                var dateString = row.Cell(1).GetValue<string>();
                if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDate))
                {
                    Console.WriteLine($"Datumtolkning misslyckades för rad: {row.RowNumber()} med datumsträngen: '{dateString}'");
                    return null;
                }

                var orderData = new OrderData
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

                if (orderData.ArticleNumber == null || orderData.ArticleNumber.Trim().Length == 0)
                {
                    Console.WriteLine($"Artikelnummer saknas för rad: {row.RowNumber()}");
                    return null;
                }

                return orderData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett oväntat fel uppstod vid behandlingen av rad: {row.RowNumber()}: {ex.Message}");
                return null;
            }
        }

    }
}
