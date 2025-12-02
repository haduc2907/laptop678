using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TTCSN.Entities;
using TTCSN.Models;
using TTCSN.Models.Review;
using TTCSN.Usecase.AdminSide;
using TTCSN.Usecase.AdminSide.Review;
using TTCSN.Usecase.UserSide;

namespace TTCSN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductControllerRepository _productRepository;
        private readonly CategoryControllerRepository _categoryRepository;
        private readonly ReviewControllerRepository _reviewController;
        private readonly UserControllerRepository _userRepository;
        private readonly ILogger<ProductController> _logger;
        public ProductController(UserControllerRepository userRepository,
            CategoryControllerRepository controllerRepository,
            ProductControllerRepository productRepository,
            ILogger<ProductController> logger,
            ReviewControllerRepository reviewController)
        {
            _reviewController = reviewController;
            _userRepository = userRepository;
            _categoryRepository = controllerRepository;
            _productRepository = productRepository;
            _logger = logger;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var list = await _productRepository.GetProducts();
        //    var products = new ProductListViewModel
        //    {
        //        Products = list.Select(p => new ProductViewModel
        //        {
        //            Id = p.Id,
        //            Name = p.Name,
        //            Description = p.Description,
        //            Price = p.Price,
        //            StockQuantity = p.StockQuantity,
        //            Brand = p.Brand,
        //            CategoryId = p.CategoryId,
        //            CreatedAt = p.CreatedAt,
        //            UserId = p.UserId,
        //            ImageUrl = p.ImageUrl
        //        }).ToList()
        //    };
        //    return View(products);
        //}
        public async Task<IActionResult> Index(
            string? searchQuery,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            bool sortDescending = false,
            int pageNumber = 1,
            int pageSize = 5)
        {
            // Lấy danh sách sản phẩm đã lọc + sắp xếp + phân trang
            var list = await _productRepository.GetProductsAsync(
                searchQuery,
                categoryId,
                minPrice,
                maxPrice,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize
            );

            // Đếm tổng số item (cho phân trang)
            var totalItems = await _productRepository.CountProductsAsync(
                searchQuery,
                categoryId,
                minPrice,
                maxPrice
            );
            var listCategories = await _categoryRepository.GetCategories();
            var viewModel = new ProductListViewModel
            {
                Products = list.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    CreatedAt = p.CreatedAt,
                    UserId = p.UserId,
                    ImageUrl = p.ImageUrl
                }).ToList(),

                // Gửi lại các tham số để View giữ trạng thái
                SearchQuery = searchQuery,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                SortDescending = sortDescending,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,

                // Cho dropdown category
                Categories = listCategories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Addproduct()
        {
            var list = await _categoryRepository.GetCategories();
            var categories = new CategoryListViewModel
            {
                Categories = list.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };
            ViewBag.Categories = categories.Categories;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Addproduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    Brand = model.Brand,
                    CategoryId = model.CategoryId,
                    CreatedAt = DateTime.UtcNow,
                    UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                    ImageUrl = model.ImageUrl
                };
                _logger.LogInformation("Adding product: {@Product}, UserId: {@UserId}", product, product.UserId);
                var result = await _productRepository.AddProductAsync(product);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Thêm sản phẩm không thành công");
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int Id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var product = await _productRepository.GetProductById(Id);
            if (product == null)
            {
                return NotFound();
            }
            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                UserId = product.UserId,
                ImageUrl = product.ImageUrl
            };
            var user = await _userRepository.GetInfoUser(model.UserId);
            var categoriesList = await _categoryRepository.GetCategories();
            ViewBag.userName = user?.FullName ?? "NoName";
            ViewBag.Categories = categoriesList;
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    Brand = model.Brand,
                    CategoryId = model.CategoryId,
                    CreatedAt = model.CreatedAt,
                    UserId = model.UserId,
                    ImageUrl = model.ImageUrl
                };
                var result = await _productRepository.UpdateProductAsync(product);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Cập nhật sản phẩm không thành công");
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var result = await _productRepository.DeleteProductAsync(Id);
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Xóa sản phẩm không thành công");
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProductDetail(int Id, int pageNumber = 1, int pageSize = 5)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var product = await _productRepository.GetProductById(Id);
            if (product == null)
            {
                return NotFound();
            }
            var relatedProducts = await _productRepository.GetProductsAsync(
                searchQuery: null,
                categoryId: product.CategoryId,
                minPrice: null,
                maxPrice: null,
                sortBy: null,
                sortDescending: false,
                pageNumber: 1,
                pageSize: 4
            );
            var list = await _reviewController.GetReviewsAsync(Id, pageNumber, pageSize);
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var averageRating = await _reviewController.GetAverageRatingAsync(Id); // THÊM MỚI
            var totalReviews = await _reviewController.CountReviewsAsync(Id);
            var reviewItems = new ReviewListViewModel()
            {
                Reviews = await Task.WhenAll(list.Select(async r =>
                {
                    var name = await _userRepository.GetUserNameAsync(r.UserId) ?? "NoName";
                    return new ReviewViewModel
                    {
                        Id = r.Id,
                        UserName = name,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt
                    };
                })),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalReviews
            };
            var model = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                RelatedProducts = relatedProducts
                .Where(p => p.Id != Id)
                .Take(4)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl
                }).ToList(),
                Reviews = reviewItems.Reviews,
                ReviewPageNumber = reviewItems.PageNumber,
                ReviewPageSize = reviewItems.PageSize,
                ReviewTotalItems = reviewItems.TotalItems,
                AverageRating = averageRating, // THÊM MỚI
                TotalReviews = totalReviews, // THÊM MỚI
                CurrentUserId = currentUserId
            };
            var category = await _categoryRepository.GetCategory(product.CategoryId);
            ViewBag.Category = category;
            return View(model);
        }
    }
}
