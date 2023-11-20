using AuthSchemesAndOptionsPattern.Model;

namespace AuthSchemesAndOptionsPattern.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(int productId, Product product);
        Task<Product> DeleteProductAsync(int id);
    }
}
