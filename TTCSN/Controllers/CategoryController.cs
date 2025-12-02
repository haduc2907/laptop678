using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTCSN.Entities;
using TTCSN.Models;
using TTCSN.Usecase.AdminSide;

namespace TTCSN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly CategoryControllerRepository _categoryController;
        public CategoryController(CategoryControllerRepository categoryController, ILogger<CategoryController> logger)
        {
            _categoryController = categoryController;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _categoryController.GetCategories();
            var categories = new CategoryListViewModel()
            {
                Categories = list.Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
            };
            return View(categories);
        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(string Name)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError(string.Empty, "Tên không được để trống.");
                return RedirectToAction("Index");
            }
            var result = await _categoryController.AddCategoryAsync(Name);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Thêm danh mục không thành công.");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int categoryId)
        {
            Category? category = await _categoryController.GetCategory(categoryId);
            var categoryViewModel = new CategoryViewModel() 
            {
                Id = category?.Id ?? 0,
                Name = category?.Name ?? string.Empty
            };
            if (category == null)
            {
                return NotFound();
            }
            return View(categoryViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int categoryId, string categoryName)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ModelState.AddModelError(string.Empty, "Tên không được để trống.");
                return RedirectToAction("Index");
            }
            var result = await _categoryController.UpdateCategoryAsync(categoryId, categoryName);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Cập nhật danh mục không thành công.");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var result = await _categoryController.DeleteCategoryAsync(categoryId);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Xóa danh mục không thành công.");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Cancel()
        {
            return RedirectToAction("Index");
        }
    }
}
