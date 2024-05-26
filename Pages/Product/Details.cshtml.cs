using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using Examensarbete.Models;
using ProductModel = Examensarbete.Models.Product;
using CategoryModel = Examensarbete.Models.Category;
using Examensarbete.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Examensarbete.Pages.Product
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductModel Product { get; set; }

        [BindProperty]
        public IList<MaterialViewModel> ProductMaterials { get; set; }

        public List<SelectListItem> MaterialOptions { get; set; }
        public IList<CategoryModel> ProductCategories { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductMaterials)
                .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(m => m.Id == id);

            var materials = await _context.Materials.ToListAsync();
            MaterialOptions = materials.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Type
            }).ToList();

            if (Product == null)
            {
                return NotFound();
            }

            ProductCategories = Product.ProductCategories.Select(pc => pc.Category).ToList();
            ProductMaterials = await _context.ProductMaterials
                .Where(pm => pm.ProductId == id)
                .Select(pm => new MaterialViewModel
                {
                    MaterialId = pm.MaterialId,
                    MaterialName = pm.Material.Type,
                    Percentage = pm.Percentage
                }).ToListAsync();

            //ProductMaterials = Product.ProductMaterials.Select(pm => pm.Material).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostToggleInactiveAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                // Växla
                product.IsInactive = !product.IsInactive;

                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = $"Produkten har blivit {(product.IsInactive ? "inaktiverad" : "aktiverad")}";

                return RedirectToPage("./Details", new { id });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProductAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var productToUpdate = await _context.Products.FirstOrDefaultAsync(p => p.Id == Product.Id);
            if (productToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(
                productToUpdate,
                "Product",
                p => p.Name,
                p => p.ArticleNumber,
                p => p.WeightPerUnit,
                p => p.PricePerUnit
                ))
            {
                await _context.SaveChangesAsync();
                _context.Entry(productToUpdate).Reload();
                return RedirectToPage(new { id = productToUpdate.Id });
            }

            return Page();

        }

        public async Task<IActionResult> OnPostUpdateMaterialsAsync(List<ProductMaterial> UpdatedMaterials, List<ProductMaterial> NewMaterials, string RemovedMaterialIds)
        {
            // Omvanlda removedMaterialIds till en lista av ints
            var removedIds = RemovedMaterialIds?.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            var productToUpdate = await _context.Products
                .Include(p => p.ProductMaterials)
                .FirstOrDefaultAsync(p => p.Id == Product.Id);

            if (productToUpdate == null)
            {
                return NotFound("Produkten hittades inte.");
            }

            // Lägg till nya material
            if (NewMaterials != null)
            {
                foreach (var newMaterial in NewMaterials)
                {
                    productToUpdate.ProductMaterials.Add(new ProductMaterial
                    {
                        ProductId = Product.Id,
                        MaterialId = newMaterial.MaterialId,
                        Percentage = newMaterial.Percentage
                    });

                }
            }

            // Uppdatera befintliga material
            if (UpdatedMaterials != null)
            {
                foreach (var updatedMaterial in UpdatedMaterials)
                {
                    var existingMaterial = productToUpdate.ProductMaterials.FirstOrDefault(pm => pm.MaterialId == updatedMaterial.MaterialId);
                    if (existingMaterial != null)
                    {
                        existingMaterial.Percentage = updatedMaterial.Percentage;
                        // Om materialet är ändrat, uppdatera även MaterialId
                        existingMaterial.MaterialId = updatedMaterial.MaterialId;
                    }
                }
            }

            // Ta bort material
            if (removedIds != null)
            {
                foreach (var materialId in removedIds)
                {
                    var materialToRemove = productToUpdate.ProductMaterials
                        .FirstOrDefault(pm => pm.MaterialId == materialId);

                    if (materialToRemove != null)
                    {
                        _context.ProductMaterials.Remove(materialToRemove);
                    }
                }
            }

            // Uppdatera förpackningsmaterial
            TryUpdateModelAsync(productToUpdate, "Product", p => p.PackagingMaterialId);


            // Spara ändringarna i databasen
            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = productToUpdate.Id });
        }

    }
}
