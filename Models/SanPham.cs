using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; } = string.Empty;

    public string MoTa { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal Gia { get; set; }

    [Range(0, int.MaxValue)]
    public int SoLuong { get; set; }

    public bool TrangThai { get; set; } = true;

    public string HinhAnh { get; set; } = string.Empty;

    public DateTime NgayNhap { get; set; } = DateTime.Now;

    // Khóa ngoại đến LoaiSanPham
    public int MaLoai { get; set; }

    // Quan hệ
    public LoaiSanPham LoaiSanPham { get; set; } = null!;

    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        = new List<ChiTietDonHang>();
}
