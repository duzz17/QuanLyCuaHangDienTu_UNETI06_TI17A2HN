// Họ và tên: Ngô Khánh Quốc Huy
// Mã sinh viên: 23103100313
// Nội dung thực hiện: ViewModel CustomerProfileViewModel - Quản lý và xác thực dữ liệu hồ sơ cá nhân (Module 3 - M3-06)

using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class CustomerProfileViewModel
{
    public int MaKhachHang { get; set; }

    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự.")]
    [Display(Name = "Họ và tên")]
    public string DisplayName { get; set; } = string.Empty;

    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Vai trò")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống.")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.")]
    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [StringLength(250, ErrorMessage = "Địa chỉ tối đa 250 ký tự.")]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Ngày đăng ký")]
    public DateTime? RegisteredAt { get; set; }

    [Display(Name = "Ảnh đại diện")]
    public string? HinhAnh { get; set; }

    // Thống kê đơn hàng cho hồ sơ cá nhân
    public int SoLuongDonHang { get; set; }
    public decimal TongChiTieu { get; set; }
}
