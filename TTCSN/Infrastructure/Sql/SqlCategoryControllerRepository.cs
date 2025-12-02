
using TTCSN.Entities;
using TTCSN.Usecase.AdminSide;

namespace TTCSN.Infrastructure.Sql
{
    public class SqlCategoryControllerRepository : ICategoryController
    {
        private readonly string? conn;
        public SqlCategoryControllerRepository(IConfiguration config)
        {
            conn = config.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> AddCategoryAsync(string categoryName)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"INSERT INTO Categories (Name)
                  VALUES (@Name)", connection);
            cmd.Parameters.AddWithValue("@Name", categoryName);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"DELETE FROM Categories
                  WHERE Id = @CategoryId", connection);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            var categories = new List<Category>();
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("SELECT Id, Name FROM Categories", connection);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var category = new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
                categories.Add(category);
            }
            return categories;
        }

        public async Task<Category?> GetCategory(int id)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("SELECT Id, Name FROM Categories WHERE Id = @Id", connection);
            cmd.Parameters.AddWithValue("@Id", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var category = new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
                return category;
            }
            return null;
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, string categoryName)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Categories
                  SET Name = @Name
                  WHERE Id = @CategoryId", connection);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@Name", categoryName);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}
