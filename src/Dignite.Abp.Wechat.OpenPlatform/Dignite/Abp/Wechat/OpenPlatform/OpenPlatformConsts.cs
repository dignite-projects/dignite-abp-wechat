
using Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;

namespace Dignite.Abp.Wechat.OpenPlatform;

public class OpenPlatformConsts
{
    public const string HttpClientName = "DigniteAbpWechatOpenPlatformHttpClient";


    /// <summary>
    /// The name of the "LoginProvider" used for WeChat Open Platform third-party login.
    /// </summary>
    public const string OpenPlatformLoginProvider = "wechat-open-platform";

    /// <summary>
    /// Generate URL for WeChat OpenPlatform Website App connect URL.
    /// see https://developers.weixin.qq.com/doc/oplatform/Website_App/WeChat_Login/Wechat_Login.html and <see cref="IWebsiteAppApiService.GetQrConnectionUrlAsync(string)"/>
    /// </summary>
    public const string QrConnectPath = "/wechat/website-app/qrconnect";


    /// <summary>
    /// WeChat OfficialAccount authorization login url
    /// </summary>
    public const string SignInPath = "/wechat/website-app/signin";
}
