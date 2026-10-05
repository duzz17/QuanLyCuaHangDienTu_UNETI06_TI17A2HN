using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class SanPhamController : Controller
{
    private readonly ApplicationDbContext _context;

    public SanPhamController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? tuKhoa,         // M2-07: Tìm kiếm theo tên hoặc thương hiệu
        int? maLoai,            // M2-08: Lọc theo loại sản phẩm
        string? thuongHieu,     // M2-08: Lọc theo thương hiệu
        bool? trangThai,        // M2-08: Lọc theo trạng thái
        decimal? giaTu,         // M2-08: Khoảng giá từ
        decimal? giaDen,        // M2-08: Khoảng giá đến
        string? sapXep,         // M2-09: Sắp xếp
        int page = 1,           // M2-10: Trang hiện tại (mặc định trang 1)
        int pageSize = 6        // M2-10: Số sản phẩm mỗi trang (mặc định 6)
    )
    {
        pageSize = Math.Clamp(pageSize, 1, 48);
        var isManagement = HttpContext.Session.GetString("VaiTro") == "Admin"
            && HttpContext.Request.Query["view"] != "store";
        ViewData["Storefront"] = !isManagement;
        IQueryable<SanPham> query = _context.SanPhams
            .AsNoTracking()
            .Include(s => s.LoaiSanPham);

        if (!isManagement)
            query = query.Where(s => s.TrangThai);

        // M2-07: Tìm kiếm theo tên hoặc thương hiệu
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var kw = tuKhoa.Trim();
            query = query.Where(s => EF.Functions.Like(s.TenSanPham, $"%{kw}%")
                                  || EF.Functions.Like(s.ThuongHieu, $"%{kw}%"));
        }

        // M2-08: Lọc kết hợp
        if (maLoai.HasValue && maLoai.Value > 0)
        {
            query = query.Where(s => s.MaLoai == maLoai.Value);
        }

        if (!string.IsNullOrWhiteSpace(thuongHieu))
        {
            query = query.Where(s => s.ThuongHieu == thuongHieu);
        }

        if (trangThai.HasValue)
        {
            query = query.Where(s => s.TrangThai == trangThai.Value);
        }

        if (giaTu.HasValue)
        {
            query = query.Where(s => s.Gia >= giaTu.Value);
        }

        if (giaDen.HasValue)
        {
            query = query.Where(s => s.Gia <= giaDen.Value);
        }

        // M2-09: Sắp xếp bằng LINQ
        query = sapXep switch
        {
            "ten_desc" => query.OrderByDescending(s => s.TenSanPham),
            "gia_asc" => query.OrderBy(s => s.Gia),
            "gia_desc" => query.OrderByDescending(s => s.Gia),
            "ton_asc" => query.OrderBy(s => s.SoLuong),
            "ton_desc" => query.OrderByDescending(s => s.SoLuong),
            _ => query.OrderBy(s => s.TenSanPham) // Mặc định: Tên A - Z
        };

        // M2-10: Tính toán phân trang EF Core
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        if (totalPages < 1) totalPages = 1;

        // M2-11: Xử lý an toàn số trang (trang đầu / trang cuối)
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        // M2-10: Phân trang tại truy vấn EF Core bằng Skip và Take
        var sanPhams = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Chuẩn bị dữ liệu danh mục & thương hiệu cho Form lọc
        ViewBag.MaLoaiList = await CreateLoaiSanPhamSelectListAsync(maLoai);
        ViewBag.ThuongHieuList = await _context.SanPhams
            .Select(s => s.ThuongHieu)
            .Where(th => !string.IsNullOrEmpty(th))
            .Distinct()
            .ToListAsync();

        // M2-11: Giữ lại toàn bộ điều kiện lọc, tìm kiếm, sắp xếp
        ViewBag.CurrentTuKhoa = tuKhoa;
        ViewBag.CurrentMaLoai = maLoai;
        ViewBag.CurrentThuongHieu = thuongHieu;
        ViewBag.CurrentTrangThai = trangThai;
        ViewBag.CurrentGiaTu = giaTu;
        ViewBag.CurrentGiaDen = giaDen;
        ViewBag.CurrentSapXep = sapXep;

        // M2-10 & M2-11: Dữ liệu phân trang gửi sang View
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.FromItem = totalItems == 0 ? 0 : (page - 1) * pageSize + 1;
        ViewBag.ToItem = Math.Min(page * pageSize, totalItems);
        ViewBag.HasPreviousPage = page > 1;
        ViewBag.HasNextPage = page < totalPages;

        return View(sanPhams);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var sanPham = await _context.SanPhams
            .AsNoTracking()
            .Include(s => s.LoaiSanPham)
            .FirstOrDefaultAsync(s => s.MaSanPham == id);

        if (sanPham is null)
        {
            return NotFound();
        }

        return View(sanPham);
    }

    [HttpGet]
    [AdminOnly]
    public async Task<IActionResult> Create()
    {
        ViewData["MaLoai"] = await CreateLoaiSanPhamSelectListAsync();
        return View(new SanPham());
    }

    [HttpGet]
    [AdminOnly]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var sanPham = await _context.SanPhams
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.MaSanPham == id);

        if (sanPham is null)
        {
            return NotFound();
        }

        ViewData["MaLoai"] = await CreateLoaiSanPhamSelectListAsync(sanPham.MaLoai);
        return View(sanPham);
    }

    private async Task<SelectList> CreateLoaiSanPhamSelectListAsync(int? selectedValue = null)
    {
        var loaiSanPhams = await _context.LoaiSanPhams
            .AsNoTracking()
            .ToListAsync();

        return new SelectList(loaiSanPhams, "MaLoai", "TenLoai", selectedValue);
    }

    [HttpPost]
    [AdminOnly]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SanPham sanPham)
    {
        // Loại bỏ validation đối với navigation property
        ModelState.Remove(nameof(sanPham.LoaiSanPham));

        // M2-06 & M2-12: Kiểm tra các quy tắc nghiệp vụ và thông báo lỗi Validation tiếng Việt
        if (sanPham.Gia <= 0)
        {
            ModelState.AddModelError(nameof(sanPham.Gia), "Đơn giá sản phẩm phải lớn hơn 0 ₫.");
        }
        if (sanPham.SoLuong < 0)
        {
            ModelState.AddModelError(nameof(sanPham.SoLuong), "Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
        }
        if (sanPham.ThoiGianBaoHanh < 0)
        {
            ModelState.AddModelError(nameof(sanPham.ThoiGianBaoHanh), "Thời gian bảo hành phải lớn hơn hoặc bằng 0 tháng.");
        }
        if (sanPham.MaLoai <= 0)
        {
            ModelState.AddModelError(nameof(sanPham.MaLoai), "Vui lòng chọn loại sản phẩm hợp lệ.");
        }

        if (ModelState.IsValid)
        {
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm mới sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        ViewData["MaLoai"] = await CreateLoaiSanPhamSelectListAsync(sanPham.MaLoai);
        return View(sanPham);
    }

    [HttpPost]
    [AdminOnly]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SanPham sanPham)
    {
        if (id != sanPham.MaSanPham)
        {
            return NotFound();
        }

        ModelState.Remove(nameof(sanPham.LoaiSanPham));

        // M2-06 & M2-12: Kiểm tra các quy tắc nghiệp vụ và thông báo lỗi Validation tiếng Việt
        if (sanPham.Gia <= 0)
        {
            ModelState.AddModelError(nameof(sanPham.Gia), "Đơn giá sản phẩm phải lớn hơn 0 ₫.");
        }
        if (sanPham.SoLuong < 0)
        {
            ModelState.AddModelError(nameof(sanPham.SoLuong), "Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
        }
        if (sanPham.ThoiGianBaoHanh < 0)
        {
            ModelState.AddModelError(nameof(sanPham.ThoiGianBaoHanh), "Thời gian bảo hành phải lớn hơn hoặc bằng 0 tháng.");
        }
        if (sanPham.MaLoai <= 0)
        {
            ModelState.AddModelError(nameof(sanPham.MaLoai), "Vui lòng chọn loại sản phẩm hợp lệ.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(sanPham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SanPhams.AnyAsync(s => s.MaSanPham == id))
                    return NotFound();
                throw;
            }
        }

        ViewData["MaLoai"] = await CreateLoaiSanPhamSelectListAsync(sanPham.MaLoai);
        return View(sanPham);
    }
    [HttpPost]
    [AdminOnly]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id)
    {
        var sanPham = await _context.SanPhams.FindAsync(id);
        if (sanPham == null) return NotFound();

        sanPham.TrangThai = !sanPham.TrangThai;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Đã cập nhật trạng thái sản phẩm sang: {(sanPham.TrangThai ? "Đang kinh doanh" : "Ngừng kinh doanh")}";
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [AdminOnly]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var sanPham = await _context.SanPhams
            .Include(s => s.ChiTietDonHangs)
            .FirstOrDefaultAsync(s => s.MaSanPham == id);

        if (sanPham == null) return NotFound();

        // Không xóa nếu sản phẩm đã phát sinh chi tiết đơn hàng
        if (sanPham.ChiTietDonHangs.Any())
        {
            TempData["ErrorMessage"] = "Không thể xóa sản phẩm đã có trong đơn hàng! Hãy chuyển trạng thái sang Ngừng kinh doanh.";
            return RedirectToAction(nameof(Index));
        }

        _context.SanPhams.Remove(sanPham);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }
}
