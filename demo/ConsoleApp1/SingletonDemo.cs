using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hawk;
using System.Threading;

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

    public class SingletonDemo
    {
        static void Main()
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
