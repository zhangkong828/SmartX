using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXCore.Extensions
{
    public static class DateTimeExtension
    {
        public static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, 1);

        public static long ToTimestamp(this DateTime d)
        {
            return (long)(DateTime.UtcNow - Jan1St1970).TotalSeconds;
        }

        public static long ToTimestampMilli(this DateTime d)
        {
            return (long)(DateTime.UtcNow - Jan1St1970).TotalMilliseconds;
        }
    }
}
