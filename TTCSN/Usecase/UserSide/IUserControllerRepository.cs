using System.Diagnostics.Eventing.Reader;
using TTCSN.Entities;
using TTCSN.Entities.Enum;
namespace TTCSN.Usecase.UserSide
{
    public interface IUserControllerRepository
    {
        Task<User?> ValidateUserAsync(string accountName, string password);
        Task<bool> RegisterUserAsync(string accountName, string password);
        Task<bool> CheckUserExists(string accountName);
        //Task<bool> CheckAccountUser(string accountName);
        Task<bool> CheckNewPassword(string accountName, string newPassword);
        Task<bool> SavePassword(string accountName, string newPassword);
        Task<User?> GetInfoUser(int userId);
        Task<bool> UpdateUserInfo(int userId, string? fullName, string? email, string? phoneNumber, string? address);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdateActiveAsync(int userId, bool isActive);
        Task<bool> UpdateRoleAsync(int userId, UserRole role);
        Task<bool> CheckActive(string userName, string password);
        Task<string?>GetUserNameAsync(int userId);
        Task<IEnumerable<User>> GetUsersAsync(string? searchName,
            int? userId,
            bool? isActive,
            string? sortBy,
            bool sortDescending,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber = 1,
            int pageSize = 5);
        Task<int> GetUsersCountAsync(string? searchName,
            int? userId,
            bool? isActive,
            DateTime? fromDate,
            DateTime? toDate);
    }
}
