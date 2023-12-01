using Dignite.Abp.Wechat.MiniProgram.Localization;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Dignite.Abp.Wechat.MiniProgram;

[DependsOn(
    typeof(AbpAspNetCoreSerilogModule),
    typeof(DigniteAbpWechatModule)
    )]
public class DigniteAbpWechatMiniProgramModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<DigniteAbpWechatMiniProgramModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<DigniteAbpWechatMiniProgramResource>("en")
                .AddVirtualJson("/Dignite/Abp/Wechat/MiniProgram/Localization/Resources");
        });


        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Wechat.MiniProgram:", typeof(DigniteAbpWechatMiniProgramResource));
        });

        //Add WeChat mini program configuration service
        context.Services.AddWechatMiniProgram();
    }
}