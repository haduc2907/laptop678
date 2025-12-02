using TTCSN.Entities.Enum;

namespace TTCSN.Models.Order
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderViewModel> Orders { get; set; } = Enumerable.Empty<OrderViewModel>();
        public int PageNumber { get; set; } = 1;   // trang hiện tại
        public int PageSize { get; set; } = 10;    // số item mỗi trang
        public int TotalItems { get; set; }        // tổng số sản phẩm
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public DateTime? StartDate { get; set; }    // Lọc từ ngày
        public DateTime? EndDate { get; set; }      // Lọc đến ngày
        public OrderStatus? Status { get; set; }    // Lọc theo trạng thái đơn hàng


        // Sắp xếp
        public string? SortBy { get; set; }        // ví dụ: "DateOrder" , "Id"
        public bool SortDescending { get; set; }   // true = giảm dần
    }
}
