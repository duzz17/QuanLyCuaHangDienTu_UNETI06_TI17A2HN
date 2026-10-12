// Họ và tên: Phạm Văn Cường - MSSV: 23103100094
// Nội dung thực hiện: CRUD, tìm kiếm, lọc, sắp xếp và phân trang sản phẩm (Module 2)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class SanPhamController : Controller
{
    private const int MaxTuKhoaLength = 200;
    private const int MaxThuongHieuLength = 100;
    private static readonly DateTime NgayNhapToiThieu = new(2000, 1, 1);
    private static readonly HashSet<string> SapXepHopLe =
    [
        "ten_desc", "gia_asc", "gia_desc", "ton_asc", "ton_desc"
    ];
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
        TrangThaiSanPham? trangThai, // M2-08: Lọc theo trạng thái
        decimal? giaTu,         // M2-08: Khoảng giá từ
        decimal? giaDen,        // M2-08: Khoảng giá đến
        string? sapXep,         // M2-09: Sắp xếp
        int page = 1,           // M2-10: Trang hiện tại (mặc định trang 1)
        int pageSize = 6        // M2-10: Số sản phẩm mỗi trang (mặc định 6)
    )
    {
        var filterWarnings = new List<string>();
        int[] pageSizes = [6, 12, 24, 48];
        if (!ModelState.IsValid)
            filterWarnings.Add("Một số điều kiện lọc không đúng định dạng và đã được bỏ qua.");
        if (!pageSizes.Contains(pageSize))
        {
            pageSize = 6;
            filterWarnings.Add("Số sản phẩm mỗi trang không hợp lệ, hệ thống đã dùng mức 6.");
        }
        if (maLoai is <= 0)
        {
            maLoai = null;
            filterWarnings.Add("Danh mục lọc không hợp lệ và đã được bỏ qua.");
        }
        if (trangThai.HasValue && !Enum.IsDefined(trangThai.Value))
        {
            trangThai = null;
            filterWarnings.Add("Trạng thái lọc không hợp lệ và đã được bỏ qua.");
        }
        if (giaTu is < 0)
        {
            giaTu = 0;
            filterWarnings.Add("Giá từ không được âm và đã được đưa về 0.");
        }
        if (giaDen is < 0)
        {
            giaDen = 0;
            filterWarnings.Add("Giá đến không được âm và đã được đưa về 0.");
        }
        if (giaTu.HasValue && giaDen.HasValue && giaTu > giaDen)
        {
            (giaTu, giaDen) = (giaDen, giaTu);
            filterWarnings.Add("Khoảng giá nhập ngược đã được hoán đổi.");
        }
        tuKhoa = string.IsNullOrWhiteSpace(tuKhoa) ? null : tuKhoa.Trim();
        thuongHieu = string.IsNullOrWhiteSpace(thuongHieu) ? null : thuongHieu.Trim();
        if (tuKhoa?.Length > MaxTuKhoaLength)
        {
            tuKhoa = tuKhoa[..MaxTuKhoaLength];
            filterWarnings.Add($"Từ khóa chỉ được dùng tối đa {MaxTuKhoaLength} ký tự.");
        }
        if (thuongHieu?.Length > MaxThuongHieuLength)
        {
            thuongHieu = thuongHieu[..MaxThuongHieuLength];
            filterWarnings.Add($"Thương hiệu chỉ được dùng tối đa {MaxThuongHieuLength} ký tự.");
        }
        if (!string.IsNullOrEmpty(sapXep) && !SapXepHopLe.Contains(sapXep))
        {
            sapXep = null;
            filterWarnings.Add("Kiểu sắp xếp không hợp lệ, hệ thống đã dùng Tên A - Z.");
        }

        var isManagement = HttpContext.Session.GetString("VaiTro") == "Admin"
            && HttpContext.Request.Query["view"] != "store";
        ViewData["Storefront"] = !isManagement;
        IQueryable<SanPham> query = _context.SanPhams
            .AsNoTracking()
            .Include(s => s.LoaiSanPham);

        if (!isManagement)
            query = query.Where(s => s.TrangThai == TrangThaiSanPham.DangKinhDoanh);

        // M2-07: Tìm kiếm theo tên hoặc thương hiệu
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var kw = EscapeLikePattern(tuKhoa);
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

        if (trangThai.HasValue && Enum.IsDefined(trangThai.Value))
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
            "ten_desc" => query.OrderByDescending(s => s.TenSanPham).ThenBy(s => s.MaSanPham),
            "gia_asc" => query.OrderBy(s => s.Gia).ThenBy(s => s.MaSanPham),
            "gia_desc" => query.OrderByDescending(s => s.Gia).ThenBy(s => s.MaSanPham),
            "ton_asc" => query.OrderBy(s => s.SoLuong).ThenBy(s => s.MaSanPham),
            "ton_desc" => query.OrderByDescending(s => s.SoLuong).ThenBy(s => s.MaSanPham),
            _ => query.OrderBy(s => s.TenSanPham).ThenBy(s => s.MaSanPham)
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
        ViewBag.MaLoaiList = await CreateLoaiSanPhamSelectListAsync(maLoai, isManagement);
        var thuongHieuQuery = _context.SanPhams.AsNoTracking();
        if (!isManagement)
            thuongHieuQuery = thuongHieuQuery
                .Where(s => s.TrangThai == TrangThaiSanPham.DangKinhDoanh);
        ViewBag.ThuongHieuList = await thuongHieuQuery
            .Select(s => s.ThuongHieu)
            .Where(th => !string.IsNullOrEmpty(th))
            .Distinct()
            .OrderBy(th => th)
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
        ViewBag.FilterWarning = filterWarnings.Count == 0
            ? null
            : string.Join(" ", filterWarnings.Distinct());

        return View(sanPhams);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var isManagement = HttpContext.Session.GetString("VaiTro") == "Admin"
            && HttpContext.Request.Query["view"] != "store";
        var query = _context.SanPhams
            .AsNoTracking()
            .Include(s => s.LoaiSanPham)
            .AsQueryable();

        if (!isManagement)
            query = query.Where(s => s.TrangThai == TrangThaiSanPham.DangKinhDoanh);

        var sanPham = await query.FirstOrDefaultAsync(s => s.MaSanPham == id);

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

    private async Task<SelectList> CreateLoaiSanPhamSelectListAsync(
        int? selectedValue = null, bool includeInactive = false)
    {
        var loaiSanPhams = await _context.LoaiSanPhams
            .AsNoTracking()
            .Where(l => includeInactive || l.TrangThai || l.MaLoai == selectedValue)
            .OrderBy(l => l.TenLoai)
            .ToListAsync();

        return new SelectList(loaiSanPhams, "MaLoai", "TenLoai", selectedValue);
    }

    [HttpPost]
    [AdminOnly]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("TenSanPham,ThuongHieu,XuatXu,MoTa,Gia,SoLuong,ThoiGianBaoHanh,TrangThai,HinhAnh,NgayNhap,MaLoai")]
        SanPham sanPham)
    {
        // Loại bỏ validation đối với navigation property
        ModelState.Remove(nameof(sanPham.LoaiSanPham));

        await ValidateSanPhamAsync(sanPham);

        if (ModelState.IsValid)
        {
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm mới sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        ViewData["MaLoai"] = await CreateLoaiSanPhamSelectListAsync(sanPham.MaLoai);
        return View(sanPham);
    }

    [HttpPost]
    [AdminOnly]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("MaSanPham,TenSanPham,ThuongHieu,XuatXu,MoTa,Gia,SoLuong,ThoiGianBaoHanh,TrangThai,HinhAnh,NgayNhap,MaLoai")]
        SanPham sanPham)
    {
        if (id != sanPham.MaSanPham)
        {
            return NotFound();
        }

        var sanPhamHienTai = await _context.SanPhams.FirstOrDefaultAsync(s => s.MaSanPham == id);
        if (sanPhamHienTai is null)
            return NotFound();

        ModelState.Remove(nameof(sanPham.LoaiSanPham));

        await ValidateSanPhamAsync(sanPham, sanPhamHienTai.MaLoai);

        if (ModelState.IsValid)
        {
            try
            {
                sanPhamHienTai.TenSanPham = sanPham.TenSanPham;
                sanPhamHienTai.ThuongHieu = sanPham.ThuongHieu;
                sanPhamHienTai.XuatXu = sanPham.XuatXu;
                sanPhamHienTai.MoTa = sanPham.MoTa;
                sanPhamHienTai.Gia = sanPham.Gia;
                sanPhamHienTai.SoLuong = sanPham.SoLuong;
                sanPhamHienTai.ThoiGianBaoHanh = sanPham.ThoiGianBaoHanh;
                sanPhamHienTai.TrangThai = sanPham.TrangThai;
                sanPhamHienTai.HinhAnh = sanPham.HinhAnh;
                sanPhamHienTai.NgayNhap = sanPham.NgayNhap;
                sanPhamHienTai.MaLoai = sanPham.MaLoai;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
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
    public async Task<IActionResult> DoiTrangThai(int id, TrangThaiSanPham? trangThai)
    {
        var sanPham = await _context.SanPhams.FindAsync(id);
        if (sanPham == null) return NotFound();

        if (!trangThai.HasValue || !Enum.IsDefined(trangThai.Value))
        {
            TempData["Error"] = "Trạng thái sản phẩm không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        sanPham.TrangThai = trangThai.Value;
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đã cập nhật trạng thái sản phẩm sang: {GetTrangThaiText(sanPham.TrangThai)}";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateSanPhamAsync(SanPham sanPham, int? currentMaLoai = null)
    {
        sanPham.TenSanPham = sanPham.TenSanPham?.Trim() ?? string.Empty;
        sanPham.ThuongHieu = sanPham.ThuongHieu?.Trim() ?? string.Empty;
        sanPham.XuatXu = sanPham.XuatXu?.Trim() ?? string.Empty;
        sanPham.MoTa = sanPham.MoTa?.Trim() ?? string.Empty;
        sanPham.HinhAnh = sanPham.HinhAnh?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(sanPham.TenSanPham))
            ModelState.AddModelError(nameof(sanPham.TenSanPham), "Tên sản phẩm không được để trống.");
        if (string.IsNullOrWhiteSpace(sanPham.ThuongHieu))
            ModelState.AddModelError(nameof(sanPham.ThuongHieu), "Thương hiệu không được để trống.");
        if (string.IsNullOrWhiteSpace(sanPham.XuatXu))
            ModelState.AddModelError(nameof(sanPham.XuatXu), "Xuất xứ không được để trống.");
        if (!DuongDanAnhAttribute.IsValidPath(sanPham.HinhAnh))
            ModelState.AddModelError(nameof(sanPham.HinhAnh),
                "Hình ảnh phải là URL http/https hoặc đường dẫn /images/... có đuôi ảnh hợp lệ.");
        if (sanPham.NgayNhap.Date < NgayNhapToiThieu || sanPham.NgayNhap.Date > DateTime.Today)
            ModelState.AddModelError(nameof(sanPham.NgayNhap),
                $"Ngày nhập phải từ {NgayNhapToiThieu:dd/MM/yyyy} đến {DateTime.Today:dd/MM/yyyy}.");
        if (sanPham.Gia <= 0)
            ModelState.AddModelError(nameof(sanPham.Gia), "Đơn giá sản phẩm phải lớn hơn 0 ₫.");
        if (sanPham.SoLuong < 0)
            ModelState.AddModelError(nameof(sanPham.SoLuong), "Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
        if (sanPham.ThoiGianBaoHanh is < 0 or > 120)
            ModelState.AddModelError(nameof(sanPham.ThoiGianBaoHanh), "Thời gian bảo hành phải từ 0 đến 120 tháng.");
        if (!Enum.IsDefined(sanPham.TrangThai))
            ModelState.AddModelError(nameof(sanPham.TrangThai), "Trạng thái sản phẩm không hợp lệ.");
        if (sanPham.MaLoai <= 0 || !await _context.LoaiSanPhams.AnyAsync(l =>
                l.MaLoai == sanPham.MaLoai && (l.TrangThai || l.MaLoai == currentMaLoai)))
            ModelState.AddModelError(nameof(sanPham.MaLoai), "Vui lòng chọn loại sản phẩm hợp lệ.");
    }

    private static string EscapeLikePattern(string value) =>
        value.Replace("[", "[[]", StringComparison.Ordinal)
            .Replace("%", "[%]", StringComparison.Ordinal)
            .Replace("_", "[_]", StringComparison.Ordinal);

    private static string GetTrangThaiText(TrangThaiSanPham trangThai) => trangThai switch
    {
        TrangThaiSanPham.DangKinhDoanh => "Đang kinh doanh",
        TrangThaiSanPham.TamNgung => "Tạm ngừng",
        TrangThaiSanPham.NgungKinhDoanh => "Ngừng kinh doanh",
        _ => "Không xác định"
    };
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
            TempData["Error"] = "Không thể xóa sản phẩm đã có trong đơn hàng! Hãy chuyển trạng thái sang Ngừng kinh doanh.";
            return RedirectToAction(nameof(Index));
        }

        _context.SanPhams.Remove(sanPham);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }
}
