using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class DonHangController : Controller
{
    private const string CART_KEY = "GIO_HANG_SESSION";
    private readonly ApplicationDbContext _context;

    public DonHangController(ApplicationDbContext context)
    {
        _context = context;
    }

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

    // GET: /DonHang (Danh sách đơn hàng)
    [HttpGet]
    public async Task<IActionResult> Index(string? tuKhoa, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        var maKhachHangSession = HttpContext.Session.GetInt32("MaKhachHang");

        // Nếu chưa đăng nhập, hướng dẫn đăng nhập
        if (string.IsNullOrEmpty(vaiTro))
        {
            TempData["Loi"] = "Vui lòng đăng nhập để xem danh sách đơn hàng.";
            return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action("Index", "DonHang") });
        }

        var query = _context.DonHangs
            .Include(d => d.KhachHang)
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(ct => ct.SanPham)
            .AsNoTracking();

        // Khách hàng thường chỉ xem đơn của chính mình
        if (vaiTro != "Admin")
        {
            if (maKhachHangSession.HasValue)
            {
                query = query.Where(d => d.MaKhachHang == maKhachHangSession.Value);
            }
            else
            {
                return View(new List<DonHang>());
            }
        }

        // Lọc theo từ khóa
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            string search = tuKhoa.Trim().ToLower();
            query = query.Where(d =>
                d.MaDonHang.ToString().Contains(search) ||
                d.KhachHang.HoTen.ToLower().Contains(search) ||
                d.SoDienThoaiGiaoHang.Contains(search) ||
                d.DiaChiGiaoHang.ToLower().Contains(search));
        }

        // Lọc theo trạng thái
        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(d => d.TrangThai == trangThai);
        }

        // Lọc theo ngày
        if (tuNgay.HasValue)
        {
            query = query.Where(d => d.NgayDat >= tuNgay.Value.Date);
        }
        if (denNgay.HasValue)
        {
            query = query.Where(d => d.NgayDat <= denNgay.Value.Date.AddDays(1).AddTicks(-1));
        }

        ViewBag.CurrentTuKhoa = tuKhoa;
        ViewBag.CurrentTrangThai = trangThai;
        ViewBag.CurrentTuNgay = tuNgay?.ToString("yyyy-MM-dd");
        ViewBag.CurrentDenNgay = denNgay?.ToString("yyyy-MM-dd");

        var danhSach = await query.OrderByDescending(d => d.NgayDat).ToListAsync();
        return View(danhSach);
    }

    // GET: /DonHang/DatHang (Trang thông tin đặt hàng / Checkout)
    [HttpGet]
    public async Task<IActionResult> DatHang()
    {
        var cart = GetCartFromSession();
        if (!cart.Any())
        {
            TempData["Loi"] = "Giỏ hàng của bạn đang trống! Vui lòng chọn sản phẩm trước khi đặt hàng.";
            return RedirectToAction("Index", "SanPham");
        }

        var model = new DatHangViewModel
        {
            GioHang = new GioHangViewModel { Items = cart }
        };

        // Nếu đã đăng nhập và là khách hàng, pre-fill thông tin
        var maKhachHangSession = HttpContext.Session.GetInt32("MaKhachHang");
        if (maKhachHangSession.HasValue)
        {
            var khachHang = await _context.KhachHangs.FindAsync(maKhachHangSession.Value);
            if (khachHang != null)
            {
                model.MaKhachHang = khachHang.MaKhachHang;
                model.HoTen = khachHang.HoTen;
                model.SoDienThoai = khachHang.SoDienThoai;
                model.Email = khachHang.Email;
                model.DiaChiGiaoHang = khachHang.DiaChi;
            }
        }

        return View(model);
    }

    // POST: /DonHang/DatHang (Xử lý đặt hàng)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatHang(DatHangViewModel model)
    {
        var cart = GetCartFromSession();
        if (!cart.Any())
        {
            TempData["Loi"] = "Giỏ hàng của bạn đang trống!";
            return RedirectToAction("Index", "SanPham");
        }

        model.GioHang = new GioHangViewModel { Items = cart };

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Tìm hoặc tạo KhachHang
            KhachHang? khachHang = null;
            var maKhachHangSession = HttpContext.Session.GetInt32("MaKhachHang");

            if (maKhachHangSession.HasValue)
            {
                khachHang = await _context.KhachHangs.FindAsync(maKhachHangSession.Value);
            }

            if (khachHang == null && !string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                khachHang = await _context.KhachHangs
                    .FirstOrDefaultAsync(k => k.SoDienThoai == model.SoDienThoai.Trim());
            }

            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    HoTen = model.HoTen.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    Email = model.Email?.Trim() ?? string.Empty,
                    DiaChi = model.DiaChiGiaoHang.Trim(),
                    NgayDangKy = DateTime.Now
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
            }

            // 2. Tạo đơn hàng mới
            var donHang = new DonHang
            {
                MaKhachHang = khachHang.MaKhachHang,
                NgayDat = DateTime.Now,
                TongTien = cart.Sum(i => i.ThanhTien),
                TrangThai = "ChoXacNhan",
                DiaChiGiaoHang = model.DiaChiGiaoHang.Trim(),
                SoDienThoaiGiaoHang = model.SoDienThoai.Trim()
            };
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // 3. Thêm chi tiết đơn hàng & Trừ tồn kho sản phẩm
            foreach (var item in cart)
            {
                var sanPham = await _context.SanPhams.FindAsync(item.MaSanPham);
                if (sanPham == null || !sanPham.TrangThai)
                {
                    await transaction.RollbackAsync();
                    TempData["Loi"] = $"Sản phẩm '{item.TenSanPham}' hiện không khả dụng.";
                    return View(model);
                }

                if (sanPham.SoLuong < item.SoLuong)
                {
                    await transaction.RollbackAsync();
                    TempData["Loi"] = $"Sản phẩm '{sanPham.TenSanPham}' chỉ còn {sanPham.SoLuong} sản phẩm trong kho.";
                    return View(model);
                }

                // Trừ số lượng tồn kho
                sanPham.SoLuong -= item.SoLuong;
                _context.SanPhams.Update(sanPham);

                var chiTiet = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaSanPham = item.MaSanPham,
                    SoLuong = item.SoLuong,
                    DonGia = item.Gia
                };
                _context.ChiTietDonHangs.Add(chiTiet);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Xóa giỏ hàng Session
            HttpContext.Session.Remove(CART_KEY);

            TempData["ThongBao"] = $"Đặt hàng thành công! Mã đơn hàng của bạn là #{donHang.MaDonHang}.";
            return RedirectToAction(nameof(Details), new { id = donHang.MaDonHang });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình xử lý đơn hàng: " + ex.Message);
            return View(model);
        }
    }

    // GET: /DonHang/Details/5 (Chi tiết đơn hàng)
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var donHang = await _context.DonHangs
            .Include(d => d.KhachHang)
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(ct => ct.SanPham)
                    .ThenInclude(sp => sp.LoaiSanPham)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.MaDonHang == id);

        if (donHang == null)
        {
            return NotFound();
        }

        var vaiTro = HttpContext.Session.GetString("VaiTro");
        var maKhachHangSession = HttpContext.Session.GetInt32("MaKhachHang");

        // Quyền truy cập: Admin xem được hết, Khách hàng chỉ xem đơn của mình
        if (vaiTro != "Admin")
        {
            if (!maKhachHangSession.HasValue || donHang.MaKhachHang != maKhachHangSession.Value)
            {
                // Nếu là khách hàng vừa mới đặt hàng xong không đăng nhập thì vẫn cho xem thông qua TempData hoặc link xem đơn
                if (TempData["ThongBao"] == null)
                {
                    TempData["Loi"] = "Bạn không có quyền xem thông tin đơn hàng này.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        return View(donHang);
    }

    // POST: /DonHang/CapNhatTrangThai (Chỉ dành cho Admin)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id, string trangThaiMoi)
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro != "Admin")
        {
            TempData["Loi"] = "Bạn không có quyền thực hiện thao tác này!";
            return RedirectToAction(nameof(Index));
        }

        var donHang = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .FirstOrDefaultAsync(d => d.MaDonHang == id);

        if (donHang == null)
        {
            return NotFound();
        }

        string trangThaiCu = donHang.TrangThai;
        if (trangThaiCu == trangThaiMoi)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        // Nếu chuyển sang Đã Hủy từ trạng thái chưa hủy -> Trả lại số lượng tồn kho sản phẩm
        if (trangThaiMoi == "DaHuy" && trangThaiCu != "DaHuy")
        {
            foreach (var ct in donHang.ChiTietDonHangs)
            {
                var sp = await _context.SanPhams.FindAsync(ct.MaSanPham);
                if (sp != null)
                {
                    sp.SoLuong += ct.SoLuong;
                    _context.SanPhams.Update(sp);
                }
            }
        }

        donHang.TrangThai = trangThaiMoi;
        _context.DonHangs.Update(donHang);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Cập nhật trạng thái đơn hàng #{donHang.MaDonHang} thành công sang '{trangThaiMoi}'!";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /DonHang/HuyDon (Khách hàng hoặc Admin hủy đơn ChoXacNhan)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HuyDon(int id)
    {
        var donHang = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .FirstOrDefaultAsync(d => d.MaDonHang == id);

        if (donHang == null)
        {
            return NotFound();
        }

        if (donHang.TrangThai != "ChoXacNhan")
        {
            TempData["Loi"] = "Chỉ có thể hủy các đơn hàng đang ở trạng thái 'Chờ xác nhận'.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Hoàn trả tồn kho
        foreach (var ct in donHang.ChiTietDonHangs)
        {
            var sp = await _context.SanPhams.FindAsync(ct.MaSanPham);
            if (sp != null)
            {
                sp.SoLuong += ct.SoLuong;
                _context.SanPhams.Update(sp);
            }
        }

        donHang.TrangThai = "DaHuy";
        _context.DonHangs.Update(donHang);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Hủy đơn hàng #{donHang.MaDonHang} thành công!";
        return RedirectToAction(nameof(Details), new { id });
    }
}
