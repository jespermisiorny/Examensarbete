using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using FilterModel = Examensarbete.Models.Filter;

namespace Examensarbete.Pages.Filter
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filter = await _context.Filters.FindAsync(id);
            if (filter != null)
            {
                Filter = filter;
                _context.Filters.Remove(Filter);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
