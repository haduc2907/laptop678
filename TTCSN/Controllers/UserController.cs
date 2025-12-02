using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTCSN.Entities.Enum;
using TTCSN.Models.User;
using TTCSN.Usecase.UserSide;

namespace TTCSN.Controllers
{
    public class UserController : Controller
    {
        private readonly UserControllerRepository _userControllerRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(UserControllerRepository userControllerRepository
            ,ILogger<UserController> logger)
        {
            _userControllerRepository = userControllerRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(
            string? searchName,
            int? userId,
            bool? isActive,
            string? sortBy,
            bool sortDescending = true,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var list = await _userControllerRepository.GetUsersAsync(
                searchName, userId, isActive, sortBy, sortDescending,
                fromDate, toDate, pageNumber, pageSize);

            var totalUser = await _userControllerRepository.GetUsersCountAsync(
                searchName, userId, isActive, fromDate, toDate);
            var userViews = new UserListViewModel()
            {
                Users = list.Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    AccountName = u.AccountName,
                    Address = u.Address,
                    CreatedAt = u.CreatedAt,
                    Email = u.Email,
                    FullName = u.FullName,
                    IsActive = u.IsActive,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role
                }).ToList(),
                IsActive = isActive,
                FromDate = fromDate,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchName = searchName,
                SortBy = sortBy,
                SortDescending = sortDescending,
                ToDate = toDate,
                UserId = userId,
                TotalItems = totalUser,
            };

            return View(userViews);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int userId, UserRole newRole)
        {
            if (userId <= 0)
            {
                TempData["ErrorMessage"] = "ID người dùng không hợp lệ";
                return RedirectToAction("Index");
            }

            try
            {
                var check = await _userControllerRepository.UpdateRoleAsync(userId, newRole);

                if (!check)
                {
                    TempData["ErrorMessage"] = "Không cập nhật được role người dùng";
                }
                else
                {
                    TempData["SuccessMessage"] = "Cập nhật vai trò thành công";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateActive(int userId, bool newIsActive)
        {
            if (userId <= 0)
            {
                TempData["ErrorMessage"] = "ID người dùng không hợp lệ";
                return RedirectToAction("Index");
            }
            _logger.LogInformation("FORM newIsActive = {v}", Request.Form["newIsActive"]);
            _logger.LogInformation("id {a}", userId);

            try
            {
                var check = await _userControllerRepository.UpdateActiveAsync(userId, newIsActive);

                if (!check)
                {
                    TempData["ErrorMessage"] = "Không cập nhật được trạng thái người dùng";
                }
                else
                {
                    TempData["SuccessMessage"] = newIsActive
                        ? "Đã kích hoạt tài khoản thành công"
                        : "Đã vô hiệu hóa tài khoản thành công";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int userId)
        {
            if (userId <= 0)
            {
                TempData["ErrorMessage"] = "ID người dùng không hợp lệ";
                return RedirectToAction("Index");
            }

            try
            {
                var check = await _userControllerRepository.DeleteUserAsync(userId);

                if (!check)
                {
                    TempData["ErrorMessage"] = "Xóa người dùng không thành công";
                }
                else
                {
                    TempData["SuccessMessage"] = "Đã xóa người dùng thành công";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}