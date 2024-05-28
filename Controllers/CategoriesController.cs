using Examensarbete.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IProductService _productService;

    public CategoriesController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("subcategories/{parentId}")]
    public async Task<IActionResult> GetSubCategories(int parentId)
    {
        var subCategories = await _productService.GetSubCategoriesAsync(parentId);
        // Lägg till loggning
        if (!subCategories.Any())
        {
            return NotFound("Inga underkategorier hittades.");
        }
        return Ok(subCategories);
    }
}
