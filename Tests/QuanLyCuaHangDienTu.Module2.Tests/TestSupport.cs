// Họ và tên: Phạm Văn Cường - MSSV: 23103100094
// Nội dung thực hiện: Hạ tầng dùng chung cho kiểm thử sản phẩm (Module 2)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu.Module2.Tests;

internal static class TestSupport
{
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public static SanPhamController CreateController(
        ApplicationDbContext context,
        bool isAdmin = false,
        string queryString = "")
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        if (isAdmin)
            httpContext.Session.SetString("VaiTro", "Admin");
        if (!string.IsNullOrEmpty(queryString))
            httpContext.Request.QueryString = new QueryString(queryString);

        var controller = new SanPhamController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };

        return controller;
    }

    public static SanPham Product(
        string name,
        int categoryId,
        string brand = "Test Brand",
        string origin = "Việt Nam",
        decimal price = 1_000_000m,
        int stock = 10,
        TrangThaiSanPham status = TrangThaiSanPham.DangKinhDoanh)
    {
        return new SanPham
        {
            TenSanPham = name,
            MaLoai = categoryId,
            ThuongHieu = brand,
            XuatXu = origin,
            Gia = price,
            SoLuong = stock,
            ThoiGianBaoHanh = 12,
            TrangThai = status
        };
    }
}

internal sealed class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _values = new(StringComparer.Ordinal);

    public bool IsAvailable => true;
    public string Id { get; } = Guid.NewGuid().ToString();
    public IEnumerable<string> Keys => _values.Keys;

    public void Clear() => _values.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _values.Remove(key);

    public void Set(string key, byte[] value) => _values[key] = value;

    public bool TryGetValue(string key, out byte[] value)
    {
        if (_values.TryGetValue(key, out var storedValue))
        {
            value = storedValue;
            return true;
        }

        value = Array.Empty<byte>();
        return false;
    }
}

internal sealed class TestTempDataProvider : ITempDataProvider
{
    public IDictionary<string, object> LoadTempData(HttpContext context) =>
        new Dictionary<string, object>();

    public void SaveTempData(HttpContext context, IDictionary<string, object> values)
    {
    }
}
