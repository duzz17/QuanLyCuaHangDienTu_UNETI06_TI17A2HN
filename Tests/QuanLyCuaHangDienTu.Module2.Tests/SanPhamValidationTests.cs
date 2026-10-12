// Họ và tên: Phạm Văn Cường - MSSV: 23103100094
// Nội dung thực hiện: Kiểm thử validation dữ liệu sản phẩm (Module 2)
using System.ComponentModel.DataAnnotations;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using Xunit;

namespace QuanLyCuaHangDienTu.Module2.Tests;

public class SanPhamValidationTests
{
    [Theory]
    [InlineData(nameof(SanPham.TenSanPham), null)]
    [InlineData(nameof(SanPham.TenSanPham), "")]
    [InlineData(nameof(SanPham.TenSanPham), "   ")]
    [InlineData(nameof(SanPham.ThuongHieu), null)]
    [InlineData(nameof(SanPham.ThuongHieu), "")]
    [InlineData(nameof(SanPham.ThuongHieu), "   ")]
    public void TenVaThuongHieu_BatBuoc(string propertyName, string? value)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        typeof(SanPham).GetProperty(propertyName)!.SetValue(product, value);

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(propertyName));
    }

    [Theory]
    [InlineData(nameof(SanPham.TenSanPham), 200)]
    [InlineData(nameof(SanPham.ThuongHieu), 100)]
    [InlineData(nameof(SanPham.XuatXu), 100)]
    [InlineData(nameof(SanPham.MoTa), 2000)]
    public void Chuoi_ChapNhanDungDoDaiToiDa(string propertyName, int maxLength)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        typeof(SanPham).GetProperty(propertyName)!.SetValue(product, new string('A', maxLength));

        var errors = Validate(product);

        Assert.DoesNotContain(errors, error => error.MemberNames.Contains(propertyName));
    }

    [Theory]
    [InlineData(nameof(SanPham.TenSanPham), 201)]
    [InlineData(nameof(SanPham.ThuongHieu), 101)]
    [InlineData(nameof(SanPham.XuatXu), 101)]
    [InlineData(nameof(SanPham.MoTa), 2001)]
    public void Chuoi_TuChoiKhiVuotDoDaiToiDa(string propertyName, int invalidLength)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        typeof(SanPham).GetProperty(propertyName)!.SetValue(product, new string('A', invalidLength));

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(propertyName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void XuatXu_BatBuocVaKhongChapNhanKhoangTrang(string? origin)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.XuatXu = origin!;

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.XuatXu)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MaLoai_PhaiLaSoNguyenDuong(int invalidCategoryId)
    {
        var product = TestSupport.Product("Sản phẩm", invalidCategoryId);

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.MaLoai)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("/images/products/dien-thoai.jpg")]
    [InlineData("~/images/products/may-tinh.webp")]
    [InlineData("https://cdn.example.com/products/a.png")]
    [InlineData("https://cdn.example.com/products/a.svg?v=2#preview")]
    [InlineData("https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400")]
    public void HinhAnh_ChapNhanDuongDanHopLe(string imagePath)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.HinhAnh = imagePath;

        var errors = Validate(product);

        Assert.DoesNotContain(errors, error => error.MemberNames.Contains(nameof(SanPham.HinhAnh)));
    }

    [Theory]
    [InlineData("anh-san-pham.jpg")]
    [InlineData("/uploads/a.jpg")]
    [InlineData("/images/a.exe")]
    [InlineData("ftp://example.com/a.jpg")]
    [InlineData("javascript:alert(1)")]
    public void HinhAnh_TuChoiDuongDanKhongHopLe(string imagePath)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.HinhAnh = imagePath;

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.HinhAnh)));
    }

    [Fact]
    public void HinhAnh_TuChoiKhiVuotNamTramKyTu()
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.HinhAnh = $"https://example.com/{new string('a', 480)}.jpg";

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.HinhAnh)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Gia_PhaiLonHonKhong(double invalidPrice)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.Gia = (decimal)invalidPrice;

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.Gia)));
    }

    [Fact]
    public void SoLuong_KhongDuocAm()
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.SoLuong = -1;

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.SoLuong)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(121)]
    public void ThoiGianBaoHanh_PhaiTrongKhoangChoPhep(int invalidWarranty)
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.ThoiGianBaoHanh = invalidWarranty;

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.ThoiGianBaoHanh)));
    }

    [Fact]
    public void TrangThai_KhongChapNhanGiaTriNgoaiEnum()
    {
        var product = TestSupport.Product("Sản phẩm", 1);
        product.TrangThai = (TrangThaiSanPham)999;

        var errors = Validate(product);

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(SanPham.TrangThai)));
    }

    [Theory]
    [InlineData(TrangThaiSanPham.DangKinhDoanh)]
    [InlineData(TrangThaiSanPham.TamNgung)]
    [InlineData(TrangThaiSanPham.NgungKinhDoanh)]
    public void TrangThai_ChapNhanDuBaGiaTriHopLe(TrangThaiSanPham status)
    {
        var product = TestSupport.Product("Sản phẩm", 1, status: status);

        var errors = Validate(product);

        Assert.DoesNotContain(errors, error => error.MemberNames.Contains(nameof(SanPham.TrangThai)));
    }

    private static IReadOnlyList<ValidationResult> Validate(SanPham product)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            product,
            new ValidationContext(product),
            results,
            validateAllProperties: true);
        return results;
    }
}
