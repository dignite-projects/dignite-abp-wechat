using Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dignite.Abp.Wechat.OpenPlatform;

public static class OpenPlatformExtensions
{
    public static IServiceCollection AddWechatOpenPlatform(this IServiceCollection services)
    {
        services.AddHttpClient(OpenPlatformConsts.HttpClientName, client =>
        {
            // Unify the client configuration here
        });

        return services;
    }

    public static IApplicationBuilder UseWechatOpenPlatform(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<QrConnectUrlMiddleware>()
            .UseMiddleware<WebsiteAppGrantValidationMiddleware>();
    }

    /// <summary>
    /// After getting the information of the WeChat official account user, call <see cref="IGrantValidationSender.ValidateAsync(string, string)"/> to call this method inside it to validate the user authorization logic, and ultimately in the IdentityServer or OpenIddict Wrapping authorization information inside the IdentityServer or OpenIddict
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebsiteAppGrantValidationHandler<THandler>(this IServiceCollection services)
           where THandler : class, IGrantValidationHandler
    {
        return services.AddScoped<IGrantValidationHandler, THandler>();
    }
}
