using TTCSN.Models.Report;
using static TTCSN.Models.Report.RevenueReportViewModel;

namespace TTCSN.Usecase.AdminSide.Report
{
    public interface IReportController
    {
        Task<decimal> GetTotalRevenueAsync(DateTime? fromDate, DateTime? toDate);
        Task<int> GetTotalOrdersAsync(DateTime? fromDate, DateTime? toDate);

        // Báo cáo theo tháng
        Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync(int year);

        // Báo cáo theo năm
        Task<List<YearlyRevenueData>> GetYearlyRevenueAsync();

        // Top sản phẩm
        Task<List<TopProductData>> GetTopProductsAsync(int? year, int? month, int topCount = 10);

        // Lấy danh sách năm có dữ liệu
        Task<List<int>> GetAvailableYearsAsync();
    }
}
