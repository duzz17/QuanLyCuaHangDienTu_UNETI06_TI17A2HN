// Họ và tên: Phạm Văn Cường - MSSV: 23103100094
// Nội dung thực hiện: Kiểm thử CRUD, tìm kiếm, lọc, sắp xếp và phân trang sản phẩm (Module 2)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using Xunit;

namespace QuanLyCuaHangDienTu.Module2.Tests;

public class SanPhamControllerTests
{
    [Fact]
    public async Task Index_KhachHang_ChiThaySanPhamDangKinhDoanh()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(
            TestSupport.Product("Đang bán", 1, status: TrangThaiSanPham.DangKinhDoanh),
            TestSupport.Product("Tạm ngừng", 1, status: TrangThaiSanPham.TamNgung),
            TestSupport.Product("Ngừng bán", 1, status: TrangThaiSanPham.NgungKinhDoanh));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        var product = Assert.Single(products);
        Assert.Equal("Đang bán", product.TenSanPham);
        Assert.Equal(TrangThaiSanPham.DangKinhDoanh, product.TrangThai);
    }

    [Theory]
    [InlineData("Phone", "Alpha Phone")]
    [InlineData("Samsung", "Beta Tablet")]
    [InlineData("khong-ton-tai", null)]
    public async Task Index_TimKiemTheoTenHoacThuongHieu(string keyword, string? expectedName)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(
            TestSupport.Product("Alpha Phone", 1, "Acme"),
            TestSupport.Product("Beta Tablet", 1, "Samsung"),
            TestSupport.Product("Gamma Camera", 1, "Sony"));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, tuKhoa: keyword));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        if (expectedName is null)
            Assert.Empty(products);
        else
            Assert.Equal(expectedName, Assert.Single(products).TenSanPham);
    }

    [Fact]
    public async Task Index_Admin_LocKetHopLoaiThuongHieuTrangThaiVaGia()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.AddRange(
            new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" },
            new LoaiSanPham { MaLoai = 2, TenLoai = "Laptop" });
        context.SanPhams.AddRange(
            TestSupport.Product("Khớp", 1, "Samsung", price: 15_000_000m, status: TrangThaiSanPham.TamNgung),
            TestSupport.Product("Sai loại", 2, "Samsung", price: 15_000_000m, status: TrangThaiSanPham.TamNgung),
            TestSupport.Product("Sai hãng", 1, "Apple", price: 15_000_000m, status: TrangThaiSanPham.TamNgung),
            TestSupport.Product("Sai trạng thái", 1, "Samsung", price: 15_000_000m),
            TestSupport.Product("Giá quá thấp", 1, "Samsung", price: 5_000_000m, status: TrangThaiSanPham.TamNgung),
            TestSupport.Product("Giá quá cao", 1, "Samsung", price: 25_000_000m, status: TrangThaiSanPham.TamNgung));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller,
            maLoai: 1,
            thuongHieu: "Samsung",
            trangThai: TrangThaiSanPham.TamNgung,
            giaTu: 10_000_000m,
            giaDen: 20_000_000m));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Equal("Khớp", Assert.Single(products).TenSanPham);
    }

    [Theory]
    [InlineData(TrangThaiSanPham.DangKinhDoanh, "Đang bán")]
    [InlineData(TrangThaiSanPham.TamNgung, "Tạm ngừng")]
    [InlineData(TrangThaiSanPham.NgungKinhDoanh, "Ngừng bán")]
    public async Task Index_Admin_LocRiengTheoTungTrangThai(
        TrangThaiSanPham status,
        string expectedName)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(
            TestSupport.Product("Đang bán", 1, status: TrangThaiSanPham.DangKinhDoanh),
            TestSupport.Product("Tạm ngừng", 1, status: TrangThaiSanPham.TamNgung),
            TestSupport.Product("Ngừng bán", 1, status: TrangThaiSanPham.NgungKinhDoanh));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, trangThai: status));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        var product = Assert.Single(products);
        Assert.Equal(expectedName, product.TenSanPham);
        Assert.Equal(status, product.TrangThai);
    }

    [Theory]
    [InlineData(null, "Alpha", "Beta", "Gamma")]
    [InlineData("ten_desc", "Gamma", "Beta", "Alpha")]
    [InlineData("gia_asc", "Beta", "Gamma", "Alpha")]
    [InlineData("gia_desc", "Alpha", "Gamma", "Beta")]
    [InlineData("ton_asc", "Gamma", "Alpha", "Beta")]
    [InlineData("ton_desc", "Beta", "Alpha", "Gamma")]
    public async Task Index_SapXepTheoTieuChi(
        string? sort,
        string first,
        string second,
        string third)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(
            TestSupport.Product("Gamma", 1, price: 200m, stock: 10),
            TestSupport.Product("Alpha", 1, price: 300m, stock: 20),
            TestSupport.Product("Beta", 1, price: 100m, stock: 30));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, sapXep: sort));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Equal(new[] { first, second, third }, products.Select(product => product.TenSanPham));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-5)]
    public async Task Index_TrangDauVaTrangAm_DuaVeTrangDau(int requestedPage)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(Enumerable.Range(1, 13)
            .Select(index => TestSupport.Product($"Sản phẩm {index}", 1)));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, page: requestedPage, pageSize: 6));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Equal(6, products.Count);
        Assert.Equal(1, Assert.IsType<int>(result.ViewData["CurrentPage"]));
        Assert.False(Assert.IsType<bool>(result.ViewData["HasPreviousPage"]));
        Assert.True(Assert.IsType<bool>(result.ViewData["HasNextPage"]));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(99)]
    public async Task Index_TrangCuoiVaTrangVuotBien_DuaVeTrangCuoi(int requestedPage)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(Enumerable.Range(1, 13)
            .Select(index => TestSupport.Product($"Sản phẩm {index:00}", 1)));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, page: requestedPage, pageSize: 6));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Single(products);
        Assert.Equal(3, Assert.IsType<int>(result.ViewData["CurrentPage"]));
        Assert.Equal(3, Assert.IsType<int>(result.ViewData["TotalPages"]));
        Assert.True(Assert.IsType<bool>(result.ViewData["HasPreviousPage"]));
        Assert.False(Assert.IsType<bool>(result.ViewData["HasNextPage"]));
    }

    [Theory]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(48)]
    public async Task Index_ChapNhanTatCaPageSizeChoPhep(int pageSize)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(Enumerable.Range(1, 50)
            .Select(index => TestSupport.Product($"Sản phẩm {index:00}", 1)));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, pageSize: pageSize));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Equal(pageSize, products.Count);
        Assert.Equal(pageSize, Assert.IsType<int>(result.ViewData["PageSize"]));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(49)]
    public async Task Index_PageSizeKhongHopLe_DuaVeMacDinhSauSanPham(int invalidPageSize)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(Enumerable.Range(1, 8)
            .Select(index => TestSupport.Product($"Sản phẩm {index}", 1)));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, pageSize: invalidPageSize));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Equal(6, products.Count);
        Assert.Equal(6, Assert.IsType<int>(result.ViewData["PageSize"]));
    }

    [Fact]
    public async Task Index_ChuyenTrang_GiuToanBoDieuKienLocVaSapXep()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.AddRange(
            new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" },
            new LoaiSanPham { MaLoai = 2, TenLoai = "Laptop" });
        context.SanPhams.AddRange(Enumerable.Range(1, 8)
            .Select(index => TestSupport.Product(
                $"Phone {index:00}",
                1,
                "Samsung",
                price: 100m + index,
                status: TrangThaiSanPham.TamNgung)));
        context.SanPhams.Add(TestSupport.Product(
            "Phone không khớp",
            2,
            "Apple",
            price: 999m,
            status: TrangThaiSanPham.DangKinhDoanh));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(
            controller,
            tuKhoa: "Phone",
            maLoai: 1,
            thuongHieu: "Samsung",
            trangThai: TrangThaiSanPham.TamNgung,
            giaTu: 100m,
            giaDen: 200m,
            sapXep: "gia_desc",
            page: 2,
            pageSize: 6));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Equal(2, products.Count);
        Assert.Equal("Phone", Assert.IsType<string>(result.ViewData["CurrentTuKhoa"]));
        Assert.Equal(1, Assert.IsType<int>(result.ViewData["CurrentMaLoai"]));
        Assert.Equal("Samsung", Assert.IsType<string>(result.ViewData["CurrentThuongHieu"]));
        Assert.Equal(
            TrangThaiSanPham.TamNgung,
            Assert.IsType<TrangThaiSanPham>(result.ViewData["CurrentTrangThai"]));
        Assert.Equal(100m, Assert.IsType<decimal>(result.ViewData["CurrentGiaTu"]));
        Assert.Equal(200m, Assert.IsType<decimal>(result.ViewData["CurrentGiaDen"]));
        Assert.Equal("gia_desc", Assert.IsType<string>(result.ViewData["CurrentSapXep"]));
        Assert.Equal(2, Assert.IsType<int>(result.ViewData["CurrentPage"]));
        Assert.Equal(6, Assert.IsType<int>(result.ViewData["PageSize"]));
    }

    [Theory]
    [InlineData(TrangThaiSanPham.TamNgung)]
    [InlineData(TrangThaiSanPham.NgungKinhDoanh)]
    [InlineData(TrangThaiSanPham.DangKinhDoanh)]
    public async Task DoiTrangThai_DatDungGiaTriDuocChon(TrangThaiSanPham targetStatus)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        var product = TestSupport.Product("Sản phẩm", 1);
        context.SanPhams.Add(product);
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = await controller.DoiTrangThai(product.MaSanPham, targetStatus);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(targetStatus, (await context.SanPhams.FindAsync(product.MaSanPham))!.TrangThai);
    }

    [Fact]
    public async Task Create_TuChoiMaLoaiKhongTonTai()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var product = TestSupport.Product("Sản phẩm", categoryId: 999);

        var result = await controller.Create(product);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(SanPham.MaLoai)));
        Assert.Empty(context.SanPhams);
    }

    [Fact]
    public async Task Create_TuChoiXuatXuChiCoKhoangTrang()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var product = TestSupport.Product("Sản phẩm", 1, origin: "   ");

        var result = await controller.Create(product);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(SanPham.XuatXu)));
        Assert.Empty(context.SanPhams);
    }

    [Fact]
    public async Task Create_TuChoiDuongDanAnhKhongHopLe()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var product = TestSupport.Product("Sản phẩm", 1);
        product.HinhAnh = "javascript:alert(1)";

        var result = await controller.Create(product);

        Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey(nameof(SanPham.HinhAnh)));
        Assert.Empty(context.SanPhams);
    }

    [Theory]
    [MemberData(nameof(NgayNhapKhongHopLe))]
    public async Task Create_TuChoiNgayNhapNgoaiKhoang(DateTime invalidDate)
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var product = TestSupport.Product("Sản phẩm", 1);
        product.NgayNhap = invalidDate;

        var result = await controller.Create(product);

        Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey(nameof(SanPham.NgayNhap)));
        Assert.Empty(context.SanPhams);
    }

    [Fact]
    public async Task Create_ChuanHoaMoTaVaDuongDanAnhTruocKhiLuu()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);
        var product = TestSupport.Product("Sản phẩm", 1);
        product.MoTa = "  Mô tả sản phẩm  ";
        product.HinhAnh = "  /images/products/a.jpg  ";

        var result = await controller.Create(product);

        Assert.IsType<RedirectToActionResult>(result);
        var saved = Assert.Single(context.SanPhams);
        Assert.Equal("Mô tả sản phẩm", saved.MoTa);
        Assert.Equal("/images/products/a.jpg", saved.HinhAnh);
    }

    [Fact]
    public async Task Index_KhoangGiaNhapNguoc_DuocHoanDoiVaCanhBao()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.Add(TestSupport.Product("Sản phẩm", 1, price: 150m));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, giaTu: 200m, giaDen: 100m));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Single(products);
        Assert.Equal(100m, Assert.IsType<decimal>(result.ViewData["CurrentGiaTu"]));
        Assert.Equal(200m, Assert.IsType<decimal>(result.ViewData["CurrentGiaDen"]));
        Assert.NotNull(result.ViewData["FilterWarning"]);
    }

    [Fact]
    public async Task Index_KieuSapXepKhongHopLe_DuaVeTenTangDan()
    {
        await using var context = TestSupport.CreateContext();
        context.LoaiSanPhams.Add(new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" });
        context.SanPhams.AddRange(
            TestSupport.Product("Beta", 1),
            TestSupport.Product("Alpha", 1));
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = Assert.IsType<ViewResult>(await IndexAsync(controller, sapXep: "khong-hop-le"));
        var products = Assert.IsAssignableFrom<IEnumerable<SanPham>>(result.Model).ToList();

        Assert.Equal(new[] { "Alpha", "Beta" }, products.Select(product => product.TenSanPham));
        Assert.Null(result.ViewData["CurrentSapXep"]);
        Assert.NotNull(result.ViewData["FilterWarning"]);
    }

    [Fact]
    public async Task Delete_ChanSanPhamDaCoChiTietDonHang()
    {
        await using var context = TestSupport.CreateContext();
        var category = new LoaiSanPham { MaLoai = 1, TenLoai = "Điện thoại" };
        var product = TestSupport.Product("Sản phẩm", 1);
        var customer = new KhachHang
        {
            HoTen = "Khách hàng",
            Email = "khach@example.com",
            SoDienThoai = "0900000000"
        };
        var order = new DonHang
        {
            KhachHang = customer,
            DiaChiGiaoHang = "Hà Nội",
            SoDienThoaiGiaoHang = "0900000000"
        };
        var detail = new ChiTietDonHang
        {
            DonHang = order,
            SanPham = product,
            SoLuong = 1,
            DonGia = product.Gia
        };
        context.AddRange(category, product, customer, order, detail);
        await context.SaveChangesAsync();
        var controller = TestSupport.CreateController(context, isAdmin: true);

        var result = await controller.Delete(product.MaSanPham);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(await context.SanPhams.FindAsync(product.MaSanPham));
        Assert.True(controller.TempData.ContainsKey("Error"));
    }

    private static Task<IActionResult> IndexAsync(
        SanPhamController controller,
        string? tuKhoa = null,
        int? maLoai = null,
        string? thuongHieu = null,
        TrangThaiSanPham? trangThai = null,
        decimal? giaTu = null,
        decimal? giaDen = null,
        string? sapXep = null,
        int page = 1,
        int pageSize = 6) =>
        controller.Index(tuKhoa, maLoai, thuongHieu, trangThai, giaTu, giaDen, sapXep, page, pageSize);

    public static IEnumerable<object[]> NgayNhapKhongHopLe()
    {
        yield return [new DateTime(1999, 12, 31)];
        yield return [DateTime.Today.AddDays(1)];
    }
}
