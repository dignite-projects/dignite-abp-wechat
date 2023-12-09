using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Dignite.Abp.Wechat.OpenPlatform.Localization;

namespace Dignite.Abp.Wechat.OpenPlatform;
[DependsOn(
    typeof(AbpAspNetCoreSerilogModule),
    typeof(DigniteAbpWechatModule)
    )]
public class DigniteAbpWechatOpenPlatformModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<DigniteAbpWechatOpenPlatformModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<DigniteAbpWechatOpenPlatformResource>("en")
                .AddVirtualJson("/Dignite/Wechat/OpenPlatform/Localization/Resources");
        });


        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Wechat.OpenPlatform:", typeof(DigniteAbpWechatOpenPlatformModule));
        });
    }
}
