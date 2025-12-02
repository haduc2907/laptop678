using TTCSN.Entities;
using TTCSN.Usecase.AdminSide.Review;

namespace TTCSN.Infrastructure.Sql
{
    public class SqlReviewControllerRepository : IReviewController
    {
        private readonly string? conn;
        public SqlReviewControllerRepository(IConfiguration config)
        {
            conn = config.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> AddReviewAsync(Review review)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"INSERT INTO Reviews (ProductId, UserId, Rating, Comment, CreatedAt, UpdatedAt)
            VALUES (@ProductId, @UserId, @Rating, @Comment, @CreatedAt, @UpdatedAt)", connection);
            cmd.Parameters.AddWithValue("@ProductId", review!.ProductId);
            cmd.Parameters.AddWithValue("@UserId", review.UserId);
            cmd.Parameters.AddWithValue("@Rating", review.Rating);
            cmd.Parameters.AddWithValue("@Comment", (object?)review.Comment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", review.CreatedAt);
            cmd.Parameters.AddWithValue("@UpdatedAt", (object?)review.UpdatedAt ?? DBNull.Value);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("DELETE FROM Reviews WHERE Id = @ReviewId", connection);
            cmd.Parameters.AddWithValue("@ReviewId", reviewId);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Review>> GetReviewsAsync(int productId, int pageNumber, int pageSize)
        {
            var reviews = new List<Review>();
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            var query = "SELECT * FROM Reviews WHERE 1=1";
            query += "AND ProductId = @ProductId";
            query += " ORDER BY CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var review = new Review
                {
                    Id = reader.GetInt32(0),
                    ProductId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    Rating = reader.GetInt32(3),
                    Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5),
                    UpdatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                };
                reviews.Add(review);
            }
            return reviews;


        }

        public async Task<bool> UpdateReviewAsync(int reviewId, string? content, int rating)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Reviews
                SET Comment = @Comment,
                    Rating = @Rating,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @ReviewId", connection);
            cmd.Parameters.AddWithValue("@ReviewId", reviewId);
            cmd.Parameters.AddWithValue("@Comment", (object?)content ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Rating", rating);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<int> CountReviewsAsync(int productId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("SELECT COUNT(*) FROM Reviews WHERE ProductId = @ProductId", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            if (await cmd.ExecuteScalarAsync() is int count)
            {
                return count;
            }
            return 0;
        }
        public async Task<double> GetAverageRatingAsync(int productId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand("SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews WHERE ProductId = @ProductId", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var result = await cmd.ExecuteScalarAsync();
            if (result != DBNull.Value && result is double averageRating)
            {
                return averageRating;
            }
            return 0.0;
        }
    }
}
