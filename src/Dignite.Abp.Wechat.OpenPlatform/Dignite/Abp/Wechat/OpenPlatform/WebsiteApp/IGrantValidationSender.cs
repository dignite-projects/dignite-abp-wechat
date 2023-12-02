using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;

/// <summary>
/// 微信公众号登陆的发起者，分别由IdentityServer或OpenIddict实现
/// 由<see cref="WebsiteAppGrantValidationMiddleware"/>发起本接口的调用。
/// </summary>
public interface IGrantValidationSender : ITransientDependency
{
    Task<OAuthAccessToken> ValidateAsync(string code, string state);
}
