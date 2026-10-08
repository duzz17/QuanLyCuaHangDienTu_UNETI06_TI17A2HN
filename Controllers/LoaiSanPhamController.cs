// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: CRUD quản lý loại sản phẩm - danh sách, chi tiết, thêm, sửa, xóa, đổi trạng thái, kiểm tra trùng tên và chặn xóa khi có sản phẩm liên quan (Module 1 - Tuần 2: M1-08, M1-09, M1-10 | Tuần 3: Bổ sung TrangThai đầy đủ)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

// M1-08: Phân quyền Admin ở cấp độ Controller - Chặn mọi truy cập trực tiếp từ URL nếu không có quyền Admin
[AdminOnly]
public class LoaiSanPhamController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoaiSanPhamController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /LoaiSanPham (M1-09: Xem danh sách loại sản phẩm)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var danhSach = await _context.LoaiSanPhams
            .AsNoTracking()
            .Include(l => l.SanPhams)
            .OrderBy(l => l.TenLoai)
            .ToListAsync();

        return View(danhSach);
    }

    // GET: /LoaiSanPham/Details/5 (M1-09: Xem chi tiết loại sản phẩm)
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loaiSanPham = await _context.LoaiSanPhams
            .AsNoTracking()
            .Include(l => l.SanPhams)
            .FirstOrDefaultAsync(l => l.MaLoai == id);

        if (loaiSanPham == null)
        {
            return NotFound();
        }

        return View(loaiSanPham);
    }

    // GET: /LoaiSanPham/Create (M1-09: Form thêm loại sản phẩm)
    [HttpGet]
    public IActionResult Create()
    {
        return View(new LoaiSanPham());
    }

    // POST: /LoaiSanPham/Create (M1-09, M1-10: Thêm mới, kiểm tra trùng tên, Validation; Tuần 3: Bind TrangThai)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TenLoai,MoTa,TrangThai")] LoaiSanPham loaiSanPham)
    {
        // M1-10: Kiểm tra tên loại sản phẩm không để trống
        if (string.IsNullOrWhiteSpace(loaiSanPham.TenLoai))
        {
            ModelState.AddModelError(nameof(loaiSanPham.TenLoai), "Vui lòng nhập tên loại sản phẩm.");
        }
        else
        {
            // M1-10: Kiểm tra tên loại không được trùng bằng EF Core / LINQ
            string tenChuanHoa = loaiSanPham.TenLoai.Trim().ToLower();
            bool daTonTai = await _context.LoaiSanPhams
                .AnyAsync(l => l.TenLoai.ToLower() == tenChuanHoa);

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(loaiSanPham.TenLoai), "Tên loại sản phẩm đã tồn tại. Vui lòng chọn tên khác.");
            }
        }

        if (ModelState.IsValid)
        {
            loaiSanPham.TenLoai = loaiSanPham.TenLoai.Trim();
            loaiSanPham.MoTa = loaiSanPham.MoTa?.Trim() ?? string.Empty;

            _context.Add(loaiSanPham);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Thêm loại sản phẩm mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        return View(loaiSanPham);
    }

    // GET: /LoaiSanPham/Edit/5 (M1-09: Form sửa loại sản phẩm)
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loaiSanPham = await _context.LoaiSanPhams.FindAsync(id);
        if (loaiSanPham == null)
        {
            return NotFound();
        }

        return View(loaiSanPham);
    }

    // POST: /LoaiSanPham/Edit/5 (M1-09, M1-10: Cập nhật, kiểm tra trùng tên; Tuần 3: Bind TrangThai)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("MaLoai,TenLoai,MoTa,TrangThai")] LoaiSanPham loaiSanPham)
    {
        if (id != loaiSanPham.MaLoai)
        {
            return NotFound();
        }

        // M1-10: Kiểm tra tên loại không để trống
        if (string.IsNullOrWhiteSpace(loaiSanPham.TenLoai))
        {
            ModelState.AddModelError(nameof(loaiSanPham.TenLoai), "Vui lòng nhập tên loại sản phẩm.");
        }
        else
        {
            // M1-10: Kiểm tra tên loại không trùng với các loại sản phẩm khác
            string tenChuanHoa = loaiSanPham.TenLoai.Trim().ToLower();
            bool daTonTai = await _context.LoaiSanPhams
                .AnyAsync(l => l.TenLoai.ToLower() == tenChuanHoa && l.MaLoai != loaiSanPham.MaLoai);

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(loaiSanPham.TenLoai), "Tên loại sản phẩm đã tồn tại. Vui lòng chọn tên khác.");
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                loaiSanPham.TenLoai = loaiSanPham.TenLoai.Trim();
                loaiSanPham.MoTa = loaiSanPham.MoTa?.Trim() ?? string.Empty;

                _context.Update(loaiSanPham);
                await _context.SaveChangesAsync();

                TempData["ThongBao"] = "Cập nhật loại sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await LoaiSanPhamExists(loaiSanPham.MaLoai))
                {
                    return NotFound();
                }
                throw;
            }
        }

        return View(loaiSanPham);
    }

    // GET: /LoaiSanPham/Delete/5 (M1-09, M1-10: Xác nhận xóa, kiểm tra ràng buộc sản phẩm)
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loaiSanPham = await _context.LoaiSanPhams
            .AsNoTracking()
            .Include(l => l.SanPhams)
            .FirstOrDefaultAsync(l => l.MaLoai == id);

        if (loaiSanPham == null)
        {
            return NotFound();
        }

        // M1-10: Kiểm tra có sản phẩm liên quan hay không để cảnh báo trước trên View
        bool dangCoSanPham = await _context.SanPhams.AnyAsync(sp => sp.MaLoai == id);
        int soLuongSanPham = await _context.SanPhams.CountAsync(sp => sp.MaLoai == id);

        ViewBag.DangCoSanPham = dangCoSanPham;
        ViewBag.SoLuongSanPham = soLuongSanPham;

        return View(loaiSanPham);
    }

    // POST: /LoaiSanPham/Delete/5 (M1-09, M1-10: Thực hiện xóa với kiểm tra an toàn)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // M1-10: Không xóa loại sản phẩm nếu gây mất hợp lệ dữ liệu sản phẩm liên quan
        bool dangCoSanPham = await _context.SanPhams.AnyAsync(sp => sp.MaLoai == id);
        if (dangCoSanPham)
        {
            TempData["Loi"] = "Không thể xóa loại sản phẩm này vì vẫn còn sản phẩm thuộc loại này! Vui lòng xóa hoặc chuyển các sản phẩm liên quan trước.";
            return RedirectToAction(nameof(Index));
        }

        var loaiSanPham = await _context.LoaiSanPhams.FindAsync(id);
        if (loaiSanPham == null)
        {
            return NotFound();
        }

        _context.LoaiSanPhams.Remove(loaiSanPham);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = "Xóa loại sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }

    // POST: /LoaiSanPham/DoiTrangThai/5 (M1-09, Tuần 3: Toggle trạng thái và lưu vào DB)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id)
    {
        var loaiSanPham = await _context.LoaiSanPhams.FindAsync(id);
        if (loaiSanPham == null)
        {
            return NotFound();
        }

        // Tuần 3: Toggle trạng thái và lưu vào database
        loaiSanPham.TrangThai = !loaiSanPham.TrangThai;
        _context.Update(loaiSanPham);
        await _context.SaveChangesAsync();

        string trangThaiMoi = loaiSanPham.TrangThai ? "Đang hoạt động" : "Ngừng hoạt động";
        TempData["ThongBao"] = $"Đã chuyển trạng thái loại sản phẩm '{loaiSanPham.TenLoai}' sang '{trangThaiMoi}'.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> LoaiSanPhamExists(int id)
    {
        return await _context.LoaiSanPhams.AnyAsync(e => e.MaLoai == id);
    }
}
