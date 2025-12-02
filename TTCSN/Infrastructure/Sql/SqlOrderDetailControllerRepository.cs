using TTCSN.Entities;
using TTCSN.Usecase.AdminSide;

namespace TTCSN.Infrastructure.Sql
{
    public class SqlOrderDetailControllerRepository : IOrderDetailController
    {
        private readonly string? conn;
        public SqlOrderDetailControllerRepository(IConfiguration config)
        {
            conn = config.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> AddOrderDetail(OrderDetail orderDetail)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, ProductName, ImageUrl)
            VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @ProductName, @ImageUrl)", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderDetail.OrderId);
            cmd.Parameters.AddWithValue("@ProductId", orderDetail.ProductId);
            cmd.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
            cmd.Parameters.AddWithValue("@UnitPrice", orderDetail.UnitPrice);
            cmd.Parameters.AddWithValue("@ProductName", orderDetail.ProductName);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)orderDetail.ImageUrl ?? DBNull.Value);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<bool> UpdateIsReviewed(int orderId, int productId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE OrderDetails SET IsReviewed = 1 WHERE OrderId = @OrderId AND ProductId = @ProductId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<bool> UpdateIsDelivered(int orderId, int productId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE OrderDetails SET IsDelivered = 1 WHERE OrderId = @OrderId AND ProductId = @ProductId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<int> GetIsReviewed(int orderId, int productId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT IsReviewed FROM OrderDetails WHERE OrderId = @OrderId AND ProductId = @ProductId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return -1; // Hoặc ném ngoại lệ tùy theo yêu cầu của bạn
            }
            return Convert.ToInt32(result);
        }
        public async Task<int> GetIsDelivered(int orderId, int productId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT IsDelivered FROM OrderDetails WHERE OrderId = @OrderId AND ProductId = @ProductId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return -1; // Hoặc ném ngoại lệ tùy theo yêu cầu của bạn
            }
            return Convert.ToInt32(result);
        }
    }
}
