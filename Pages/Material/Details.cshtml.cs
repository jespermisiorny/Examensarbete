using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Examensarbete.Services.Interfaces;
using MaterialModel = Examensarbete.Models.Material;
using Microsoft.EntityFrameworkCore;

namespace Examensarbete.Pages.Material
{
    public class DetailsModel : PageModel
    {
        private readonly IMaterialService _materialService;

        public DetailsModel(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [BindProperty]
        public MaterialModel Material { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _materialService.GetMaterialByIdAsync(id.Value);
            if (material == null)
            {
                return NotFound();
            }
            else
            {
                Material = material;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _materialService.UpdateMaterialAsync(Material);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MaterialExists(Material.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = Material.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material != null)
            {
                await _materialService.DeleteMaterialAsync(id);
                return RedirectToPage("./Index");
            }

            return NotFound();
        }

        private async Task<bool> MaterialExists(int id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            return material != null;
        }
    }
}
