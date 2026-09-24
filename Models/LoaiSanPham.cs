namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class LoaiSanPham
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = string.Empty;

    public string MoTa { get; set; } = string.Empty;

    // Quan hệ với SanPham
    public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}