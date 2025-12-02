namespace TTCSN.Models.Review
{
    public class ReviewListViewModel
    {
        public IEnumerable<ReviewViewModel> Reviews { get; set; } = Enumerable.Empty<ReviewViewModel>();
        //Pagination
        public int PageNumber { get; set; } = 1;   // trang hiện tại
        public int PageSize { get; set; } = 10;    // số item mỗi trang
        public int TotalItems { get; set; }        // tổng số sản phẩm
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
