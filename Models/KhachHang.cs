// Họ và tên: Ngô Khánh Quốc Huy
// Mã sinh viên: 23103100313
// Nội dung thực hiện: Entity KhachHang - thêm DataAnnotation và Validation (Module 3 - M3-01)

using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class KhachHang
{
    [Key]
    [Display(Name = "Mã khách hàng")]
    public int MaKhachHang { get; set; }

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "Địa chỉ tối đa 250 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string DiaChi { get; set; } = string.Empty;

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }

    [Display(Name = "Ngày đăng ký")]
    public DateTime NgayDangKy { get; set; } = DateTime.Now;

    // Quan hệ với DonHang
    public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    // Quan hệ với TaiKhoan
    public TaiKhoan? TaiKhoan { get; set; }
}