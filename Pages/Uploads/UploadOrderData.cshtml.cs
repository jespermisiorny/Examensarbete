using Examensarbete.DTO;
using Examensarbete.Models;
using Examensarbete.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

                // Spara uppladdade data i sessionen
                var uploadedData = await _orderDataService.GetRecentlyUploadedOrdersAsync(result.RecordsAdded);
                UploadedData = uploadedData; // Lägg till detta för att sätta den nyligen uppladdade datan i modellen
                HttpContext.Session.SetString("UploadedData", JsonSerializer.Serialize(uploadedData));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateIncompleteProductAsync(int id)
        {
            var result = await _orderDataService.CreateIncompleteProductAsync(id);
            if (result.Success)
            {
                // Update the session
                var uploadedData = await _orderDataService.GetRecentlyUploadedOrdersAsync(UploadedData.Count);
                HttpContext.Session.SetString("UploadedData", JsonSerializer.Serialize(uploadedData));

                return new JsonResult(new { success = true, message = "Produkt har skapats.", id = id });
            }
            else
            {
                return new JsonResult(new { success = false, message = result.ErrorMessage });
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
