using TTCSN.Entities;
using TTCSN.Entities.Enum;

namespace TTCSN.Usecase.AdminSide
{
    public interface IOrderController
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status, string? Note);
        Task<IEnumerable<Order>> GetOrdersAsync(
            OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            string? sortBy,
            bool sortDescending,
            int pageNumber,
            int pageSize);
        Task<int> CountOrdersAsync(
            OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<Order?> CreateNewOrder(Order order);
        Task<bool> UpdateOrderTotalAmountAsync(int orderId, decimal totalAmount);
        Task<int> GetStatusOrderById(int orderId);
    }
    
}
