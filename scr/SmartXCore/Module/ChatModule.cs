using SmartXCore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartXCore.Model;
using SmartXCore.Extensions;
using System.ComponentModel;

namespace SmartXCore.Module
{
    [Description("发送消息模块")]
    public class ChatModule : BaseModule, IChatModule
    {
        public ChatModule(IContext context) : base(context)
        {
        }

        public bool SendMsg(MessageSent msg)
        {
            var url = string.Format(ApiUrls.SendMsg, _session.BaseUrl);
            var obj = new
            {
                _session.BaseRequest,
                Msg = msg
            };
            var response = _httpClient.PostJsonAsync(url, obj.ToJson()).Result;
            var json = response.RawText().ToJToken();
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
