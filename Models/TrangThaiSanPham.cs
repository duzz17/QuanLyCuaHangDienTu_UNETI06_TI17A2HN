// Họ và tên: Phạm Văn Cường - MSSV: 23103100094
// Module 2: Quản lý và tra cứu sản phẩm
using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

public enum TrangThaiSanPham
{
    [Display(Name = "Ngừng kinh doanh")]
    NgungKinhDoanh = 0,

    [Display(Name = "Đang kinh doanh")]
    DangKinhDoanh = 1,

    [Display(Name = "Tạm ngừng")]
    TamNgung = 2
}
