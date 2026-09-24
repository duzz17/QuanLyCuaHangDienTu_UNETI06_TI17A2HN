namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class KhachHang
{
    public int MaKhachHang { get; set; }

    public string HoTen { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SoDienThoai { get; set; } = string.Empty;

    public string DiaChi { get; set; } = string.Empty;

    public DateTime NgayDangKy { get; set; } = DateTime.Now;

    // Quan hệ với DonHang
    public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    // Quan hệ với TaiKhoan
    public TaiKhoan? TaiKhoan { get; set; }
}