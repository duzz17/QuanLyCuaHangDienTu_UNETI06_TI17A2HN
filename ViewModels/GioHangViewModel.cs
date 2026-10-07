namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class GioHangViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

    public decimal TongTien => Items.Sum(i => i.ThanhTien);

    public int TongSoLuong => Items.Sum(i => i.SoLuong);
}
