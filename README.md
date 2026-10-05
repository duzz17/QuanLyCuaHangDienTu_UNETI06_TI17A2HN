# 🛒 Quản Lý Cửa Hàng Điện Tử – UNETI

## 1. Clone project

```bash
git clone https://github.com/duzz17/QuanLyCuaHangDienTu_UNETI06_TI17A2HN.git
cd QuanLyCuaHangDienTu_UNETI06_TI17A2HN
```

## 2. Cài đặt môi trường

Yêu cầu:

* Git
* .NET SDK
* Visual Studio 2022 hoặc VS Code
* SQL Server (nếu sử dụng Database)

Kiểm tra:

```bash
git --version
dotnet --version
```

## 3. Cài thư viện

```bash
dotnet restore
```

## 4. Build project

```bash
dotnet build
```

## 5. Chạy project

```bash
dotnet run
```

Hoặc mở file `.sln` bằng Visual Studio và nhấn **F5**.

## 6. Database

Nếu project sử dụng Database, hãy cấu hình `ConnectionStrings` trong:

```text
appsettings.json
```

Sau đó chạy migration nếu cần:

```bash
dotnet ef database update
```

## 7. Làm việc với Git

Lấy code mới nhất:

```bash
git pull
```

Tạo branch mới:

```bash
git switch -c feature/ten-chuc-nang
```

Sau khi hoàn thành:

```bash
git add .
git commit -m "feat: mo ta thay doi"
git push -u origin feature/ten-chuc-nang
```

Sau đó tạo **Pull Request** trên GitHub.

> Không commit các file chứa mật khẩu, API key, connection string bí mật hoặc thư mục `bin/`, `obj/`, `.vs/`.

## Giao diện cửa hàng và dashboard quản trị

- Trang chủ khách hàng: `/Home/Index?view=store`, với danh mục và các sản phẩm mới nhập.
- Danh sách sản phẩm: `/SanPham/Index?view=store`, lọc theo từ khóa, danh mục, thương hiệu, khoảng giá và sắp xếp; giữ điều kiện khi chuyển trang.
- Dashboard admin: `/Dashboard/Index` (cần đăng nhập Admin). Chọn 7, 30 hoặc 90 ngày để xem doanh thu, đơn hàng, sản phẩm bán chạy; theo dõi đơn gần đây và cảnh báo tồn kho.
- Doanh thu được tính từ đơn `DaGiao`, theo **ngày đặt đơn**. Số sản phẩm đang bán và hồ sơ khách hàng là số liệu toàn hệ thống; cảnh báo kho áp dụng cho sản phẩm đang bán còn tối đa 5 đơn vị.
- Admin đăng nhập thành công sẽ vào dashboard; menu **Xem cửa hàng** mở giao diện mua sắm. Các màn hình quản lý danh mục, khách hàng và sản phẩm vẫn dùng các module hiện có.
- Giỏ hàng, thông tin giao hàng, đăng nhập và theo dõi đơn hàng dùng chung phong cách; giao diện thích ứng với máy tính và điện thoại.
- Ảnh sản phẩm được lấy từ dữ liệu hiện có. Khi ảnh bị thiếu hoặc không tải được, giao diện dùng hình minh họa cục bộ và ghi rõ **Hình minh họa**, không thay đổi đường dẫn ảnh trong CSDL.

Chạy `dotnet run`, mở địa chỉ được in trong terminal (profile HTTP mặc định: `http://localhost:5126`). Thiết kế giao diện không yêu cầu thay đổi schema hoặc tạo migration mới.
