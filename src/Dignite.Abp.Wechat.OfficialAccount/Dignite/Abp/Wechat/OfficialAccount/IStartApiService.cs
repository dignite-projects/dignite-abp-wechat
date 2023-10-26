using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Dignite.Abp.Wechat.OfficialAccount;

public interface IStartApiService : ITransientDependency
{
    /// <summary>
    /// Get the global AccessToken of WeChat OfficialAccount
    /// </summary>
    /// <returns></returns>
    Task<GlobalAccessToken> GetAccessTokenAsync();

    /// <summary>
    /// Refresh the global AccessToken of the WeChat OfficialAccount
    /// </summary>
    /// <returns></returns>
    Task<GlobalAccessToken> RefreshAccessTokenAsync();
}
