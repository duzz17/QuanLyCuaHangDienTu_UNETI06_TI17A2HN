// Họ và tên: Ngô Khánh Quốc Huy
// Mã sinh viên: 23103100313
// Nội dung thực hiện: Entity DonHang - thêm DataAnnotation và Validation (Module 3 - M3-01)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class DonHang
{
    [Key]
    [Display(Name = "Mã đơn hàng")]
    public int MaDonHang { get; set; }

    [Display(Name = "Ngày đặt")]
    public DateTime NgayDat { get; set; } = DateTime.Now;

    [Display(Name = "Tổng tiền")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTien { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = "ChoXacNhan";

    [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
    [StringLength(250, ErrorMessage = "Địa chỉ giao hàng tối đa 250 ký tự")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string DiaChiGiaoHang { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại giao hàng không được để trống")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
    [Display(Name = "Số điện thoại giao hàng")]
    public string SoDienThoaiGiaoHang { get; set; } = string.Empty;

    // Khóa ngoại đến KhachHang
    [Display(Name = "Khách hàng")]
    public int MaKhachHang { get; set; }

    // Quan hệ
    public KhachHang KhachHang { get; set; } = null!;

    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        = new List<ChiTietDonHang>();
}