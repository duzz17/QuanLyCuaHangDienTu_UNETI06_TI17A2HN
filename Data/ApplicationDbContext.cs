using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<KhachHang> KhachHangs { get; set; }

    public DbSet<TaiKhoan> TaiKhoans { get; set; }

    public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }

    public DbSet<SanPham> SanPhams { get; set; }

    public DbSet<DonHang> DonHangs { get; set; }

    public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // KhachHang - DonHang: 1 - nhiều
        modelBuilder.Entity<DonHang>()
            .HasOne(d => d.KhachHang)
            .WithMany(k => k.DonHangs)
            .HasForeignKey(d => d.MaKhachHang)
            .OnDelete(DeleteBehavior.Restrict);

        // KhachHang - TaiKhoan: 1 - 1
        modelBuilder.Entity<TaiKhoan>()
            .HasOne(t => t.KhachHang)
            .WithOne(k => k.TaiKhoan)
            .HasForeignKey<TaiKhoan>(t => t.MaKhachHang)
            .OnDelete(DeleteBehavior.Cascade);

        // LoaiSanPham - SanPham: 1 - nhiều
        modelBuilder.Entity<SanPham>()
            .HasOne(s => s.LoaiSanPham)
            .WithMany(l => l.SanPhams)
            .HasForeignKey(s => s.MaLoai)
            .OnDelete(DeleteBehavior.Restrict);

        // DonHang - ChiTietDonHang: 1 - nhiều
        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(ct => ct.DonHang)
            .WithMany(d => d.ChiTietDonHangs)
            .HasForeignKey(ct => ct.MaDonHang)
            .OnDelete(DeleteBehavior.Cascade);

        // SanPham - ChiTietDonHang: 1 - nhiều
        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(ct => ct.SanPham)
            .WithMany(s => s.ChiTietDonHangs)
            .HasForeignKey(ct => ct.MaSanPham)
            .OnDelete(DeleteBehavior.Restrict);

        // Giá sản phẩm
        modelBuilder.Entity<SanPham>()
            .Property(s => s.Gia)
            .HasPrecision(18, 2);

        // Tổng tiền đơn hàng
        modelBuilder.Entity<DonHang>()
            .Property(d => d.TongTien)
            .HasPrecision(18, 2);

        // Đơn giá chi tiết đơn hàng
        modelBuilder.Entity<ChiTietDonHang>()
            .Property(ct => ct.DonGia)
            .HasPrecision(18, 2);

        // Không cho phép trùng tên đăng nhập
        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.TenDangNhap)
            .IsUnique();

        // Không cho phép một khách hàng có nhiều tài khoản
        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(t => t.MaKhachHang)
            .IsUnique();
    }
}