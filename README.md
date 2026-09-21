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
