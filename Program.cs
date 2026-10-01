// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Cấu hình kết nối SQL Server, đăng ký MVC, Session và pipeline HTTP (Module 1 - M1-02, M1-07)

using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Data;

var builder = WebApplication.CreateBuilder(args);

// Kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

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

// Khởi tạo CSDL & Khởi tạo dữ liệu mẫu nếu chưa có
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra trong quá trình khởi tạo CSDL.");
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