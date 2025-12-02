using TTCSN.Entities;
using TTCSN.Entities.Enum;

namespace TTCSN.Models.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public TTCSN.Entities.User? Customer { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public PaymentMethods PaymentMethod { get; set; }
        public string? Note { get; set; }
    }
}
