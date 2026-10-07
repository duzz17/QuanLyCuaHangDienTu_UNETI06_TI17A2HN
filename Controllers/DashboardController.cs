using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

[AdminOnly]
public class DashboardController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(int days = 30)
    {
        days = days is 7 or 30 or 90 ? days : 30;
        var end = DateTime.Today.AddDays(1);
        var start = end.AddDays(-days);
        var orders = context.DonHangs.AsNoTracking()
            .Where(d => d.NgayDat >= start && d.NgayDat < end);
        var dailyRevenue = await orders.Where(d => d.TrangThai == "DaGiao")
            .GroupBy(d => d.NgayDat.Date)
            .Select(g => new { Date = g.Key, Amount = g.Sum(d => d.TongTien) })
            .ToDictionaryAsync(g => g.Date, g => g.Amount);
        var statusCounts = await orders.GroupBy(d => d.TrangThai)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count);
        var products = context.SanPhams.AsNoTracking().Where(s => s.TrangThai);
        var lowStock = products.Where(s => s.SoLuong <= 5);
        var bestSellers = await context.ChiTietDonHangs.AsNoTracking()
            .Where(c => c.DonHang.TrangThai == "DaGiao"
                && c.DonHang.NgayDat >= start && c.DonHang.NgayDat < end)
            .GroupBy(c => new { c.MaSanPham, c.SanPham.TenSanPham, c.SanPham.HinhAnh })
            .Select(g => new
            {
                Id = g.Key.MaSanPham,
                Name = g.Key.TenSanPham,
                Image = g.Key.HinhAnh,
                Quantity = g.Sum(c => c.SoLuong),
                Revenue = g.Sum(c => c.SoLuong * c.DonGia)
            })
            .OrderByDescending(p => p.Quantity).ThenBy(p => p.Id).Take(4).ToListAsync();
        var model = new DashboardViewModel
        {
            Days = days,
            Revenue = dailyRevenue.Values.Sum(),
            OrderCount = statusCounts.Values.Sum(),
            CustomerCount = await context.KhachHangs.CountAsync(),
            ProductCount = await products.CountAsync(),
            StockCount = await products.SumAsync(s => (int?)s.SoLuong) ?? 0,
            LowStockCount = await lowStock.CountAsync(),
            RecentOrders = await orders.Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayDat).ThenByDescending(d => d.MaDonHang)
                .Take(6).ToListAsync(),
            LowStockProducts = await lowStock.OrderBy(s => s.SoLuong)
                .ThenBy(s => s.TenSanPham).Take(5).ToListAsync(),
            RevenuePoints = Enumerable.Range(0, days)
                .Select(i => new RevenuePoint(start.AddDays(i), dailyRevenue.GetValueOrDefault(start.AddDays(i))))
                .ToList(),
            BestSellers = bestSellers.Select(p => new BestSellingProduct(p.Id, p.Name,
                p.Image ?? "", p.Quantity, p.Revenue)).ToList()
        };
        var statuses = new[]
        {
            ("ChoXacNhan", "Chờ xác nhận", "pending"),
            ("DaXacNhan", "Đã xác nhận", "neutral"),
            ("DangGiao", "Đang giao", "shipping"),
            ("DaGiao", "Đã giao", "completed"),
            ("DaHuy", "Đã hủy", "cancelled")
        };
        model.Statuses = statuses.Select(s => new OrderStatusSummary(s.Item1, s.Item2,
            s.Item3, statusCounts.GetValueOrDefault(s.Item1))).ToList();
        foreach (var status in statusCounts.Where(s => !statuses.Any(known => known.Item1 == s.Key)))
            model.Statuses.Add(new(status.Key, status.Key, "neutral", status.Value));
        return View(model);
    }
}
