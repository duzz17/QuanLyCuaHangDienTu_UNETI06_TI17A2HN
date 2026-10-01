using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

[AdminOnly]
public class KhachHangController : Controller
{
    private readonly ApplicationDbContext _context;

    public KhachHangController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /KhachHang (Xem danh sách khách hàng & tra cứu)
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

    // GET: /KhachHang/Details/5 (Xem chi tiết khách hàng & Lịch sử đơn hàng)
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

    // GET: /KhachHang/Create (Form thêm khách hàng mới)
    [HttpGet]
    public IActionResult Create()
    {
        return View(new KhachHang());
    }

    // POST: /KhachHang/Create (Lưu thông tin khách hàng)
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

    // GET: /KhachHang/Edit/5 (Form chỉnh sửa thông tin khách hàng)
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

    // POST: /KhachHang/Edit/5 (Cập nhật thông tin khách hàng)
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

    // GET: /KhachHang/Delete/5 (Trang xác nhận xóa khách hàng)
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

    // POST: /KhachHang/Delete/5 (Thực hiện xóa)
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

    private async Task<bool> KhachHangExists(int id)
    {
        return await _context.KhachHangs.AnyAsync(e => e.MaKhachHang == id);
    }
}
