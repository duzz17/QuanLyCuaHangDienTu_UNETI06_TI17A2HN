using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;

public sealed class ProtectedRouteAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!string.IsNullOrWhiteSpace(context.HttpContext.Session.GetString("VaiTro")))
        {
            base.OnActionExecuting(context);
            return;
        }

        var request = context.HttpContext.Request;
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var returnUrl = request.PathBase + request.Path + request.QueryString;

        if (!HttpMethods.IsGet(request.Method))
        {
            returnUrl = (controllerName, actionName) switch
            {
                ("DonHang", "DatHang") => "/DonHang/DatHang",
                ("GioHang", "ThemVaoGio") => GetLocalReferrer(request) ?? "/SanPham?view=store",
                _ => "/GioHang"
            };
        }

        context.Result = new RedirectToActionResult(
            "Login",
            "TaiKhoan",
            new { returnUrl });
    }

    private static string? GetLocalReferrer(HttpRequest request)
    {
        var referer = request.Headers.Referer.ToString();
        if (!Uri.TryCreate(referer, UriKind.Absolute, out var uri) ||
            !string.Equals(uri.Scheme, request.Scheme, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.Authority, request.Host.Value, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return uri.PathAndQuery;
    }
}
