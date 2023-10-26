using Dignite.Abp.Wechat.MiniProgram.Login;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;

namespace Dignite.Abp.Wechat.MiniProgram.OpenIddict;

[DependsOn(
    typeof(AbpOpenIddictAspNetCoreModule),
    typeof(DigniteAbpWechatMiniProgramModule)
)]
public class DigniteAbpWechatMiniProgramOpenIddictModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<OpenIddictServerBuilder>(builder =>
        {
            builder.Configure(options =>
            {
                options.GrantTypes.Add(OpenIddictConsts.ExtensionGrantName);
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpOpenIddictExtensionGrantsOptions>(options =>
        {
            options.Grants.Add(OpenIddictConsts.ExtensionGrantName, new OpenIddictGrantValidator());
        });
    }
}