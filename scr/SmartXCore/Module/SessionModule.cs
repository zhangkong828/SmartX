using System;
using System.Collections.Generic;
using System.Linq;
using SmartXCore.Core;
using SmartXCore.Extensions;
using SmartXCore.Model;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace SmartXCore.Module
{
    public class SessionModule : IBaseModule
    {
        private readonly IDictionary<string, string> _request = new Dictionary<string, string>();
        private readonly Random _random = new Random();
        public SessionModule()
        {
            _cookies = new CookieContainer();
            _httpClientHandler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = _cookies,
                AllowAutoRedirect = true,
                
            };
            _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", ApiUrls.UserAgent);
            _httpClient.DefaultRequestHeaders.Add("KeepAlive", "true");
        }

        public HttpClientHandler _httpClientHandler;

        public CookieContainer _cookies;

        public HttpClient _httpClient { get; set; }


        public SessionState State { get; set; } = SessionState.Offline;

        public long Seq { get; set; }

        public string Uuid { get; set; }

        public string BaseUrl { get; set; }

        public string LoginUrl { get; set; }

        public string SyncUrl { get; set; }

        public string PassTicket { get; set; }

        public JToken SyncKey { get; set; }

        public string SyncKeyStr => SyncKey?["List"]?.ToArray().Select(m => $"{m["Key"]}_{m["Val"]}").JoinWith("|");

        public JToken UserToken { get; set; }

        public ContactMember User { get; set; }

        public string Sid
        {
            get { return BaseRequest.GetOrDefault(nameof(Sid)); }
            set { BaseRequest[nameof(Sid)] = value; }
        }

        public string Uin
        {
            get { return BaseRequest.GetOrDefault(nameof(Uin)); }
            set { BaseRequest[nameof(Uin)] = value; }
        }

        public string Skey
        {
            get { return BaseRequest.GetOrDefault(nameof(Skey)); }
            set { BaseRequest[nameof(Skey)] = value; }
        }

        public string DeviceId => BaseRequest.GetOrDefault(nameof(DeviceId));

        public IDictionary<string, string> BaseRequest
        {
            get
            {
                var seed = _random.NextDouble();
                var id = $"e{ seed.ToString("f15").Split('.')[1] }";
                _request[nameof(DeviceId)] = id;
                return _request;
            }
        }
    }

    public enum SessionState
    {
        /// <summary>
        /// 离线
        /// </summary>
        [Description("离线")]
        Offline,

        /// <summary>
        /// 在线
        /// </summary>
        [Description("在线")]
        Online,
    }
}
