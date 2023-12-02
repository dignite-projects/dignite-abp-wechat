using Dignite.Abp.Wechat.OpenPlatform.WebsiteApp;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Dignite.Abp.Wechat.OpenPlatform.IdentityServer;

[DependsOn(
    typeof(DigniteAbpWechatOpenPlatformModule)
)]
public class DigniteAbpWechatOpenPlatformIdentityServerModule : AbpModule
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