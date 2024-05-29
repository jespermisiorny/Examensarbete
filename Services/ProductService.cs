using Examensarbete.Data;
using Examensarbete.Models;
using Examensarbete.Services.Interfaces;
using Examensarbete.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Examensarbete.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;

        public ProductService(ApplicationDbContext context, IMaterialService materialService, ICategoryService categoryService)
        {
            _context = context;
            _materialService = materialService;
            _categoryService = categoryService;
        }


        // Produkt
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }
        public async Task<IEnumerable<Product>> CreateProductsAsync(IEnumerable<Product> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            return products;
        }
        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }


        // Material
        public async Task<List<SelectListItem>> GetMaterialOptionsAsync()
        {
            return await _materialService.GetMaterialOptionsAsync();
        }
        public async Task<IList<MaterialViewModel>> GetProductMaterialsAsync(int productId)
        {
            return await _materialService.GetProductMaterialsAsync(productId);
        }
        public async Task UpdateMaterialsAsync(Product product, List<ProductMaterial> updatedMaterials, List<ProductMaterial> newMaterials, List<int> removedMaterialIds)
        {
            await _materialService.UpdateMaterialsAsync(product, updatedMaterials, newMaterials, removedMaterialIds);
        }


        // Kategorier
        public async Task<List<Category>> GetProductCategoriesAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product?.ProductCategories.Select(pc => pc.Category).ToList();
        }
        public async Task<List<SelectListItem>> GetAvailableCategoriesAsync(int productId)
        {
            var productCategories = await GetProductCategoriesAsync(productId);
            var allCategories = await _categoryService.GetMainCategoriesAsync();

            var productCategoryIds = productCategories.Select(pc => pc.Id).ToList();

            return allCategories
                .Where(c => !productCategoryIds.Contains(int.Parse(c.Value)))
                .Select(c => new SelectListItem { Value = c.Value, Text = c.Text })
                .ToList();
        }
        public async Task<List<SelectListItem>> GetMainCategoriesAsync()
        {
            return await _categoryService.GetMainCategoriesAsync();
        }
        public async Task<List<Category>> GetSubCategoriesAsync(int parentId)
        {
            return await _categoryService.GetSubCategoriesAsync(parentId);
        }
        public async Task UpdateCategoriesAsync(int productId, List<int> addedCategoryIds, List<int> removedCategoryIds)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new Exception("Produkten hittades inte.");
            }

            // Ta bort relationer till kategorier som finns i removedCategoryIds
            if (removedCategoryIds != null)
            {
                foreach (var categoryId in removedCategoryIds)
                {
                    var categoryToRemove = product.ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
                    if (categoryToRemove != null)
                    {
                        product.ProductCategories.Remove(categoryToRemove);
                    }
                }
            }

            // Lägg till nya relationer till kategorier från addedCategoryIds
            if (addedCategoryIds != null)
            {
                foreach (var categoryId in addedCategoryIds)
                {
                    if (!product.ProductCategories.Any(pc => pc.CategoryId == categoryId))
                    {
                        product.ProductCategories.Add(new ProductCategory { ProductId = productId, CategoryId = categoryId });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }


        // Misc
        public async Task ToggleProductInactiveStatusAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsInactive = !product.IsInactive;
                await _context.SaveChangesAsync();
            }
        }


    }
}



