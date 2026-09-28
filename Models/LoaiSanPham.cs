// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Entity LoaiSanPham - thêm DataAnnotation và Validation (Module 1 - M1-03, M1-10)

using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class LoaiSanPham
{
    [Key]
    [Display(Name = "Mã loại")]
    public int MaLoai { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên loại sản phẩm")]
    [Display(Name = "Tên loại sản phẩm")]
    public string TenLoai { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    [DataType(DataType.MultilineText)]
    public string MoTa { get; set; } = string.Empty;

    // Quan hệ với SanPham (1 - nhiều)
    public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}