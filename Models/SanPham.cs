namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; } = string.Empty;

    public string MoTa { get; set; } = string.Empty;

    public decimal Gia { get; set; }

    public int SoLuong { get; set; }

    public string HinhAnh { get; set; } = string.Empty;

    public DateTime NgayNhap { get; set; } = DateTime.Now;

    // Khóa ngoại đến LoaiSanPham
    public int MaLoai { get; set; }

    // Quan hệ
    public LoaiSanPham LoaiSanPham { get; set; } = null!;

    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        = new List<ChiTietDonHang>();
}