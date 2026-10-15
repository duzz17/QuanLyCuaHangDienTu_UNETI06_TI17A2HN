// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Xử lý đăng nhập bằng EF Core/LINQ, kiểm tra tài khoản, trạng thái khóa, quản lý Session và đăng xuất; hỗ trợ returnUrl để quay lại giỏ hàng sau đăng nhập (Module 1 - Tuần 2: M1-06, M1-07, M1-10 | Tuần 3: Tích hợp returnUrl giỏ hàng)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
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

        if (!string.IsNullOrEmpty(taiKhoan.KhachHang?.HinhAnh))
        {
            HttpContext.Session.SetString("Avatar", taiKhoan.KhachHang.HinhAnh);
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

    // =========================================================================
    // Đăng ký tài khoản khách hàng mới
    // =========================================================================

    // GET: /TaiKhoan/Register
    [HttpGet]
    public IActionResult Register()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("VaiTro")))
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new RegisterViewModel());
    }

    // POST: /TaiKhoan/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string username = model.TenDangNhap.Trim();
        bool daTonTaiUser = await _context.TaiKhoans
            .AnyAsync(tk => tk.TenDangNhap.ToLower() == username.ToLower());
        if (daTonTaiUser)
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), $"Tên đăng nhập '{username}' đã được sử dụng. Vui lòng chọn tên khác.");
            return View(model);
        }

        string sdt = model.SoDienThoai.Trim();
        bool daTonTaiSdt = await _context.KhachHangs
            .AnyAsync(kh => kh.SoDienThoai == sdt);
        if (daTonTaiSdt)
        {
            ModelState.AddModelError(nameof(model.SoDienThoai), $"Số điện thoại '{sdt}' đã được đăng ký trong hệ thống.");
            return View(model);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var khachHang = new KhachHang
            {
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                SoDienThoai = sdt,
                DiaChi = model.DiaChi?.Trim() ?? string.Empty,
                NgaySinh = model.NgaySinh,
                NgayDangKy = DateTime.Now
            };
            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = username,
                MatKhau = model.MatKhau,
                VaiTro = "KhachHang",
                TrangThai = true,
                MaKhachHang = khachHang.MaKhachHang
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Tự động đăng nhập luôn sau khi đăng ký
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", khachHang.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);
            HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);
            HttpContext.Session.SetInt32("MaKhachHang", khachHang.MaKhachHang);

            TempData["ThongBao"] = $"Chúc mừng {khachHang.HoTen}, bạn đã đăng ký tài khoản thành công!";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình đăng ký tài khoản: " + ex.Message);
            return View(model);
        }
    }
}
