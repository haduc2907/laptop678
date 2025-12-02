namespace TTCSN.Models
{
    public class ProductListViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; } = Enumerable.Empty<ProductViewModel>();
        public IEnumerable<CategoryViewModel> Categories { get; set; } = Enumerable.Empty<CategoryViewModel>();

        // Tìm kiếm
        public string? SearchQuery { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Phân trang
        public int PageNumber { get; set; } = 1;   // trang hiện tại
        public int PageSize { get; set; } = 10;    // số item mỗi trang
        public int TotalItems { get; set; }        // tổng số sản phẩm
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);


        // Sắp xếp
        public string? SortBy { get; set; }        // ví dụ: "price" , "name"
        public bool SortDescending { get; set; }   // true = giảm dần
    }
}
