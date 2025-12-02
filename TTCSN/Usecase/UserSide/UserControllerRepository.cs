
using TTCSN.Entities;
using TTCSN.Entities.Enum;

namespace TTCSN.Usecase.UserSide
{
    public class UserControllerRepository
    {
        private readonly IUserControllerRepository repo;
        public UserControllerRepository(IUserControllerRepository repository)
        {
            repo = repository;
        }
        public Task<bool> RegisterUserAsync(string accountName, string password)
        {
         return repo.RegisterUserAsync(accountName, password);
        }

        public Task<User?> ValidateUserAsync(string accountName, string password)
        {
            return repo.ValidateUserAsync(accountName, password);
        }
        public Task<bool> CheckUserExists(string accountName)
        {
            return repo.CheckUserExists(accountName);
        }
        //public Task<bool> CheckAccountUser(string accountName)
        //{
        //    return repo.CheckAccountUser(accountName);
        //}
        public Task<bool> CheckNewPassword(string accountName, string newPassword)
        {
            return repo.CheckNewPassword(accountName, newPassword);
        }
        public Task<bool> SavePassword(string accountName, string newPassword)
        {
            return repo.SavePassword(accountName, newPassword);
        }
        public Task<User?> GetInfoUser(int userId)
        {
            return repo.GetInfoUser(userId);
        }
        public Task<bool> UpdateUserInfo(int userId, string? fullName, string? email, string? phoneNumber, string? address)
        {
            return repo.UpdateUserInfo(userId, fullName, email, phoneNumber, address);
        }
        public Task<string?> GetUserNameAsync(int userId)
        {
            return repo.GetUserNameAsync(userId);
        }
        public Task<IEnumerable<User>> GetUsersAsync(string? searchName,
            int? userId,
            bool? isActive,
            string? sortBy,
            bool sortDescending,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber = 1,
            int pageSize = 5)
        {
            return repo.GetUsersAsync(searchName, userId, isActive, sortBy, sortDescending, fromDate, toDate, pageNumber, pageSize);
        }
        public Task<int> GetUsersCountAsync(string? searchName,
            int? userId,
            bool? isActive,
            DateTime? fromDate,
            DateTime? toDate)
        {
            return repo.GetUsersCountAsync(searchName, userId, isActive, fromDate, toDate);
        }
        public Task<bool> DeleteUserAsync(int userId)
        {
            return repo.DeleteUserAsync(userId);
        }
        public Task<bool> UpdateActiveAsync(int userId, bool isActive)
        {
            return repo.UpdateActiveAsync(userId, isActive);
        }
        public Task<bool> UpdateRoleAsync(int userId, UserRole role)
        {
            return repo.UpdateRoleAsync(userId, role);
        }
        public Task<bool> CheckActive(string userName, string password)
        {
            return repo.CheckActive(userName, password);
        }
    }
}
