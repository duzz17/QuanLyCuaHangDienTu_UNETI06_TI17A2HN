using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Tự động tạo cơ sở dữ liệu và áp dụng các Migration mới nhất
        context.Database.Migrate();

        // Kiểm tra xem đã có dữ liệu tài khoản chưa
        if (context.TaiKhoans.Any())
        {
            return; // Đã khởi tạo dữ liệu
        }

        // 1. Tạo Khách hàng ban đầu
        var adminKhachHang = new KhachHang
        {
            HoTen = "Quản trị viên Hệ thống",
            Email = "admin@uneti.edu.vn",
            SoDienThoai = "0988888888",
            DiaChi = "Đại học UNETI - Hà Nội",
            NgayDangKy = DateTime.Now.AddMonths(-3)
        };

        var userKhachHang1 = new KhachHang
        {
            HoTen = "Nguyễn Văn A",
            Email = "nguyenvana@gmail.com",
            SoDienThoai = "0912345678",
            DiaChi = "218 Lĩnh Nam, Hoàng Mai, Hà Nội",
            NgayDangKy = DateTime.Now.AddMonths(-2)
        };

        var userKhachHang2 = new KhachHang
        {
            HoTen = "Trần Thị B",
            Email = "tranthib@gmail.com",
            SoDienThoai = "0976543210",
            DiaChi = "456 Minh Khai, Hai Bà Trưng, Hà Nội",
            NgayDangKy = DateTime.Now.AddMonths(-1)
        };

        context.KhachHangs.AddRange(adminKhachHang, userKhachHang1, userKhachHang2);
        context.SaveChanges();

        // 2. Tạo Tài khoản đăng nhập (Admin & Khách hàng)
        var adminTaiKhoan = new TaiKhoan
        {
            TenDangNhap = "admin",
            MatKhau = "admin123",
            VaiTro = "Admin",
            TrangThai = true,
            MaKhachHang = adminKhachHang.MaKhachHang
        };

        var khachHangTaiKhoan = new TaiKhoan
        {
            TenDangNhap = "khachhang",
            MatKhau = "123456",
            VaiTro = "KhachHang",
            TrangThai = true,
            MaKhachHang = userKhachHang1.MaKhachHang
        };

        context.TaiKhoans.AddRange(adminTaiKhoan, khachHangTaiKhoan);
        context.SaveChanges();

        // 3. Tạo Loại sản phẩm
        var loaiDienThoai = new LoaiSanPham
        {
            TenLoai = "Điện thoại & Máy tính bảng",
            MoTa = "Điện thoại thông minh, iPad, máy tính bảng chính hãng"
        };
        var loaiLaptop = new LoaiSanPham
        {
            TenLoai = "Laptop & Máy tính xách tay",
            MoTa = "Laptop văn phòng, laptop gaming, MacBook các loại"
        };
        var loaiPhuKien = new LoaiSanPham
        {
            TenLoai = "Phụ kiện & Thiết bị số",
            MoTa = "Tai nghe bluetooth, bàn phím cơ, chuột, sạc dự phòng"
        };

        context.LoaiSanPhams.AddRange(loaiDienThoai, loaiLaptop, loaiPhuKien);
        context.SaveChanges();

        // 4. Tạo Sản phẩm điện tử mẫu
        var sp1 = new SanPham
        {
            TenSanPham = "iPhone 15 Pro Max 256GB",
            ThuongHieu = "Apple",
            Gia = 32990000m,
            SoLuong = 15,
            ThoiGianBaoHanh = 12,
            TrangThai = true,
            HinhAnh = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=400",
            MoTa = "Chip A17 Pro mạnh mẽ, khung Titan cao cấp, camera 48MP zoom 5x.",
            MaLoai = loaiDienThoai.MaLoai
        };

        var sp2 = new SanPham
        {
            TenSanPham = "Samsung Galaxy S24 Ultra 5G",
            ThuongHieu = "Samsung",
            Gia = 29990000m,
            SoLuong = 20,
            ThoiGianBaoHanh = 12,
            TrangThai = true,
            HinhAnh = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=400",
            MoTa = "Galaxy AI đột phá, màn hình Dynamic AMOLED 2X, bút S-Pen quyền năng.",
            MaLoai = loaiDienThoai.MaLoai
        };

        var sp3 = new SanPham
        {
            TenSanPham = "Laptop Dell XPS 15 9530 Core i7",
            ThuongHieu = "Dell",
            Gia = 45500000m,
            SoLuong = 8,
            ThoiGianBaoHanh = 24,
            TrangThai = true,
            HinhAnh = "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=400",
            MoTa = "Intel Core i7-13700H, RAM 16GB, SSD 512GB, RTX 4050 6GB.",
            MaLoai = loaiLaptop.MaLoai
        };

        var sp4 = new SanPham
        {
            TenSanPham = "MacBook Pro 14 inch M3 Pro",
            ThuongHieu = "Apple",
            Gia = 49990000m,
            SoLuong = 10,
            ThoiGianBaoHanh = 12,
            TrangThai = true,
            HinhAnh = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400",
            MoTa = "Apple M3 Pro 11-core CPU, 14-core GPU, RAM 18GB, SSD 512GB.",
            MaLoai = loaiLaptop.MaLoai
        };

        var sp5 = new SanPham
        {
            TenSanPham = "Tai nghe Sony WH-1000XM5 Chống ồn",
            ThuongHieu = "Sony",
            Gia = 7490000m,
            SoLuong = 25,
            ThoiGianBaoHanh = 12,
            TrangThai = true,
            HinhAnh = "https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400",
            MoTa = "Công nghệ chống ồn hàng đầu thế giới, thời lượng pin đến 30 giờ.",
            MaLoai = loaiPhuKien.MaLoai
        };

        var sp6 = new SanPham
        {
            TenSanPham = "Bàn phím cơ không dây Logitech MX Keys S",
            ThuongHieu = "Logitech",
            Gia = 2890000m,
            SoLuong = 30,
            ThoiGianBaoHanh = 24,
            TrangThai = true,
            HinhAnh = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=400",
            MoTa = "Gõ phím mượt mà, đèn nền thông minh, kết nối 3 thiết bị cùng lúc.",
            MaLoai = loaiPhuKien.MaLoai
        };

        context.SanPhams.AddRange(sp1, sp2, sp3, sp4, sp5, sp6);
        context.SaveChanges();

        // 5. Tạo 1 Đơn hàng mẫu ban đầu
        var donHangMau = new DonHang
        {
            MaKhachHang = userKhachHang1.MaKhachHang,
            NgayDat = DateTime.Now.AddDays(-2),
            TongTien = 40480000m,
            TrangThai = "ChoXacNhan",
            SoDienThoaiGiaoHang = userKhachHang1.SoDienThoai,
            DiaChiGiaoHang = userKhachHang1.DiaChi
        };
        context.DonHangs.Add(donHangMau);
        context.SaveChanges();

        var chiTiet1 = new ChiTietDonHang
        {
            MaDonHang = donHangMau.MaDonHang,
            MaSanPham = sp1.MaSanPham,
            SoLuong = 1,
            DonGia = sp1.Gia
        };
        var chiTiet2 = new ChiTietDonHang
        {
            MaDonHang = donHangMau.MaDonHang,
            MaSanPham = sp5.MaSanPham,
            SoLuong = 1,
            DonGia = sp5.Gia
        };

        context.ChiTietDonHangs.AddRange(chiTiet1, chiTiet2);
        context.SaveChanges();
    }
}
