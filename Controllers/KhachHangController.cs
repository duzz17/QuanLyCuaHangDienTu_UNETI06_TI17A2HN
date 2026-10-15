// Họ và tên: Ngô Khánh Quốc Huy - MSSV: 23103100313
// Module 3: M3-06 Khách hàng đăng nhập xem/sửa trường thông tin cá nhân được phép; Validation họ tên, email, điện thoại, ngày sinh

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class KhachHangController : Controller
{
    private readonly ApplicationDbContext _context;

    public KhachHangController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /KhachHang (Xem danh sách khách hàng & tra cứu - Admin)
    [AdminOnly]
    [HttpGet]
    public async Task<IActionResult> Index(string? tuKhoa)
    {
        var query = _context.KhachHangs
            .Include(k => k.DonHangs)
            .Include(k => k.TaiKhoan)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            string search = tuKhoa.Trim().ToLower();
            query = query.Where(k =>
                k.HoTen.ToLower().Contains(search) ||
                k.Email.ToLower().Contains(search) ||
                k.SoDienThoai.Contains(search) ||
                k.DiaChi.ToLower().Contains(search));
        }

        ViewBag.CurrentTuKhoa = tuKhoa;
        var danhSach = await query.OrderByDescending(k => k.NgayDangKy).ToListAsync();
        return View(danhSach);
    }

    // GET: /KhachHang/Details/5 (Xem chi tiết khách hàng & Lịch sử đơn hàng - Admin)
    [AdminOnly]
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var khachHang = await _context.KhachHangs
            .Include(k => k.TaiKhoan)
            .Include(k => k.DonHangs)
                .ThenInclude(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.MaKhachHang == id);

        if (khachHang == null)
        {
            return NotFound();
        }

        return View(khachHang);
    }

    // GET: /KhachHang/Create (Form thêm khách hàng mới - Admin)
    [AdminOnly]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new KhachHang());
    }

    // POST: /KhachHang/Create (Lưu thông tin khách hàng - Admin)
    [AdminOnly]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("HoTen,Email,SoDienThoai,DiaChi")] KhachHang khachHang)
    {
        if (string.IsNullOrWhiteSpace(khachHang.HoTen))
        {
            ModelState.AddModelError(nameof(khachHang.HoTen), "Vui lòng nhập họ và tên khách hàng.");
        }

        if (string.IsNullOrWhiteSpace(khachHang.SoDienThoai))
        {
            ModelState.AddModelError(nameof(khachHang.SoDienThoai), "Vui lòng nhập số điện thoại.");
        }

        if (ModelState.IsValid)
        {
            khachHang.HoTen = khachHang.HoTen.Trim();
            khachHang.Email = khachHang.Email?.Trim() ?? string.Empty;
            khachHang.SoDienThoai = khachHang.SoDienThoai.Trim();
            khachHang.DiaChi = khachHang.DiaChi?.Trim() ?? string.Empty;
            khachHang.NgayDangKy = DateTime.Now;

            _context.Add(khachHang);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"Thêm mới khách hàng '{khachHang.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        return View(khachHang);
    }

    // GET: /KhachHang/Edit/5 (Form chỉnh sửa thông tin khách hàng - Admin)
    [AdminOnly]
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var khachHang = await _context.KhachHangs.FindAsync(id);
        if (khachHang == null)
        {
            return NotFound();
        }

        return View(khachHang);
    }

    // POST: /KhachHang/Edit/5 (Cập nhật thông tin khách hàng - Admin)
    [AdminOnly]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("MaKhachHang,HoTen,Email,SoDienThoai,DiaChi,NgayDangKy")] KhachHang khachHang)
    {
        if (id != khachHang.MaKhachHang)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(khachHang.HoTen))
        {
            ModelState.AddModelError(nameof(khachHang.HoTen), "Vui lòng nhập họ và tên khách hàng.");
        }

        if (string.IsNullOrWhiteSpace(khachHang.SoDienThoai))
        {
            ModelState.AddModelError(nameof(khachHang.SoDienThoai), "Vui lòng nhập số điện thoại.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                khachHang.HoTen = khachHang.HoTen.Trim();
                khachHang.Email = khachHang.Email?.Trim() ?? string.Empty;
                khachHang.SoDienThoai = khachHang.SoDienThoai.Trim();
                khachHang.DiaChi = khachHang.DiaChi?.Trim() ?? string.Empty;

                _context.Update(khachHang);
                await _context.SaveChangesAsync();

                TempData["ThongBao"] = $"Cập nhật thông tin khách hàng '{khachHang.HoTen}' thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await KhachHangExists(khachHang.MaKhachHang))
                {
                    return NotFound();
                }
                throw;
            }
        }

        return View(khachHang);
    }

    // GET: /KhachHang/Delete/5 (Trang xác nhận xóa khách hàng - Admin)
    [AdminOnly]
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var khachHang = await _context.KhachHangs
            .Include(k => k.DonHangs)
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.MaKhachHang == id);

        if (khachHang == null)
        {
            return NotFound();
        }

        bool hasOrders = khachHang.DonHangs.Any();
        ViewBag.HasOrders = hasOrders;
        ViewBag.OrderCount = khachHang.DonHangs.Count;

        return View(khachHang);
    }

    // POST: /KhachHang/Delete/5 (Thực hiện xóa - Admin)
    [AdminOnly]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var khachHang = await _context.KhachHangs
            .Include(k => k.DonHangs)
            .FirstOrDefaultAsync(k => k.MaKhachHang == id);

        if (khachHang == null)
        {
            return NotFound();
        }

        if (khachHang.DonHangs.Any())
        {
            TempData["Loi"] = $"Không thể xóa khách hàng '{khachHang.HoTen}' vì khách hàng này đã có {khachHang.DonHangs.Count} đơn hàng trong hệ thống!";
            return RedirectToAction(nameof(Index));
        }

        _context.KhachHangs.Remove(khachHang);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Xóa khách hàng '{khachHang.HoTen}' thành công!";
        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // M3-06: Khách hàng đăng nhập xem / sửa hồ sơ cá nhân & Validation
    // =========================================================================

    // GET: /KhachHang/HoSo
    [HttpGet]
    public async Task<IActionResult> HoSo()
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        var maKhachHangSession = HttpContext.Session.GetInt32("MaKhachHang");
        var maTaiKhoanSession = HttpContext.Session.GetInt32("MaTaiKhoan");

        if (string.IsNullOrEmpty(vaiTro) || (!maKhachHangSession.HasValue && !maTaiKhoanSession.HasValue))
        {
            TempData["Loi"] = "Vui lòng đăng nhập để xem thông tin hồ sơ cá nhân.";
            return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action("HoSo", "KhachHang") });
        }

        KhachHang? khachHang = null;
        if (maKhachHangSession.HasValue)
        {
            khachHang = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .Include(k => k.DonHangs)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhachHang == maKhachHangSession.Value);
        }
        else if (maTaiKhoanSession.HasValue)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.KhachHang)
                    .ThenInclude(k => k.DonHangs)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoanSession.Value);
            khachHang = taiKhoan?.KhachHang;
        }

        if (khachHang == null)
        {
            TempData["Loi"] = "Không tìm thấy thông tin hồ sơ khách hàng liên kết.";
            return RedirectToAction("Index", "Home");
        }

        if (khachHang.TaiKhoan != null && !khachHang.TaiKhoan.TrangThai)
        {
            TempData["Loi"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.";
            return RedirectToAction("Logout", "TaiKhoan");
        }

        var donHangsHopLe = khachHang.DonHangs?.Where(d => d.TrangThai != "DaHuy").ToList();
        var model = new CustomerProfileViewModel
        {
            MaKhachHang = khachHang.MaKhachHang,
            DisplayName = khachHang.HoTen,
            Username = khachHang.TaiKhoan?.TenDangNhap ?? (HttpContext.Session.GetString("TenDangNhap") ?? string.Empty),
            Role = khachHang.TaiKhoan?.VaiTro ?? (HttpContext.Session.GetString("VaiTro") ?? "KhachHang"),
            Email = khachHang.Email,
            Phone = khachHang.SoDienThoai,
            Address = khachHang.DiaChi,
            DateOfBirth = khachHang.NgaySinh,
            HinhAnh = khachHang.HinhAnh,
            RegisteredAt = khachHang.NgayDangKy,
            SoLuongDonHang = khachHang.DonHangs?.Count ?? 0,
            TongChiTieu = donHangsHopLe?.Sum(d => d.TongTien) ?? 0
        };

        return View(model);
    }

    // POST: /KhachHang/HoSo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HoSo(CustomerProfileViewModel model, IFormFile? avatarFile)
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        var maKhachHangSession = HttpContext.Session.GetInt32("MaKhachHang");

        if (string.IsNullOrEmpty(vaiTro) || !maKhachHangSession.HasValue)
        {
            TempData["Loi"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
            return RedirectToAction("Login", "TaiKhoan");
        }

        // Đảm bảo người dùng chỉ sửa hồ sơ của chính mình
        if (model.MaKhachHang != maKhachHangSession.Value)
        {
            TempData["Loi"] = "Bạn không có quyền chỉnh sửa hồ sơ này.";
            return RedirectToAction("Index", "Home");
        }

        // Kiểm tra validation ngày sinh: không được là ngày trong tương lai
        if (model.DateOfBirth.HasValue && model.DateOfBirth.Value.Date > DateTime.Today)
        {
            ModelState.AddModelError(nameof(model.DateOfBirth), "Ngày sinh không được vượt quá ngày hiện tại.");
        }

        // Kiểm tra định dạng file ảnh đại diện nếu có tải lên
        if (avatarFile != null && avatarFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("avatarFile", "Chỉ chấp nhận file ảnh có định dạng .jpg, .jpeg, .png, .webp, .gif.");
            }
            else if (avatarFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("avatarFile", "Dung lượng ảnh tối đa là 5MB.");
            }
        }

        if (!ModelState.IsValid)
        {
            // Nạp lại các trường chỉ đọc để hiển thị giao diện đúng
            var currentKh = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .Include(k => k.DonHangs)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhachHang == model.MaKhachHang);

            if (currentKh != null)
            {
                model.Username = currentKh.TaiKhoan?.TenDangNhap ?? string.Empty;
                model.Role = currentKh.TaiKhoan?.VaiTro ?? "KhachHang";
                model.HinhAnh = currentKh.HinhAnh;
                model.RegisteredAt = currentKh.NgayDangKy;
                model.SoLuongDonHang = currentKh.DonHangs?.Count ?? 0;
                model.TongChiTieu = currentKh.DonHangs?.Where(d => d.TrangThai != "DaHuy").Sum(d => d.TongTien) ?? 0;
            }
            return View(model);
        }

        var khachHang = await _context.KhachHangs
            .Include(k => k.TaiKhoan)
            .FirstOrDefaultAsync(k => k.MaKhachHang == model.MaKhachHang);

        if (khachHang == null)
        {
            return NotFound();
        }

        if (khachHang.TaiKhoan != null && !khachHang.TaiKhoan.TrangThai)
        {
            TempData["Loi"] = "Tài khoản của bạn đã bị khóa.";
            return RedirectToAction("Logout", "TaiKhoan");
        }

        // Xử lý lưu file ảnh đại diện nếu người dùng tải file mới
        if (avatarFile != null && avatarFile.Length > 0)
        {
            var ext = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }
            var fileName = $"avatar_{khachHang.MaKhachHang}_{DateTime.Now.Ticks}{ext}";
            var filePath = Path.Combine(uploadDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }
            khachHang.HinhAnh = "/uploads/avatars/" + fileName;
            HttpContext.Session.SetString("Avatar", khachHang.HinhAnh);
        }
        else if (!string.IsNullOrWhiteSpace(model.HinhAnh))
        {
            khachHang.HinhAnh = model.HinhAnh.Trim();
            HttpContext.Session.SetString("Avatar", khachHang.HinhAnh);
        }

        // Cập nhật các trường được phép sửa
        khachHang.HoTen = model.DisplayName.Trim();
        khachHang.Email = model.Email?.Trim() ?? string.Empty;
        khachHang.SoDienThoai = model.Phone?.Trim() ?? string.Empty;
        khachHang.DiaChi = model.Address?.Trim() ?? string.Empty;
        khachHang.NgaySinh = model.DateOfBirth;

        _context.KhachHangs.Update(khachHang);
        await _context.SaveChangesAsync();

        // Đồng bộ họ tên mới vào Session
        HttpContext.Session.SetString("HoTen", khachHang.HoTen);

        TempData["ThongBao"] = "Cập nhật thông tin cá nhân và ảnh đại diện thành công!";
        return RedirectToAction(nameof(HoSo));
    }

    private async Task<bool> KhachHangExists(int id)
    {
        return await _context.KhachHangs.AnyAsync(e => e.MaKhachHang == id);
    }
}
