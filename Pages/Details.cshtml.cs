using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Examensarbete.Data;
using Examensarbete.Models;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public OrderData OrderData { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        OrderData = await _context.OrderData.FirstOrDefaultAsync(m => m.Id == id);

        if (OrderData == null)
        {
            return NotFound();
        }
        return Page();
    }
}
