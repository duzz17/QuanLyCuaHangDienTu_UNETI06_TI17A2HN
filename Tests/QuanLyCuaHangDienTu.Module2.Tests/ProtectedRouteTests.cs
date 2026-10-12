using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using QuanLyCuaHangDienTu_UNETI06_TI17A2HN.Filters;
using Xunit;

namespace QuanLyCuaHangDienTu.Module2.Tests;

public class ProtectedRouteTests
{
    [Fact]
    public void AnonymousPostToAddCartRedirectsToLoginAndReturnsToReferrer()
    {
        var context = CreateContext("GioHang", "ThemVaoGio", HttpMethods.Post);
        context.HttpContext.Request.Scheme = "https";
        context.HttpContext.Request.Host = new HostString("shop.test");
        context.HttpContext.Request.Headers.Referer = "https://shop.test/SanPham/Details/7?view=store";

        new ProtectedRouteAttribute().OnActionExecuting(context);

        var redirect = Assert.IsType<RedirectToActionResult>(context.Result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("/SanPham/Details/7?view=store", redirect.RouteValues?["returnUrl"]);
    }

    [Fact]
    public void AuthenticatedRequestContinues()
    {
        var context = CreateContext("GioHang", "Index", HttpMethods.Get);
        context.HttpContext.Session.SetString("VaiTro", "KhachHang");

        new ProtectedRouteAttribute().OnActionExecuting(context);

        Assert.Null(context.Result);
    }

    private static ActionExecutingContext CreateContext(
        string controller,
        string action,
        string method)
    {
        var httpContext = new DefaultHttpContext
        {
            Session = new TestSession()
        };
        httpContext.Request.Method = method;
        var routeData = new RouteData();
        routeData.Values["controller"] = controller;
        routeData.Values["action"] = action;
        var actionContext = new ActionContext(
            httpContext,
            routeData,
            new ActionDescriptor(),
            new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object());
    }
}
