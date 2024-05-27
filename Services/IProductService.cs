using Examensarbete.Models;

namespace Examensarbete.Services
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product);
        Task<IEnumerable<Product>> CreateProductsAsync(IEnumerable<Product> products);
    }
}
