using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hawk.Common;

namespace ConsoleApp1
{
    public class LogExp
    {
        static void Main()
        {

            LogHelper.Info("我能说什么呢", "xdf");
            LogHelper.Error("这是一个错误");
            LogHelper.Error("这是一个错误", "xer");

            string root = AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine(root);
        }
    }
}
