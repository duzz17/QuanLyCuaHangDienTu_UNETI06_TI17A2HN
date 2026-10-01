namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class CartItemViewModel
{
    public int MaSanPham { get; set; }
    public string TenSanPham { get; set; } = string.Empty;
    public string HinhAnh { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public int SoLuong { get; set; }
    public decimal ThanhTien => Gia * SoLuong;
}
