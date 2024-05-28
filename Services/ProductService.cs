using Examensarbete.Data;
using Examensarbete.Models;
using Examensarbete.Services.Interfaces;

namespace Examensarbete.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<IEnumerable<Product>> CreateProductsAsync(IEnumerable<Product> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            return products;
        }
    }
}