using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartXCore.Extensions
{
    public static class HttpClientExtension
    {
        #region Get

        public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, string url, string referer = null, int timeout = 20)
        {
            var clientRreferer = client.DefaultRequestHeaders.Referrer;

            if (!string.IsNullOrEmpty(referer))
            {
                client.DefaultRequestHeaders.Referrer = new Uri(referer);
            }
            var response = client.GetAsync(url);
            response.Wait();
            // 恢复原来的referer
            client.DefaultRequestHeaders.Referrer = clientRreferer;

            return await response;
        }


        public static async Task<string> GetStringAsync(this HttpClient client, string url, string referer = null, int timeout = 20)
        {
            var clientRreferer = client.DefaultRequestHeaders.Referrer;

            if (!string.IsNullOrEmpty(referer))
            {
                client.DefaultRequestHeaders.Referrer = new Uri(referer);
            }
            var response = client.GetStringAsync(url);
            response.Wait();
            // 恢复原来的referer
            client.DefaultRequestHeaders.Referrer = clientRreferer;

            return await response;
        }


        public static async Task<Stream> GetStreamAsync(this HttpClient client, string url, string referer = null, int timeout = 20)
        {
            var clientRreferer = client.DefaultRequestHeaders.Referrer;

            if (!string.IsNullOrEmpty(referer))
            {
                client.DefaultRequestHeaders.Referrer = new Uri(referer);
            }
            var response = client.GetStreamAsync(url);
            response.Wait();
            // 恢复原来的referer
            client.DefaultRequestHeaders.Referrer = clientRreferer;

            return await response;
        }


        #endregion


        #region Post

        public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string url, string postData, string referer = null, int timeout = 20)
        {
            try
            {
                //当前post请求url的origin
                var origin = url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal));

                var clientRreferer = client.DefaultRequestHeaders.Referrer;

                if (!string.IsNullOrEmpty(referer))
                {
                    client.DefaultRequestHeaders.Referrer = new Uri(referer);
                }

                var hasOrigin = client.DefaultRequestHeaders.TryGetValues("Origin", out IEnumerable<string> clientOrigin);
                if (hasOrigin)
                {
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Add("Origin", origin);
                }
                else
                {
                    client.DefaultRequestHeaders.Add("Origin", origin);
                }


                HttpContent content = new StringContent(postData, Encoding.UTF8);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/ x-www-form-urlencoded; charset=UTF-8");

                var response = client.PostAsync(url, content);
                response.Wait();

                // 恢复原来的origin
                if (hasOrigin)
                {
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Add("Origin", clientOrigin);
                }
                else
                {
                    client.DefaultRequestHeaders.Remove("Origin");
                }
                // 恢复原来的referer
                client.DefaultRequestHeaders.Referrer = clientRreferer;

                return await response;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string url, string json, string referer = null, int timeout = 20)
        {
            try
            {
                //当前post请求url的origin
                var origin = url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal));

                var clientRreferer = client.DefaultRequestHeaders.Referrer;

                if (!string.IsNullOrEmpty(referer))
                {
                    client.DefaultRequestHeaders.Referrer = new Uri(referer);
                }

                var hasOrigin = client.DefaultRequestHeaders.TryGetValues("Origin", out IEnumerable<string> clientOrigin);
                if (hasOrigin)
                {
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Add("Origin", origin);
                }
                else
                {
                    client.DefaultRequestHeaders.Add("Origin", origin);
                }


                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = client.PostAsync(url, content);
                response.Wait();

                // 恢复原来的origin
                if (hasOrigin)
                {
                    client.DefaultRequestHeaders.Remove("Origin");
                    client.DefaultRequestHeaders.Add("Origin", clientOrigin);
                }
                else
                {
                    client.DefaultRequestHeaders.Remove("Origin");
                }
                // 恢复原来的referer
                client.DefaultRequestHeaders.Referrer = clientRreferer;

                return await response;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<HttpResponseMessage> PostWithRetry(this HttpClient client, string url, string postData, int retryTimes = 3, string referer = null, int timeout = 20)
        {
            Task<HttpResponseMessage> response;
            do
            {
                response = client.PostAsync(url, postData, referer, timeout);
                response.Wait();
                retryTimes--;
            }
            while (retryTimes >= 0 && response.Result.StatusCode != System.Net.HttpStatusCode.OK);
            return await response;
        }

        public static async Task<HttpResponseMessage> PostJsonWithRetry(this HttpClient client, string url, string json, int retryTimes = 3, string referer = null, int timeout = 20)
        {
            Task<HttpResponseMessage> response;
            do
            {
                response = client.PostJsonAsync(url, json, referer, timeout);
                response.Wait();
                retryTimes--;
            }
            while (retryTimes >= 0 && response.Result.StatusCode != System.Net.HttpStatusCode.OK);
            return await response;
        }

        #endregion


        public static async Task<string> RawTextAsync(this HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string RawText(this HttpResponseMessage response)
        {
            try
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static byte[] RawByteArray(this HttpResponseMessage response)
        {
            try
            {
                return response.Content.ReadAsByteArrayAsync().Result;
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}
