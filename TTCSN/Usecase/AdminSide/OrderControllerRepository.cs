using TTCSN.Entities;
using TTCSN.Entities.Enum;

namespace TTCSN.Usecase.AdminSide
{
    public class OrderControllerRepository
    {
        private readonly IOrderController repo;
        public OrderControllerRepository(IOrderController repository)
        {
            repo = repository;
        }
        public Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            return repo.GetOrderDetailsByOrderIdAsync(orderId);
        }
        public Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status, string? note)
        {
            return repo.UpdateOrderStatusAsync(orderId, status, note);
        }
        public Task<IEnumerable<Order>> GetOrdersAsync(
            OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            string? sortBy,
            bool sortDescending,
            int pageNumber,
            int pageSize)
        {
            return repo.GetOrdersAsync(
                status,
                startDate,
                endDate,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize);
        }
        public Task<int> CountOrdersAsync(
            OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            return repo.CountOrdersAsync(
                status,
                startDate,
                endDate);
        }
        public Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            return repo.GetOrdersByUserId(userId);
        }
        public Task<bool> DeleteOrderAsync(int orderId)
        {
            return repo.DeleteOrderAsync(orderId);
        }
        public Task<Order?> CreateNewOrder(Order order)
        {
            return repo.CreateNewOrder(order);
        }
        public Task<bool> UpdateOrderTotalAmountAsync(int orderId, decimal totalAmount)
        {
            return repo.UpdateOrderTotalAmountAsync(orderId, totalAmount);
        }
        public Task<int> GetStatusOrderById(int orderId)
        {
            return repo.GetStatusOrderById(orderId);
        }
    }
}
