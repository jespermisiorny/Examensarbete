using Examensarbete.Data;
using Examensarbete.Models;
using Examensarbete.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Examensarbete.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }
        public async Task<IEnumerable<Category>> CreateCategoriesAsync(IEnumerable<Category> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            return categories;
        }
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SelectListItem>> GetMainCategoriesAsync()
        {
            var mainCategories = await _context.Categories
                .Where(c => c.ParentId == null)
                .ToListAsync();

            return mainCategories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
        public async Task<List<Category>> GetSubCategoriesAsync(int parentId)
        {
            var subCategories = await _context.Categories.Where(c => c.ParentId == parentId).ToListAsync();

            if (!subCategories.Any())
            {
                Console.WriteLine($"Inga underkategorier hittades för huvudkategori med ID {parentId}");
            }

            return subCategories;
        }
    }
}