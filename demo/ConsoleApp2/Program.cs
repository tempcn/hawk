using System;
using Hawk;
using Hawk.Common;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using log4net;
using log4net.Repository;
using log4net.Config;
using Newtonsoft.Json;
using System.Configuration;

namespace ConsoleApp2
{
    class Program
    {
        static void Main4()
        {
            string repoName = "NetCoreRepo";
            ILoggerRepository repo = LogManager.CreateRepository(repoName);
            BasicConfigurator.Configure(repo);

            //XmlConfigurator.Configure(repo, new FileInfo("log4net.config"));

            //log4net.ILog logger = log4net.LogManager.GetLogger(repo.Name, "NetCore");

            //logger.Info("netcore log");

            LogHelper.Info("我能说什么呢");

            Console.Read();
        }
            

        static void Main(string[] args)
        {
            //string repoName = "NetCoreRepo";
            //ILoggerRepository repo = LogManager.CreateRepository(repoName);
            //BasicConfigurator.Configure(repo);
            //log4net.ILog logger = log4net.LogManager.GetLogger(repo.Name, "NetCore");
           
            //XmlConfigurator.Configure(repo, new FileInfo("log4net.config"));
            //logger.Info("控制台的日志");

 

            LogHelper.Info("我能说什么呢", "xdf");
            LogHelper.Error("这是一个错误");
            LogHelper.Error("这是一个错误", "xer");

            string root = AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine(root);

            var c = ConfigurationManager.AppSettings["ask"];
            var c2 = ConfigurationManager.ConnectionStrings["nihao"].ConnectionString;

            Console.WriteLine(c);
            Console.WriteLine(c2);

            DataEx.AssemblyName = "mysql.data";
            DataEx.FullName = "MySql.Data.MySqlClient.MySqlClientFactory";

            var sql = @"SELECT a.RoleId,a.MenuId,a.Power,a.Privilege,
            b.RoleName,b.IsAdmin,b.Remark as RoleRemark,
            c.`Name` as MenuName,c.DisplayName,c.ParentId,c.Url,c.Sort,c.Icon,c.Visible,c.Permission,c.Remark as MenuRemark
            FROM role_meun a LEFT JOIN role b ON a.RoleId = b.Id
            LEFT JOIN menu c on a.MenuId = c.Id WHERE RoleId IN
              (SELECT RoleId FROM user_role WHERE UserId = 5)";

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            IEnumerable<UserRole> roles = DataEx.Get<UserRole>(sql);
            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            int count = roles.Count();

            Console.WriteLine(count);
            IDictionary<string, Privilege> dict = new Dictionary<string, Privilege>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in roles)
            {
                var menuName = item.MenuName;//.ToLower();//转换为小写

                IList<Operate> pp = GetPermission(item.Privilege);

                //var dd = pp.Sum(x => (int)x);

                if (dict.ContainsKey(menuName))
                {
                    dict[menuName].Permission =
                        dict[menuName].Permission.Concat(pp).Distinct();

                    dict[menuName].Power = dict[menuName].Permission.Sum(x => (int)x);
                }
                else
                    dict[menuName] = new Privilege()
                    {
                        MenuName = menuName,
                        Power = item.Power,
                        Permission = pp
                    };
            }

            foreach (var d in dict)
            {
                Console.WriteLine("拥有的权限菜单是:{0}", d.Key);
                Console.WriteLine(JsonConvert.SerializeObject(d.Value));
            }

            Console.Read();
        }

        public static IList<Operate> GetPermission(string p)
        {
            IList<Operate> pp = new List<Operate>();

            if (!string.IsNullOrEmpty(p))
            {
                var s = p.Split('#'); int i;
                foreach (var item in s)
                {
                    if (int.TryParse(item, out i))
                        if (!pp.Contains((Operate)i))
                            pp.Add((Operate)i);
                }
            }
            return pp;
        }
    }
}
