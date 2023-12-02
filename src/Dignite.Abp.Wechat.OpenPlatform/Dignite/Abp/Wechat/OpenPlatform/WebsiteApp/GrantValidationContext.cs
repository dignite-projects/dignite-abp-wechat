using Microsoft.AspNetCore.Http;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;

/// <summary>
/// 授权验证的上下文信息
/// </summary>
public class GrantValidationContext
{
    public HttpContext HttpContext { get; }

    public WebsiteAppUserAccessToken AccessToken { get; }

    public WebsiteAppUserInfo UserInfo { get; }

    public GrantValidationContext(HttpContext httpContext, WebsiteAppUserAccessToken accessToken, WebsiteAppUserInfo userInfo = null)
    {
        HttpContext = httpContext;
        UserInfo = userInfo;
        AccessToken = accessToken;
    }
}
