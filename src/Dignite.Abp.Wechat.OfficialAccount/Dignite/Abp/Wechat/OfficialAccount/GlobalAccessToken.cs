using System.Text.Json.Serialization;

namespace Dignite.Abp.Wechat.OfficialAccount;

/// Global access token for WeChat OfficialAccount
public class GlobalAccessToken : WechatResult
{
    /// <summary>
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// validity time
    /// Unit:seconds
    /// Currently within 7200 seconds
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
