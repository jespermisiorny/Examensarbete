using ProductModel = Examensarbete.Models.Product;
using CategoryModel = Examensarbete.Models.Category;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Examensarbete.Models;
using Examensarbete.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Examensarbete.Services.Interfaces;

namespace Examensarbete.Pages.Product
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService;

        public DetailsModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public ProductModel Product { get; set; }

        [BindProperty]
        public IList<MaterialViewModel> ProductMaterials { get; set; }

        [BindProperty]
        public IList<CategoryModel> ProductCategories { get; set; }

        public List<SelectListItem> MaterialOptions { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public List<SelectListItem> MainCategories { get; set; } = new List<SelectListItem>();


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _productService.GetProductByIdAsync(id.Value);

            if (Product == null)
            {
                return NotFound();
            }

            await LoadDataAsync(Product.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostToggleInactiveAsync(int id)
        {
            await _productService.ToggleProductInactiveStatusAsync(id);
            TempData["StatusMessage"] = "Produkten har blivit inaktiverad.";
            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostUpdateProductAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _productService.UpdateProductAsync(Product);
            TempData["StatusMessage"] = "Produkten har uppdaterats.";
            return RedirectToPage(new { id = Product.Id });
        }

        public async Task<IActionResult> OnPostUpdateCategoriesAsync(string addedCategoryIds)
        {
            if (string.IsNullOrEmpty(addedCategoryIds))
            {
                return Page();
            }

            var addedIds = addedCategoryIds.Split(',')
                                           .Where(x => !string.IsNullOrEmpty(x))
                                           .Select(int.Parse)
                                           .ToList();

            // Fetch current categories from database instead of using the binded property
            var currentCategories = await _productService.GetProductCategoriesAsync(Product.Id);
            var currentCategoryIds = currentCategories.Select(c => c.Id).ToList();
            var removedIds = currentCategoryIds.Except(addedIds).ToList();

            await _productService.UpdateCategoriesAsync(Product.Id, addedIds, removedIds);

            await LoadDataAsync(Product.Id);

            return RedirectToPage(new { id = Product.Id });
        }

        public async Task<IActionResult> OnPostUpdateMaterialsAsync(List<ProductMaterial> UpdatedMaterials, List<ProductMaterial> NewMaterials, string RemovedMaterialIds)
        {
            var removedIds = RemovedMaterialIds?.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            await _productService.UpdateMaterialsAsync(Product, UpdatedMaterials, NewMaterials, removedIds);
            return RedirectToPage(new { id = Product.Id });
        }

        private async Task LoadDataAsync(int productId)
        {
            ProductMaterials = await _productService.GetProductMaterialsAsync(productId);
            ProductCategories = await _productService.GetProductCategoriesAsync(productId);
            MaterialOptions = await _productService.GetMaterialOptionsAsync();
            MainCategories = await _productService.GetMainCategoriesAsync();
        }
    }
}