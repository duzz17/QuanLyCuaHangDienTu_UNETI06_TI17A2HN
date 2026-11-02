// Họ và tên: Phạm Văn Cương - MSSV: 23103100094
// Chức năng nâng cao: Mã hóa mật khẩu bảo mật bằng thuật toán SHA256
using System.Security.Cryptography;
using System.Text;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Security;

public static class PasswordHelper
{
    /// <summary>
    /// Mã hóa mật khẩu sang chuỗi Hex băm SHA-256
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }

    /// <summary>
    /// Xác thực mật khẩu đăng nhập.
    /// Hỗ trợ cả mật khẩu băm SHA-256 lẫn tự động nhận diện mật khẩu cũ (Backward-compatible).
    /// </summary>
    public static bool VerifyPassword(string inputPassword, string storedPassword)
    {
        if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
            return false;

        // 1. Đối chiếu dạng băm SHA-256
        var hashedInput = HashPassword(inputPassword);
        if (string.Equals(hashedInput, storedPassword, StringComparison.OrdinalIgnoreCase))
            return true;

        // 2. Nhận diện mật khẩu cũ chưa mã hóa
        if (string.Equals(inputPassword, storedPassword, StringComparison.Ordinal))
            return true;

        return false;
    }
}
