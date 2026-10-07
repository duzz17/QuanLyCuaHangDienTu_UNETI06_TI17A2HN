// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Xử lý đăng nhập bằng EF Core/LINQ, kiểm tra tài khoản, trạng thái khóa, quản lý Session và đăng xuất (Module 1 - Tuần 2: M1-06, M1-07, M1-10)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class TaiKhoanController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaiKhoanController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /TaiKhoan/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Nếu đã đăng nhập thì điều hướng về trang chủ hoặc returnUrl (M1-07)
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("VaiTro")))
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    // POST: /TaiKhoan/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // M1-06: Đăng nhập bằng EF Core / LINQ; kiểm tra tài khoản hợp lệ
        var taiKhoan = await _context.TaiKhoans
            .Include(tk => tk.KhachHang)
            .FirstOrDefaultAsync(tk => tk.TenDangNhap == model.TenDangNhap && tk.MatKhau == model.MatKhau);

        if (taiKhoan == null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // M1-06: Kiểm tra trạng thái khóa của tài khoản
        if (!taiKhoan.TrangThai)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // M1-07: Lưu MaTaiKhoan, HoTen, VaiTro vào Session
        string hoTen = taiKhoan.KhachHang?.HoTen ?? taiKhoan.TenDangNhap;
        HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
        HttpContext.Session.SetString("HoTen", hoTen);
        HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);
        HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);

        if (taiKhoan.MaKhachHang > 0)
        {
            HttpContext.Session.SetInt32("MaKhachHang", taiKhoan.MaKhachHang);
        }

        TempData["ThongBao"] = $"Đăng nhập thành công! Xin chào {hoTen}.";

        // Chuyển hướng lại URL ban đầu nếu có, hoặc về trang chủ
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        // Nếu là Admin, có thể chuyển về trang Quản lý loại sản phẩm hoặc Trang chủ
        if (taiKhoan.VaiTro == "Admin")
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return RedirectToAction("Index", "Home");
    }

    // GET & POST: /TaiKhoan/Logout
    [HttpGet]
    [HttpPost]
    public IActionResult Logout()
    {
        // M1-07: Đăng xuất xóa toàn bộ thông tin Session
        HttpContext.Session.Clear();
        TempData["ThongBao"] = "Bạn đã đăng xuất khỏi hệ thống thành công.";
        return RedirectToAction("Index", "Home");
    }

    // M1-10: Hỗ trợ kiểm tra tên đăng nhập không bị trùng
    [HttpGet]
    public async Task<IActionResult> KiemTraTenDangNhap(string tenDangNhap)
    {
        if (string.IsNullOrWhiteSpace(tenDangNhap))
        {
            return Json(true);
        }

        bool daTonTai = await _context.TaiKhoans
            .AnyAsync(tk => tk.TenDangNhap.ToLower() == tenDangNhap.Trim().ToLower());

        if (daTonTai)
        {
            return Json($"Tên đăng nhập '{tenDangNhap}' đã tồn tại trong hệ thống.");
        }

        return Json(true);
    }
}
