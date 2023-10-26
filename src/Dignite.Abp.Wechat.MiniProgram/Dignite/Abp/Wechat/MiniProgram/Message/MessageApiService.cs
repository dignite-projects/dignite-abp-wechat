using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dignite.Abp.Wechat.MiniProgram.Message;

public class MessageApiService : ApiService, IMessageApiService
{
    private int ErrCode40001Count = 0;
    private readonly IStartApiService _startApiService;

    public MessageApiService(
        IStartApiService startApiService
        )
    {
        _startApiService = startApiService;
    }


    /// <summary>
    /// 发送订阅消息
    /// </summary>
    /// <param name="touser">接收者（用户）的 openid</param>
    /// <param name="template_id">所需下发的订阅模板id</param>
    /// <param name="data">模板内容，格式形如 { "key1": { "value": any }, "key2": { "value": any } }的object</param>
    /// <param name="page">点击模板卡片后的跳转页面，仅限本小程序内的页面。支持带参数,（示例index?foo=bar）。该字段不填则模板无跳转</param>
    /// <returns></returns>
    public async Task<WechatResult> SendSubscribeMessageAsync(string touser, string template_id, IDictionary<string, SubscribeMessageData> data, string page = null)
    {
        var access_token = (await _startApiService.GetAccessTokenAsync()).AccessToken;
        //var miniprogram_state = "developer";
        var json = JsonSerializer.Serialize(new {
            touser,
            template_id,
            // miniprogram_state,
            page,
            data
        });
        var content = new StringContent(json.Replace('[', '}').Replace(']', '}'), Encoding.UTF8, "application/json");


        var client = ClientFactory.CreateClient(Wechat.MiniProgram.MiniProgramConsts.HttpClientName);
        var response = await client.PostAsync("https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token=" + access_token, content);

        var result = JsonSerializer.Deserialize<WechatResult>(await response.Content.ReadAsStringAsync());

        if (result.ErrorCode != 0)
        {
            if (result.ErrorCode == 40001 && ErrCode40001Count <= 5)
            {
                ErrCode40001Count++;
                await _startApiService.RefreshAccessTokenAsync();
                await SendSubscribeMessageAsync(touser, template_id, data, page);
            }
        }
        return result;
    }
}
