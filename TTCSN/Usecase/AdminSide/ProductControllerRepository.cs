using TTCSN.Entities;

namespace TTCSN.Usecase.AdminSide
{
    public class ProductControllerRepository
    {
        private readonly IProductController repo;
        public ProductControllerRepository(IProductController repository)
        {
            repo = repository;
        }
        public Task<bool> AddProductAsync(Product? product)
        {
            return repo.AddProductAsync(product);
        }
        public Task<bool> UpdateProductAsync(Product? product)
        {
            return repo.UpdateProductAsync(product);
        }
        public Task<bool> DeleteProductAsync(int productId)
        {
            return repo.DeleteProductAsync(productId);
        }
        public Task<IEnumerable<Product>> GetProducts()
        {
            return repo.GetProducts();
        }
        public Task<Product?> GetProductById(int productId)
        {
            return repo.GetProductById(productId);
        }
        //public Task<IEnumerable<Product>> SearchProducts(string? searchQuery, int? categoryId, decimal? minPrice, decimal? maxPrice)
        //{
        //    return repo.SearchProducts(searchQuery, categoryId, minPrice, maxPrice);
        //}
        //public Task<IEnumerable<Product>> SortProducts(string? sortBy, bool sortDescending)
        //{
        //    return repo.SortProducts(sortBy, sortDescending);
        //}
        //public Task<IEnumerable<Product>> GetProductsPaged(int pageNumber, int pageSize)
        //{
        //    return repo.GetProductsPaged(pageNumber, pageSize);
        //}
        public Task<IEnumerable<Product>> GetProductsAsync(
            string? searchQuery,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            bool sortDescending,
            int pageNumber,
            int pageSize)
        {
            return repo.GetProductsAsync(
                searchQuery,
                categoryId,
                minPrice,
                maxPrice,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize);
        }
        public Task<int> CountProductsAsync(
            string? searchQuery,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            return repo.CountProductsAsync(
                searchQuery,
                categoryId,
                minPrice,
                maxPrice);
        }
        public Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            return repo.GetProductsByCategory(categoryId);
        }
        public Task<bool> ReduceProductStockAsync(int productId, int quantity)
        {
            return repo.ReduceProductStockAsync(productId, quantity);
        }
        public Task<bool> CheckProductStockAsync(int productId, int requiredQuantity)
        {
            return repo.CheckProductStockAsync(productId, requiredQuantity);
        }
        public Task<bool> IncreaseProductStockAsync(int productId, int quantity)
        {
            return repo.IncreaseProductStockAsync(productId, quantity);
        }
    }
}
