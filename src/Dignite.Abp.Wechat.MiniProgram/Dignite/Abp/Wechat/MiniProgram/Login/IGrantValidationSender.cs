using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Dignite.Abp.Wechat.MiniProgram.Login;

/// <summary>
/// 微信小程序在拿到code，向授权中心发起登录.
/// 授权中心使用code进一步获取到用户的openId
/// </summary>
public interface IGrantValidationSender : ITransientDependency
{
    Task<OAuthAccessToken> ValidateAsync(string code);
}
