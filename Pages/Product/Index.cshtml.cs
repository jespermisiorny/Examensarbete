using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Examensarbete.Data;
using Examensarbete.Models;
using ProductModel = Examensarbete.Models.Product;
using System.Text;


namespace Examensarbete.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ProductModel> AllProducts { get; set; }

        public async Task OnGetAsync()
        {
            AllProducts = await _context.Products.ToListAsync();
        }

        public string GetCategoryHierarchy(Models.Category category)
        {
            var hierarchy = new StringBuilder();
            AppendCategoryHierarchy(category, hierarchy, 0);
            return hierarchy.ToString();
        }

        private void AppendCategoryHierarchy(Models.Category category, StringBuilder hierarchy, int level)
        {
            if (category == null) return;

            // Lägg till indrag för att visa hierarkinivå visuellt
            hierarchy.AppendLine(new string(' ', level * 2) + category.Name);

            foreach (var subCategory in category.SubCategories)
            {
                AppendCategoryHierarchy(subCategory, hierarchy, level + 1);
            }
        }

    }
}
