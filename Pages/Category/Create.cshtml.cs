using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using CategoryModel = Examensarbete.Models.Category;

namespace Examensarbete.Pages.Category
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
            CategoriesList = new List<CategoryModel>();
        }

        [BindProperty]
        public CategoryModel NewCategory { get; set; } = new CategoryModel();
        [BindProperty]
        public bool IsMainCategory { get; set; }
        public List<CategoryModel> MainCategories { get; set; }
        public List<CategoryModel> CategoriesList { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategoriesList();

            MainCategories = CategoriesList
                .Where(c => c.ParentId == null)
                .ToList();

            ViewData["MainCategories"] = new SelectList(MainCategories, "Id", "Name");

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                await LoadCategoriesList();
                return Page();
            }

            var existingCategory = await _context.Categories
                .AnyAsync(c => EF.Functions.Like(c.Name, NewCategory.Name));

            if (existingCategory)
            {
                ModelState.AddModelError("NewCategory.Name", "En kategori med detta namn existerar redan.");
                CategoriesList = await _context.Categories.ToListAsync();
                MainCategories = CategoriesList.Where(c => c.ParentId == null).ToList();
                return Page();
            }

            if (IsMainCategory)
            {
                NewCategory.ParentId = null;
            }
            NewCategory.ParentCategory = null;


            _context.Categories.Add(NewCategory);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadCategoriesList()
        {
            CategoriesList = await _context.Categories.ToListAsync();
        }
    }
}
