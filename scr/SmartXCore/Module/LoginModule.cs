using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartXCore.Core;
using System.Text.RegularExpressions;
using SmartXCore.Extensions;
using SmartXCore.Event;
using SmartXCore.Model;
using System.Threading;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace SmartXCore.Module
{
    [Description("登录模块")]
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
            _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.Error, "获取UUID失败"));
            return false;
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        public bool GetQRCode()
        {
            var url = string.Format(ApiUrls.GetQRCode, _session.Uuid);
            var param = new Dictionary<string, object>()
            {
                { "t","webwx"},
                { "_",_session.Seq++}
            };
            var response = _httpClient.PostAsync(url, param.ToQueryString()).Result;
            var bytes = response.RawByteArray();
            if (bytes != null)
            {
                _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.QRCodeReady, bytes));
                return true;
            }
            else
            {
                _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.Error, "获取二维码失败"));
                return false;
            }

        }

        /// <summary>
        /// 等待扫码登录
        /// </summary>
        public WatiForLoginResult WatiForLogin()
        {
            var loginResult = WatiForLoginResult.Initial;
            var _regCode = new Regex(@"window.code=(\d+);");
            var _regUrl = new Regex(@"window.redirect_uri=""(\S+?)"";");
            var _tip = 1;
            var url = ApiUrls.CheckQRCode;
            var param = new Dictionary<string, object>()
            {
                { "loginicon","true"},
                { "tip",_tip},
                { "uuid",_session.Uuid },
                { "r",~_timestamp},
                { "_",_session.Seq++}
            };
            var response = _httpClient.PostAsync(url, param.ToQueryString()).Result;
            var str = response.RawText();
            var match = _regCode.Match(str);
            if (match.Success)
            {
                var code = match.Groups[1].Value;
                Enum.TryParse(code, out loginResult);
                switch (loginResult)
                {
                    case WatiForLoginResult.Success:
                        {
                            var m = _regUrl.Match(str);
                            if (m.Success)
                            {
                                _session.LoginUrl = $"{m.Groups[1].Value}&fun=new&version=v2";
                                _session.BaseUrl = _session.LoginUrl.Substring(0, _session.LoginUrl.LastIndexOf("/", StringComparison.OrdinalIgnoreCase));
                            }
                            _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.QRCodeSuccess, loginResult));
                            break;
                        }

                    case WatiForLoginResult.ScanCode:
                        _tip = 0;
                        _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.QRCodeScanCode, loginResult));
                        break;

                    case WatiForLoginResult.QRCodeInvalid:
                        _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.QRCodeInvalid, loginResult));
                        break;
                }

            }

            return loginResult;
        }

        /// <summary>
        /// 获取登录参数
        /// </summary>
        public bool WebLogin()
        {
            var url = _session.LoginUrl;
            var response = _httpClient.GetAsync(url).Result;
            var str = response.RawText();

            var root = XDocument.Parse(str).Root;
            _session.Skey = root.Element("skey").Value;
            _session.Sid = root.Element("wxsid").Value;
            _session.Uin = root.Element("wxuin").Value;
            _session.PassTicket = root.Element("pass_ticket").Value;

            _session.State = SessionState.Online;
            return true;
        }

        /// <summary>
        /// 微信初始化
        /// </summary>
        public bool WebwxInit()
        {
            var url = string.Format(ApiUrls.WebwxInit, _session.BaseUrl, _session.PassTicket, _session.Skey, _timestamp);
            var obj = new { _session.BaseRequest };
            /*
               {
                   "BaseRequest": {
                       "DeviceId": "e650946746417762",
                       "Skey": "@crypt_c498484a_1d7a344b3232380eb1aa33c16690399a",
                       "Sid": "PhHAnhCRcFDCA219",
                       "Uin": "463678295"
                   }
               }             
           */
            var response = _httpClient.PostJsonAsync(url, obj.ToJson()).Result;
            var str = response.RawText();
            if (!str.IsNullOrEmpty())
            {
                var json = JObject.Parse(str);
                if (json["BaseResponse"]["Ret"].ToString() == "0")
                {
                    _session.SyncKey = json["SyncKey"];
                    _session.UserToken = json["User"];
                    _session.User = json["User"].ToObject<ContactMember>();
                    return true;
                }
                else
                {
                    var error = json["BaseResponse"]["ErrMsg"].ToString();
                    _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.Error, "微信初始化失败"));
                    return false;
                }

            }
            return false;
        }

        /// <summary>
        /// 开启状态通知
        /// </summary>
        /// <returns></returns>
        public bool StatusNotify()
        {
            var url = string.Format(ApiUrls.StatusNotify, _session.BaseUrl, _session.PassTicket);
            var obj = new
            {
                _session.BaseRequest,
                Code = 3,
                FromUserName = _session.UserToken["UserName"],
                ToUserName = _session.UserToken["UserName"],
                ClientMsgId = _timestamp
            };
            var response = _httpClient.PostJsonAsync(url, obj.ToJson()).Result;
            var str = response.RawText();
            if (!str.IsNullOrEmpty())
            {
                var json = JObject.Parse(str);
                if (json["BaseResponse"]["Ret"].ToString() == "0")
                {
                    return true;
                }
                else
                {
                    var error = json["BaseResponse"]["ErrMsg"].ToString();
                    _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.Error, "开启状态通知失败"));
                }

            }
            return false;
        }

        /// <summary>
        /// 获取联系人
        /// </summary>
        public bool GetContact()
        {
            var url = string.Format(ApiUrls.GetContact, _session.BaseUrl, _session.PassTicket, _session.Skey, _timestamp);
            var obj = new { _session.BaseRequest };
            var response = _httpClient.PostJsonAsync(url, obj.ToJson()).Result;
            var json = response.RawText().ToJToken();
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                _store.MemberCount = json["MemberCount"].ToObject<int>();
                var list = json["MemberList"].ToObject<ContactMember[]>();
                _store.ContactMemberDic.ReplaceBy(list, m => m.UserName);
                
                _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.LoginSuccess));
                return true;
            }
            else
            {
                _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.Error, "获取联系人失败"));
                return false;
            }
        }

        public bool Login()
        {
            if (GetUuid() && GetQRCode())
            {
                //等待扫码登录
                while (true)
                {
                    var loginResult = WatiForLogin();
                    if (loginResult == WatiForLoginResult.Success)
                        break;
                    if (loginResult == WatiForLoginResult.QRCodeInvalid)
                        return false;
                    Thread.Sleep(500);
                }
                if (WebLogin() && WebwxInit() && StatusNotify() && GetContact())
                    return true;
            }
            return false;
        }


        public void BeginSyncCheck()
        {
            while (true)
            {
                var syncCheckResult = false;
                try
                {
                    syncCheckResult = SyncCheck();
                }
                catch (Exception)
                {
                    break;
                }
                if (syncCheckResult)
                    WebwxSync();
                Thread.Sleep(200);
            }
        }

        private int _hostIndex = 0;
        /// <summary>
        /// 同步检测
        /// </summary>
        public bool SyncCheck()
        {
            var syncCheckResult = SyncCheckResult.Nothing;
            var _reg = new Regex(@"window.synccheck={retcode:""(\d+)"",selector:""(\d+)""}");

            var url = _session.SyncUrl;
            if (_session.SyncUrl == null)
            {
                var host = ApiUrls.SyncHosts[_hostIndex];
                url = $"https://{host}/cgi-bin/mmwebwx-bin/synccheck";
                _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.BeginSyncCheck, host));
            }

            //此处需要将key都变成小写
            var param = _session.BaseRequest.ToDictionary(pair => pair.Key.ToLower(), pair => pair.Value).ToQueryString();
            param += "&synckey=" + _session.SyncKeyStr;
            param += "&r=" + _timestamp;
            param += "&_=" + _session.Seq++;
            url += "?" + param;
            var response = _httpClient.GetAsync(url).Result;
            var str = response.RawText();
            var match = _reg.Match(str);
            if (match.Success)
            {
                var retcode = match.Groups[1].Value;
                // retcode
                // 1100-
                // 1101-参数错误
                // 1102-cookie错误
                if (_session.SyncUrl == null)
                {
                    if (retcode != "0")
                    {
                        //切换host
                        if (_hostIndex < ApiUrls.SyncHosts.Length - 1)
                        {
                            _hostIndex++;
                            return false;
                        }
                        else
                        {
                            _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.SyncCheckError));
                            throw new Exception();
                        }
                    }
                    else
                    {
                        _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.SyncCheckSuccess));
                        _session.SyncUrl = url.Split('?')[0];
                        return false;
                    }
                }

                if (retcode != "0")
                {
                    _session.State = SessionState.Offline;
                    _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.Offline));
                    throw new Exception();
                }
                else
                {
                    var selector = match.Groups[2].Value;
                    Enum.TryParse(selector, out syncCheckResult);
                    switch (syncCheckResult)
                    {
                        case SyncCheckResult.Nothing:
                            return false;
                        case SyncCheckResult.RedEnvelope:
                        //_context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.RedEnvelope));
                        //return false;
                        case SyncCheckResult.NewMsg:
                        case SyncCheckResult.UsingPhone:
                            return true;
                    }
                }

            }
            _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.SyncCheckError));
            throw new Exception();
        }

        /// <summary>
        /// 消息同步
        /// </summary>
        public void WebwxSync()
        {
            var url = string.Format(ApiUrls.WebwxSync, _session.BaseUrl, _session.Sid, _session.Skey, _session.PassTicket);
            var obj = new
            {
                _session.BaseRequest,
                _session.SyncKey,
                rr = ~_timestamp // 注意是按位取反
            };
            var referrer = "https://wx.qq.com/?&lang=zh_CN";
            var response = _httpClient.PostJsonAsync(url, obj.ToJson(), referrer).Result;
            var json = response.RawText().ToJToken();
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                _session.SyncKey = json["SyncCheckKey"];
                var list = json["AddMsgList"].ToObject<List<Message>>();
                var newMsgs = list.Where(m => m.MsgType != MessageType.WxInit).ToList();
                newMsgs.ForEach(m =>
                {
                    m.FromUser = _store.ContactMemberDic.GetOrDefault(m.FromUserName);
                    _context.FireNotifyAsync(NotifyEvent.CreateEvent(NotifyEventType.Message, m));
                });

            }
        }
    }
}
