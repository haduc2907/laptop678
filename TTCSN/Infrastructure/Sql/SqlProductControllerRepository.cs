using Microsoft.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using TTCSN.Entities;
using TTCSN.Usecase.AdminSide;

namespace TTCSN.Infrastructure.Sql
{
    public class SqlProductControllerRepository : IProductController
    {
        private readonly string? conn;
        public SqlProductControllerRepository(IConfiguration config)
        {
            conn = config.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> AddProductAsync(Product? product)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"INSERT INTO Products (Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl)
                  VALUES (@Name, @Description, @Price, @StockQuantity, @Brand, @CategoryId, @CreatedAt, @UserId, @ImageUrl)", connection);
            cmd.Parameters.AddWithValue("@Name", product?.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@Description", (object?)product?.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", product?.Price ?? 0);
            cmd.Parameters.AddWithValue("@StockQuantity", product?.StockQuantity ?? 0);
            cmd.Parameters.AddWithValue("@Brand", (object?)product?.Brand ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoryId", product?.CategoryId ?? 0);
            cmd.Parameters.AddWithValue("@CreatedAt", product?.CreatedAt ?? DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@UserId", product?.UserId ?? 0);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)product?.ImageUrl ?? DBNull.Value);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<int> CountProductsAsync(string? searchQuery, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT COUNT(*) FROM Products
                  WHERE (@SearchQuery IS NULL OR Name LIKE '%' + @SearchQuery + '%' OR Description LIKE '%' + @SearchQuery + '%')
                  AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
                  AND (@MinPrice IS NULL OR Price >= @MinPrice)
                  AND (@MaxPrice IS NULL OR Price <= @MaxPrice)", connection);
            cmd.Parameters.AddWithValue("@SearchQuery", (object?)searchQuery ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoryId", (object?)categoryId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MinPrice", (object?)minPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MaxPrice", (object?)maxPrice ?? DBNull.Value);
            if (await cmd.ExecuteScalarAsync() is int count)
            {
                return count;
            }
            return 0;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"DELETE FROM Products
                  WHERE Id = @ProductId", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<Product?> GetProductById(int productId)
        {
            using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            using var cmd = new SqlCommand("SELECT Id, Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl FROM Products WHERE Id = @ProductId", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var product = new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    Brand = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CategoryId = reader.GetInt32(6),
                    CreatedAt = reader.GetDateTime(7),
                    UserId = reader.GetInt32(8),
                    ImageUrl = reader.IsDBNull(9) ? null : reader.GetString(9)
                };
                return product;
            }
            return null;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products = new List<Product>();
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("SELECT Id, Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl FROM Products", connection);
            await using var reader = await cmd.ExecuteReaderAsync();
            while ( await reader.ReadAsync())
            {
                var product = new Product
                {
                    Id =  reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    Brand = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CategoryId = reader.GetInt32(6),
                    CreatedAt = reader.GetDateTime(7),
                    UserId = reader.GetInt32(8),
                    ImageUrl = reader.IsDBNull(9) ? null : reader.GetString(9)
                };
                products.Add(product);
            }
            return products;
        }

        //public async Task<IEnumerable<Product>> GetProductsPaged(int pageNumber, int pageSize)
        //{
        //    var products = new List<Product>();
        //    await using var connection = new SqlConnection(conn);
        //    await connection.OpenAsync();
        //    await using var cmd = new SqlCommand(@"SELECT Id, Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl 
        //                                          FROM Products
        //                                          ORDER BY Id
        //                                          OFFSET @Offset ROWS
        //                                          FETCH NEXT @PageSize ROWS ONLY", connection);
        //    cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
        //    cmd.Parameters.AddWithValue("@PageSize", pageSize);
        //    await using var reader = await cmd.ExecuteReaderAsync();
        //    while ( await reader.ReadAsync())
        //    {
        //        var product = new Product
        //        {
        //            Id = reader.GetInt32(0),
        //            Name = reader.GetString(1),
        //            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
        //            Price = reader.GetDecimal(3),
        //            StockQuantity = reader.GetInt32(4),
        //            Brand = reader.IsDBNull(5) ? null : reader.GetString(5),
        //            CategoryId = reader.GetInt32(6),
        //            CreatedAt = reader.GetDateTime(7),
        //            UserId = reader.GetInt32(8),
        //            ImageUrl = reader.IsDBNull(9) ? null : reader.GetString(9)
        //        };
        //        products.Add(product);
        //    }
        //    return products;
        //}

        //public async Task<IEnumerable<Product>> SearchProducts(string? searchQuery, int? categoryId, decimal? minPrice, decimal? maxPrice)
        //{
        //    var products = new List<Product>();
        //    await using var connection = new SqlConnection(conn);
        //    await connection.OpenAsync();
        //    var query = @"SELECT Id, Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl 
        //                  FROM Products
        //                  WHERE (@SearchQuery IS NULL OR Name LIKE '%' + @SearchQuery + '%' OR Description LIKE '%' + @SearchQuery + '%')
        //                  AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
        //                  AND (@MinPrice IS NULL OR Price >= @MinPrice)
        //                  AND (@MaxPrice IS NULL OR Price <= @MaxPrice)";
        //    await using var cmd = new SqlCommand(query, connection);
        //    cmd.Parameters.AddWithValue("@SearchQuery", (object?)searchQuery ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@CategoryId", (object?)categoryId ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@MinPrice", (object?)minPrice ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@MaxPrice", (object?)maxPrice ?? DBNull.Value);
        //    await using var reader = await cmd.ExecuteReaderAsync();
        //    while ( await reader.ReadAsync())
        //    {
        //        var product = new Product
        //        {
        //            Id = reader.GetInt32(0),
        //            Name = reader.GetString(1),
        //            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
        //            Price = reader.GetDecimal(3),
        //            StockQuantity = reader.GetInt32(4),
        //            Brand = reader.IsDBNull(5) ? null : reader.GetString(5),
        //            CategoryId = reader.GetInt32(6),
        //            CreatedAt = reader.GetDateTime(7),
        //            UserId = reader.GetInt32(8),
        //            ImageUrl = reader.IsDBNull(9) ? null : reader.GetString(9)
        //        };
        //        products.Add(product);
        //    }
        //    return products;
        //}

        //public async Task<IEnumerable<Product>> SortProducts(string? sortBy, bool sortDescending)
        //{
        //    var products = new List<Product>();
        //    await using var connection = new SqlConnection(conn);
        //    await connection.OpenAsync();
        //    var orderByClause = sortBy switch
        //    {
        //        "Name" => "Name",
        //        "Price" => "Price",
        //        "CreatedAt" => "CreatedAt",
        //        _ => "Id"
        //    };
        //    var direction = sortDescending ? "DESC" : "ASC";
        //    var query = $@"SELECT Id, Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl 
        //                   FROM Products
        //                   ORDER BY {orderByClause} {direction}";
        //    await using var cmd = new SqlCommand(query, connection);
        //    await using var reader = await cmd.ExecuteReaderAsync();
        //    while ( await reader.ReadAsync())
        //    {
        //        var product = new Product
        //        {
        //            Id = reader.GetInt32(0),
        //            Name = reader.GetString(1),
        //            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
        //            Price = reader.GetDecimal(3),
        //            StockQuantity = reader.GetInt32(4),
        //            Brand = reader.IsDBNull(5) ? null : reader.GetString(5),
        //            CategoryId = reader.GetInt32(6),
        //            CreatedAt = reader.GetDateTime(7),
        //            UserId = reader.GetInt32(8),
        //            ImageUrl = reader.IsDBNull(9) ? null : reader.GetString(9)
        //        };
        //        products.Add(product);
        //    }
        //    return products;
        //}
        public async Task<IEnumerable<Product>> GetProductsAsync(
            string? searchQuery,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            bool sortDescending,
            int pageNumber,
            int pageSize)
        {
            var products = new List<Product>();

            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            // Base query
            var query = @"
        SELECT Id, Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl
        FROM Products
        WHERE 1 = 1 ";

            // ---- SEARCH ----
            if (!string.IsNullOrEmpty(searchQuery))
                query += " AND (Name LIKE '%' + @SearchQuery + '%' OR Description LIKE '%' + @SearchQuery + '%')";

            // ---- FILTER ----
            if (categoryId.HasValue)
                query += " AND CategoryId = @CategoryId";

            if (minPrice.HasValue)
                query += " AND Price >= @MinPrice";

            if (maxPrice.HasValue)
                query += " AND Price <= @MaxPrice";

            // ---- SORT ----
            var orderBy = sortBy switch
            {
                "Name" => "Name",
                "Price" => "Price",
                "CreatedAt" => "CreatedAt",
                _ => "Id"
            };

            var direction = sortDescending ? "DESC" : "ASC";
            query += $" ORDER BY {orderBy} {direction}";

            // ---- PAGING ----
            query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            await using var cmd = new SqlCommand(query, connection);

            // Add parameters
            cmd.Parameters.AddWithValue("@SearchQuery", (object?)searchQuery ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoryId", (object?)categoryId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MinPrice", (object?)minPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MaxPrice", (object?)maxPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    Brand = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CategoryId = reader.GetInt32(6),
                    CreatedAt = reader.GetDateTime(7),
                    UserId = reader.GetInt32(8),
                    ImageUrl = reader.IsDBNull(9) ? null : reader.GetString(9)
                });
            }

            return products;
        }


        public async Task<bool> UpdateProductAsync(Product? product)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Products
                  SET Name = @Name,
                      Description = @Description,
                      Price = @Price,
                      StockQuantity = @StockQuantity,
                      Brand = @Brand,
                      CategoryId = @Category,
                      ImageUrl = @ImageUrl
                        Where Id = @Id" , connection);
            cmd.Parameters.AddWithValue("@Id", product?.Id ?? 0);
            cmd.Parameters.AddWithValue("@Name", product?.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@Description", (object?)product?.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Price", product?.Price ?? 0);
            cmd.Parameters.AddWithValue("@StockQuantity", product?.StockQuantity ?? 0);
            cmd.Parameters.AddWithValue("@Brand", (object?)product?.Brand ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", product?.CategoryId ?? 0);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)product?.ImageUrl ?? DBNull.Value);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            var products = new List<Product>();
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("SELECT Id, Name, Description, Price, StockQuantity, Brand, CategoryId, CreatedAt, UserId, ImageUrl FROM Products WHERE CategoryId = @Category", connection);
            cmd.Parameters.AddWithValue("@Category", categoryId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while ( await reader.ReadAsync())
            {
                var product = new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    Brand = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CategoryId = reader.GetInt32(6),
                    CreatedAt = reader.GetDateTime(7),
                    UserId = reader.GetInt32(8),
                    ImageUrl = reader.IsDBNull(9) ? null : reader.GetString(9)
                };
                products.Add(product);
            }
            return products;
        }

        public async Task<bool> ReduceProductStockAsync(int productId, int quantity)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Products
                  SET StockQuantity = StockQuantity - @Quantity
                  WHERE Id = @ProductId AND StockQuantity >= @Quantity", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Quantity", quantity);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> CheckProductStockAsync(int productId, int requiredQuantity)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT StockQuantity
                  FROM Products
                  WHERE Id = @ProductId", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return false;
            }
            var stockQuantity = Convert.ToInt32(result);
            return stockQuantity >= requiredQuantity;
        }
        public async Task<bool> IncreaseProductStockAsync(int productId, int quantity)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Products
                  SET StockQuantity = StockQuantity + @Quantity
                  WHERE Id = @ProductId", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Quantity", quantity);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}
