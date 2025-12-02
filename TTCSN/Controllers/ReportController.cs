using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTCSN.Models.Report;
using TTCSN.Usecase.AdminSide.Report;


namespace TTCSN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ReportControllerRepository _reportRepository;

        public ReportController(ReportControllerRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<IActionResult> Revenue(int? year, int? month, DateTime? fromDate, DateTime? toDate)
        {
            // Mặc định là năm hiện tại nếu không chọn
            var selectedYear = year ?? DateTime.Now.Year;

            var model = new RevenueReportViewModel
            {
                Year = selectedYear,
                Month = month,
                FromDate = fromDate,
                ToDate = toDate
            };

            // Lấy tổng doanh thu
            model.TotalRevenue = await _reportRepository.GetTotalRevenueAsync(fromDate, toDate);
            model.TotalOrders = await _reportRepository.GetTotalOrdersAsync(fromDate, toDate);
            model.AverageOrderValue = model.TotalOrders > 0
                ? model.TotalRevenue / model.TotalOrders
                : 0;

            // Lấy dữ liệu theo tháng
            model.MonthlyData = await _reportRepository.GetMonthlyRevenueAsync(selectedYear);

            // Lấy dữ liệu theo năm
            model.YearlyData = await _reportRepository.GetYearlyRevenueAsync();

            // Lấy top sản phẩm
            model.TopProducts = await _reportRepository.GetTopProductsAsync(year, month, 10);

            // Lấy danh sách năm
            model.AvailableYears = await _reportRepository.GetAvailableYearsAsync();

            return View(model);
        }
    }
}