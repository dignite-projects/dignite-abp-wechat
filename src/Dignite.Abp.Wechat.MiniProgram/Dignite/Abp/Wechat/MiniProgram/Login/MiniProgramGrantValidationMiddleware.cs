using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dignite.Abp.Wechat.MiniProgram.Login;

/// <summary>
/// 微信公众号登陆中间件，向<see cref="IGrantValidationSender.ValidateAsync(string, string)"/>发起验证请求
/// </summary>
public class MiniProgramGrantValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IGrantValidationSender _grantValidationSender;

    public MiniProgramGrantValidationMiddleware(RequestDelegate next, IGrantValidationSender grantValidationSender)
    {
        _next = next;
        _grantValidationSender = grantValidationSender;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;

        if (HttpMethods.IsGet(request.Method))
        {
            //若当前请求不是获取微信网页授权的地址，则跳过处理，直接执行后续中间件
            if (!MiniProgramConsts.SignInPath.Equals(request.Path, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            var code = request.Query["code"];
            var result = await _grantValidationSender.ValidateAsync(code);

            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
        else
        {
            await _next(context);
            return;
        }
    }
}
