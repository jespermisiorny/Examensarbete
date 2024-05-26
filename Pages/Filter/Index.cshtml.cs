using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using FilterModel = Examensarbete.Models.Filter;

namespace Examensarbete.Pages.Filter
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<FilterModel> Filter { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Filter = await _context.Filters.ToListAsync();
        }
    }
}
