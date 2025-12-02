using TTCSN.Entities;

namespace TTCSN.Usecase.AdminSide
{
    public interface IOrderDetailController
    {
        Task<bool> AddOrderDetail(OrderDetail orderDetail);
        Task<bool> UpdateIsReviewed(int orderId, int productId);
        Task<bool> UpdateIsDelivered(int orderId, int productId);
        Task<int> GetIsReviewed(int orderId, int productId);
        Task<int> GetIsDelivered(int orderId, int productId);
    }
}
