using TTCSN.Models.Review;

namespace TTCSN.Models.User
{
    public class UserListViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; } = Enumerable.Empty<UserViewModel>();
        public int PageNumber { get; set; } = 1;   // trang hiện tại
        public int PageSize { get; set; } = 5;    // số item mỗi trang
        public int TotalItems { get; set; }        // tổng số sản phẩm
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        // Search & Filter
        public string? SearchName { get; set; }   // tìm kiếm theo tên user
        public int? UserId { get; set; }        // lọc theo user cụ thể
        public bool? IsActive { get; set; }        // lọc theo trạng thái hoạt động

        // Sort Options
        public string? SortBy { get; set; }        // sắp xếp: "Date", "Rating", "UserName"
        public bool SortDescending { get; set; } = true;  // mặc định mới nhất trước
                                                          // Date Filter
        public DateTime? FromDate { get; set; }    // lọc từ ngày
        public DateTime? ToDate { get; set; }      // lọc đến ngày
    }
}
