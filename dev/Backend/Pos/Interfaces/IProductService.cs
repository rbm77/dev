using Pos.Models;

namespace Pos.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();

        Task<Product?> GetProductById(string productId);

        Task<string> CreateProduct(Product product);

        Task<bool> UpdateProduct(string productId, Product product);

        Task<bool> DeleteProduct(string productId);
    }
}
