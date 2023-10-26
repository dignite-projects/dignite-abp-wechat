using Dignite.Abp.Wechat.MiniProgram.Login;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Dignite.Abp.Wechat.MiniProgram.IdentityServer;

[DependsOn(
    typeof(DigniteAbpWechatMiniProgramModule)
)]
public class DigniteAbpWechatMiniProgramIdentityServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        //配置Identity Server授权验证
        PreConfigure<IIdentityServerBuilder>(builder =>
        {
            builder.AddExtensionGrantValidator<IdentityServerGrantValidator>();
        });
    }
}