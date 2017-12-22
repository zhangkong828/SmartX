using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXCore.Extensions
{
    public static class DictionaryExtension
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            TValue result;
            if (!dic.TryGetValue(key, out result))
            {
                return default(TValue);
            }
            return result;
        }

        public static string ToQueryString<TKey, TValue>(this IDictionary<TKey, TValue> dic, string separator = "&")
        {
            var list = new List<string>();
            foreach (var item in dic)
            {
                list.Add(item.Key.ToString() + "=" + item.Value.ToString());
            }
            return list.JoinWith(separator);
        }

    }
}
