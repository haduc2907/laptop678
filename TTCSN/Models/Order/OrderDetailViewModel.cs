using TTCSN.Entities.Enum;

namespace TTCSN.Models.Order
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsReviewed { get; set; }
        public bool IsDelivered { get; set; }

    }
}
