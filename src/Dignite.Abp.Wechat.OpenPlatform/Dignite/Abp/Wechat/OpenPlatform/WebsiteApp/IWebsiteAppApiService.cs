using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;

public interface IWebsiteAppApiService : ITransientDependency
{    
    Task<string> GetQrConnectionUrlAsync(string callbackUrl);


    /// <summary>
    /// 获取微信用户的AccessToken信息
    /// </summary>
    /// <param name="openId"></param>
    /// <returns></returns>
    Task<WebsiteAppUserAccessToken> GetAccessTokenAsync(string openId);

    /// <summary>
    /// 用 code 换取AccessToken信息
    /// </summary>
    /// <param name="code"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    Task<WebsiteAppUserAccessToken> ExchangeAccessTokenAsync(string code, string state);

    /// <summary>
    /// 获取微信用户信息
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    Task<WebsiteAppUserInfo> GetUserInfoAsync(WebsiteAppUserAccessToken accessToken);

    /// <summary>
    /// 刷新AccessToken信息
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    Task RefreshTokenAsync(WebsiteAppUserAccessToken accessToken);
}
