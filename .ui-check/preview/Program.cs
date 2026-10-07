using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../.."));
var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, WebRootPath = Path.Combine(projectRoot, "wwwroot") });
if (args.Contains("--verify-queries"))
{
    // SQL translation only: this context never opens a connection.
    using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer("Server=invalid-test-server;Database=UiQueryCheck;Integrated Security=True;Encrypt=False").Options);
    var start = DateTime.Today.AddDays(-29);
    var end = DateTime.Today.AddDays(1);
    var sql = new[]
    {
        context.DonHangs.Where(d => d.NgayDat >= start && d.NgayDat < end && d.TrangThai == "DaGiao")
            .GroupBy(d => d.NgayDat.Date).Select(g => new { Date = g.Key, Amount = g.Sum(d => d.TongTien) }).ToQueryString(),
        context.DonHangs.Where(d => d.NgayDat >= start && d.NgayDat < end)
            .GroupBy(d => d.TrangThai).Select(g => new { Status = g.Key, Count = g.Count() }).ToQueryString(),
        context.ChiTietDonHangs.Where(c => c.DonHang.TrangThai == "DaGiao" && c.DonHang.NgayDat >= start && c.DonHang.NgayDat < end)
            .GroupBy(c => new { c.MaSanPham, c.SanPham.TenSanPham, c.SanPham.HinhAnh })
            .OrderByDescending(g => g.Sum(c => c.SoLuong))
            .Select(g => new BestSellingProduct(g.Key.MaSanPham, g.Key.TenSanPham, g.Key.HinhAnh, g.Sum(c => c.SoLuong), g.Sum(c => c.SoLuong * c.DonGia))).Take(4).ToQueryString(),
        context.LoaiSanPhams.OrderBy(l => l.MaLoai)
            .Select(l => new StoreCategory(l.MaLoai, l.TenLoai, l.SanPhams.Count(s => s.TrangThai))).ToQueryString()
    };
    Console.WriteLine($"Verified {sql.Length} dashboard/storefront SQL translations without database access.");
    return;
}
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(projectRoot, ".ui-check", "keys"))).DisableAutomaticKeyGeneration().UseEphemeralDataProtectionProvider();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddControllersWithViews().AddApplicationPart(typeof(HomeController).Assembly);
var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllers();
app.Run();

// Isolated view fixtures: no connection to the project's database or real accounts.
[Route("preview")]
public class UiPreviewController : Controller
{
    private static readonly LoaiSanPham category = new() { MaLoai = 1, TenLoai = "Laptop & máy tính" };
    private static readonly List<SanPham> products = Enumerable.Range(1, 8).Select(i => new SanPham
    {
        MaSanPham = i, TenSanPham = i % 2 == 0 ? "Laptop sáng tạo Pro 14 OLED" : "Laptop văn phòng Air 13 mỏng nhẹ",
        ThuongHieu = i % 2 == 0 ? "UNETI Pro" : "UNETI Air", Gia = 14990000 + i * 1500000,
        SoLuong = i % 3 == 0 ? 0 : i + 2, TrangThai = true, MaLoai = 1, LoaiSanPham = category,
        ThoiGianBaoHanh = 12, HinhAnh = "/images/product-placeholder.svg", MoTa = "Thiết bị dành cho công việc và sáng tạo. Thiết kế gọn nhẹ, màn hình rõ nét và hiệu năng phù hợp với nhu cầu mỗi ngày."
    }).ToList();
    private static readonly KhachHang customer = new() { MaKhachHang = 1, HoTen = "Khách kiểm thử", Email = "test@example.invalid", SoDienThoai = "0912345678", DiaChi = "Địa chỉ kiểm thử, Hà Nội" };
    private static readonly List<DonHang> orders = Enumerable.Range(1, 6).Select(i => new DonHang
    {
        MaDonHang = i, NgayDat = DateTime.Today.AddDays(-i), TongTien = 16490000 + i * 1000000,
        KhachHang = customer, MaKhachHang = 1, TrangThai = new[] { "ChoXacNhan", "DangGiao", "DaGiao", "DaHuy" }[i % 4],
        DiaChiGiaoHang = customer.DiaChi, SoDienThoaiGiaoHang = customer.SoDienThoai,
        ChiTietDonHangs = [new() { SanPham = products[i - 1], SoLuong = 1, DonGia = products[i - 1].Gia }]
    }).ToList();
    private void SessionFor(string role)
    {
        HttpContext.Session.SetString("VaiTro", role);
        HttpContext.Session.SetString("HoTen", role == "Admin" ? "Quản trị kiểm thử" : customer.HoTen);
    }
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        SessionFor("Admin");
        ViewData["Storefront"] = false;
        var empty = HttpContext.Request.Query["empty"] == "true";
        var model = new DashboardViewModel
        {
            Days = 30, Revenue = empty ? 0 : 186750000, OrderCount = empty ? 0 : 28,
            CustomerCount = empty ? 0 : 142, ProductCount = empty ? 0 : 25, StockCount = empty ? 0 : 470,
            LowStockCount = empty ? 0 : 3,
            RevenuePoints = Enumerable.Range(0, 30).Select(i => new RevenuePoint(DateTime.Today.AddDays(i - 29), empty ? 0 : (i % 7 + 1) * 1850000)).ToList(),
            Statuses = [new("ChoXacNhan", "Chờ xác nhận", "pending", empty ? 0 : 6), new("DangGiao", "Đang giao hàng", "shipping", empty ? 0 : 7), new("DaGiao", "Hoàn thành", "completed", empty ? 0 : 12), new("DaHuy", "Đã hủy", "cancelled", empty ? 0 : 3)],
            RecentOrders = empty ? [] : orders,
            LowStockProducts = empty ? [] : products.Where(p => p.SoLuong <= 5).ToList(),
            BestSellers = empty ? [] : products.Take(4).Select(p => new BestSellingProduct(p.MaSanPham, p.TenSanPham, p.HinhAnh, 8, p.Gia * 8)).ToList()
        };
        return View("~/Views/Dashboard/Index.cshtml", model);
    }
    [HttpGet("checkout")]
    public IActionResult Checkout()
    {
        SessionFor("KhachHang");
        return View("~/Views/DonHang/DatHang.cshtml", new DatHangViewModel { HoTen = customer.HoTen, SoDienThoai = customer.SoDienThoai, Email = customer.Email, DiaChiGiaoHang = customer.DiaChi, GioHang = new() { Items = products.Take(2).Select(p => new CartItemViewModel { MaSanPham = p.MaSanPham, TenSanPham = p.TenSanPham, HinhAnh = p.HinhAnh, Gia = p.Gia, SoLuong = 1 }).ToList() } });
    }
    [HttpGet("products")]
    public IActionResult Products()
    {
        SessionFor("Admin");
        ViewBag.CurrentPage = 1; ViewBag.TotalPages = 2; ViewBag.PageSize = 6; ViewBag.TotalItems = 8;
        ViewBag.FromItem = 1; ViewBag.ToItem = 6;
        ViewBag.MaLoaiList = new SelectList(new[] { category }, "MaLoai", "TenLoai");
        ViewBag.ThuongHieuList = new[] { "UNETI Pro", "UNETI Air" };
        return View("~/Views/SanPham/Index.cshtml", products.Take(6));
    }
    [HttpGet("orders")]
    public IActionResult Orders()
    {
        SessionFor("Admin");
        return View("~/Views/DonHang/Index.cshtml", orders);
    }
    [HttpGet("order")]
    public IActionResult Order()
    {
        SessionFor(HttpContext.Request.Query["customer"] == "true" ? "KhachHang" : "Admin");
        return View("~/Views/DonHang/Details.cshtml", orders[3]);
    }
    [HttpGet("category")]
    public IActionResult Category()
    {
        SessionFor("Admin");
        return View("~/Views/LoaiSanPham/Index.cshtml", new[] { category });
    }
    [HttpGet("customers")]
    public IActionResult Customers()
    {
        SessionFor("Admin");
        return View("~/Views/KhachHang/Index.cshtml", new[] { customer });
    }
    [HttpGet("create-product")]
    public IActionResult CreateProduct()
    {
        SessionFor("Admin");
        ViewData["MaLoai"] = new SelectList(new[] { category }, "MaLoai", "TenLoai");
        return View("~/Views/SanPham/Create.cshtml", new SanPham());
    }
}
