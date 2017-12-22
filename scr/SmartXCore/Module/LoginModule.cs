using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartXCore.Core;
using System.Text.RegularExpressions;
using SmartXCore.Extensions;
using SmartXCore.Event;

namespace SmartXCore.Module
{
    public class LoginModule : BaseModule, ILoginModule
    {
        public LoginModule(IContext context) : base(context)
        {
        }

        /// <summary>
        /// 获取UUID
        /// </summary>
        public bool GetUuid()
        {
            var reg = new Regex(@"window.QRLogin.code = (\d+); window.QRLogin.uuid = ""(\S+?)""");
            var url = ApiUrls.GetUuid;
            var param = new Dictionary<string, object>()
            {
                { "appid",ApiUrls.Appid},
                { "fun","new"},
                { "lang","zh_CN"},
                { "_",_session.Seq++}
            };
            var response = _httpClient.PostAsync(url, param.ToQueryString()).Result;
            var str = response.RawText();
            var match = reg.Match(str);
            if (match.Success && match.Groups.Count > 2 && match.Groups[1].Value == "200")
            {
                _session.Uuid = match.Groups[2].Value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        public void GetQRCode()
        {
            var url = ApiUrls.GetQRCode;
            var param = new Dictionary<string, object>()
            {
                { "t","webwx"},
                { "_",_session.Seq++}
            };
            var response = _httpClient.PostAsync(url, param.ToQueryString()).Result;

            _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.QRCodeReady, response));
        }

        public bool Login()
        {
            if (GetUuid())
            {
                GetQRCode();
            }
            return false;
        }

        public void BeginSyncCheck()
        {
            throw new NotImplementedException();
        }

    }
}
