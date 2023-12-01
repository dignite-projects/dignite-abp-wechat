using Dignite.Abp.Wechat.OfficialAccount.WebApp;

namespace Dignite.Abp.Wechat.OfficialAccount;

public class OfficialAccountConsts
{
    public const string HttpClientName = "DigniteAbpWechatOfficialAccountHttpClient";


    /// <summary>
    /// The name of the "LoginProvider" used for WeChat web app third-party login.
    /// </summary>
    public const string OfficialAccountLoginProvider = "wechat-official-account";


    /// <summary>
    /// Generate URL for WeChat OfficialAccount Webapp authorization URL.
    /// see https://developers.weixin.qq.com/doc/offiaccount/OA_Web_Apps/Wechat_webpage_authorization.html and <see cref="IWebAppApiService.GetAuthorizeUrlAsync(AuthencationScope, string)"/>
    /// </summary>
    public const string AuthorizationPath = "/wechat/webapp/authorization";

    /// <summary>
    /// WeChat OfficialAccount authorization login url
    /// </summary>
    public const string SignInPath = "/wechat/webapp/signin";

    /// <summary>
    /// 获取jsapi加密签名的地址
    /// </summary>
    public const string JsapiSignaturePath = "/api/wechat/webapp/get-jsapi-signature";
}
