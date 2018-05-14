using System;

namespace Hawk
{
    public class Singleton<T> where T : class//, new()//new不支持非公共的无参构造函数 
    {
        #region 双重检查
        //    static readonly object _lock = new object();
        //    Singleton() { }
        //    static T _instance;
        //    public static T Instance
        //    {
        //        get
        //        {
        //            if (_instance == null)
        //                lock (_lock)
        //                {
        //                    if (_instance == null)
        //                    {
        //                        Console.WriteLine("创建了一个对象");
        //                        _instance = new T();
        //                    }
        //                }
        //            return _instance;
        //        }
        //    }
        #endregion

        #region 延迟加载        
        public static T Instance
        {
            get { return Delay._instance; }
        }

        internal class Delay
        {
            //static Delay() { }
            //internal static readonly T _instance = new T();
            internal static readonly T _instance = (T)Activator.CreateInstance(typeof(T), true);
        }
        #endregion
    }
}
