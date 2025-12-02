namespace TTCSN.Usecase.AdminSide.Review
{
    public class ReviewControllerRepository
    {
        private readonly IReviewController repo;
        public ReviewControllerRepository(IReviewController repository)
        {
            repo = repository;
        }
        public Task<bool> DeleteReviewAsync(int reviewId)
        {
            return repo.DeleteReviewAsync(reviewId);
        }
        public Task<bool> UpdateReviewAsync(int reviewId, string? content, int rating)
        {
            return repo.UpdateReviewAsync(reviewId, content, rating);
        }
        public Task<IEnumerable<Entities.Review>> GetReviewsAsync(
            int productId,
            int pageNumber,
            int pageSize)
        {
            return repo.GetReviewsAsync(
                productId,
                pageNumber,
                pageSize);
        }
        public Task<bool> AddReviewAsync(Entities.Review review)
        {
            return repo.AddReviewAsync(review);
        }
        public Task<int> CountReviewsAsync(int productId)
        {
            return repo.CountReviewsAsync(productId);
        }
        public Task<double> GetAverageRatingAsync(int productId)
        {
            return repo.GetAverageRatingAsync(productId);
        }
    }
}
