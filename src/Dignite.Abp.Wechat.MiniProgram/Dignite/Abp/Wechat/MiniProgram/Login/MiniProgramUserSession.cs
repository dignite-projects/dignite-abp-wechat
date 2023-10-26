using System.Text.Json.Serialization;

namespace Dignite.Abp.Wechat.MiniProgram.Login;

/// <summary>
/// 
/// </summary>
public class MiniProgramUserSession : WechatResult
{
    /// <summary>
    /// 用户唯一标识
    /// </summary>
    [JsonPropertyName("OpenId")]
    public string OpenId { get; set; }

    /// <summary>
    /// 会话密钥
    /// </summary>
    [JsonPropertyName("session_key")]
    public string SessionKey { get; set; }

    /// <summary>
    /// 用户在开放平台的唯一标识符，若当前小程序已绑定到微信开放平台账号下会返回，详见 UnionID 机制说明。
    /// </summary>
    [JsonPropertyName("unionid")]
    public string UnionId { get; set; }
}
