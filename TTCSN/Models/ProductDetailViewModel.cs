using TTCSN.Models.Review;

namespace TTCSN.Models
{
    public class ProductDetailViewModel
    {
        public IEnumerable<ProductViewModel> RelatedProducts { get; set; } = Enumerable.Empty<ProductViewModel>();
        public IEnumerable<ReviewViewModel> Reviews { get; set; } = Enumerable.Empty<ReviewViewModel>();
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Brand { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public int ReviewPageNumber { get; set; } = 1;
        public int ReviewPageSize { get; set; } = 5;
        public int ReviewTotalItems { get; set; }
        public int ReviewTotalPages => (int)Math.Ceiling((double)ReviewTotalItems / ReviewPageSize);
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Current user ID để so sánh - THÊM MỚI
        public int? CurrentUserId { get; set; }

    }
}
