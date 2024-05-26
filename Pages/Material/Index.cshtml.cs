using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using MaterialModel = Examensarbete.Models.Material;

namespace Examensarbete.Pages.Material
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<MaterialModel> Material { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Material = await _context.Materials.ToListAsync();
        }
    }
}
