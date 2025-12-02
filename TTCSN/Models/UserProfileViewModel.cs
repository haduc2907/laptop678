using TTCSN.Entities.Enum;

namespace TTCSN.Models
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        
    }
}
