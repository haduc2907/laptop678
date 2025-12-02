using System.Security.Cryptography.Pkcs;

namespace TTCSN.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsReviewed { get; set; }
        public bool IsDelivered { get; set; }
    }
}
