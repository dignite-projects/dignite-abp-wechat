using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp.Authentication;
public class WechatOptions : OAuthOptions
{
    public WechatOptions()
    {
        CallbackPath = WechatDefaults.CallbackPath;
        AuthorizationEndpoint = WechatDefaults.AuthorizationEndpoint;
        TokenEndpoint = WechatDefaults.TokenEndpoint;
        UserInformationEndpoint = WechatDefaults.UserInformationEndpoint;
        LanguageCode = WeixinOpenLanguageCodes.zh_CN;
        Scope.Add("snsapi_login");

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");

        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.UnionId, "unionid");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.OpenId, "openid");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.NickName, "nickname");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.Sex, "sex");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.Province, "province");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.Country, "country");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.HeadImageUrl, "headimgurl");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.Privilege, "privilege");
        ClaimActions.MapJsonKey(WeixinOpenClaimTypes.Scope, "scope");
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
    public WeixinOpenLanguageCodes LanguageCode { get; set; }
}
