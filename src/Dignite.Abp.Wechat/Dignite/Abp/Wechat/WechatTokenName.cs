namespace Dignite.Abp.Wechat
{
    //
    // Summary:
    //     Defines constants for the well-known claim types that can be assigned to a subject.
    //     This class cannot be inherited.
    public static class WechatTokenName
    {
        /// <summary>
        /// openid
        /// </summary>
        public const string OpenId = "openid";

        /// <summary>
        /// unionid
        /// </summary>
        public const string UnionId = "unionid";

        /// <summary>
        /// scope
        /// </summary>
        public const string Scope = "scope";

        /// <summary>
        /// access_token
        /// </summary>
        public const string AccessToken = "access_token";

        /// <summary>
        /// refresh_token
        /// </summary>
        public const string RefreshToken = "refresh_token";

        /// <summary>
        /// expires_at
        /// </summary>
        public const string ExpiresAt = "expires_at";
    }
}
