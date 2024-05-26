using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Examensarbete.Data;
using Examensarbete.Models;
using Microsoft.EntityFrameworkCore;

public class ReviewModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ReviewModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<OrderData> OrderDataList { get; set; }

    [BindProperty]
    public DateTime? FilterStartDate { get; set; }

    [BindProperty]
    public DateTime? FilterEndDate { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        OrderDataList = await _context.OrderData.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return Page();
    }

    public IActionResult OnPostReset()
    {
        return RedirectToPage("./Review");
    }

    public async Task<IActionResult> OnPostAllOrderDataAsync()
    {
        var query = _context.OrderData.AsQueryable();
        ApplyDateFilter(ref query);
        OrderDataList = await query.ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostMatchedOrderDataAsync()
    {
        var query = _context.OrderData.Where(od => od.ProductId.HasValue);
        ApplyDateFilter(ref query);
        OrderDataList = await query.ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostUnmatchedOrderDataAsync()
    {
        var query = _context.OrderData.Where(od => !od.ProductId.HasValue);
        ApplyDateFilter(ref query);
        OrderDataList = await query.ToListAsync();

        return Page();
    }

    public void ApplyDateFilter(ref IQueryable<OrderData> query)
    {
        if (FilterStartDate.HasValue && FilterEndDate.HasValue)
        {
            query = query.Where(od => od.OrderDate.Date >= FilterStartDate.Value.Date && od.OrderDate.Date <= FilterEndDate.Value.Date);
        }
        else if (FilterStartDate.HasValue)
        {
            query = query.Where(od => od.OrderDate.Date >= FilterStartDate.Value.Date);
        }
        else if (FilterEndDate.HasValue)
        {
            query = query.Where(od => od.OrderDate.Date <= FilterEndDate.Value.Date);
        }
    }





}
