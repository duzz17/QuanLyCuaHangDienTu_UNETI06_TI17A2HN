using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;
using Xunit;

namespace QuanLyCuaHangDienTu.Module2.Tests;

public class GioHangControllerTests
{
    [Fact]
    public async Task ThemVaoGio_Ajax_TraJsonVaKhongChuyenTrang()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        var product = TestSupport.Product("Điện thoại thử nghiệm", 1, stock: 5);
        context.SanPhams.Add(product);
        await context.SaveChangesAsync();
        var controller = CreateController(context, ajax: true);

        var result = Assert.IsType<JsonResult>(await controller.ThemVaoGio(product.MaSanPham, 2));
        var json = JsonDocument.Parse(JsonSerializer.Serialize(result.Value));

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(2, json.RootElement.GetProperty("cartCount").GetInt32());
        var cart = ReadCart(controller.HttpContext.Session);
        var item = Assert.Single(cart);
        Assert.Equal(product.MaSanPham, item.MaSanPham);
        Assert.Equal(2, item.SoLuong);
    }

    [Fact]
    public async Task ThemVaoGio_MuaNgay_ThemSanPhamVaChuyenDenDatHang()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        var product = TestSupport.Product("Điện thoại thử nghiệm", 1, stock: 5);
        context.SanPhams.Add(product);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = Assert.IsType<RedirectToActionResult>(
            await controller.ThemVaoGio(product.MaSanPham, 1, muaNgay: true));

        Assert.Equal("DatHang", result.ActionName);
        Assert.Equal("DonHang", result.ControllerName);
        Assert.Single(ReadCart(controller.HttpContext.Session));
    }

    [Fact]
    public async Task ThemVaoGio_Ajax_HetHangTraLoiKhongThanhCong()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        var product = TestSupport.Product("Điện thoại hết hàng", 1, stock: 0);
        context.SanPhams.Add(product);
        await context.SaveChangesAsync();
        var controller = CreateController(context, ajax: true);

        var result = Assert.IsType<BadRequestObjectResult>(
            await controller.ThemVaoGio(product.MaSanPham));

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Empty(ReadCart(controller.HttpContext.Session));
    }

    private static GioHangController CreateController(
        ApplicationDbContext context,
        bool ajax = false)
    {
        var httpContext = new DefaultHttpContext { Session = new TestSession() };
        httpContext.Session.SetString("VaiTro", "KhachHang");
        if (ajax)
            httpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        return new GioHangController(context)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };
    }

    private static List<CartItemViewModel> ReadCart(ISession session)
    {
        var json = session.GetString("GIO_HANG_SESSION");
        return string.IsNullOrEmpty(json)
            ? []
            : JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? [];
    }
}
