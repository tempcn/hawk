using System;
using System.Reflection;

namespace Hawk.Common
{
    public class RefectHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="fullName">类的完整名称(命名空间+类名)</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string assemblyName, string fullName) where T : class
        {
            try
            {
                return (T)Assembly.Load(assemblyName).CreateInstance(fullName);
                //第二种写法
                //string path = fullName + "." + assemblyName;
                //Type t = Type.GetType(path, false, true);
                //if (t != null)
                //{
                //    object obj = Activator.CreateInstance(t, true);
                //    return (T)obj;
                //}
            }
            catch
            {
                return null;// default(T);
            }
        }
    }
}
