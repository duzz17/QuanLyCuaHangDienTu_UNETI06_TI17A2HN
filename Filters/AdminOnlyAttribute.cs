// Họ và tên: Nguyễn Xuân Đức
// Mã sinh viên: 23103100062
// Nội dung thực hiện: Bộ lọc kiểm tra quyền Admin tại Controller, bảo vệ truy cập trực tiếp qua URL ở server (Module 1 - Tuần 2: M1-08)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;

/// <summary>
/// Bộ lọc phân quyền Admin ở cấp độ Controller / Action (M1-08).
/// Kiểm tra quyền truy cập ở phía Server, ngăn chặn việc truy cập trái phép qua URL thay vì chỉ ẩn menu trên giao diện.
/// </summary>
public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var vaiTro = context.HttpContext.Session.GetString("VaiTro");

        // Trường hợp 1: Chưa đăng nhập
        if (string.IsNullOrEmpty(vaiTro))
        {
            context.Result = new RedirectToActionResult(
                "Login",
                "TaiKhoan",
                new { returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString });
            return;
        }

        // Trường hợp 2: Đã đăng nhập nhưng không phải Admin (ví dụ: Khách hàng cố tình gõ URL admin)
        if (vaiTro != "Admin")
        {
            if (context.Controller is Controller controller)
            {
                controller.TempData["Loi"] = "Bạn không có quyền truy cập trang này. Chức năng chỉ dành cho Quản trị viên (Admin).";
            }
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
