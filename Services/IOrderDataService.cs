using Examensarbete.DTO;
using Examensarbete.Models;

namespace Examensarbete.Services
{
    public interface IOrderDataService
    {
        Task<ImportResultDTO> ImportOrderDataAsync(IFormFile fileUpload);
        Task<IEnumerable<OrderData>> RetrieveUnmatchedOrders();
        Task<CreateAllProductsDTO> CreateAllIncompleteProducts(string jsonData);
        Task<CreateOneProductDTO> CreateIncompleteProductAsync(int orderId);
        Task<ImportResultDTO> ProcessUploadedFileAsync(IFormFile fileUpload);
        Task<List<OrderData>> GetRecentlyUploadedOrdersAsync(int numberOfRecords);

    }
}
