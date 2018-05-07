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
        => dateTime.ToString(Field.DATE_FULL_STRING);

        /// <summary>
        /// 格式化为{yyyy-MM-dd HH:mm:ss}
        /// </summary>
        /// <returns></returns>
        public static string ToDateString(this DateTime dateTime)
        => dateTime.ToString(Field.DATE_STRING);


        /// <summary>
        /// 格式化为{yyyy-MM-dd}
        /// </summary>
        /// <returns></returns>
        public static string ToShortString(this DateTime dateTime)
        => dateTime.ToString(Field.DATE_SHORT_STRING);

        /// <summary>
        ///  返回unix时间戳
        /// </summary>
        /// <param name="milli">毫秒</param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime date, bool milli = false)
            => (date.ToUniversalTime().Ticks - 621355968000000000L) / (milli ? 10000L : 10000000L);

        /// <summary>
        ///  返回unix时间戳
        /// </summary>
        /// <param name="milli">毫秒</param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTimeOffset date, bool milli = false)
        {
#if NET40 || NET45
            return (date.ToUniversalTime().Ticks - 621355968000000000L) / (milli ? 10000L : 10000000L);
#else
            return milli ? date.ToUnixTimeMilliseconds() : date.ToUnixTimeSeconds();
#endif
        }

        public static DateTime ToDate(this int timestamp)
            => (Field.EPOCH + TimeSpan.FromSeconds(timestamp)).ToLocalTime();
    }
}
