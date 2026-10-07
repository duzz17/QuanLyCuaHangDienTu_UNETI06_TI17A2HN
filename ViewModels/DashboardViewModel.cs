using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class DashboardViewModel
{
    public int Days { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
    public int CustomerCount { get; set; }
    public int ProductCount { get; set; }
    public int StockCount { get; set; }
    public int LowStockCount { get; set; }
    public List<RevenuePoint> RevenuePoints { get; set; } = [];
    public List<OrderStatusSummary> Statuses { get; set; } = [];
    public List<DonHang> RecentOrders { get; set; } = [];
    public List<SanPham> LowStockProducts { get; set; } = [];
    public List<BestSellingProduct> BestSellers { get; set; } = [];
}

public record RevenuePoint(DateTime Date, decimal Amount);
public record OrderStatusSummary(string Status, string Label, string CssClass, int Count);
public record BestSellingProduct(int Id, string Name, string Image, int Quantity, decimal Revenue);
