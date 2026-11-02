// Họ và tên: Phạm Văn Cương - MSSV: 23103100094
// Chức năng nâng cao: Giỏ hàng nâng cao tích hợp Mã giảm giá / Coupon / Voucher
namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class GioHangViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

    public decimal TongTien => Items.Sum(i => i.ThanhTien);

    public int TongSoLuong => Items.Sum(i => i.SoLuong);

    // Chức năng nâng cao: Mã giảm giá (Coupon/Voucher)
    public string? MaGiamGia { get; set; }
    public decimal SoTienGiam { get; set; }
    public decimal TongTienSauGiam => Math.Max(0, TongTien - SoTienGiam);
}
