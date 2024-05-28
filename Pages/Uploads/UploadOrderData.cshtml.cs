using Examensarbete.Models;
using Examensarbete.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Examensarbete.Pages.Uploads
{
    public class UploadOrderDataModel : PageModel
    {
        private readonly IOrderDataService _orderDataService;

        public UploadOrderDataModel(IOrderDataService orderDataService)
        {
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
            var result = await _orderDataService.ProcessUploadedFileAsync(fileUpload);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                SetMessage(false, result.ErrorMessage);
                ShowUploadForm = false;
            }
            else
            {
                SetMessage(true, $"{result.RecordsAdded} rader importerades");
                Success = true;

                var uploadedData = await _orderDataService.GetRecentlyUploadedOrdersAsync(result.RecordsAdded);
                UploadedData = uploadedData;
                HttpContext.Session.SetString("UploadedData", JsonSerializer.Serialize(uploadedData));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateIncompleteProductAsync(int id)
        {
            var result = await _orderDataService.CreateIncompleteProductAsync(id);
            if (result.Success)
            {                
                var uploadedData = await _orderDataService.GetRecentlyUploadedOrdersAsync(UploadedData.Count);
                HttpContext.Session.SetString("UploadedData", JsonSerializer.Serialize(uploadedData));

                return new JsonResult(new { success = true, message = "Produkt skapad och matchad med orderdata.", id = id });
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

            return new JsonResult(new { success = true, message = $"Produktskapande lyckades!</br>{result.ProductsCreated} nya produkter har matchats." });
        }


    private void SetMessage(bool isSuccess, string message)
        {
            var key = isSuccess ? "SuccessMessage" : "ErrorMessage";
            TempData[key] = message;
        }
    }
}
