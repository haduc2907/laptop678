using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TTCSN.Entities;
using TTCSN.Entities.Enum;
using TTCSN.Models.Review;
using TTCSN.Usecase.AdminSide;
using TTCSN.Usecase.AdminSide.Review;
using TTCSN.Usecase.UserSide;

namespace TTCSN.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly OrderDetailControllerRepository _orderDetailController;
        private readonly OrderControllerRepository _orderController;
        private readonly UserControllerRepository _userController;
        private readonly ILogger<ReviewController> _logger;
        private readonly ReviewControllerRepository _reviewController;
        
        public ReviewController(ILogger<ReviewController> logger,
            ReviewControllerRepository reviewController,
            UserControllerRepository userController,
            OrderControllerRepository orderController,
            OrderDetailControllerRepository orderDetailController)
        {
            _orderDetailController = orderDetailController;
            _orderController = orderController;
            _userController = userController;
            _logger = logger;
            _reviewController = reviewController;
        }
        public async Task<IActionResult> DeleteReview(int reviewId, int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _reviewController.DeleteReviewAsync(reviewId);
            if (result)
            {
                _logger.LogInformation($"Review {reviewId} deleted successfully.");
            }
            else
            {
                _logger.LogWarning($"Failed to delete review {reviewId}.");
            }
            return RedirectToAction("ProductDetail", "Product", new {Id = productId});
        }
        public async Task<IActionResult> AddReview(Entities.Review review, int productId, int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            review.UserId = userId;
            var result = await _reviewController.AddReviewAsync(review);
            if (result)
            {
                _logger.LogInformation($"Review for product {review.ProductId} added successfully.");
            }
            else
            {
                _logger.LogWarning($"Failed to add review for product {review.ProductId}.");
            }
            var check = await _orderDetailController.UpdateIsReviewed(orderId, productId);
            if (check)
            {
                _logger.LogInformation($"OrderDetail for Order {orderId} and Product {productId} marked as reviewed.");
            }
            else
            {
                _logger.LogWarning($"Failed to mark OrderDetail for Order {orderId} and Product {productId} as reviewed.");
            }
            return RedirectToAction("ProductDetail", "Product", new { Id = productId });
        }
        public async Task<IActionResult> UpdateReview(int reviewId, string? content, int rating, int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _reviewController.UpdateReviewAsync(reviewId, content, rating);
            if (result)
            {
                _logger.LogInformation($"Review {reviewId} updated successfully.");
            }
            else
            {
                _logger.LogWarning($"Failed to update review {reviewId}.");
            }
            return RedirectToAction("ProductDetail", "Product", new { Id = productId });
        }
    }
}
