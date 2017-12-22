using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXCore.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static string JoinWith(this IEnumerable<string> strs, string separator)
        {
            return string.Join(separator, strs);
        }

        /// <summary>返回平台无关的Hashcode
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int GetStringHashcode(this string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;

            unchecked
            {
                int hash = 23;
                foreach (char c in s)
                {
                    hash = (hash << 5) - hash + c;
                }
                if (hash < 0)
                {
                    hash = Math.Abs(hash);
                }
                return hash;
            }

        }
    }
}
