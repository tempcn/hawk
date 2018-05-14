using System;

namespace Hawk
{
    public static class DateTimeHelper
    {
        //internal const string DATE_FULL_M_STRING = "yyyyMMddHHmmssfff";
        //internal const string DATE_FULL_STRING = "yyyyMMddHHmmss";
        //internal const string DATE_STRING = "yyyy-MM-dd HH:mm:ss";
        //internal const string DATE_SHORT_STRING = "yyyy-MM-dd";

        internal static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);


        ///// <summary>
        ///// 格式化为{yyyyMMddHHmmssfff}
        ///// </summary>
        ///// <returns></returns>
        //public static string ToFullMString(this DateTime dateTime)
        //=> dateTime.ToString(DATE_FULL_M_STRING);

        /// <summary>
        /// 格式化为{yyyyMMddHHmmss}
        /// </summary>
        /// <param name="milli">毫秒是7位</param>
        /// <returns></returns>
        public static string ToFullString(this DateTime dateTime, bool milli = false)
        => dateTime.ToString(milli ? "yyyyMMddHHmmssfffffff" : "yyyyMMddHHmmss");

        /// <summary>
        /// 格式化为{yyyy-MM-dd HH:mm:ss}
        /// </summary>
        /// <returns></returns>
        public static string ToStringEx(this DateTime dateTime)
        => dateTime.ToString("yyyy-MM-dd HH:mm:ss");


        /// <summary>
        /// 格式化为{yyyy-MM-dd}
        /// </summary>
        /// <returns></returns>
        public static string ToShortString(this DateTime dateTime)
        => dateTime.ToString("yyyy-MM-dd");

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
            => (EPOCH + TimeSpan.FromSeconds(timestamp)).ToLocalTime();
    }
}
