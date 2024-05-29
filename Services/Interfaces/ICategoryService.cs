using Examensarbete.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Examensarbete.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<IEnumerable<Category>> CreateCategoriesAsync(IEnumerable<Category> categories);
        Task DeleteCategoryAsync(int categoryId);
        Task UpdateCategoryAsync(Category category);
        Task<List<SelectListItem>> GetMainCategoriesAsync();
        Task<List<Category>> GetSubCategoriesAsync(int parentId);
    }
}
