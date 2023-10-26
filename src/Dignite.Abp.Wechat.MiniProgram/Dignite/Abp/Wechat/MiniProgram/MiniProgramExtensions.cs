using Dignite.Abp.Wechat.MiniProgram.Login;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dignite.Abp.Wechat.MiniProgram;

public static class MiniProgramExtensions
{
    public static IServiceCollection AddWechatMiniProgram(this IServiceCollection services)
    {
        services.AddHttpClient(MiniProgramConsts.HttpClientName, client =>
        {
            // Unify the client configuration here
        });

        return services;
    }

    /// <summary>
    /// Adding WeChat mini program login middleware to asp.net core middleware
    /// The developer initiates login authorization via the <see cref="MiniProgramConsts.SignInPath"/> url
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseWechatMiniProgram(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MiniProgramGrantValidationMiddleware>();
    }

    /// <summary>
    /// After getting the information of the WeChat miniprogram user, call <see cref="IGrantValidationSender.ValidateAsync(string, string)"/> to call this method inside it to validate the user authorization logic, and ultimately in the IdentityServer or OpenIddict Wrapping authorization information inside the IdentityServer or OpenIddict
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMiniProgramGrantValidationHandler<THandler>(this IServiceCollection services)
           where THandler : class, IGrantValidationHandler
    {
        return services.AddScoped<IGrantValidationHandler, THandler>();
    }
}
