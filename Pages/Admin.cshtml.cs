using Examensarbete.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Examensarbete.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public AdminModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostMatchOrdersAsync()
        {
            var unmatchedOrders = _context.OrderData
                .Where(od => od.ProductId == null);

            var products = _context.Products.ToDictionary(p => p.ArticleNumber, p => p.Id);

            foreach (var order in unmatchedOrders)
            {
                if (products.TryGetValue(order.ArticleNumber, out var productId))
                {
                    order.ProductId = productId;
                }
            }
            await _context.SaveChangesAsync();

            return Page();
        }
    }
}
