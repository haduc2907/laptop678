using TTCSN.Entities;
using TTCSN.Entities.Enum;
using TTCSN.Usecase.AdminSide;

namespace TTCSN.Infrastructure.Sql
{
    public class SqlOrderControllerRepository : IOrderController
    {
        private readonly string? conn;
        public SqlOrderControllerRepository(IConfiguration config)
        {
            conn = config.GetConnectionString("DefaultConnection");
        }
        public async Task<int> CountOrdersAsync(OrderStatus? status, DateTime? startDate, DateTime? endDate)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            var query = @"SELECT COUNT(*) FROM Orders WHERE 1=1";
            if (status.HasValue)
            {
                query += " AND Status = @Status";
            }
            if (startDate.HasValue)
            {
                query += " AND OrderDate >= @StartDate";
            }
            if (endDate.HasValue)
            {
                query += " AND OrderDate <= @EndDate";
            }
            await using var cmd = new SqlCommand(query, connection);
            if (status.HasValue)
            {
                cmd.Parameters.AddWithValue("@Status", (int)status.Value);
            }
            if (startDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
            }
            if (endDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
            }
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(result);

        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            var orderDetails = new List<OrderDetail>();
            await using var cmd = new SqlCommand(@"SELECT Id, OrderId, ProductId, Quantity, UnitPrice, ProductName, ImageUrl, IsReviewed, IsDelivered
            FROM OrderDetails 
            WHERE OrderId = @OrderId ", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var orderDetail = new OrderDetail
                {
                    Id = reader.GetInt32(0),
                    OrderId = reader.GetInt32(1),
                    ProductId = reader.GetInt32(2),
                    Quantity = reader.GetInt32(3),
                    UnitPrice = reader.GetDecimal(4),
                    ProductName = reader.GetString(5),
                    ImageUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
                    IsReviewed = reader.GetBoolean(7),
                    IsDelivered = reader.GetBoolean(8)
                };
                orderDetails.Add(orderDetail);
            }
            return orderDetails;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(OrderStatus? status
            , DateTime? startDate
            , DateTime? endDate
            , string? sortBy
            , bool sortDescending
            , int pageNumber
            , int pageSize)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            var orders = new List<Order>();
            var query = @"SELECT Id, UserId, OrderDate, TotalAmount, Status, PaymentMethod, Address, PhoneNumber, Note
                        FROM Orders
                        WHERE 1=1";
            if (status.HasValue)
            {
                query += " AND Status = @Status";
            }
            if (startDate.HasValue)
            {
                query += " AND OrderDate >= @StartDate";
            }
            if (endDate.HasValue)
            {
                query += " AND OrderDate <= @EndDate";
            }
            var orderBy = sortBy switch
            {
                "OrderDate" => "OrderDate",
                _ => "Id"
            };
            var direction = sortDescending ? "DESC" : "ASC";
            query += $" ORDER BY {orderBy} {direction}";
            query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            await using var cmd = new SqlCommand(query, connection);

            // Add parameters
            cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            if (status.HasValue)
            {
                cmd.Parameters.AddWithValue("@Status", (int)status.Value);
            }
            if (startDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
            }
            if (endDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
            }
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var order = new Order
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    OrderDate = reader.GetDateTime(2),
                    TotalAmount = reader.GetDecimal(3),
                    Status = (OrderStatus)reader.GetInt32(4),
                    PaymentMethod = (PaymentMethods)reader.GetInt32(5), // Default value, adjust as needed
                    Address = reader.GetString(6),
                    PhoneNumber = reader.GetString(7),
                    Note = reader.IsDBNull(8) ? null : reader.GetString(8)
                };
                orders.Add(order);
            }
            return orders;
        }


        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            var orders = new List<Order>();
            await using var cmd = new SqlCommand(@"SELECT Id, UserId, OrderDate, TotalAmount, Status, PaymentMethod, Address, PhoneNumber, Note
            FROM Orders
            WHERE UserId = @UserId", connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var order = new Order
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    OrderDate = reader.GetDateTime(2),
                    TotalAmount = reader.GetDecimal(3),
                    Status = (OrderStatus)reader.GetInt32(4),
                    PaymentMethod = (PaymentMethods)reader.GetInt32(5),
                    Address = reader.GetString(6),
                    PhoneNumber = reader.GetString(7),
                    Note = reader.IsDBNull(8) ? null : reader.GetString(8)
                };
                orders.Add(order);
            }
            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status, string? note)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Orders
                  SET Status = @Status, Note = @Note
                  WHERE Id = @OrderId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@Status", (int)status);
            cmd.Parameters.AddWithValue("@Note", (object?)note ?? DBNull.Value);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"DELETE FROM Orders WHERE Id = @OrderId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        public async Task<Order?> CreateNewOrder(Order order)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            // Dùng OUTPUT để lấy Id luôn trong 1 câu query
            await using var cmd = new SqlCommand(@"
        INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, PaymentMethod, Address, PhoneNumber, Note)
        OUTPUT INSERTED.Id
        VALUES (@UserId, @OrderDate, @TotalAmount, @Status, @PaymentMethod, @Address, @PhoneNumber, @Note)",
                connection);

            cmd.Parameters.AddWithValue("@UserId", order.UserId);
            cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
            cmd.Parameters.AddWithValue("@Status", (int)order.Status);
            cmd.Parameters.AddWithValue("@PaymentMethod", (int)order.PaymentMethod);
            cmd.Parameters.AddWithValue("@Address", order.Address);
            cmd.Parameters.AddWithValue("@PhoneNumber", order.PhoneNumber);
            cmd.Parameters.AddWithValue("@Note", (object?)order.Note ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                order.Id = Convert.ToInt32(result);
                return order;
            }

            return null;
        }
        public async Task<bool> UpdateOrderTotalAmountAsync(int orderId, decimal totalAmount)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Orders
                  SET TotalAmount = @TotalAmount
                  WHERE Id = @OrderId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;

        }
        public async Task<int> GetStatusOrderById(int orderId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT Status FROM Orders WHERE Id = @OrderId", connection);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return -1; // Hoặc ném ngoại lệ tùy theo yêu cầu của bạn
            }
            return Convert.ToInt32(result);
        }
    }
}
