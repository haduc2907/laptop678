using TTCSN.Entities;
namespace TTCSN.Usecase.AdminSide.Review
{
    public interface IReviewController
    {
        public Task<bool> DeleteReviewAsync(int reviewId);
        public Task<bool> UpdateReviewAsync(int reviewId, string? content, int rating);
        public Task<IEnumerable<Entities.Review>> GetReviewsAsync(
            int productId,
            int pageNumber,
            int pageSize);
        public Task<bool> AddReviewAsync(Entities.Review review);
        public Task<int> CountReviewsAsync(int productId);
        public Task<double> GetAverageRatingAsync(int productId);
    }
}
