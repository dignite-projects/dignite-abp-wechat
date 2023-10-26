namespace Dignite.Abp.Wechat.MiniProgram;

public class MiniProgramConsts
{
    public const string HttpClientName = "DigniteAbpWechatMiniProgramHttpClient";

    /// <summary>
    /// The name of the "LoginProvider" used for WeChat app third-party login.
    /// </summary>
    public const string MiniProgramLoginProvider = "wechat-miniprogram";


    /// <summary>
    /// WeChat mini program authorization login url
    /// </summary>
    public const string SignInPath = "/wechat/miniprogram/signin";
}
