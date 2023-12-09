using Dignite.Abp.Wechat.OpenPlatform.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Dignite.Abp.Wechat.OpenPlatform.Settings;

public class WechatOpenPlatformSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        var definitions = new SettingDefinition[] {
            new SettingDefinition(
                name:WechatOpenPlatformSettings.WebsiteAppId,
                displayName:L("WechatOpenPlatformWebsiteAppId"),
                isVisibleToClients:false,
                isEncrypted:false),

            new SettingDefinition(
                name:WechatOpenPlatformSettings.WebsiteSecret,
                displayName:L("WechatOpenPlatformWebsiteAppSecret"),
                isVisibleToClients:false,
                isEncrypted:false)
        };

        context.Add(definitions);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DigniteAbpWechatOpenPlatformResource>(name);
    }
}