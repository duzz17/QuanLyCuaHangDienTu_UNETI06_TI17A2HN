using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? view)
    {
        if (HttpContext.Session.GetString("VaiTro") == "Admin" && view != "store")
            return RedirectToAction("Index", "Dashboard");

        ViewData["Storefront"] = true;
        return View(new StorefrontViewModel
        {
            Products = await context.SanPhams.AsNoTracking().Include(s => s.LoaiSanPham)
                .Where(s => s.TrangThai).OrderByDescending(s => s.NgayNhap)
                .ThenByDescending(s => s.MaSanPham).Take(8).ToListAsync(),
            Categories = await context.LoaiSanPhams.AsNoTracking().OrderBy(l => l.MaLoai)
                .Select(l => new StoreCategory(l.MaLoai, l.TenLoai, l.SanPhams.Count(s => s.TrangThai)))
                .ToListAsync()
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }



}
