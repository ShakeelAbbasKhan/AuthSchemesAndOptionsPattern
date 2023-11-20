using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthSchemesAndOptionsPattern.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> DeleteProductAsync(int id)
        {
            var product = await _context.Products.Include(u => u.Category).FirstOrDefaultAsync(u => u.Id == id);

            if (product != null)
            {
                _context.Remove(product);

                await _context.SaveChangesAsync();
                return product;
            }
            return null;
        }

        public async Task<List<Product>> GetProductsAsync()
        {

            // Explicit Loading
            //Product? product = await _context.Products.FirstOrDefaultAsync(u => u.Id == 1);

            //if (product != null)
            //{
            //    await _context.Entry(product)
            //        .Collection(p => p.Category) // Assuming Categories is the collection property
            //        .LoadAsync();
            //}

            // Eager Loading
            var products = await _context.Products.Include(c => c.Category).ToListAsync();

            // Lazy Loading

            //List<Product> products = await _context.Products.ToListAsync();

            //foreach(var product in products )
            //{
            //    // Assuming you want to assign the first matching category, you can use FirstOrDefault
            //    product.Category = await _context.Categories
            //        .Where(u => u.Id == product.CategoryId)
            //        .FirstOrDefaultAsync();
            //}

            return products;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _context.Products.Include(c => c.Category).FirstOrDefaultAsync(u => u.Id == id);
            return product;
        }

        public async Task<Product> UpdateProductAsync(int productId, Product product)
        {
            var existingProduct = await GetProductAsync(productId);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;

                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;



                await _context.SaveChangesAsync();
                return existingProduct;
            }



            return null;
        }
    }
}
