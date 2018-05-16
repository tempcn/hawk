using System;
using Hawk;
using Hawk.Common;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.IO;
using log4net;
using log4net.Repository;
using log4net.Config;

namespace ConsoleApp2
{
    class Program
    {
        static void Main()
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
            

        static void Main2(string[] args)
        {
            //DataEx<MySqlClientFactory>.ConnectionString = "server=localhost;port=3307;user id=root;pwd=mysql;database=sports;sslmode=none;pooling=true;Charset=utf8";
            //DataEx<MySqlClientFactory>.AssemblyName = "mysql.data";
            //DataEx<MySqlClientFactory>.FullName = "MySql.Data.MySqlClient.MySqlClientFactory";

            DataEx.ConnectionString = "server=localhost;port=3307;user id=root;pwd=mysql;database=sports;sslmode=none;pooling=true;Charset=utf8";
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

            //System.Data.Common.DbProviderFactory x =
            //  (System.Data.Common.DbProviderFactory)  RefectHelper.CreateInstance<MySqlClientFactory>("mysql.data", "MySql.Data.MySqlClient.MySqlClientFactory");

            //System.Data.Common.DbConnection conn = x.CreateConnection();


        }
    }
}
