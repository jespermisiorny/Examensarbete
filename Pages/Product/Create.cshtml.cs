using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Examensarbete.Data;
using Examensarbete.Models;
using ProductModel = Examensarbete.Models.Product;
using CategoryModel = Examensarbete.Models.Category;
using Examensarbete.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Examensarbete.Pages.Product
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductViewModel ProductViewModel { get; set; }
        public List<CategoryModel> RootCategories { get; set; }
        public List<CategoryModel> FirstLevelChildCategories { get; set; }
        public List<CategoryModel> LowerLevelChildCategories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProductViewModel = new ProductViewModel();

            // Alla kategorier
            var categories = await _context.Categories.ToListAsync();

            // Kategorier med ParentId == null
            var rootCategories = categories.Where(c => c.ParentId == null).ToList();

            // Kategorier som är direkt barn till root-kategorier
            var firstLevelChildCategories = categories
                .Where(c => rootCategories.Any(rc => rc.Id == c.ParentId))
                .ToList();

            // Kategorier som är barnbarn eller längre ned i hierarkin
            var lowerLevelChildCategories = categories
                .Where(c => firstLevelChildCategories.Any(flc => flc.Id == c.ParentId))
                .ToList();

            RootCategories = rootCategories;
            FirstLevelChildCategories = firstLevelChildCategories;
            LowerLevelChildCategories = lowerLevelChildCategories;

            var materials = await _context.Materials.ToListAsync();

            foreach (var category in categories)
            {
                ProductViewModel.Categories.Add(new CategoryCheckboxViewModel()
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    IsSelected = false
                });
            }

            foreach (var material in materials)
            {
                ProductViewModel.Materials.Add(new MaterialViewModel()
                {
                    MaterialId = material.Id,
                    MaterialName = material.Type,
                    IsSelected = false
                });
            }


            return Page();
        }



        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ProductViewModel = new ProductViewModel();
                var categories = await _context.Categories.ToListAsync();

                var materials = await _context.Materials.ToListAsync();

                foreach (var category in categories)
                {
                    ProductViewModel.Categories.Add(new CategoryCheckboxViewModel()
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        IsSelected = false
                    });
                }

                foreach (var material in materials)
                {
                    ProductViewModel.Materials.Add(new MaterialViewModel()
                    {
                        MaterialId = material.Id,
                        MaterialName = material.Type,
                        IsSelected = false
                    });
                }

                return Page();
            }

            if (ProductViewModel.Categories == null || !ProductViewModel.Categories.Any())
            {
                ModelState.AddModelError("", "Du måste välja minst en kategori");
                return Page();
            }
            if (ProductViewModel.Materials == null || !ProductViewModel.Materials.Any())
            {
                ModelState.AddModelError("", "Du måste välja minst ett material.");
                return Page();
            }

            var newProduct = new ProductModel
            {
                Name = ProductViewModel.Name,
                ArticleNumber = ProductViewModel.ArticleNumber,
                PricePerUnit = ProductViewModel.PricePerUnit,
                WeightPerUnit = ProductViewModel.WeightPerUnit,
                //PackagingMaterial = ProductViewModel.PackagingMaterial,
                //Quantity = ProductViewModel.Quantity,
                //TotalCost = ProductViewModel.TotalCost,
                RecyclingRateAtEoL = ProductViewModel.RecyclingRateAtEoL,
                //ProductMaterials = new List<ProductMaterial>(),
                //ProductCategories = new List<ProductCategory>()
            };

            foreach (var categoryViewModel in ProductViewModel.Categories.Where(c => c.IsSelected))
            {
                var category = await _context.Categories
                    .SingleOrDefaultAsync(c => c.Id == categoryViewModel.CategoryId);
                if (category != null)
                {
                    newProduct.ProductCategories.Add(new ProductCategory { CategoryId = category.Id });
                }
            }

            foreach (var materialViewModel in ProductViewModel.Materials.Where(c => c.IsSelected))
            {
                if (materialViewModel != null && materialViewModel.IsSelected)
                {
                    newProduct.ProductMaterials.Add(new ProductMaterial
                    {
                        MaterialId = materialViewModel.MaterialId,
                        Percentage = materialViewModel.Percentage,
                    });
                }
            }


            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
