using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Examensarbete.Data;
using Examensarbete.Models;
using MaterialModel = Examensarbete.Models.Material;


namespace Examensarbete.Pages.Material
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public MaterialModel Material { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Material.ProductMaterials = new List<ProductMaterial>();

            _context.Materials.Add(Material);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
