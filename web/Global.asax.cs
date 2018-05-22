using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using log4net;
using log4net.Repository;

namespace Hawk.Exp
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // ILoggerRepository repo = LogManager.CreateRepository(nameof(LogHelper));
            //log4net.Config.XmlConfigurator.Configure(LogHelper.repo);//, new System.IO.FileInfo("config\\log4net.config"));
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}