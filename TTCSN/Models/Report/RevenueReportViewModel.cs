namespace TTCSN.Models.Report
{
    public class RevenueReportViewModel
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Tổng quan
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        // Dữ liệu theo tháng
        public List<MonthlyRevenueData> MonthlyData { get; set; } = new();

        // Dữ liệu theo năm
        public List<YearlyRevenueData> YearlyData { get; set; } = new();

        // Top sản phẩm bán chạy
        public List<TopProductData> TopProducts { get; set; } = new();

        // Danh sách năm để chọn
        public List<int> AvailableYears { get; set; } = new();
    }

    // Dữ liệu theo tháng
    public class MonthlyRevenueData
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string MonthName { get; set; } = string.Empty; // "Tháng 1", "Tháng 2"...
            public decimal Revenue { get; set; }
            public int OrderCount { get; set; }
            public decimal AverageOrderValue { get; set; }
        }

    public class YearlyRevenueData
    {
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal Growth { get; set; } // % tăng trưởng so với năm trước
    }

    public class TopProductData
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}
