using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartXCore.Extensions
{
    public static class JsonExtension
    {
        private static readonly JsonSerializerSettings _ignoreSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public static string ToJson(this object obj, Formatting formatting = Formatting.None, bool ignoreNull = false)
        {
            return JsonConvert.SerializeObject(obj, formatting, ignoreNull ? _ignoreSettings : null);
        }

        public static JToken ToJToken(this string str)
        {
            return JToken.Parse(str);
        }
    }
}
