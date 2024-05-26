using static Examensarbete.Pages.Uploads.UploadProductsModel;

namespace Examensarbete.Services
{
    public interface IOrderDataService
    {
        Task<ImportResult> ImportOrderDataAsync(IFormFile fileUpload);
    }
}
