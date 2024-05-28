using Examensarbete.Models;
using Examensarbete.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Examensarbete.Services.Interfaces
{
    public interface IProductService
    {

        // Produktrelaterat
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<IEnumerable<Product>> CreateProductsAsync(IEnumerable<Product> products);
        Task DeleteProductAsync(int productId);
        Task UpdateProductAsync(Product product);

        // Material
        Task<List<SelectListItem>> GetMaterialOptionsAsync();
        Task<IList<MaterialViewModel>> GetProductMaterialsAsync(int productId);
        Task UpdateMaterialsAsync(Product product, List<ProductMaterial> updatedMaterials, List<ProductMaterial> newMaterials, List<int> removedMaterialIds);

        // Kategorier
        Task<List<Category>> GetProductCategoriesAsync(int productId);
        Task<List<SelectListItem>> GetAvailableCategoriesAsync(int productId);
        Task UpdateCategoriesAsync(int productId, List<int> addedCategoryIds, List<int> removedCategoryIds);
        Task<List<SelectListItem>> GetMainCategoriesAsync();
        Task<List<Category>> GetSubCategoriesAsync(int parentId);

        // Misc
        Task ToggleProductInactiveStatusAsync(int productId);

    }
}
