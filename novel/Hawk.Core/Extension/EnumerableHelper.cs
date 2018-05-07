﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Hawk
{
   public static class EnumerableHelper
    {
        public static bool IsNotEmpty<T>(this IEnumerable<T> s)
        {
            if (s != null)
            {
                using (IEnumerator<T> e = s.GetEnumerator())
                    return e.MoveNext();
            }
            return false;

            //if (s == null)
            //    //throw new ArgumentNullException("source");// Error.ArgumentNull("source");
            //    return false;

            //using (IEnumerator<T> e = s.GetEnumerator())
            //    //if (e.MoveNext())
            //    return e.MoveNext();
            ////return false;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            if (e != null)
                foreach (var item in e)
                {
                    yield return item;
                }
            yield return value;
        }

        /// <summary>把一个列表组合成为一个字符串，默认逗号分隔</summary>
        /// <param name="value"></param>
        /// <param name="separator">组合分隔符，默认逗号</param>
        /// <returns></returns>
        public static string Join(this IEnumerable value, string separator = ",")
        {
            var sb = new StringBuilder();
            if (value != null)
            {
                foreach (var item in value)
                {
                    sb.Separate(separator).Append(item + "");
                }
            }
            return sb.ToString();
        }

        /// <summary>把一个列表组合成为一个字符串，默认逗号分隔</summary>
        /// <param name="value"></param>
        /// <param name="separator">组合分隔符，默认逗号</param>
        /// <param name="func">把对象转为字符串的委托</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> value, string separator = ",", Func<T, string> func = null)
        {
            var sb = new StringBuilder();
            if (value != null)
            {
                if (func == null) func = obj => obj + "";
                foreach (var item in value)
                {
                    sb.Separate(separator).Append(func(item));
                }
            }
            return sb.ToString();
        }

        /*
         * 
         * lambda表达式
           Func<KeyValuePair<string, string>, string> func = (x => { return x.Key + "=" + x.Value; });

           匿名方法
           Func<KeyValuePair<string, string>, string> func = delegate (KeyValuePair<string, string> x) { return x.Key + "|" + x.Value; })
         */

        //public static string Join<TValue>(this IEnumerable<KeyValuePair<string, TValue>> value, string separator = "&")
        //{
        //    var sb = new StringBuilder();
        //    if (value != null)
        //    {
        //        Func<KeyValuePair<string, TValue>, string> func = (x => { return string.Concat(x.Key, "=", x.Value); });
        //        foreach (var item in value)
        //        {
        //            sb.Separate(separator).Append(func(item));
        //        }
        //    }
        //    return sb.ToString();
        //}
    }
}
