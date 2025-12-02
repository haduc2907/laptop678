using TTCSN.Entities;

namespace TTCSN.Usecase.AdminSide
{
    public class OrderDetailControllerRepository
    {
        private readonly IOrderDetailController repo;
        public OrderDetailControllerRepository(IOrderDetailController repository)
        {
            repo = repository;
        }
        public Task<bool> AddOrderDetail(OrderDetail orderDetail)
        {
            return repo.AddOrderDetail(orderDetail);
        }
        public Task<bool> UpdateIsReviewed(int orderId, int productId)
        {
            return repo.UpdateIsReviewed(orderId, productId);
        }
        public Task<bool> UpdateIsDelivered(int orderId, int productId)
        {
            return repo.UpdateIsDelivered(orderId, productId);
        }
        public Task<int> GetIsReviewed(int orderId, int productId)
        {
            return repo.GetIsReviewed(orderId, productId);
        }
        public Task<int> GetIsDelivered(int orderId, int productId)
        {
            return repo.GetIsDelivered(orderId, productId);
        }
    }
}
