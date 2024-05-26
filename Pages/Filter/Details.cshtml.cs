using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using FilterModel = Examensarbete.Models.Filter;

namespace Examensarbete.Pages.Filter
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public FilterModel Filter { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filter = await _context.Filters.FirstOrDefaultAsync(m => m.Id == id);
            if (filter == null)
            {
                return NotFound();
            }
            else
            {
                Filter = filter;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var filter = await _context.Filters.FindAsync(id);
            if (filter != null)
            {
                _context.Filters.Remove(filter);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return NotFound();
        }
    }
}
