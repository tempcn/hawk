using System;

namespace Hawk
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 格式化为{yyyyMMddHHmmssfff}
        /// </summary>
        /// <returns></returns>
        public static string ToFullString(this DateTime dateTime)
        {
            return dateTime.ToString(Field.DATE_FULL_STRING);
        }

        /// <summary>
        /// 格式化为{yyyy-MM-dd HH:mm:ss}
        /// </summary>
        /// <returns></returns>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString(Field.DATE_STRING);
        }

        /// <summary>
        /// 格式化为{yyyy-MM-dd}
        /// </summary>
        /// <returns></returns>
        public static string ToShortString(this DateTime dateTime)
        {
            return dateTime.ToString(Field.DATE_SHORT_STRING);
        }

        public static long ToUnixSeconds(this DateTime date)
            => (date.ToUniversalTime().Ticks - 621355968000000000L) / 10000000L;

        public static long ToUnixSeconds(this DateTimeOffset date)
        {
#if NET40 || NET45
            return (date.ToUniversalTime().Ticks - 621355968000000000L) / 10000000L;
#else
            return date.ToUnixTimeSeconds();
#endif
        }
    }
}
