using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Examensarbete.Data;
using Examensarbete.Models;
using Examensarbete.Services.Interfaces;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IOrderDataService _orderDataService;

    public DetailsModel(ApplicationDbContext context, IOrderDataService orderDataService)
    {
        _context = context;
        _orderDataService = orderDataService;
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

    public async Task<IActionResult> OnPostCreateIncompleteProductAsync([FromBody] int id)
    {
        var result = await _orderDataService.CreateIncompleteProductAsync(id);
        if (result.ProductsCreated == 1)
        {
            return new JsonResult(new { success = true, message = "Produkt skapad och matchad med orderdata.", productId = result.ProductId });
        }
        else
        {
            return new JsonResult(new { success = false, message = result.ErrorMessage });
        }
    }
}
