using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class StorefrontViewModel
{
    public List<SanPham> Products { get; set; } = [];
    public List<StoreCategory> Categories { get; set; } = [];
}

public record StoreCategory(int Id, string Name, int Count);
