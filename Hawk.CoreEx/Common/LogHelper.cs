using System;

namespace Hawk.Common
{
    public class LogHelper
    {
       static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LogHelper));

        public static void Info(string s, Exception ex = null)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(s, ex);
            }
        }

        public static void Info(string s, params object[] args)
        {
            if (logger.IsInfoEnabled)
            {
                logger.InfoFormat(s, args);
            }
        }

        public static void Error(string s,  Exception ex = null)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(s, ex);
            }
        }
        public static void Error(string s, params object[] args)
        {
            if (logger.IsErrorEnabled)
            {
                logger.ErrorFormat(s, args);
            }
        }
    }
}
