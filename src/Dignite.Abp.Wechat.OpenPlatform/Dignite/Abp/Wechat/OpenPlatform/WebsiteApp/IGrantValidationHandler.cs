using System.Security.Claims;
using System.Threading.Tasks;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;

/// <summary>
/// 通过本接口实现自己的登陆逻辑。
/// </summary>
/// <remarks>
/// 在拿到公众号用户信息以后，调用<see cref="IGrantValidationSender.ValidateAsync(string, string)"/>方法，该方法内部通过“grant_type”查找到相应的扩展登陆的实现，在实现中会调用<see cref="IGrantValidationHandler.ExcuteAsync(Dignite.Abp.Wechat.OfficialAccount.WebApp.GrantValidationContext)"/>，验证公众号用户的登陆逻辑，最终返回需要的数据。
/// </remarks>
public interface IGrantValidationHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context">包含微信小程序的用户信息和其它相关信息的上下文对象</param>
    Task<ClaimsPrincipal> ExcuteAsync(GrantValidationContext context);
}
