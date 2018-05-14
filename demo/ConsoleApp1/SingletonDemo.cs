using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hawk;
using System.Threading;
using Hawk.Common;

namespace ConsoleApp1
{

    public class MyJob
    {
        private string _timeSpan;

        //public MyJob()
        // {
        //     _timeSpan = DateTime.Now.ToString("yyyy-MM-ddHH:mm:ss:ffff");// DateTime.Now.Millisecond.ToString(); //DateTime.Now.ToString("yyyy-MM-ddHH:mm:ss:fff");
        // }
        private MyJob()
        {
            _timeSpan = DateTime.Now.Millisecond.ToString();
            _timeSpan = DateTime.Now.ToFullString();
            _timeSpan = DateTime.Now.ToString("yyyy-MM-ddHH:mm:ss:fffffff");
        }
        public string Write()
        {
            return _timeSpan;
        }
    }

    public class ExeJob
    {
        static EnModl modl;

        public static void Med(EnModl enModl)
        {
            lock(enModl)
            {

            }
        }
    }

    public class SingletonDemo
    {
        static void Main()
        {
            var s = "实体缓存的命中率基本上都超过了99%，其原理是整表缓存，每60秒过期，或者该表有数据变更时过期单对象缓存命中率也超过了90%，其策略是建立一个KeyValue字典，主键为Key，实体对象为Value，每次获取对象时先去字典里面找，没有再查数据库然后加入缓存。同样支持过期时间和数据变更控制一级缓存命中率不高，只有47%，但它是无差别精确缓存，63万次查询命中了30万，减少了30万次查询。其策略是根据sql缓存该表所有查询， 除非出现针对该表的Insert/Update/Delete操作。";

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            var initial = Hawk.Common.Spell.GetInitial(s);
            stopwatch.Stop();
            Console.WriteLine(initial);
            Console.WriteLine("首字母运行时间:{0}" + stopwatch.ElapsedMilliseconds);
            stopwatch.Start();
            var get = Hawk.Common.Spell.Get(s);
            stopwatch.Stop();
            Console.WriteLine(get);

            Console.WriteLine("全拼运行时间:{0}" + stopwatch.ElapsedMilliseconds);

            int ik = 50;
            var sk = Rand.Split(ik, 8); int result = 0;

            for (int i = 0; i < sk.Length; i++)
            {
                result += sk[i];
                Console.WriteLine(sk[i]);
            }
            Console.WriteLine("最后结果是:{0},总数量是:{1}", result, sk.Length);

            int sum = 0;
            Console.WriteLine(Rand.GetMathString(out sum));
            Console.WriteLine(sum);

            Console.WriteLine(Rand.GetMath(out sum));
            Console.WriteLine(sum);

            Console.WriteLine(Rand.GetSimplifiedChinese(6));
            Console.WriteLine(Rand.GetGBKString(6));
        }

        static void Main10()
        {
            var o = DateTime.Now.ToString("G");
            Console.WriteLine("时间格式\"O\",结果是:{0}", o);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                var thread = new Thread(new ThreadStart(Run));
                thread.Start();
            }

            Console.WriteLine(DateTime.Now.ToFullString());
            Console.Read();
        }

        static void Run()
        {
            Console.WriteLine(Singleton<MyJob>.Instance.Write());
        }
    }
}
