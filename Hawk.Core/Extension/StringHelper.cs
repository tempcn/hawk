using System;
using System.Numerics;

namespace Hawk
{
    public static class StringHelper
    {
        delegate bool TryParseDelegate<T>(string s, out T result);

        //static T To<T>(string s, TryParseDelegate<T> parse)
        //    => parse(s, out T result) ? result : default(T);

        static T To<T>(string s, TryParseDelegate<T> parse, T def = default(T))
            => parse(s, out T result) ? result : def;

        public static int ToInt32(this string s, int def = 0)
          => To(s, int.TryParse, def);

        public static long ToInt64(this string s, long def = 0L)
            => To(s, long.TryParse, def);

        public static BigInteger ToBigInt(this string s, BigInteger? def = null)
          => To(s, BigInteger.TryParse, def.HasValue ? def.Value : BigInteger.Zero);

        public static DateTimeOffset ToDateTimeOffset(this string s, DateTimeOffset? def = null)
            => To(s, DateTimeOffset.TryParse, def.HasValue ? def.Value : DateTimeOffset.MinValue);

        public static DateTime ToDateTime(this string s, DateTime? def = null)
            => To(s, DateTime.TryParse, def.HasValue ? def.Value : DateTime.MinValue);

        //public static IPAddress ToIPAddress(this string value)
        //    => To(value, IPAddress.TryParse);

        public static TimeSpan ToTimeSpan(this string s, TimeSpan? def = null)
            => To(s, TimeSpan.TryParse, def.HasValue ? def.Value : TimeSpan.Zero);

        public static bool IsNullOrEmpty(this string s)
            => s == null || s.Length <= 0;

        public static bool IsNotNullOrEmpty(this string s)
            => s != null && s.Length > 0;

        ///// <summary>追加分隔符字符串，忽略开头，常用于拼接</summary>
        ///// <param name="s">字符串构造者</param>
        ///// <param name="separator">分隔符</param>
        ///// <returns></returns>
        //public static StringBuilder Separate(this StringBuilder s, string separator = null)
        //{
        //    if (s == null || string.IsNullOrEmpty(separator)) return s;

        //    if (s.Length > 0) s.Append(separator);

        //    return s;
        //}

        /// <summary>格式化字符串。特别支持无格式化字符串的时间参数</summary>
        /// <param name="value">格式字符串</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static string F(this string value, params object[] args)
        {
            if (string.IsNullOrEmpty(value)) return value;

            // 特殊处理时间格式化。这些年，无数项目实施因为时间格式问题让人发狂
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] is DateTime || args[i] is DateTimeOffset)
                {
                    // 没有写格式化字符串的时间参数，一律转为标准时间字符串
                    if (value.Contains("{" + i + "}")) args[i] = ((DateTime)args[i]).ToStringEx();
                }
            }
            return string.Format(value, args);
        }
    }
}
