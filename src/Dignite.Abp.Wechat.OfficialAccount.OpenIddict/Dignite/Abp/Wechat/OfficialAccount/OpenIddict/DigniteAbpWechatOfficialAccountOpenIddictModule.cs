using Dignite.Abp.Wechat.OfficialAccount.WebApp;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;

namespace Dignite.Abp.Wechat.OfficialAccount.OpenIddict;

[DependsOn(
    typeof(AbpOpenIddictAspNetCoreModule),
    typeof(DigniteAbpWechatOfficialAccountModule)
)]
public class DigniteAbpWechatOfficialAccountOpenIddictModule : AbpModule
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