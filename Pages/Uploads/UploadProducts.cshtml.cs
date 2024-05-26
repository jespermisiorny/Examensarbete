using ClosedXML.Excel;
using Examensarbete.Data;
using Examensarbete.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductModel = Examensarbete.Models.Product;
using MaterialModel = Examensarbete.Models.Material;
using CategoryModel = Examensarbete.Models.Category;
using Microsoft.EntityFrameworkCore;

namespace Examensarbete.Pages.Uploads
{
    public class UploadProductsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UploadProductsModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostUploadProductsAsync(IFormFile fileUpload)
        {
            if (!IsFileValid(fileUpload))
            {
                SetMessage(false, "Felaktig fil eller format. Endast Excel-filer är tillåtna.");
                return Page();
            }

            var filePath = await SaveUploadedFile(fileUpload);
            try
            {
                var importResult = await ImportProductsFromExcel(filePath);
                System.IO.File.Delete(filePath); // Rensa upp uppladdade filer efter bearbetning
                return ProcessImportResult(importResult);
            }
            catch (Exception ex)
            {
                return HandleImportError(filePath, ex);
            }
        }

        private async Task<ImportResult> ImportProductsFromExcel(string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);
            var categoryCache = await _context.Categories.ToDictionaryAsync(c => c.Name, c => c);
            var materialCache = await _context.Materials.ToDictionaryAsync(m => m.Type, m => m);
            var products = new List<ProductModel>();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var product = CreateProductFromRow(row, categoryCache, materialCache);
                if (product != null)
                    products.Add(product);
            }

            _context.Products.AddRange(products);
            int saved = await _context.SaveChangesAsync();
            return new ImportResult { RecordsAdded = saved };
        }

        private ProductModel CreateProductFromRow(IXLRow row, Dictionary<string, CategoryModel> categoryCache, Dictionary<string, MaterialModel> materialCache)
        {
            var product = new ProductModel
            {
                Name = row.Cell("D").GetValue<string>(),
                ArticleNumber = row.Cell("G").GetValue<string>(),
                PricePerUnit = row.Cell("H").GetValue<decimal>(),
                WeightPerUnit = row.Cell("I").GetValue<double>(),
                RecyclingRateAtEoL = row.Cell("J").GetValue<double>(),
                ProductMaterials = new List<ProductMaterial>(),
                ProductCategories = new List<ProductCategory>()
            };

            // Hantera material och kategorier här, exempelvis:
            // Lägg till kod för att hantera material och kategorier baserat på din Excel-struktur och affärslogik

            return product;
        }

        private void SetMessage(bool isSuccess, string message)
        {
            var key = isSuccess ? "SuccessMessage" : "ErrorMessage";
            TempData[key] = message;
        }

        private IActionResult ProcessImportResult(ImportResult result)
        {
            if (result.RecordsAdded > 0)
                SetMessage(true, $"{result.RecordsAdded} nya produkter importerades.");
            else
                SetMessage(false, "Inga nya produkter lades till.");
            return Page();
        }

        private IActionResult HandleImportError(string filePath, Exception ex)
        {
            System.IO.File.Delete(filePath); // Rensa fil vid fel
            SetMessage(false, $"Ett fel uppstod vid importen: {ex.Message}");
            return Page();
        }

        public class ImportResult
        {
            public int RecordsAdded { get; set; }
        }

        private bool IsFileValid(IFormFile file)
        {
            var validFileExtensions = new List<string> { ".xls", ".xlsx", ".xlsm" };
            return file != null && file.Length > 0 && validFileExtensions.Any(ext => file.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
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
    }
}
