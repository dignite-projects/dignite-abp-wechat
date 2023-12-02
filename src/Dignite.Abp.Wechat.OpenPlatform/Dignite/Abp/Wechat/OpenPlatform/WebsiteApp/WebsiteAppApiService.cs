using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dignite.Abp.Wechat.OpenPlatform.Settings;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp;
using Volo.Abp.Caching;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;

public class WebsiteAppApiService : ApiService, IWebsiteAppApiService
{
    private const string accessTokenCacheKey = "dignite-abp-wechat-OpenPlatform-websiteapp-accesstoken-cache-{0}";

    private readonly IHttpContextAccessor _accessor;
    private readonly IDistributedCache<WebsiteAppUserAccessToken> _accessTokenCache;

    public WebsiteAppApiService(
        IHttpContextAccessor accessor,
        IDistributedCache<WebsiteAppUserAccessToken> accessTokenCache
        )
    {
        _accessor = accessor;
        _accessTokenCache = accessTokenCache;
    }

    /// <summary>
    /// 获取微信扫码登录的URL
    /// </summary>
    /// <param name="callbackUrl">在扫码确认登录后返回的链接地址</param>
    /// <returns></returns>
    public async Task<string> GetQrConnectionUrlAsync([NotNull] string callbackUrl)
    {
        var state = Guid.NewGuid().ToString("N");
        var appId = await SettingProvider.GetOrNullAsync(WechatOpenPlatformSettings.AppId);

        var url =
            string.Format("https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect",
                            appId,
                            WebUtility.UrlEncode(callbackUrl),
                            state);

        return url;
    }

    /// <summary>
    /// 获取授权用户的Token
    /// </summary>
    /// <param name="openId"></param>
    /// <returns></returns>
    public async Task<WebsiteAppUserAccessToken> GetAccessTokenAsync([NotNull] string openId)
    {
        var result = await _accessTokenCache.GetAsync(string.Format(accessTokenCacheKey, openId));
        if (result != null)
        {
            //提前10秒刷新AccessToken
            if (result.AccessTokenExpireTime > DateTime.Now.AddSeconds(-10))
            {
                await RefreshTokenAsync(result);
            }
        }
        else
        {
            /* 在用户第一次登陆时，已通用code换取了token，这里的result不可能为null */
        }

        return result;
    }

    /// <summary>
    /// 拿code交换token
    /// </summary>
    /// <param name="code"></param>
    /// <param name="state"><see cref="GetAuthorizeUrlAsync"/>方法中设定的值，在换取Token时，需要将该值再次传到服务器做验证</param>
    /// <returns></returns>
    public async Task<WebsiteAppUserAccessToken> ExchangeAccessTokenAsync([NotNull] string code, [NotNull] string state)
    {
        ////验证state
        //if (state != _accessor.HttpContext.Session.GetString(OAuthConsts.AuthorizeStateName))
        //{
        //    _accessor.HttpContext.Session.Remove(OAuthConsts.AuthorizeStateName);

        //    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
        //    //建议用完之后就清空，将其一次性使用
        //    //实际上可以存任何想传递的数据，比如用户ID
        //    throw new Volo.Abp.UserFriendlyException("异常:传递的state值与服务器端state不一致！");
        //}
        //else
        //{
        //    _accessor.HttpContext.Session.Remove(OAuthConsts.AuthorizeStateName);
        //}

        var appId = await SettingProvider.GetOrNullAsync(WechatOpenPlatformSettings.AppId);
        var secret = await SettingProvider.GetOrNullAsync(WechatOpenPlatformSettings.Secret);

        var requestUrl = QueryHelpers.AddQueryString("https://api.weixin.qq.com/sns/oauth2/access_token", new Dictionary<string, string>
        {
            { "appid", appId },
            { "secret", secret },
            { "code",code },
            { "grant_type", "authorization_code" },
        });


        var client = ClientFactory.CreateClient(OpenPlatformConsts.HttpClientName);
        var response = await client.GetAsync(requestUrl, _accessor.HttpContext.RequestAborted);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"An error occurred when exchanging user access token information ({response.StatusCode}). Please check if the authentication information is correct.");

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<WebsiteAppUserAccessToken>(content);


        if (result.ErrorCode != 0)
        {
            throw new BusinessException(WechatOpenPlatformErrorCodes.OpenPlatform.prefix + result.ErrorCode);
        }

        //将获取到的 AccessTokenResult 添加到缓存中
        //根据 openid 从缓存中获取 AccessTokenResult
        result.AccessTokenExpireTime = DateTime.Now.AddSeconds(result.ExpiresIn);
        result.RefreshTokenExpireTime = DateTime.Now.AddDays(30);
        await _accessTokenCache.SetAsync(string.Format(accessTokenCacheKey, result.OpenId), result, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(result.ExpiresIn)
        });

        return result;
    }

    public async Task<WebsiteAppUserInfo> GetUserInfoAsync(WebsiteAppUserAccessToken accessToken)
    {
        var requestUrl = QueryHelpers.AddQueryString("https://api.weixin.qq.com/sns/userinfo", new Dictionary<string, string>
            {
                { "access_token", accessToken.AccessToken },
                { "openid", accessToken.OpenId },
                { "lang", Language.zh_CN.ToString("g") },
            });

        var client = ClientFactory.CreateClient(OpenPlatformConsts.HttpClientName);
        var response = await client.GetAsync(requestUrl, _accessor.HttpContext.RequestAborted);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"An error occurred when retrieving wechat user information ({response.StatusCode}). ");

        var content = await response.Content.ReadAsStringAsync();
        try
        {
            //因为这里还不确定用户是否关注本微信公众号，所以只能试探性地获取一下
            var userInfo = JsonSerializer.Deserialize<WebsiteAppUserInfo>(content);
            return userInfo;
        }
        catch (Exception ex)
        {
            //未关注，无法得到详细信息
            //这里的 content 可能为："{\"errcode\":40003,\"errmsg\":\"invalid openid\"}"
            //todo：修改AuthencationUserInfo，继承自WechatResult，开发者通过errcode判断是否成功请求到数据；
            //todo: 第二种方案，在第一次获取到用户信息后，存储到自己的服务器上，以免频繁获取微信上的用户信息。

            return null;
        }

    }
    public async Task RefreshTokenAsync(WebsiteAppUserAccessToken accessToken)
    {
        var appId = await SettingProvider.GetOrNullAsync(WechatOpenPlatformSettings.AppId);

        var requestUrl = QueryHelpers.AddQueryString("https://api.weixin.qq.com/sns/oauth2/refresh_token", new Dictionary<string, string>
        {
            { "appid", appId },
            { "grant_type", "refresh_token" },
            { "refresh_token", accessToken.RefreshToken }
        });

        var client = ClientFactory.CreateClient(OpenPlatformConsts.HttpClientName);
        var response = await client.GetAsync(requestUrl, _accessor.HttpContext.RequestAborted);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"An error occurred when refreshing token ({response.StatusCode}).");

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<WebsiteAppUserAccessToken>(content);


        if (result.ErrorCode != 0)
        {
            throw new BusinessException(WechatOpenPlatformErrorCodes.OpenPlatform.prefix + result.ErrorCode);
        }

        //将获取到的 AccessTokenResult 添加到缓存中
        //根据 openid 从缓存中获取 AccessTokenResult
        accessToken.AccessTokenExpireTime = DateTime.Now.AddSeconds(result.ExpiresIn);
        accessToken.AccessToken = result.AccessToken;
        await _accessTokenCache.SetAsync(string.Format(accessTokenCacheKey, result.OpenId), accessToken, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(result.ExpiresIn)
        });

    }
}
