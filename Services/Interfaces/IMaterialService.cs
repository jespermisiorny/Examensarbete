using Examensarbete.Models;
using Examensarbete.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Examensarbete.Services.Interfaces
{
    public interface IMaterialService
    {
        Task<Material> GetMaterialByIdAsync(int id);
        Task<Material> CreateMaterialAsync(Material material);
        Task<IEnumerable<Material>> CreateMaterialsAsync(IEnumerable<Material> materials);
        Task DeleteMaterialAsync(int materialId);
        Task UpdateMaterialAsync(Material material);
        Task<List<SelectListItem>> GetMaterialOptionsAsync();
        Task<IList<MaterialViewModel>> GetProductMaterialsAsync(int productId);
        Task UpdateMaterialsAsync(Product product, List<ProductMaterial> updatedMaterials, List<ProductMaterial> newMaterials, List<int> removedMaterialIds);
        Task<List<Material>> GetAllMaterialsAsync();
    }
}
