using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

[ProtectedRoute]
public class GioHangController : Controller
{
    private const string CART_KEY = "GIO_HANG_SESSION";
    public const string COUPON_KEY = "GIO_HANG_COUPON";

    public static decimal CalculateDiscount(decimal tongTien, string? couponCode)
    {
        if (string.IsNullOrWhiteSpace(couponCode) || tongTien <= 0) return 0;
        couponCode = couponCode.Trim().ToUpperInvariant();
        return couponCode switch
        {
            "UNETI10" => Math.Min(tongTien * 0.10m, 1000000m), // Giảm 10%, tối đa 1.000.000đ
            "GIAM50K" => tongTien >= 100000m ? 50000m : 0m,   // Giảm 50.000đ cho đơn từ 100.000đ
            "DIENTU2026" => tongTien * 0.05m,                // Giảm 5%
            _ => 0m
        };
    }
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

    // GET: /GioHang (Trang xem giỏ hàng - Tích hợp Giỏ hàng nâng cao)
    [HttpGet]
    public IActionResult Index()
    {
        var cart = GetCartFromSession();
        var coupon = HttpContext.Session.GetString(COUPON_KEY);
        var subTotal = cart.Sum(i => i.ThanhTien);
        var discount = CalculateDiscount(subTotal, coupon);

        var model = new GioHangViewModel
        {
            Items = cart,
            MaGiamGia = coupon,
            SoTienGiam = discount
        };
        return View(model);
    }

    // POST: /GioHang/ThemVaoGio (Thêm sản phẩm vào giỏ)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemVaoGio(int maSanPham, int soLuong = 1, bool muaNgay = false)
    {
        var ajaxRequest = string.Equals(
            Request.Headers["X-Requested-With"],
            "XMLHttpRequest",
            StringComparison.OrdinalIgnoreCase);
        if (soLuong <= 0) soLuong = 1;

        var sanPham = await _context.SanPhams.FirstOrDefaultAsync(sp => sp.MaSanPham == maSanPham);
        if (sanPham == null || sanPham.TrangThai != TrangThaiSanPham.DangKinhDoanh)
        {
            const string errorMessage = "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh!";
            if (ajaxRequest)
                return NotFound(new { success = false, message = errorMessage });
            TempData["Loi"] = errorMessage;
            return RedirectToAction("Index", "SanPham");
        }

        if (sanPham.SoLuong <= 0)
        {
            var errorMessage = $"Sản phẩm '{sanPham.TenSanPham}' hiện đã hết hàng!";
            if (ajaxRequest)
                return BadRequest(new { success = false, message = errorMessage });
            TempData["Loi"] = errorMessage;
            return RedirectToAction("Index", "SanPham");
        }

        // NẾU BẤM "MUA HÀNG" (muaNgay = true):
        // KHÔNG đưa vào giỏ hàng chính (GIO_HANG_SESSION) trước
        // Chỉ lưu tạm vào MUA_NGAY_SESSION để chuyển thẳng sang trang Đặt hàng (DatHang).
        // Chỉ khi người dùng bấm "Hoàn tất đặt hàng", đơn hàng mới chính thức được ghi nhận!
        if (muaNgay)
        {
            var buyNowItem = new CartItemViewModel
            {
                MaSanPham = sanPham.MaSanPham,
                TenSanPham = sanPham.TenSanPham,
                HinhAnh = sanPham.HinhAnh ?? string.Empty,
                Gia = sanPham.Gia,
                SoLuong = Math.Min(soLuong, sanPham.SoLuong)
            };
            HttpContext.Session.SetString("MUA_NGAY_SESSION", JsonSerializer.Serialize(buyNowItem));
            return RedirectToAction("DatHang", "DonHang", new { direct = true });
        }

        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(i => i.MaSanPham == maSanPham);
        string message;
        var warning = false;

        if (item != null)
        {
            int soLuongMoi = item.SoLuong + soLuong;
            if (soLuongMoi > sanPham.SoLuong)
            {
                item.SoLuong = sanPham.SoLuong;
                warning = true;
                message = $"Chỉ có thể thêm tối đa {sanPham.SoLuong} sản phẩm '{sanPham.TenSanPham}' vào giỏ hàng.";
            }
            else
            {
                item.SoLuong = soLuongMoi;
                message = $"Đã cập nhật số lượng sản phẩm '{sanPham.TenSanPham}' trong giỏ hàng thành {item.SoLuong}.";
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
            warning = slThem < soLuong;
            message = warning
                ? $"Chỉ có thể thêm tối đa {slThem} sản phẩm '{sanPham.TenSanPham}' vào giỏ hàng."
                : $"Đã thêm '{sanPham.TenSanPham}' vào giỏ hàng thành công!";
        }

        SaveCartToSession(cart);
        if (ajaxRequest)
        {
            return Json(new
            {
                success = true,
                warning,
                message,
                cartCount = cart.Sum(cartItem => cartItem.SoLuong)
            });
        }

        TempData[warning ? "Loi" : "ThongBao"] = message;
        return muaNgay
            ? RedirectToAction("DatHang", "DonHang")
            : RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/CapNhatGioHang (Hỗ trợ AJAX cập nhật thời gian thực không tải lại trang)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatGioHang(int maSanPham, int soLuong)
    {
        var ajaxRequest = string.Equals(
            Request.Headers["X-Requested-With"],
            "XMLHttpRequest",
            StringComparison.OrdinalIgnoreCase);

        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(i => i.MaSanPham == maSanPham);
        string message = "";

        if (item != null)
        {
            if (soLuong <= 0)
            {
                cart.Remove(item);
                message = $"Đã xóa sản phẩm '{item.TenSanPham}' khỏi giỏ hàng.";
                TempData["ThongBao"] = message;
            }
            else
            {
                var sanPham = await _context.SanPhams.FindAsync(maSanPham);
                if (sanPham != null && soLuong > sanPham.SoLuong)
                {
                    item.SoLuong = sanPham.SoLuong;
                    message = $"Số lượng vượt quá tồn kho hiện tại ({sanPham.SoLuong}).";
                    TempData["Loi"] = message;
                }
                else
                {
                    item.SoLuong = soLuong;
                    message = $"Cập nhật số lượng '{item.TenSanPham}' thành công!";
                    TempData["ThongBao"] = message;
                }
            }
            SaveCartToSession(cart);
        }

        if (ajaxRequest)
        {
            var coupon = HttpContext.Session.GetString(COUPON_KEY);
            var subTotal = cart.Sum(i => i.ThanhTien);
            var discount = CalculateDiscount(subTotal, coupon);
            return Json(new
            {
                success = true,
                message,
                cartCount = cart.Sum(i => i.SoLuong),
                itemQuantity = item?.SoLuong ?? 0,
                itemTotal = item?.ThanhTien ?? 0,
                subTotal = subTotal,
                discount = discount,
                finalTotal = Math.Max(0, subTotal - discount)
            });
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/ApDungMaGiamGia (Chức năng nâng cao: Áp dụng voucher)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApDungMaGiamGia(string maGiamGia)
    {
        var cart = GetCartFromSession();
        var subTotal = cart.Sum(i => i.ThanhTien);

        if (string.IsNullOrWhiteSpace(maGiamGia))
        {
            TempData["Loi"] = "Vui lòng nhập mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }

        var code = maGiamGia.Trim().ToUpperInvariant();
        var discount = CalculateDiscount(subTotal, code);

        if (discount > 0)
        {
            HttpContext.Session.SetString(COUPON_KEY, code);
            TempData["ThongBao"] = $"Áp dụng mã voucher '{code}' thành công! Bạn được giảm {discount:N0} ₫.";
        }
        else
        {
            TempData["Loi"] = $"Mã giảm giá '{code}' không tồn tại hoặc đơn hàng chưa đủ điều kiện tối thiểu.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/XoaMaGiamGia (Hủy áp dụng mã giảm giá)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult XoaMaGiamGia()
    {
        HttpContext.Session.Remove(COUPON_KEY);
        TempData["ThongBao"] = "Đã hủy áp dụng mã giảm giá.";
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
