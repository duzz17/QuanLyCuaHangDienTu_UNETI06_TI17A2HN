namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class DonHang
{
    public int MaDonHang { get; set; }

    public DateTime NgayDat { get; set; } = DateTime.Now;

    public decimal TongTien { get; set; }

    public string TrangThai { get; set; } = "ChoXacNhan";

    public string DiaChiGiaoHang { get; set; } = string.Empty;

    public string SoDienThoaiGiaoHang { get; set; } = string.Empty;

    // Khóa ngoại đến KhachHang
    public int MaKhachHang { get; set; }

    // Quan hệ
    public KhachHang KhachHang { get; set; } = null!;

    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        = new List<ChiTietDonHang>();
}