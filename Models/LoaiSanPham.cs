// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Entity LoaiSanPham - thêm DataAnnotation và Validation, bổ sung TrangThai (Module 1 - M1-03, M1-10, Tuần 3)

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

    // M1-11 (Tuần 3): Trạng thái loại sản phẩm - true: Đang hoạt động, false: Ngừng hoạt động
    [Display(Name = "Trạng thái")]
    public bool TrangThai { get; set; } = true;

    // Quan hệ với SanPham (1 - nhiều)
    public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}