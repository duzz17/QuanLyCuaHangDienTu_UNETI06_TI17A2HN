// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Entity TaiKhoan - thêm DataAnnotation và Validation (Module 1 - M1-03, M1-10)

using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class TaiKhoan
{
    [Key]
    [Display(Name = "Mã tài khoản")]
    public int MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vai trò")]
    public string VaiTro { get; set; } = "KhachHang";

    [Display(Name = "Trạng thái")]
    public bool TrangThai { get; set; } = true;

    // Khóa ngoại đến KhachHang
    public int MaKhachHang { get; set; }

    // Quan hệ
    public KhachHang KhachHang { get; set; } = null!;
}