#if NET40 || NET45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Hawk.Common
{
    public class IniHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName">要读区的区域名。若传入null值，第4个参数returnBuffer将会获得所有的section name。</param>
        /// <param name="key">key的名称。若传入null值，第4个参数returnBuffer将会获得所有的指定sectionName下的所有key name</param>
        /// <param name="defValue">key没找到时的返回值。</param>
        /// <param name="returnBuffer">key所对应的值</param>
        /// <param name="size"></param>
        /// <param name="file">ini文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string sectionName, string key, string defValue, byte[] returnBuffer, int size, string file);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName">要写入的区域名</param>
        /// <param name="key">key的名称。若传入null值，将移除指定的section。</param>
        /// <param name="value">设置key所对应的值。若传入null值，将移除指定的key。</param>
        /// <param name="file">ini文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string sectionName, string key, string value, string file);


        public static string GetValue(string sectionName, string key, string file, string def = null)
        {
            var buf = new byte[2048];
            int len = GetPrivateProfileString(sectionName, key, def ?? "", buf, 999, file);
            return Encoding.UTF8.GetString(buf, 0, len);
        }

        public static bool SetValue(string sectionName,string key,string value,string file)
        {
            return WritePrivateProfileString(sectionName, key, value, file) > 0;
        }

        /// <summary>
        /// 移除指定的Section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool Remove(string sectionName,  string file)
        {
            return WritePrivateProfileString(sectionName, null, "", file) > 0;
        }

        /// <summary>
        /// 移除指定的Key
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool Remove(string sectionName, string key, string file)
        {
            return WritePrivateProfileString(sectionName, key, null, file) > 0;
        }

        //public static string GetValue(string sectionName,string key,string file)
        //{
        //    var buf = new byte[2048];

        //}
    }
}
#endif