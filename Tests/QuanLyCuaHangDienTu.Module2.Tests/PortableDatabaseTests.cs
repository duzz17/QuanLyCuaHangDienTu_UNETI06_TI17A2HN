using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;
using Xunit;

namespace QuanLyCuaHangDienTu.Module2.Tests;

public class PortableDatabaseTests
{
    [Fact]
    public async Task InitializeAsync_TaoDatabaseSQLiteVaDuLieuMauTrenMayMoi()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"qlchdt-{Guid.NewGuid():N}.db");

        try
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            await using (var context = new ApplicationDbContext(options))
            {
                await DbInitializer.InitializeAsync(context);

                Assert.True(File.Exists(databasePath));
                Assert.Equal(5, await context.LoaiSanPhams.CountAsync());
                Assert.Equal(25, await context.SanPhams.CountAsync());
                Assert.Equal(3, await context.KhachHangs.CountAsync());
                Assert.Equal(2, await context.TaiKhoans.CountAsync());
                Assert.Single(await context.DonHangs.ToListAsync());
                Assert.Equal(2, await context.ChiTietDonHangs.CountAsync());
            }

            await using (var context = new ApplicationDbContext(options))
            {
                await DbInitializer.InitializeAsync(context);

                Assert.Equal(5, await context.LoaiSanPhams.CountAsync());
                Assert.Equal(25, await context.SanPhams.CountAsync());
                Assert.Single(await context.DonHangs.ToListAsync());
            }
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(databasePath);
            File.Delete($"{databasePath}-shm");
            File.Delete($"{databasePath}-wal");
        }
    }
}
