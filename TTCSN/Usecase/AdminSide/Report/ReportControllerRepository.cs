using TTCSN.Models.Report;
using static TTCSN.Models.Report.RevenueReportViewModel;

namespace TTCSN.Usecase.AdminSide.Report
{
    public class ReportControllerRepository
    {
        private readonly IReportController reportController;
        public ReportControllerRepository(IReportController reportController)
        {
            this.reportController = reportController;
        }
        public Task<decimal> GetTotalRevenueAsync(DateTime? fromDate, DateTime? toDate)
        {
            return reportController.GetTotalRevenueAsync(fromDate, toDate);
        }
        public Task<int> GetTotalOrdersAsync(DateTime? fromDate, DateTime? toDate)
        {
            return reportController.GetTotalOrdersAsync(fromDate, toDate);
        }
         
         // Báo cáo theo tháng
        public Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync(int year)
        {
            return reportController.GetMonthlyRevenueAsync(year);
        }
         
         // Báo cáo theo năm
        public Task<List<YearlyRevenueData>> GetYearlyRevenueAsync()
        {
            return reportController.GetYearlyRevenueAsync();
        }
         
         // Top sản phẩm
        public Task<List<TopProductData>> GetTopProductsAsync(int? year, int? month, int topCount = 10)
        {
            return reportController.GetTopProductsAsync(year, month, topCount);
        }
         
         // Lấy danh sách năm có dữ liệu
        public Task<List<int>> GetAvailableYearsAsync()
        {
            return reportController.GetAvailableYearsAsync();
        }
    }    
}
