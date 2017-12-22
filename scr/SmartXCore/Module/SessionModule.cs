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
                AllowAutoRedirect = true
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
            get { return Request.GetOrDefault(nameof(Sid)); }
            set { Request[nameof(Sid)] = value; }
        }

        public string Uin
        {
            get { return Request.GetOrDefault(nameof(Uin)); }
            set { Request[nameof(Uin)] = value; }
        }

        public string Skey
        {
            get { return Request.GetOrDefault(nameof(Skey)); }
            set { Request[nameof(Skey)] = value; }
        }

        public string DeviceId => Request.GetOrDefault(nameof(DeviceId));

        public IDictionary<string, string> Request
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
        [Description("离线")]
        Offline,

        [Description("在线")]
        Online,
    }
}
