using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class SanPhamController : Controller
{
    private readonly ApplicationDbContext _context;

    public SanPhamController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sanPhams = await _context.SanPhams
            .AsNoTracking()
            .Include(s => s.LoaiSanPham)
            .ToListAsync();

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
    public async Task<IActionResult> Create()
    {
        ViewData["MaLoai"] = await CreateLoaiSanPhamSelectListAsync();
        return View(new SanPham());
    }

    [HttpGet]
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
}
