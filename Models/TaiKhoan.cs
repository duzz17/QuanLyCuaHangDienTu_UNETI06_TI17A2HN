namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    public string TenDangNhap { get; set; } = string.Empty;

    public string MatKhau { get; set; } = string.Empty;

    public string VaiTro { get; set; } = "KhachHang";

    public bool TrangThai { get; set; } = true;

    // Khóa ngoại đến KhachHang
    public int MaKhachHang { get; set; }

    // Quan hệ
    public KhachHang KhachHang { get; set; } = null!;
}