using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Dignite.Abp.Wechat
{
    internal static class WechatOAuthTokenResponseExtensions
    {
        public static string GetByKey(this OAuthTokenResponse response, string key)
        {
            return response.Response.RootElement.GetString(key);
        }

        public static string GetUnionId(this OAuthTokenResponse response)
        {
            return response.GetByKey("unionid");
        }

        public static string GetOpenId(this OAuthTokenResponse response)
        {
            return response.GetByKey("openid");
        }

        public static string GetScope(this OAuthTokenResponse response)
        {
            return response.GetByKey("scope");
        }

        public static string GetErrorCode(this OAuthTokenResponse response)
        {
            return response.GetByKey("errcode");
        }

        public static string GetErrorMsg(this OAuthTokenResponse response)
        {
            return response.GetByKey("errmsg");
        }
    }
}
