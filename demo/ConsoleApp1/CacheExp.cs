using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hawk;
using Hawk.Common;
using Quartz;
using Quartz.Impl;
using System.ComponentModel;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ConsoleApp1
{
    public enum TestEnum
    {
        [Description("我是MD5的描述")]
        MD5,
        SHA1
    }
    public class CacheExp
    {
        static void Main6()
        {
            TestEnum k = TestEnum.SHA1;
            Console.WriteLine(k.Description());
            Console.WriteLine(k.Description().IsNotNullOrEmpty());

            IList<int> ki = new List<int>();
            //for (int i = 0; i < 10; i++)
            //{
            //    ki.Add(i);
            //}
            ki.Add(2);

            Console.WriteLine(ki.JoinEx());

            //RedisHelper.Config = "127.0.0.1:4963,password=IAmWoker!520LoverForever*(annotation)LinuxWindowsMac,ssl=false";

            //string script = "redis.call('set',@key,@value)";

            //var ss = RedisHelper.ExecScript(script, new
            //{
            //  key = "nihao2", value = "值1" 
            //}
            //);
            //Console.WriteLine(ss.IsNull);
            //Console.WriteLine(!ss.IsNull ? ss.ToString() : "null");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                var thread = new Thread(new ThreadStart(Run));
                thread.Start();
                //Task.Run(() => Run());
            }

            //Console.WriteLine(RedisHelper.Config);
            Console.Read();
        }
        static void Run()
        {
            Console.WriteLine(DateTime.Now.ToFullString());

            var s = RedisHelper.Set("nihao", "中华人民共和国", DateTimeOffset.Now.AddSeconds(10));
            Console.WriteLine(s);
            var g = RedisHelper.Get("nihao");
            Console.WriteLine(g);
        }

        static ISchedulerFactory sf = new StdSchedulerFactory();

        static IScheduler scheduler = null;

        static void Main000()
        {
            LogHelper.Info("控制台的日志");
            Console.WriteLine(DateTime.Now.ToStringEx());

            //Task.Run(() => { 
            //JobExtend.JobsEx<EventJob>("jobName", "TrigName",
            //     item: new KeyValuePair<string, object>("key1", "这是传递的值"),
            //     seconds: 3, count: 5);

            //JobExtend.JobsEx<EventJob2>("jobName", "TrigName",
            //     new KeyValuePair<string, object>("key1", "这是传递的值2"), 4, 3);

           // });
            //JobEx.JobsFactory<EventJob>("jobName", "TrigName", "SchedName",
            //    item: new KeyValuePair<string, object>("key1", "这是传递的值"),
            //    seconds: 3, count: 5);


            //var schedName = "SchedName";
            //scheduler = sf.GetScheduler().Result;
            //scheduler.Start();

            //IJobDetail detail = JobBuilder.Create<EventJob>().WithIdentity("jobName").Build();

            //detail.JobDataMap.Add("key1", "value1");

            //ITrigger builder = TriggerBuilder.Create()
            //    .WithIdentity("king")
            //    .StartNow()
            //    //.WithSimpleSchedule()
            //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).WithRepeatCount(4))
            //    .Build();

            //scheduler.ScheduleJob(detail, builder);

            Console.Read();
        }
    }

    public class EventJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                var s = context.JobDetail.JobDataMap.Get("key1");

                Console.WriteLine("quartz is run at:" + s + "," + DateTime.Now.ToString());
            });
        }
    }

    public class EventJob2 : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                var s = context.JobDetail.JobDataMap.Get("key1");

                Console.WriteLine("quartz is run at:" + s + "," + DateTime.Now.ToString());
            });
        }
    }
}
