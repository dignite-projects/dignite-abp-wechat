using Dignite.Abp.Wechat.OfficialAccount.WebApp;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Dignite.Abp.Wechat.OfficialAccount.IdentityServer;

[DependsOn(
    typeof(DigniteAbpWechatOfficialAccountModule)
)]
public class DigniteAbpWechatOfficialAccountIdentityServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        //配置 IdentityServer 授权验证
        PreConfigure<IIdentityServerBuilder>(builder =>
        {
            builder.AddExtensionGrantValidator<IdentityServerGrantValidator>();
        });
    }
}