using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FilterModel = Examensarbete.Models.Filter;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using Examensarbete.Models;
using Examensarbete.ViewModels;

namespace Examensarbete.Pages.Filter
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FilterCreateViewModel FilterViewModel { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            FilterViewModel = new FilterCreateViewModel();
            var categories = await _context.Categories.ToListAsync();
            foreach (var category in categories)
            {
                FilterViewModel.Categories.Add(new CategoryCheckboxViewModel()
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    IsSelected = false
                });
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                var categories = await _context.Categories.ToListAsync();
                foreach (var category in categories)
                {
                    FilterViewModel.Categories.Add(new CategoryCheckboxViewModel()
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        IsSelected = false
                    });
                }

                return Page();
            }

            if (FilterViewModel.Categories == null || !FilterViewModel.Categories.Any())
            {
                ModelState.AddModelError("", "Du måste välja minst en kategori");
                return Page();
            }

            var newFilter = new FilterModel
            {
                Text = FilterViewModel.Text,
                FilterCategories = new List<FilterCategory>()
            };

            foreach (var categoryViewModel in FilterViewModel.Categories.Where(c => c.IsSelected))
            {
                if (categoryViewModel != null && categoryViewModel.IsSelected)
                {
                    newFilter.FilterCategories.Add(new FilterCategory
                    {
                        CategoryId = categoryViewModel.CategoryId,
                    });
                }
            }

            _context.Filters.Add(newFilter);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
