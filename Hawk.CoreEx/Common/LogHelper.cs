using System;
using log4net;
using log4net.Repository;
using log4net.Config;
using System.IO;

namespace Hawk.Common
{
    public class LogHelper
    {
        public static ILoggerRepository Repo { get; private set; } = LogManager.CreateRepository(nameof(LogHelper));

        //static string Root = AppDomain.CurrentDomain.BaseDirectory;

        static LogHelper()
        {
            //if (File.Exists(Root + "log4net.config"))
            //    Configure(Root + "log4net.config");
            //else if (File.Exists(Root + "log4net"))
            //    Configure(Root + "log4net");
            //else if (File.Exists(Root + "config\\log4net.config"))
            //    Configure(Root + "config\\log4net.config");
            if (File.Exists("log4net.config"))
                XmlConfigurator.Configure(Repo, new FileInfo("log4net.config"));
            else
            {
                XmlConfigurator.Configure(Repo);
                //BasicConfigurator.Configure(repo);
            }
        }

        //static void Configure(string file)
        //    => XmlConfigurator.Configure(repo, new FileInfo(file));

        static ILog GetLogger(string name = nameof(LogHelper)) => LogManager.GetLogger(Repo.Name, name ?? nameof(LogHelper)); //LogManager.GetLogger(repo.Name, string.IsNullOrEmpty(name) ? nameof(LogHelper) : name);


        static ILog logger = GetLogger();// LogManager.GetLogger(repo.Name, nameof(LogHelper));

        public static void Info(string s, string name = "Info", Exception ex = null)
        {
            ILog log = GetLogger(name);
            if (log.IsInfoEnabled)
            {
                log.Info(s, ex);
            }
        }

        public static void Info(string s, params object[] args)
        {
            if (logger.IsInfoEnabled) logger.InfoFormat(s, args);
        }

        public static void Debug(string s, string name = "Debug", Exception ex = null)
        {
            ILog log = GetLogger(name);
            if (log.IsDebugEnabled)
            {
                log.Debug(s, ex);
            }
        }

        public static void Debug(string s, params object[] args)
        {
            if (logger.IsDebugEnabled) logger.DebugFormat(s, args);
        }

        public static void Error(string s, string name = "Error", Exception ex = null)
        {
            ILog log = GetLogger(name);
            if (log.IsErrorEnabled)
            {
                log.Error(s, ex);
            }
        }
        public static void Error(string s, params object[] args)
        {
            if (logger.IsErrorEnabled) logger.ErrorFormat(s, args);
        }
    }
}
