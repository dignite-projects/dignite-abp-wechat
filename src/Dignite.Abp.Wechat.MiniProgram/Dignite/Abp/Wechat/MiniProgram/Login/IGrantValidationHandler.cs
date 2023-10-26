
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dignite.Abp.Wechat.MiniProgram.Login;

/// <summary>
/// 通过本接口定制自己的登陆逻辑。
/// </summary>
/// <remarks>
/// 在拿到微信小程序用户信息以后，调用<see cref="IGrantValidationSender.ValidateAsync(string, string)"/>方法，该方法内部通过“grant_type”查找到相应的扩展登陆的实现，在实现中会调用<see cref="IGrantValidationHandler.ExcuteAsync(Dignite.Abp.Wechat.MiniProgram.Login.GrantValidationContext)"/>，验证微信小程序用户的登陆逻辑，最终返回需要的数据。
/// </remarks>
public interface IGrantValidationHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context">包含微信小程序的用户信息和其它相关信息的上下文对象</param>
    Task<ClaimsPrincipal> ExcuteAsync(GrantValidationContext context);
}
