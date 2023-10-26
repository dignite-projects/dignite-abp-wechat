using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Dignite.Abp.Wechat.MiniProgram;

public interface IStartApiService : ITransientDependency
{
    /// <summary>
    /// Get the global AccessToken of WeChat MiniProgram
    /// </summary>
    /// <returns></returns>
    Task<GlobalAccessToken> GetAccessTokenAsync();

    /// <summary>
    /// Refresh the global AccessToken of the WeChat MiniProgram
    /// </summary>
    /// <returns></returns>
    Task<GlobalAccessToken> RefreshAccessTokenAsync();
}
