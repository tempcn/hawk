using System;
using log4net;
using log4net.Repository;
using log4net.Config;
using System.IO;

namespace Hawk.Common
{
    public class LogHelper
    {
        static ILoggerRepository repo = LogManager.CreateRepository(nameof(LogHelper));
        static string config = AppDomain.CurrentDomain.BaseDirectory + "Config\\log4net.config";
        static LogHelper()
        {
            if (File.Exists(config))
                XmlConfigurator.Configure(repo, new FileInfo(config));
            else
            {
                //XmlConfigurator.Configure(repo);
                BasicConfigurator.Configure(repo);
            }
        }

        static ILog GetLogger(string name = nameof(LogHelper))
        => LogManager.GetLogger(repo.Name, string.IsNullOrEmpty(name) ? nameof(LogHelper) : name);


        //static ILog logger = LogManager.GetLogger(repo.Name, typeof(LogHelper));

        public static void Info(string s, string name = null, Exception ex = null)
        {
            ILog log = GetLogger(name);
            if (log.IsInfoEnabled)
            {
                log.Info(s, ex);
            }
        }

        public static void Info(string s, string name = null, params object[] args)
        {
            ILog log = GetLogger(name);
            if (log.IsInfoEnabled)
            {
                log.InfoFormat(s, args);
            }
        }

        public static void Error(string s, string name = null, Exception ex = null)
        {
            ILog log = GetLogger(name);
            if (log.IsErrorEnabled)
            {
                log.Error(s, ex);
            }
        }
        public static void Error(string s, string name = null, params object[] args)
        {
            ILog log = GetLogger(name);
            if (log.IsErrorEnabled)
            {
                log.ErrorFormat(s, args);
            }
        }
    }
}
