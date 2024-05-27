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
        private readonly IOrderDataService _orderDataService;
        private readonly IProductService _productService;

        public UploadOrderDataModel(IOrderDataService orderDataService, IProductService productService)
        {
            _orderDataService = orderDataService;
            _productService = productService;
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
            var result = await _orderDataService.ProcessUploadedFileAsync(fileUpload);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                SetMessage(false, result.ErrorMessage);
                ShowUploadForm = false;
            }
            else
            {
                SetMessage(true, $"{result.RecordsAdded} rader importerades.");
                Success = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateIncompleteProductAsync(int id)
        {
            var jsonData = HttpContext.Session.GetString("UploadedData");
            if (string.IsNullOrEmpty(jsonData))
            {
                return new JsonResult(new { success = false, message = "Sessionen har gått ut eller data finns inte." });
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

                await _productService.CreateProductAsync(newProduct);

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

            var result = await _orderDataService.CreateAllIncompleteProducts(jsonData);
            if (!result.Success)
            {
                return new JsonResult(new { success = false, message = result.ErrorMessage });
            }

            return new JsonResult(new { success = true, message = $"{result.ProductsCreated} omatchade produkter har skapats." });
        }



        private void SetMessage(bool isSuccess, string message)
        {
            var key = isSuccess ? "SuccessMessage" : "ErrorMessage";
            TempData[key] = message;
        }
    }
}
