using TTCSN.Entities;

namespace TTCSN.Usecase.AdminSide
{
    public interface IProductController
    {
        Task<bool> AddProductAsync(Product? product);
        Task<bool> UpdateProductAsync(Product? product);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> GetProducts();
        Task<Product?> GetProductById(int productId);
        //Task<IEnumerable<Product>> SearchProducts(string? searchQuery, int? categoryId, decimal? minPrice, decimal? maxPrice);
        //Task<IEnumerable<Product>> SortProducts(string? sortBy, bool sortDescending);
        //Task<IEnumerable<Product>> GetProductsPaged(int pageNumber, int pageSize);
        Task<IEnumerable<Product>> GetProductsAsync(
            string? searchQuery,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            bool sortDescending,
            int pageNumber,
            int pageSize);
        Task<int> CountProductsAsync(
            string? searchQuery,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice);
        Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
        Task<bool> ReduceProductStockAsync(int productId, int quantity);
        Task<bool> CheckProductStockAsync(int productId, int requiredQuantity);
        Task<bool> IncreaseProductStockAsync(int productId, int quantity);
    }
}
