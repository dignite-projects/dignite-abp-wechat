using Dignite.Abp.Wechat.OfficialAccount.WebApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dignite.Abp.Wechat.OfficialAccount;

public static class OfficialAccountExtensions
{
    public static IServiceCollection AddWechatOfficialAccount(this IServiceCollection services)
    {
        services.AddHttpClient(OfficialAccountConsts.HttpClientName, client =>
        {
            // Unify the client configuration here
        });

        return services;
    }

    /// <summary>
    /// Adding WeChat Official Account Webapp login middleware to asp.net core middleware
    /// The developer initiates login authorization via the <see cref="OfficialAccountConsts.SignInPath"/> url
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseWechatOfficialAccount(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<WebAppGrantValidationMiddleware>()
            .UseMiddleware<AuthorizationUrlMiddleware>()
            .UseMiddleware<JsapiSignatureMiddleware>();
    }

    /// <summary>
    /// After getting the information of the WeChat official account user, call <see cref="IGrantValidationSender.ValidateAsync(string, string)"/> to call this method inside it to validate the user authorization logic, and ultimately in the IdentityServer or OpenIddict Wrapping authorization information inside the IdentityServer or OpenIddict
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebAppGrantValidationHandler<THandler>(this IServiceCollection services)
           where THandler : class, IGrantValidationHandler
    {
        return services.AddScoped<IGrantValidationHandler, THandler>();
    }
}
