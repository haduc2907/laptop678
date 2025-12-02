using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTCSN.Models;
using TTCSN.Usecase.AdminSide;
using TTCSN.Usecase.UserSide;

namespace TTCSN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CategoryControllerRepository _categoryController;
        private readonly ProductControllerRepository _productController; 
        private readonly UserControllerRepository _userController;

        public HomeController(CategoryControllerRepository categoryController, ProductControllerRepository productController,
            UserControllerRepository userController, ILogger<HomeController> logger)
        {
            _categoryController = categoryController;
            _productController = productController;
            _userController = userController;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchQuery,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            bool sortDescending = false,
            int pageNumber = 1,
            int pageSize = 9)
        {
            // L?y danh sách s?n ph?m ?ã l?c + s?p x?p + phân trang
            var list = await _productController.GetProductsAsync(
                searchQuery,
                categoryId,
                minPrice,
                maxPrice,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize
            );

            // ??m t?ng s? item (cho phân trang)
            var totalItems = await _productController.CountProductsAsync(
                searchQuery,
                categoryId,
                minPrice,
                maxPrice
            );
            var listCategories = await _categoryController.GetCategories();
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

                // G?i l?i các tham s? ?? View gi? tr?ng thái
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
        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
