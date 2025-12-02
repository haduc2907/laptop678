using TTCSN.Entities.Enum;

namespace TTCSN.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public PaymentMethods PaymentMethod { get; set; } = PaymentMethods.Cod;
        public string? Note { get; set; }
    }
}
