using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TTCSN.Entities;
using TTCSN.Entities.Enum;
using TTCSN.Models;
using TTCSN.Services;
using TTCSN.Usecase.UserSide;
namespace TTCSN.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserControllerRepository _userRepo;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        public AccountController(UserControllerRepository userRepo, 
            ILogger<AccountController> logger,
            IEmailService email,
            IOtpService otp)
        {
            _emailService = email;
            _otpService = otp;
            _userRepo = userRepo;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel userView)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userRepo.ValidateUserAsync(userView.AccountName, userView.Password);
            var checkActive = await _userRepo.CheckActive(userView.AccountName, userView.Password);
            if (!checkActive)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa vui lòng liên hệ lại cho bên hỗ trợ để biết thêm thông tin.");
                return View();
            }
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("AccountName", user.AccountName),
                    new Claim(ClaimTypes.Name, user.FullName ?? user.AccountName ?? "User"),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("Address", user.Address ?? "")
                };
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Thông tin tài khoản hoặc mật khẩu không chính xác.");
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel userView)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var isexit = await _userRepo.CheckUserExists(userView.AccountName);
            if (isexit)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản đã tồn tại");
                return View();
            }
            if (userView.Password != userView.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp.");
                return View();
            }
            var success = await _userRepo.RegisterUserAsync(userView.AccountName, userView.Password);
            if (success)
            {
                TempData["Success"] = "Đăng ký thành công!";
                return RedirectToAction("Login", "Account");
            }
            ModelState.AddModelError(string.Empty, "Đăng ký không thành công. Vui lòng thử lại.");
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public IActionResult Profile()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var model = new UserProfileViewModel
            {
                Id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                AccountName = User.FindFirstValue("AccountName") ?? "",
                Role = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role) ?? "User"),
                FullName = User.Identity?.Name ?? "",
                Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
                PhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone) ?? "",
                Address = User.FindFirstValue("Address") ?? ""
            };

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }
            _logger.LogInformation("Updating user info for user ID: {UserId}", model.Id);
            _logger.LogInformation("New FullName: {FullName}, Email: {Email}, PhoneNumber: {PhoneNumber}, Address: {Address}",
                model.FullName, model.Email, model.PhoneNumber, model.Address);
            var update = await _userRepo.UpdateUserInfo(model.Id, model.FullName, model.Email, model.PhoneNumber, model.Address);
            if (update)
            {
                var user = await _userRepo.GetInfoUser(model.Id);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString() ?? "0"),
                    new Claim(ClaimTypes.Name, user?.FullName ?? user?.Email ?? user?.PhoneNumber ?? "User"),
                    new Claim(ClaimTypes.Role, user?.Role.ToString() ?? "User"),
                    new Claim(ClaimTypes.MobilePhone, user?.PhoneNumber ?? ""),
                    new Claim(ClaimTypes.Email, user?.Email ?? ""),
                    new Claim("Address", user?.Address ?? "")

                };

                // Xóa cookie cũ
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Tạo principal mới
                var principal = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                );

                // Ghi cookie mới
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            ModelState.AddModelError(string.Empty, "Cập nhật không thành công. Vui lòng thử lại.");
            return View("Profile", model);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string accountName)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (string.IsNullOrEmpty(accountName))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập email hoặc số điện thoại.");
                return View();
            }
            var isCheck = await _userRepo.CheckUserExists(accountName);
            if (!isCheck)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại.");
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, accountName)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            var otp = _otpService.GenerateOtp();
            _otpService.StoreOtp(accountName, otp);
            await _emailService.SendOtpEmailAsync(accountName, otp);
            HttpContext.Session.SetString("accountname", accountName);
            TempData["Success"] = "Gửi otp thành công!";
            return RedirectToAction("ForgotPasswordConfirmation");

        }
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPasswordConfirmation(string? otp)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (string.IsNullOrEmpty(otp))
            {
                ModelState.AddModelError(string.Empty, "Mã OTP không hợp lệ.");
                return View();
            }
            var user = HttpContext.Session.GetString("accountname");
            if (string.IsNullOrEmpty(user))
            {
                _logger.LogInformation("Loi deo tim thay tai khoan luu trong session");
                return View();
            }
            var isValid = _otpService.ValidateOtp(user, otp);
            if (!isValid)
            {
                TempData["Error"] = "Otp không đúng hoặc đã hết hạn! Vui lòng thử lại sau";
                return View();
            }
            HttpContext.Session.Clear();
            return RedirectToAction("ChangePassword");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string newPassword, string confirmNewPassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập mật khẩu mới và xác nhận mật khẩu.");
                return View();
            }
            if (newPassword != confirmNewPassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp.");
                return View();
            }
            var check = await _userRepo.CheckNewPassword(User.Identity?.Name ?? "", newPassword);
            if (check)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu mới không được trùng với mật khẩu cũ.");
                return View();
            }
            var save = await _userRepo.SavePassword(User.Identity?.Name ?? "", newPassword);
            if (!save)
            {
                ModelState.AddModelError(string.Empty, "Đổi mật khẩu không thành công. Vui lòng thử lại.");
                return View();
            }
            TempData["Success"] = "Thay đổi mật khẩu thành công!";
            return RedirectToAction("Login", "Account");
        }
    }
}
