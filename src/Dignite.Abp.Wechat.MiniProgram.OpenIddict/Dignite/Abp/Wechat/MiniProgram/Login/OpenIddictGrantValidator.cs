using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Dignite.Abp.Wechat.MiniProgram.OpenIddict;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;

namespace Dignite.Abp.Wechat.MiniProgram.Login;

public class OpenIddictGrantValidator : ITokenExtensionGrant
{
    public string Name => OpenIddictConsts.ExtensionGrantName;

    public async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
    {
        try
        {
            var _loginApiService = context.HttpContext.RequestServices.GetService<ILoginApiService>();
            var handler = context.HttpContext.RequestServices.GetService<IGrantValidationHandler>();

            if (handler == null)
                throw new Exception($"请实现{nameof(IGrantValidationHandler)}，并注册依赖！");

            //微信小程序登陆的code
            var code = context.HttpContext.Request.Form["code"];
            var sessionResult = await _loginApiService.GetSessionTokenAsync(code);

            var claimsPrincipal = await handler.ExcuteAsync(
                new GrantValidationContext(context.HttpContext, sessionResult)
                );

            if (claimsPrincipal!=null)
            {
                var scopes = context.Request.GetScopes();
                claimsPrincipal.SetScopes(scopes);
                var resources = await GetResourcesAsync(context,scopes);
                claimsPrincipal.SetResources(resources);

                return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
            }
            else
            {
                return ForbidResult("User not found.");
            }
        }
        catch (Exception ex)
        {
            return ForbidResult(ex.Message);
        }
    }

    private ForbidResult ForbidResult(string msg)
    {
        return new ForbidResult(
            new[] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme },
            GetAuthenticationProperties(msg));
    }

    private AuthenticationProperties GetAuthenticationProperties(string msg)
    {
        return new AuthenticationProperties(new Dictionary<string, string>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = msg
        });
    }

    protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ExtensionGrantContext context, ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in context.HttpContext.RequestServices.GetRequiredService<IOpenIddictScopeManager>().ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }
        return resources;
    }
}


