using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;

/// <summary>
/// 用于用于扫码登录的URL的中间件
/// </summary>
public class QrConnectUrlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebsiteAppApiService _websiteAppApiService;

    public QrConnectUrlMiddleware(RequestDelegate next, IWebsiteAppApiService websiteAppApiService)
    {
        _next = next;
        _websiteAppApiService = websiteAppApiService;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        
        if (!OpenPlatformConsts.QrConnectPath.Equals(request.Path, StringComparison.InvariantCultureIgnoreCase))
        {
            await _next(context);
            return;
        }

        var returnUrl = request.Query["callbackUrl"].ToString();

        var authorizeUrl = await _websiteAppApiService.GetQrConnectionUrlAsync(returnUrl);
        response.Redirect(authorizeUrl, false);
    }
}
