using TTCSN.Models.Report;
using TTCSN.Usecase.AdminSide.Report;
using static TTCSN.Models.Report.RevenueReportViewModel;

namespace TTCSN.Infrastructure.Sql
{
    public class SqlReportControllerRepository : IReportController
    {
        private readonly string? conn;

        public SqlReportControllerRepository(IConfiguration config)
        {
            conn = config.GetConnectionString("DefaultConnection");
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate, DateTime? toDate)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = @"
                SELECT ISNULL(SUM(TotalAmount), 0)
                FROM Orders
                WHERE Status = 3";

            if (fromDate.HasValue)
                query += " AND OrderDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND OrderDate <= @ToDate";

            await using var cmd = new SqlCommand(query, connection);

            if (fromDate.HasValue)
                cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

            if (toDate.HasValue)
                cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

            var result = await cmd.ExecuteScalarAsync();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        public async Task<int> GetTotalOrdersAsync(DateTime? fromDate, DateTime? toDate)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = @"
                SELECT COUNT(*)
                FROM Orders
                WHERE Status = 3";

            if (fromDate.HasValue)
                query += " AND OrderDate >= @FromDate";

            if (toDate.HasValue)
                query += " AND OrderDate <= @ToDate";

            await using var cmd = new SqlCommand(query, connection);

            if (fromDate.HasValue)
                cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);

            if (toDate.HasValue)
                cmd.Parameters.AddWithValue("@ToDate", toDate.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync(int year)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    YEAR(OrderDate) as Year,
                    MONTH(OrderDate) as Month,
                    ISNULL(SUM(TotalAmount), 0) as Revenue,
                    COUNT(*) as OrderCount
                FROM Orders
                WHERE YEAR(OrderDate) = @Year 
                    AND Status = 3
                GROUP BY YEAR(OrderDate), MONTH(OrderDate)
                ORDER BY Month";

            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Year", year);

            var result = new List<MonthlyRevenueData>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var month = reader.GetInt32(1);
                var revenue = reader.GetDecimal(2);
                var orderCount = reader.GetInt32(3);

                result.Add(new MonthlyRevenueData
                {
                    Year = reader.GetInt32(0),
                    Month = month,
                    MonthName = $"Tháng {month}",
                    Revenue = revenue,
                    OrderCount = orderCount,
                    AverageOrderValue = orderCount > 0 ? revenue / orderCount : 0
                });
            }

            // Thêm các tháng chưa có dữ liệu = 0
            for (int i = 1; i <= 12; i++)
            {
                if (!result.Any(x => x.Month == i))
                {
                    result.Add(new MonthlyRevenueData
                    {
                        Year = year,
                        Month = i,
                        MonthName = $"Tháng {i}",
                        Revenue = 0,
                        OrderCount = 0,
                        AverageOrderValue = 0
                    });
                }
            }

            return result.OrderBy(x => x.Month).ToList();
        }

        public async Task<List<YearlyRevenueData>> GetYearlyRevenueAsync()
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    YEAR(OrderDate) as Year,
                    ISNULL(SUM(TotalAmount), 0) as Revenue,
                    COUNT(*) as OrderCount
                FROM Orders
                WHERE Status = 3
                GROUP BY YEAR(OrderDate)
                ORDER BY Year DESC";

            await using var cmd = new SqlCommand(query, connection);
            var result = new List<YearlyRevenueData>();
            await using var reader = await cmd.ExecuteReaderAsync();

            YearlyRevenueData? previousYear = null;

            while (await reader.ReadAsync())
            {
                var current = new YearlyRevenueData
                {
                    Year = reader.GetInt32(0),
                    Revenue = reader.GetDecimal(1),
                    OrderCount = reader.GetInt32(2),
                    Growth = 0
                };

                // Tính % tăng trưởng so với năm trước
                if (previousYear != null && previousYear.Revenue > 0)
                {
                    current.Growth = ((current.Revenue - previousYear.Revenue) / previousYear.Revenue) * 100;
                }

                result.Add(current);
                previousYear = current;
            }

            return result.OrderBy(x => x.Year).ToList();
        }

        public async Task<List<TopProductData>> GetTopProductsAsync(int? year, int? month, int topCount = 10)
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = @"
                SELECT TOP (@TopCount)
                    p.Id,
                    p.Name,
                    SUM(od.Quantity) as TotalQuantity,
                    SUM(od.Quantity * od.UnitPrice) as TotalRevenue
                FROM OrderDetails od
                INNER JOIN Products p ON od.ProductId = p.Id
                INNER JOIN Orders o ON od.OrderId = o.Id
                WHERE o.Status = 3";

            if (year.HasValue)
                query += " AND YEAR(o.OrderDate) = @Year";

            if (month.HasValue)
                query += " AND MONTH(o.OrderDate) = @Month";

            query += @"
                GROUP BY p.Id, p.Name
                ORDER BY TotalRevenue DESC";

            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@TopCount", topCount);

            if (year.HasValue)
                cmd.Parameters.AddWithValue("@Year", year.Value);

            if (month.HasValue)
                cmd.Parameters.AddWithValue("@Month", month.Value);

            var result = new List<TopProductData>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new TopProductData
                {
                    ProductId = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    QuantitySold = reader.GetInt32(2),
                    Revenue = reader.GetDecimal(3)
                });
            }

            return result;
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            await using var connection = new SqlConnection(conn);
            await connection.OpenAsync();

            var query = @"
                SELECT DISTINCT YEAR(OrderDate) as Year
                FROM Orders
                WHERE Status = 3
                ORDER BY Year DESC";

            await using var cmd = new SqlCommand(query, connection);
            var result = new List<int>();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(reader.GetInt32(0));
            }

            // Nếu không có dữ liệu, thêm năm hiện tại
            if (!result.Any())
            {
                result.Add(DateTime.Now.Year);
            }

            return result;
        }
    }
}
