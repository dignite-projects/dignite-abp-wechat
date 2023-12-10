using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp.Authentication;
public class WebsiteAppOptions : OAuthOptions
{
    public WebsiteAppOptions()
    {
        CallbackPath = WebsiteAppDefaults.CallbackPath;
        AuthorizationEndpoint = WebsiteAppDefaults.AuthorizationEndpoint;
        TokenEndpoint = WebsiteAppDefaults.TokenEndpoint;
        UserInformationEndpoint = WebsiteAppDefaults.UserInformationEndpoint;
        LanguageCode = Language.zh_CN;
        Scope.Add("snsapi_login");

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");

        ClaimActions.MapJsonKey(WechatClaimTypes.UnionId, "unionid");
        ClaimActions.MapJsonKey(WechatClaimTypes.OpenId, "openid");
        ClaimActions.MapJsonKey(WechatClaimTypes.NickName, "nickname");
        ClaimActions.MapJsonKey(WechatClaimTypes.Sex, "sex");
        ClaimActions.MapJsonKey(WechatClaimTypes.Province, "province");
        ClaimActions.MapJsonKey(WechatClaimTypes.Country, "country");
        ClaimActions.MapJsonKey(WechatClaimTypes.HeadImageUrl, "headimgurl");
        ClaimActions.MapJsonKey(WechatClaimTypes.Privilege, "privilege");
        ClaimActions.MapJsonKey(WechatClaimTypes.Scope, "scope");
    }

    /// <summary>
    /// Check that the options are valid.  Should throw an exception if things are not ok.
    /// </summary>
    public override void Validate()
    {
        if (string.IsNullOrEmpty(AppId))
        {
            throw new ArgumentException($"{nameof(AppId)} must be provided", nameof(AppId));
        }

        if (string.IsNullOrEmpty(AppSecret))
        {
            throw new ArgumentException($"{nameof(AppSecret)} must be provided", nameof(AppSecret));
        }

        base.Validate();
    }


    // Wechat uses a non-standard term for this field.
    /// <summary>
    /// Gets or sets the Wechat-assigned App ID.
    /// </summary>
    public string AppId {
        get { return ClientId; }
        set { ClientId = value; }
    }

    // Wechat uses a non-standard term for this field.
    /// <summary>
    /// Gets or sets the Wechat-assigned app secret.
    /// </summary>
    public string AppSecret {
        get { return ClientSecret; }
        set { ClientSecret = value; }
    }

    /// <summary>
    /// 国家地区语言版本，支持zh_CN 简体（默认），zh_TW 繁体，en 英语等三种。
    /// </summary>
    /// <remarks>在获取用户信息时用到</remarks>
    public Language LanguageCode { get; set; }
}
