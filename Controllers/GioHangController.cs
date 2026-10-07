using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class GioHangController : Controller
{
    private const string CART_KEY = "GIO_HANG_SESSION";
    private readonly ApplicationDbContext _context;

    public GioHangController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách giỏ hàng từ Session
    private List<CartItemViewModel> GetCartFromSession()
    {
        var sessionData = HttpContext.Session.GetString(CART_KEY);
        if (string.IsNullOrEmpty(sessionData))
        {
            return new List<CartItemViewModel>();
        }
        try
        {
            return JsonSerializer.Deserialize<List<CartItemViewModel>>(sessionData) ?? new List<CartItemViewModel>();
        }
        catch
        {
            return new List<CartItemViewModel>();
        }
    }

    // Lưu danh sách giỏ hàng vào Session
    private void SaveCartToSession(List<CartItemViewModel> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CART_KEY, json);
    }

    // GET: /GioHang (Trang xem giỏ hàng)
    [HttpGet]
    public IActionResult Index()
    {
        var cart = GetCartFromSession();
        var model = new GioHangViewModel
        {
            Items = cart
        };
        return View(model);
    }

    // POST: /GioHang/ThemVaoGio (Thêm sản phẩm vào giỏ)
    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> ThemVaoGio(int maSanPham, int soLuong = 1)
    {
        if (soLuong <= 0) soLuong = 1;

        var sanPham = await _context.SanPhams.FirstOrDefaultAsync(sp => sp.MaSanPham == maSanPham);
        if (sanPham == null || !sanPham.TrangThai)
        {
            TempData["Loi"] = "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh!";
            return RedirectToAction("Index", "SanPham");
        }

        if (sanPham.SoLuong <= 0)
        {
            TempData["Loi"] = $"Sản phẩm '{sanPham.TenSanPham}' hiện đã hết hàng!";
            return RedirectToAction("Index", "SanPham");
        }

        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(i => i.MaSanPham == maSanPham);

        if (item != null)
        {
            int soLuongMoi = item.SoLuong + soLuong;
            if (soLuongMoi > sanPham.SoLuong)
            {
                item.SoLuong = sanPham.SoLuong;
                TempData["Loi"] = $"Chỉ có thể thêm tối đa {sanPham.SoLuong} sản phẩm '{sanPham.TenSanPham}' vào giỏ hàng.";
            }
            else
            {
                item.SoLuong = soLuongMoi;
                TempData["ThongBao"] = $"Đã cập nhật số lượng sản phẩm '{sanPham.TenSanPham}' trong giỏ hàng thành {item.SoLuong}.";
            }
        }
        else
        {
            int slThem = Math.Min(soLuong, sanPham.SoLuong);
            cart.Add(new CartItemViewModel
            {
                MaSanPham = sanPham.MaSanPham,
                TenSanPham = sanPham.TenSanPham,
                HinhAnh = sanPham.HinhAnh ?? string.Empty,
                Gia = sanPham.Gia,
                SoLuong = slThem
            });
            TempData["ThongBao"] = $"Đã thêm '{sanPham.TenSanPham}' vào giỏ hàng thành công!";
        }

        SaveCartToSession(cart);
        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/CapNhatGioHang
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatGioHang(int maSanPham, int soLuong)
    {
        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(i => i.MaSanPham == maSanPham);

        if (item != null)
        {
            if (soLuong <= 0)
            {
                cart.Remove(item);
                TempData["ThongBao"] = $"Đã xóa sản phẩm '{item.TenSanPham}' khỏi giỏ hàng.";
            }
            else
            {
                var sanPham = await _context.SanPhams.FindAsync(maSanPham);
                if (sanPham != null && soLuong > sanPham.SoLuong)
                {
                    item.SoLuong = sanPham.SoLuong;
                    TempData["Loi"] = $"Số lượng vượt quá tồn kho hiện tại ({sanPham.SoLuong}).";
                }
                else
                {
                    item.SoLuong = soLuong;
                    TempData["ThongBao"] = $"Cập nhật số lượng '{item.TenSanPham}' thành công!";
                }
            }
            SaveCartToSession(cart);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/XoaKhoiGio
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult XoaKhoiGio(int maSanPham)
    {
        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(i => i.MaSanPham == maSanPham);
        if (item != null)
        {
            cart.Remove(item);
            SaveCartToSession(cart);
            TempData["ThongBao"] = $"Đã xóa '{item.TenSanPham}' khỏi giỏ hàng.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/XoaGioHang
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult XoaGioHang()
    {
        HttpContext.Session.Remove(CART_KEY);
        TempData["ThongBao"] = "Đã làm trống giỏ hàng thành công.";
        return RedirectToAction(nameof(Index));
    }
}
