using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using Xunit;

namespace QuanLyCuaHangDienTu.Module2.Tests;

public class SanPhamCrudTests
{
    [Fact]
    public async Task Create_ThemSanPhamThanhCong()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var product = TestSupport.Product(
            "  Điện thoại mới  ",
            1,
            "  Samsung  ",
            "  Hàn Quốc  ",
            12_000_000m,
            25,
            TrangThaiSanPham.DangKinhDoanh);

        var result = await controller.Create(product);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(controller.Index), redirect.ActionName);
        var saved = await context.SanPhams.SingleAsync();
        Assert.Equal("Điện thoại mới", saved.TenSanPham);
        Assert.Equal("Samsung", saved.ThuongHieu);
        Assert.Equal("Hàn Quốc", saved.XuatXu);
        Assert.Equal(12_000_000m, saved.Gia);
        Assert.Equal(25, saved.SoLuong);
        Assert.True(controller.TempData.ContainsKey("Success"));
    }

    [Fact]
    public async Task Details_DocDungSanPhamVaLoaiSanPham()
    {
        await using var context = TestSupport.CreateContext();
        var category = new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" };
        var product = TestSupport.Product("Điện thoại A", 1, "Samsung", "Hàn Quốc");
        context.AddRange(category, product);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await controller.Details(product.MaSanPham));

        var model = Assert.IsType<SanPham>(result.Model);
        Assert.Equal(product.MaSanPham, model.MaSanPham);
        Assert.Equal("Điện thoại A", model.TenSanPham);
        Assert.Equal("Điện thoại", model.LoaiSanPham?.TenLoai);
    }

    [Fact]
    public async Task Edit_CapNhatSanPhamThanhCong()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.AddRange(
            new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" },
            new LoaiSanPham { MaLoai = 2, TenLoai = "Laptop" });
        var existing = TestSupport.Product("Tên cũ", 1, "Hãng cũ", "Việt Nam", 100m, 1);
        context.SanPhams.Add(existing);
        await context.SaveChangesAsync();
        var id = existing.MaSanPham;
        context.ChangeTracker.Clear();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var submitted = TestSupport.Product(
            "  Tên mới  ",
            2,
            "  Hãng mới  ",
            "  Nhật Bản  ",
            250m,
            9,
            TrangThaiSanPham.TamNgung);
        submitted.MaSanPham = id;
        submitted.MoTa = "Mô tả mới";
        submitted.HinhAnh = "/images/new.jpg";
        submitted.ThoiGianBaoHanh = 24;
        submitted.NgayNhap = new DateTime(2026, 10, 10);

        var result = await controller.Edit(id, submitted);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(controller.Index), redirect.ActionName);
        context.ChangeTracker.Clear();
        var saved = await context.SanPhams.SingleAsync(product => product.MaSanPham == id);
        Assert.Equal("Tên mới", saved.TenSanPham);
        Assert.Equal("Hãng mới", saved.ThuongHieu);
        Assert.Equal("Nhật Bản", saved.XuatXu);
        Assert.Equal("Mô tả mới", saved.MoTa);
        Assert.Equal(250m, saved.Gia);
        Assert.Equal(9, saved.SoLuong);
        Assert.Equal(24, saved.ThoiGianBaoHanh);
        Assert.Equal(TrangThaiSanPham.TamNgung, saved.TrangThai);
        Assert.Equal("/images/new.jpg", saved.HinhAnh);
        Assert.Equal(new DateTime(2026, 10, 10), saved.NgayNhap);
        Assert.Equal(2, saved.MaLoai);
    }

    [Fact]
    public async Task Edit_KhongGhiDeNavigationNgoaiBind()
    {
        await using var context = TestSupport.CreateContext();
        var category = new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" };
        var existing = TestSupport.Product("Sản phẩm cũ", 1);
        context.AddRange(category, existing);
        await context.SaveChangesAsync();
        var id = existing.MaSanPham;
        context.ChangeTracker.Clear();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var submitted = TestSupport.Product("Sản phẩm mới", 1);
        submitted.MaSanPham = id;
        submitted.LoaiSanPham = new LoaiSanPham { MaLoai = 999, TenLoai = "Loại chèn thêm" };
        submitted.ChiTietDonHangs.Add(new ChiTietDonHang
        {
            MaSanPham = id,
            SoLuong = 99,
            DonGia = 1m,
            DonHang = new DonHang
            {
                DiaChiGiaoHang = "Địa chỉ chèn thêm",
                SoDienThoaiGiaoHang = "0900000000",
                KhachHang = new KhachHang
                {
                    HoTen = "Khách chèn thêm",
                    Email = "chen@example.com",
                    SoDienThoai = "0900000000"
                }
            },
            SanPham = submitted
        });

        var result = await controller.Edit(id, submitted);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(1, await context.LoaiSanPhams.CountAsync());
        Assert.Empty(context.ChiTietDonHangs);
        Assert.Empty(context.DonHangs);
        Assert.Empty(context.KhachHangs);
        var saved = await context.SanPhams.Include(product => product.LoaiSanPham).SingleAsync();
        Assert.Equal("Điện thoại", saved.LoaiSanPham?.TenLoai);
    }

    [Fact]
    public async Task Edit_TuChoiMaSanPhamKhacRoute()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        var existing = TestSupport.Product("Tên gốc", 1);
        context.SanPhams.Add(existing);
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var submitted = TestSupport.Product("Tên bị chèn", 1);
        submitted.MaSanPham = existing.MaSanPham + 999;

        var result = await controller.Edit(existing.MaSanPham, submitted);

        Assert.IsType<NotFoundResult>(result);
        Assert.Equal("Tên gốc", (await context.SanPhams.FindAsync(existing.MaSanPham))!.TenSanPham);
    }

    [Fact]
    public async Task Delete_XoaSanPhamChuaPhatSinhDonHangThanhCong()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        var product = TestSupport.Product("Sản phẩm", 1);
        context.SanPhams.Add(product);
        await context.SaveChangesAsync();
        var id = product.MaSanPham;
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = await controller.Delete(id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(controller.Index), redirect.ActionName);
        Assert.Null(await context.SanPhams.FindAsync(id));
        Assert.True(controller.TempData.ContainsKey("Success"));
    }
}
