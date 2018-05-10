﻿using System;
using System.Text;

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

        public static DateTimeOffset ToDateTimeOffset(this string s, DateTimeOffset? def = null)
            => To(s, DateTimeOffset.TryParse, def.HasValue ? def.Value : DateTimeOffset.MinValue);

        public static DateTime ToDateTime(this string s, DateTime? def = null)
            => To(s, DateTime.TryParse, def.HasValue ? def.Value : DateTime.MinValue);

        //public static IPAddress ToIPAddress(this string value)
        //    => To(value, IPAddress.TryParse);

        public static TimeSpan ToTimeSpan(this string s, TimeSpan? def = null)
            => To<TimeSpan>(s, TimeSpan.TryParse, def.HasValue ? def.Value : TimeSpan.Zero);

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
    }
}
