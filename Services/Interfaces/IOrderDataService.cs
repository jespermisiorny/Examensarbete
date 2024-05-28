using Examensarbete.DTO;
using Examensarbete.Models;

namespace Examensarbete.Services.Interfaces
{
    public interface IOrderDataService
    {
        Task<ImportResultDTO> ImportOrderDataAsync(IFormFile fileUpload);
        Task<IEnumerable<OrderData>> RetrieveUnmatchedOrders();
        Task<CreateProductDTO> CreateAllIncompleteProducts(string jsonData);
        Task<CreateProductDTO> CreateIncompleteProductAsync(int orderId);
        Task<ImportResultDTO> ProcessUploadedFileAsync(IFormFile fileUpload);
        Task<List<OrderData>> GetRecentlyUploadedOrdersAsync(int numberOfRecords);

    }
}
