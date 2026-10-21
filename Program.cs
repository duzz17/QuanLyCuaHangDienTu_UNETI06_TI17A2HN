// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Cấu hình cơ sở dữ liệu, đăng ký MVC, Session và pipeline HTTP (Module 1 - M1-02, M1-07)

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;

// Cấu hình Culture đảm bảo tương thích định dạng số phân cách thập phân chuẩn
var cultureInfo = new CultureInfo("vi-VN");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);

var databaseDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(databaseDirectory);
var databasePath = Path.Combine(databaseDirectory, "QuanLyCuaHangDienTu.db");

// SQLite lưu toàn bộ dữ liệu trong một tệp đi cùng ứng dụng và không cần cài SQL Server.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={databasePath}");
});

// Cấu hình Session (M1-02, M1-07)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Hỗ trợ truy cập HttpContext trong View/Filter nếu cần
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Tự tạo CSDL và dữ liệu mẫu khi chạy trên máy mới.
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra trong quá trình khởi tạo CSDL.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Middleware Session phải đặt trước UseAuthorization (M1-07)
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
