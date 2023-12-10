namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp.Authentication;
public static class WebsiteAppDefaults
{
    /// <summary>
    /// The default scheme for Wechat Open Platform Website authentication. The value is <c>wechat-open-website</c>.
    /// </summary>
    public const string AuthenticationScheme = "wechat-open-website";

    /// <summary>
    /// The default display name for Wechat Open Platform Website authentication. Defaults to <c>Wechat</c>.
    /// </summary>
    public static readonly string DisplayName = "微信扫码登录";

    /// <summary>
    /// /signin-weixinopen
    /// </summary>
    public const string CallbackPath = "/signin-wechat-open";


    /// <summary>
    /// The default endpoint used to perform Google authentication.
    /// </summary>
    /// <remarks>
    /// For more details about this endpoint, see <see href="https://developers.weixin.qq.com/doc/oplatform/Website_App/WeChat_Login/Wechat_Login.html"/>.
    /// </remarks>
    public static readonly string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/qrconnect";

    /// <summary>
    /// The OAuth endpoint used to exchange access tokens.
    /// </summary>
    public static readonly string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

    /// <summary>
    /// The Google endpoint that is used to gather additional user information.
    /// </summary>
    public static readonly string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";
}
