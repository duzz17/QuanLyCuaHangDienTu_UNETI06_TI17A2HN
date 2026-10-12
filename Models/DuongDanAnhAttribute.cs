// Họ và tên: Phạm Văn Cường - MSSV: 23103100094
// Nội dung thực hiện: Validation đường dẫn hình ảnh sản phẩm (Module 2)
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Models;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class DuongDanAnhAttribute : RegularExpressionAttribute
{
    private const string ImagePathPattern =
        @"^\s*(?:https?://[^\s]+|~?/images/[^\s]+\.(?:avif|gif|jpe?g|png|svg|webp)(?:\?[^\s#]*)?(?:#[^\s]*)?)\s*$";

    public DuongDanAnhAttribute() : base(ImagePathPattern)
    {
        ErrorMessage = "Hình ảnh phải là URL http/https hoặc đường dẫn /images/... có đuôi ảnh hợp lệ";
    }

    public static bool IsValidPath(string? value) =>
        string.IsNullOrWhiteSpace(value)
        || Regex.IsMatch(value, ImagePathPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
}
