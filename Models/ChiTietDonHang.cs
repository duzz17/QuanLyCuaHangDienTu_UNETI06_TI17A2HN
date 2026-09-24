namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class ChiTietDonHang
{
    public int MaChiTiet { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    // Khóa ngoại đến DonHang
    public int MaDonHang { get; set; }

    // Khóa ngoại đến SanPham
    public int MaSanPham { get; set; }

    // Quan hệ
    public DonHang DonHang { get; set; } = null!;

    public SanPham SanPham { get; set; } = null!;
}