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

        public ProductService(ApplicationDbContext context, IMaterialService materialService)
        {
            _context = context;
            _materialService = materialService;
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
            var allCategories = await _context.Categories.ToListAsync();

            return allCategories
                .Where(c => !productCategories.Any(pc => pc.Id == c.Id))
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
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

            // Ta bort kategorier som finns i removedCategoryIds
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

            // Lägg till nya kategorier från addedCategoryIds
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
            var subCategories = await _context.Categories
                .Where(c => c.ParentId == parentId)
                .ToListAsync();

            // Loggning
            if (!subCategories.Any())
            {
                Console.WriteLine($"Inga underkategorier hittades för huvudkategori med ID {parentId}");
            }
            return subCategories;
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



//public async Task<List<SelectListItem>> GetMaterialOptionsAsync()
//{
//    var materials = await _context.Materials.ToListAsync();
//    return materials.Select(m => new SelectListItem
//    {
//        Value = m.Id.ToString(),
//        Text = m.Type
//    }).ToList();
//}
//public async Task<IList<MaterialViewModel>> GetProductMaterialsAsync(int productId)
//{
//    return await _context.ProductMaterials
//        .Where(pm => pm.ProductId == productId)
//        .Select(pm => new MaterialViewModel
//        {
//            MaterialId = pm.MaterialId,
//            MaterialName = pm.Material.Type,
//            Percentage = pm.Percentage
//        }).ToListAsync();
//}
//public async Task UpdateMaterialsAsync(Product product, List<ProductMaterial> updatedMaterials, List<ProductMaterial> newMaterials, List<int> removedMaterialIds)
//{
//    // Hämta produkten från databasen
//    var productToUpdate = await _context.Products
//        .Include(p => p.ProductMaterials)
//        .FirstOrDefaultAsync(p => p.Id == product.Id);

//    if (productToUpdate == null)
//    {
//        throw new Exception("Produkten hittades inte.");
//    }

//    // Lägg till nya material
//    if (newMaterials != null)
//    {
//        foreach (var newMaterial in newMaterials)
//        {
//            productToUpdate.ProductMaterials.Add(new ProductMaterial
//            {
//                ProductId = productToUpdate.Id,
//                MaterialId = newMaterial.MaterialId,
//                Percentage = newMaterial.Percentage
//            });
//        }
//    }

//    // Uppdatera befintliga material
//    if (updatedMaterials != null)
//    {
//        foreach (var updatedMaterial in updatedMaterials)
//        {
//            var existingMaterial = productToUpdate.ProductMaterials.FirstOrDefault(pm => pm.MaterialId == updatedMaterial.MaterialId);
//            if (existingMaterial != null)
//            {
//                existingMaterial.Percentage = updatedMaterial.Percentage;
//                existingMaterial.MaterialId = updatedMaterial.MaterialId;
//            }
//        }
//    }

//    // Ta bort material
//    if (removedMaterialIds != null)
//    {
//        foreach (var materialId in removedMaterialIds)
//        {
//            var materialToRemove = productToUpdate.ProductMaterials.FirstOrDefault(pm => pm.MaterialId == materialId);
//            if (materialToRemove != null)
//            {
//                _context.ProductMaterials.Remove(materialToRemove);
//            }
//        }
//    }

//    // Uppdatera förpackningsmaterial
//    productToUpdate.PackagingMaterialId = product.PackagingMaterialId;

//    await _context.SaveChangesAsync();
//}