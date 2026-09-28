// Họ và tên: Phạm Văn Cương - MSSV: 23103100094
// Module 2: Quản lý và tra cứu sản phẩm
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public class SanPham
{
    [Display(Name = "Mã sản phẩm")]
    public int MaSanPham { get; set; }

    [Display(Name = "Tên sản phẩm")]
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự")]
    public string TenSanPham { get; set; } = string.Empty;

    [Display(Name = "Thương hiệu")]
    [Required(ErrorMessage = "Thương hiệu không được để trống")]
    [StringLength(100, ErrorMessage = "Thương hiệu tối đa 100 ký tự")]
    public string ThuongHieu { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    public string MoTa { get; set; } = string.Empty;

    [Display(Name = "Đơn giá")]
    [Required(ErrorMessage = "Đơn giá không được để trống")]
    [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Đơn giá phải lớn hơn 0")]
    public decimal Gia { get; set; }

    [Display(Name = "Số lượng tồn kho")]
    [Required(ErrorMessage = "Số lượng tồn kho không được để trống")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
    public int SoLuong { get; set; }

    [Display(Name = "Thời gian bảo hành (tháng)")]
    [Required(ErrorMessage = "Thời gian bảo hành không được để trống")]
    [Range(0, 120, ErrorMessage = "Thời gian bảo hành phải từ 0 đến 120 tháng")]
    public int ThoiGianBaoHanh { get; set; } = 12;

    [Display(Name = "Trạng thái kinh doanh")]
    public bool TrangThai { get; set; } = true;

    [Display(Name = "Hình ảnh")]
    public string HinhAnh { get; set; } = string.Empty;

    [Display(Name = "Ngày nhập")]
    public DateTime NgayNhap { get; set; } = DateTime.Now;

    [Display(Name = "Loại sản phẩm")]
    [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
    public int MaLoai { get; set; }
    
    public LoaiSanPham? LoaiSanPham { get; set; }
    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } 
        = new List<ChiTietDonHang>();
}
