using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl;

namespace Hawk.Common
{
    public class QuartzEx
    {
        //static log4net.ILog logger = log4net.LogManager.GetLogger("JobExtend");

        static ISchedulerFactory sf = new StdSchedulerFactory();

        static IScheduler scheduler = null;//            sf.GetScheduler();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="triggerName"></param>
        /// <param name="item"></param>
        /// <param name="seconds">多久执行一次,</param>
        /// <param name="count">执行多少次</param>
        /// <param name="date"></param>
        public static void JobsEx<T>(string jobName, string triggerName, KeyValuePair<string, object>? item = null,
             int seconds = 1, int count = 1, DateTimeOffset? date = null)
        {

#if NET40 || NET45
                scheduler = sf.GetScheduler();
#else
            scheduler = sf.GetScheduler().Result;
#endif
            scheduler.Start();

            IJobDetail detail = JobBuilder.Create(typeof(T)).WithIdentity(jobName).Build(); //new JobDetailImpl(jobName, typeof(T));

            if (item.HasValue)
                detail.JobDataMap.Add(item.Value);

            TriggerBuilder builder = TriggerBuilder.Create().WithIdentity(triggerName); //TriggerBuilder.Create().WithIdentity(triggerName).StartAt(date.HasValue ? date.Value : DateTimeOffset.Now);
            if (date.HasValue)
                builder = builder.StartAt(date.Value);
            else
                builder = builder.StartNow();

            if (count > 1)
            {
                if (seconds < 1)
                    seconds = 1;
                builder = builder.WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).WithRepeatCount(count - 1));
            }
            else
                builder = builder.WithSimpleSchedule();

            ITrigger trigger = builder.Build();
            scheduler.ScheduleJob(detail, trigger);

            LogHelper.Info("scheduler is start " + DateTime.Now.ToStringEx());
            //if (item.HasValue)
            //    logger.Info("scheduler param[" + JsonConvert.SerializeObject(item.Value) + "]", nameof(JobExtend));

            //logger.Info("scheduler is shutdown:" + scheduler.IsShutdown);
            //Console.WriteLine("scheduler is start" + DateTime.Now.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="triggerName"></param>
        /// <param name="corn"></param>
        /// <param name="item"></param>
        public static void JobsExCorn<T>(string jobName, string triggerName, string corn, KeyValuePair<string, object>? item = null)
        {

#if NET40 || NET45
                scheduler = sf.GetScheduler();
#else
            scheduler = sf.GetScheduler().Result;
#endif
            scheduler.Start();

            IJobDetail detail = JobBuilder.Create(typeof(T)).WithIdentity(jobName).Build(); //new JobDetailImpl(jobName, typeof(T));

            if (item.HasValue)
                detail.JobDataMap.Add(item.Value);

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(triggerName)
                .StartNow()
                .WithCronSchedule(corn)
                .Build();

            scheduler.ScheduleJob(detail, trigger);

            LogHelper.Info("scheduler is start " + DateTime.Now.ToStringEx());
        }
    }
}