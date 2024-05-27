using Examensarbete.DTO;
using Examensarbete.Models;

namespace Examensarbete.Services
{
    public interface IOrderDataService
    {
        Task<ImportResultDTO> ImportOrderDataAsync(IFormFile fileUpload);
        Task<IEnumerable<OrderData>> RetrieveUnmatchedOrders();
        Task<CreateIncompleteProductsResultDTO> CreateAllIncompleteProducts(string jsonData);
        Task<ImportResultDTO> ProcessUploadedFileAsync(IFormFile fileUpload);

    }
}
