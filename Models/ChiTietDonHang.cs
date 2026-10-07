// Họ và tên: Ngô Khánh Quốc Huy
// Mã sinh viên: 23103100313
// Nội dung thực hiện: Entity ChiTietDonHang - thêm DataAnnotation và Validation (Module 3 - M3-01)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class ChiTietDonHang
{
    [Key]
    [Display(Name = "Mã chi tiết")]
    public int MaChiTiet { get; set; }

    [Required(ErrorMessage = "Số lượng không được để trống")]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; }

    [Required(ErrorMessage = "Đơn giá không được để trống")]
    [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Đơn giá phải lớn hơn 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    // Khóa ngoại đến DonHang
    [Display(Name = "Mã đơn hàng")]
    public int MaDonHang { get; set; }

    // Khóa ngoại đến SanPham
    [Display(Name = "Mã sản phẩm")]
    public int MaSanPham { get; set; }

    // Quan hệ
    public DonHang DonHang { get; set; } = null!;

    public SanPham SanPham { get; set; } = null!;
}