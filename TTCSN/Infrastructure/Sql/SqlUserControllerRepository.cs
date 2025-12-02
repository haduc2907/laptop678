using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Numerics;
using System.Text.RegularExpressions;
using TTCSN.Entities;
using TTCSN.Entities.Enum;
using TTCSN.Usecase.UserSide;

namespace TTCSN.Infrastructure.Sql
{
    public class SqlUserControllerRepository : IUserControllerRepository
    {
        private readonly string? conn;
        public SqlUserControllerRepository(IConfiguration config)
        {
            conn = config.GetConnectionString("DefaultConnection");
        }
        private bool IsPhone(string input)
        {
            return Regex.IsMatch(input, @"^(0|\+84)[0-9]{9}$");
        }
        private bool IsEmail(string input)
        {
            return Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        public async Task<bool> RegisterUserAsync(string accountName, string password)
        {
            string? phone = null;
            string? email = null;
            if (IsPhone(accountName))
            {
                phone = accountName;
            }
            else if (IsEmail(accountName)){
                email = accountName;
            }
            else
            {
                return false;
            }
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"INSERT INTO Users (AccountName, Email, PhoneNumber, Password)
                  VALUES (@AccountName, @Email, @PhoneNumber, @Password)", connection);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Password", password);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<User?> ValidateUserAsync(string accountName, string password)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT Id, AccountName, Email, PhoneNumber, Role, FullName, Address, Password
                  FROM Users
                  WHERE AccountName = @AccountName AND Password = @Password", connection);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@Password", password);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    AccountName = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PhoneNumber = reader.IsDBNull(3) ? null :  reader.GetString(3),
                    Role = (UserRole)reader.GetInt32(4),
                    FullName = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Address = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Password = reader.GetString(7)
                };
            }
            return null;
        }

        public async Task<bool> CheckUserExists(string accountName)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT COUNT(*)
                  FROM Users
                  WHERE AccountName = @AccountName", connection);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return false;
            }
            var count = Convert.ToInt32(result);
            return count > 0;
        }

        //public async Task<bool> CheckAccountUser(string accountName)
        //{
        //    string? phone = null;
        //    string? email = null;
        //    if (IsPhone(accountName))
        //    {
        //        phone = accountName;
        //    }
        //    else if (IsEmail(accountName))
        //    {
        //        email = accountName;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //    await using var connection = new SqlConnection(conn);
        //    await connection.OpenAsync();
        //    await using var cmd = new SqlCommand(@"SELECT COUNT(*)
        //          FROM Users
        //          WHERE Email = @Email OR PhoneNumber = @PhoneNumber", connection);
        //    cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@PhoneNumber", (object?)phone ?? DBNull.Value);
        //    var result = await cmd.ExecuteScalarAsync();
        //    if (result == null || result == DBNull.Value)
        //    {
        //        return false;
        //    }
        //    var count = Convert.ToInt32(result);
        //    return count > 0;
        //}

        public async Task<bool> CheckNewPassword(string accountName, string newPassword)
        {

            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT COUNT(*)
                  FROM Users
                  WHERE AccountName = @AccountName AND Password = @Password", connection);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@Password", newPassword);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return false;
            }
            var count = Convert.ToInt32(result);
            return count > 0;
        }

        public async Task<bool> SavePassword(string accountName, string newPassword)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Users
                  SET Password = @Password
                  WHERE AccountName = @AccountName", connection);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@Password", newPassword);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<User?> GetInfoUser(int userId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"SELECT Id, FullName, Email, PhoneNumber, Address, IsActive, CreatedAt, Role, AccountName
                  FROM Users
                  WHERE Id = @UserId", connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PhoneNumber = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                    IsActive = reader.GetBoolean(5),
                    CreatedAt = reader.GetDateTime(6),
                    Role = (UserRole)reader.GetInt32(7),
                    AccountName = reader.GetString(8)
                };
            }
            return null;

        }

        public async Task<bool> UpdateUserInfo(int userId, string? fullName, string? email, string? phoneNumber, string? address)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            await using var cmd = new SqlCommand(@"UPDATE Users
                  SET FullName = @FullName,
                      Email = @Email,
                      PhoneNumber = @PhoneNumber,
                      Address = @Address
                  WHERE Id = @UserId", connection);
            cmd.Parameters.AddWithValue("@FullName", (object?)fullName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)phoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object?)address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserId", userId);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
        public async Task<string?> GetUserNameAsync(int userId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand(@"
        SELECT COALESCE(FullName, AccountName)
        FROM Users
        WHERE Id = @UserId
    ", connection);

            cmd.Parameters.AddWithValue("@UserId", userId);

            var result = await cmd.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? null : result.ToString();
        }
        public async Task<IEnumerable<User>> GetUsersAsync(
     string? searchName,
     int? userId,
     bool? isActive,
     string? sortBy,
     bool sortDescending,
     DateTime? fromDate,
     DateTime? toDate,
     int pageNumber = 1,
     int pageSize = 5)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            var users = new List<User>();

            var query = @"SELECT Id, AccountName, FullName, Email, PhoneNumber, Address, IsActive, CreatedAt, Role
                FROM Users
                WHERE 1=1";

            // Add search conditions
            if (!string.IsNullOrEmpty(searchName))
            {
                query += " AND (AccountName LIKE @SearchName OR FullName LIKE @SearchName OR Email LIKE @SearchName)";
            }

            if (userId.HasValue)
            {
                query += " AND Id = @UserId";
            }

            if (isActive.HasValue)
            {
                query += " AND IsActive = @IsActive";
            }

            if (fromDate.HasValue)
            {
                query += " AND CreatedAt >= @FromDate";
            }

            if (toDate.HasValue)
            {
                query += " AND CreatedAt <= @ToDate";
            }

            // Sort order
            var orderBy = sortBy switch
            {
                "AccountName" => "AccountName",
                "FullName" => "FullName",
                "Email" => "Email",
                "CreatedAt" => "CreatedAt",
                "Role" => "Role",
                _ => "Id"
            };
            var direction = sortDescending ? "DESC" : "ASC";
            query += $" ORDER BY {orderBy} {direction}";

            // Pagination
            query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            await using var cmd = new SqlCommand(query, connection);

            // Add parameters
            cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            if (!string.IsNullOrEmpty(searchName))
            {
                cmd.Parameters.AddWithValue("@SearchName", "%" + searchName + "%");
            }

            if (userId.HasValue)
            {
                cmd.Parameters.AddWithValue("@UserId", userId.Value);
            }

            if (isActive.HasValue)
            {
                cmd.Parameters.AddWithValue("@IsActive", isActive.Value);
            }

            if (fromDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);
            }

            if (toDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@ToDate", toDate.Value);
            }

            // Execute and read data
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt32(0),
                    AccountName = reader.GetString(1),
                    FullName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                    PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                    IsActive = reader.GetBoolean(6),
                    CreatedAt = reader.GetDateTime(7),
                    Role = (UserRole)reader.GetInt32(8)
                };
                users.Add(user);
            }

            return users;
        }

        // Thêm method để đếm tổng số users (cho pagination)
        public async Task<int> GetUsersCountAsync(
            string? searchName,
            int? userId,
            bool? isActive,
            DateTime? fromDate,
            DateTime? toDate)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM Users WHERE 1=1";

            if (!string.IsNullOrEmpty(searchName))
            {
                query += " AND (AccountName LIKE @SearchName OR FullName LIKE @SearchName OR Email LIKE @SearchName)";
            }

            if (userId.HasValue)
            {
                query += " AND Id = @UserId";
            }

            if (isActive.HasValue)
            {
                query += " AND IsActive = @IsActive";
            }

            if (fromDate.HasValue)
            {
                query += " AND CreatedAt >= @FromDate";
            }

            if (toDate.HasValue)
            {
                query += " AND CreatedAt <= @ToDate";
            }

            await using var cmd = new SqlCommand(query, connection);

            if (!string.IsNullOrEmpty(searchName))
            {
                cmd.Parameters.AddWithValue("@SearchName", "%" + searchName + "%");
            }

            if (userId.HasValue)
            {
                cmd.Parameters.AddWithValue("@UserId", userId.Value);
            }

            if (isActive.HasValue)
            {
                cmd.Parameters.AddWithValue("@IsActive", isActive.Value);
            }

            if (fromDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);
            }

            if (toDate.HasValue)
            {
                cmd.Parameters.AddWithValue("@ToDate", toDate.Value);
            }
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(result);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            // Xóa thật khỏi database
            var query = "DELETE FROM Users WHERE Id = @UserId";
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<bool> UpdateActiveAsync(int userId, bool isActive)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = "UPDATE Users SET IsActive = @IsActive WHERE Id = @UserId";
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@IsActive", isActive);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<bool> UpdateRoleAsync(int userId, UserRole newRole)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = "UPDATE Users SET Role = @Role WHERE Id = @UserId";
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Role", (int)newRole);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        public async Task<bool> CheckActive(string userName, string password)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();
            var query = "SELECT IsActive FROM Users WHERE AccountName = @AccountName AND Password = @Password";
            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@AccountName", userName);
            cmd.Parameters.AddWithValue ("Password", password);
            var result = await cmd.ExecuteScalarAsync();

            // Nếu không tìm thấy user hoặc result null -> return false
            if (result == null || result == DBNull.Value)
            {
                return false;
            }

            // Convert result sang bool
            return Convert.ToBoolean(result);
        }

     }
}

