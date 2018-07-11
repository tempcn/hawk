using System;
using System.Reflection;
using System.Linq;

namespace Hawk.Common
{
    public sealed class Singleton<T> where T : class//, new()//new不支持非公共的无参构造函数 
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
        //public static T Instance
        //{
        //    get { return Delay._instance; }
        //}

        //internal class Delay
        //{
        //    //static Delay() { }
        //    //internal static readonly T _instance = new T();
        //    internal static readonly T _instance = (T)Activator.CreateInstance(typeof(T), true);
        //}
        #endregion

        #region Lazy延迟加载
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (ctors.Length != 1)
                throw new InvalidOperationException(string.Format("Type {0} must have exactly one constructor.", typeof(T)));

            var ctor = ctors.SingleOrDefault(c => c.GetParameters().Count() == 0 && c.IsPrivate);
            if (ctor == null)
                throw new InvalidOperationException(string.Format("The constructor for {0} must be private and take no parameters.", typeof(T)));
            return (T)ctor.Invoke(null);
        });

        public static T Instance
        {
            get { return _instance.Value; }
        }
        #endregion
    }
}
