using System.Numerics;
using System.Text;
using System;
using System.Diagnostics.Contracts;

namespace System
{
    public static class StringHelper
    {
#if NETSTANDARD2_0
        static StringHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        delegate bool TryParseDelegate<T>(string s, out T result) where T : struct;
        delegate bool TryParseDelegateIgnoreCase<T>(string s, bool ignoreCase, out T result) where T : struct;

        //static T To<T>(string s, TryParseDelegate<T> parse)
        //    => parse(s, out T result) ? result : default(T);

        static T To<T>(string s, TryParseDelegate<T> parse, T def = default(T)) where T : struct
            => parse(s, out T result) ? result : def;

        static T ToIgnoreCase<T>(string s, TryParseDelegateIgnoreCase<T> parse, T def = default(T)) where T : struct
        => parse(s, true, out T result) ? result : def;

        public static T ToEnum<T>(this string s, T def = default(T)) where T : struct
            => ToIgnoreCase<T>(s, Enum.TryParse, def);

        public static int ToInt32(this string s, int def = 0) => To(s, int.TryParse, def);

        public static long ToInt64(this string s, long def = 0L) => To(s, long.TryParse, def);

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

        [Pure]
        public static bool IsNullOrEmpty(this string s) => s == null || s.Length <= 0;      
        public static bool IsNotNullOrEmpty(this string s) => s != null && s.Length > 0;
        public static bool IsNullOrWhiteSpace(this string s)
        {
            if (s == null || s.Length <= 0) return true;
            for (int i = 0; i < s.Length; i++)
            {
                if (!Char.IsWhiteSpace(s[i])) return false;
            }
            return true;
        }

        public static bool ToBoolean(this string s, bool def = false) => To(s, bool.TryParse, def);

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

        public static string ToGBEncode(this string s, string prefix = null, bool upper = false)
        {
            var buf = Encoding.GetEncoding(936).GetBytes(s);
            return ToEncode(buf, 2, prefix, upper);
        }

        public static string ToUtf8Encode(this string s, string prefix = null, bool upper = false)
        {
            var buf = Encoding.UTF8.GetBytes(s);
            return ToEncode(buf, 3, prefix, upper);
        }

        public static string ToUnicodeEncode(this string s, string prefix = null, bool upper = false)
        {
            var buf = Encoding.Unicode.GetBytes(s);
            return ToEncode(buf, 2, prefix, upper);
        }

        /// <summary>
        /// 获取字符串的编码,如果包含的字符编码不在相应编码范围中,则结果可能会出现错误
        /// </summary>
        /// <param name="buf">字符串的字节序列</param>
        /// <param name="offset">单个字符串相应编码字节数量</param>
        /// <param name="prefix">每个字符串编码的前缀</param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static string ToEncode(this byte[] buf, int offset = 2, string prefix = null, bool upper = false)
        {
            if (buf == null || buf.Length == 0) return string.Empty;
            string format = upper ? "X2" : "x2";
            int capacity = prefix != null ? prefix.Length * buf.Length / offset : 0 + buf.Length * 2;
            var sb = new StringBuilder(capacity);
            for (int i = 0; i < buf.Length / offset; i++)
            {
                if (prefix.IsNotNullOrEmpty())
                    sb.Append(prefix);
                for (int j = offset * i; j < offset * (i + 1); j++)
                    sb.Append(buf[j].ToString(format));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取字符串的编码
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <param name="prefix">每个字符串编码的前缀</param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static string ToEncode(this string s, Encoding encode = null, string prefix = null, bool upper = false)
        {
            if (s.IsNullOrEmpty()) return string.Empty;
            string format = upper ? "X2" : "x2";
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                var buf = (encode ?? Encoding.UTF8).GetBytes(s.Substring(i, 1));
                if (prefix.IsNotNullOrEmpty())
                    sb.Append(prefix);
                if (buf.Length > 1)//多字节合并
                    for (int j = 0; j < buf.Length; j++)
                        sb.Append(buf[j].ToString(format));
                else
                    sb.Append(buf[0].ToString(format));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Byte字节数组转换为十六进制的字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes, bool upper = false)
        {
            var s = new StringBuilder(bytes.Length * 2);

            #region 第一种实现
            string format = upper ? "{0:X2}" : "{0:x2}";
            for (int i = 0; i < bytes.Length; i++)
                s.AppendFormat(format, bytes[i]);
            #endregion

            #region 第二种实现        
            //char[] bcdLookup = new char[16] { '0', '1', '2', '3', '4', '5',
            //'6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
            //if (upper)
            //    bcdLookup = new char[16] {
            //    '0', '1', '2', '3', '4', '5',
            //'6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    s.Append(bcdLookup[(((byte)bytes[i]) >> 4) & 0x0f])
            //        .Append(bcdLookup[(byte)bytes[i] & 0x0f]);
            //}
            #endregion
            return s.ToString();
        }

        /// <summary>
        /// 将16进制字符串转化为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToHex(this string value)
        {
            if (!value.IsHex()) return null;
            int len = value.Length / 2;
            byte[] ret = new byte[len];
            for (int i = 0; i < len; i++)
            {
                //int k;
                //System.Globalization.CultureInfo provider = new System.Globalization.CultureInfo("zh-cn");
                //if (Int32.TryParse(value.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier, provider, out k))
                //    ret[i] = (byte)k;
                ret[i] = (byte)(Convert.ToInt32(value.Substring(i * 2, 2), 16));
            }
            return ret;
        }

        ///// <summary>
        ///// 获取字符的编码
        ///// </summary>
        ///// <param name="encode"></param>
        ///// <param name="upper"></param>
        ///// <returns></returns>
        //public static int ToHex(this char c, Encoding encode = null, bool upper = false)
        //{
        //    string format = upper ? "X2" : "x2";
        //    var buffer = (encode ?? Encoding.UTF8).GetBytes(c.ToString());
        //    int code = buffer[0];
        //    if (buffer.Length > 1)
        //    {
        //        string tmp = "";
        //        for (int j = 0; j < buffer.Length; j++)
        //            tmp += buffer[j].ToString(format);
        //        code = int.Parse(tmp, System.Globalization.NumberStyles.AllowHexSpecifier);
        //    }
        //    return code;
        //}

        ///// <summary>
        ///// 获取字符串的编码
        ///// </summary>
        ///// <param name="separator"></param>
        ///// <param name="encode"></param>
        ///// <param name="upper"></param>
        ///// <returns></returns>
        //public static string ToHex(this string s, string separator = null, Encoding encode = null, bool upper = false)
        //{
        //    if (s.IsNullOrEmpty()) return string.Empty;

        //    string format = upper ? "X2" : "x2";
        //    var sb = new StringBuilder();
        //    #region 方法一
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        var buffer = (encode ?? Encoding.UTF8).GetBytes(s.Substring(i, 1));
        //        if (separator.IsNotNullOrEmpty())
        //            sb.Append(separator);
        //        if (buffer.Length > 1)
        //            for (int j = 0; j < buffer.Length; j++)
        //                sb.Append(buffer[j].ToString(format));
        //        else
        //            sb.Append(buffer[0].ToString(format));
        //    }
        //    #endregion
        //    #region 方法二
        //    //var ch = s.ToCharArray();
        //    //for (int i = 0; i < ch.Length; i++)
        //    //{
        //    //    int hex = ToHex(ch[i], encode, upper);
        //    //    if (separator.IsNotNullOrEmpty())
        //    //        sb.Append(separator);
        //    //    sb.Append(hex.ToString(format));
        //    //}
        //    #endregion
        //    return sb.ToString();
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
                if (args[i] is DateTime && value.Contains("{" + i + "}"))
                {
                    args[i] = ((DateTime)args[i]).ToStringEx();
                    continue;
                }
                if (args[i] is DateTimeOffset && value.Contains("{" + i + "}"))
                    args[i] = ((DateTimeOffset)args[i]).ToStringEx();

                //if (args[i] is DateTimeOffset || args[i] is DateTime)
                //{
                //    // 没有写格式化字符串的时间参数，一律转为标准时间字符串
                //    if (value.Contains("{" + i + "}"))
                //        args[i] = ((DateTimeOffset)args[i]).ToStringEx();
                //}
            }
            return string.Format(value, args);
        }
    }
}
