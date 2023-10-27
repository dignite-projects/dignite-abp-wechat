using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Dignite.Abp.Wechat.MiniProgram.IdentityServer;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dignite.Abp.Wechat.MiniProgram.Login;

public class IdentityServerGrantValidator : IExtensionGrantValidator
{
    private readonly ILoginApiService _loginApiService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityServerGrantValidator(
        ILoginApiService loginApiService,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _loginApiService = loginApiService;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GrantType => IdentityServerConsts.WechatMiniProgramGrantType;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var handler = httpContext.RequestServices.GetService<IGrantValidationHandler>();


            if (handler == null)
                throw new Exception($"请实现{nameof(IGrantValidationHandler)}，并注册依赖！");

            //微信小程序登陆的code
            var code = context.Request.Raw["code"];
            var sessionResult = await _loginApiService.GetSessionTokenAsync(code);

            var claimsPrincipal = await handler.ExcuteAsync(
                new GrantValidationContext(httpContext, sessionResult)
                );


            if (claimsPrincipal != null)
            {
                //授权通过返回
                context.Result = new IdentityServer4.Validation.GrantValidationResult
                (
                    subject: claimsPrincipal.FindUserId()?.ToString(),
                    authenticationMethod: GrantType,
                    claims: claimsPrincipal.Claims
                );
            }
            else
            {
                context.Result = new IdentityServer4.Validation.GrantValidationResult()
                {
                    IsError = true,
                    Error = "User not found."
                };
            }
        }
        catch (Exception ex)
        {
            context.Result = new IdentityServer4.Validation.GrantValidationResult()
            {
                IsError = true,
                Error = ex.Message
            };
        }
    }
}