using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class DatHangViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên người nhận.")]
    [Display(Name = "Họ và tên người nhận")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại giao hàng.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại giao hàng")]
    public string SoDienThoai { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email người nhận (không bắt buộc)")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng cụ thể.")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string DiaChiGiaoHang { get; set; } = string.Empty;

    [Display(Name = "Ghi chú đơn hàng")]
    public string? GhiChu { get; set; }

    public int? MaKhachHang { get; set; }

    public GioHangViewModel GioHang { get; set; } = new GioHangViewModel();
}
