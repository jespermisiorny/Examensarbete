using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Examensarbete.Services.Interfaces;
using MaterialModel = Examensarbete.Models.Material;

namespace Examensarbete.Pages.Material
{
    public class IndexModel : PageModel
    {
        private readonly IMaterialService _materialService;

        public IndexModel(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        public IList<MaterialModel> Material { get; set; } = default!;

        [BindProperty]
        public MaterialModel NewMaterial { get; set; } = new MaterialModel();

        public async Task OnGetAsync()
        {
            Material = await _materialService.GetAllMaterialsAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); 
                return Page();
            }

            await _materialService.CreateMaterialAsync(NewMaterial);
            return RedirectToPage("./Index");
        }
    }
}
