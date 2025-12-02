using System.ComponentModel.DataAnnotations;

namespace TTCSN.Models
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        public string AccountName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage ="Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
