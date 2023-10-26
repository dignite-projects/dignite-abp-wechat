using Microsoft.AspNetCore.Http;

namespace Dignite.Abp.Wechat.OfficialAccount.WebApp;

/// <summary>
/// 授权验证的上下文信息
/// </summary>
public class GrantValidationContext
{
    public HttpContext HttpContext { get; }

    public OfficialAccountUserAccessToken AccessToken { get; }

    public OfficialAccountUserInfo UserInfo { get; }

    public GrantValidationContext(HttpContext httpContext, OfficialAccountUserAccessToken accessToken, OfficialAccountUserInfo userInfo = null)
    {
        HttpContext = httpContext;
        UserInfo = userInfo;
        AccessToken = accessToken;
    }
}
