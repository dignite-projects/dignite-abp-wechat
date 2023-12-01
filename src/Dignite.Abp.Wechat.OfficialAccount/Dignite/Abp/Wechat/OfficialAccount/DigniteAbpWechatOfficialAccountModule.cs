using Dignite.Abp.Wechat.OfficialAccount.Localization;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Dignite.Abp.Wechat.OfficialAccount;

[DependsOn(
    typeof(AbpAspNetCoreSerilogModule),
    typeof(DigniteAbpWechatModule)
    )]
public class DigniteAbpWechatOfficialAccountModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<DigniteAbpWechatOfficialAccountModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<DigniteAbpWechatOfficialAccountResource>("en")
                .AddVirtualJson("/Dignite/Wechat/OfficialAccount/Localization/Resources");
        });


        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Wechat.OfficialAccount:", typeof(DigniteAbpWechatOfficialAccountResource));
        });

        //Add WeChat official account configuration service
        context.Services.AddWechatOfficialAccount();
    }

}