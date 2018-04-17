using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using Dapper;
using MySql.Data;
using System.Configuration;
using Newtonsoft.Json;
using log4net;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Hawk.Demo
{
    class Program
    {
        static void Main44(string[] args)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            dict["ab"] = "你好";
            dict["AB"] = "中国";
            //dict["aB"] = "说什么呢";

            if (dict.ContainsKey("ab"))
                Console.WriteLine("包含,值是:{0}", dict["Ab"]);
            else
                Console.WriteLine("nothing");
        }
        static void Main(string[] args)
        {
            var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            AppSettings settings = new AppSettings(conf);

          

           ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            // 默认简单配置，输出至控制台
            log4net.Config.BasicConfigurator.Configure(repository);
            ILog log = LogManager.GetLogger(repository.Name, "NETCorelog4net");

            log.Info("我是log4net输出:hello");

            log.InfoFormat("我是log4net输出:hello2");
            log.InfoFormat("我是log4net输出:{0}", "格式化输出");

            Console.WriteLine("Hello World!");

            IDictionary<string, Privilege> dict = new Dictionary<string, Privilege>(StringComparer.OrdinalIgnoreCase);

            var sql = @"SELECT a.RoleId,a.MenuId,a.Power,a.Privilege,
b.RoleName,b.IsAdmin,b.Remark as RoleRemark,
c.`Name` as MenuName,c.DisplayName,c.ParentId,c.Url,c.Sort,c.Icon,c.Visible,c.Permission,c.Remark as MenuRemark
FROM role_meun a LEFT JOIN role b ON a.RoleId = b.Id
LEFT JOIN menu c on a.MenuId = c.Id WHERE RoleId IN
  (SELECT RoleId FROM user_role WHERE UserId = 5)";

            IEnumerable<UserRole> roles = Helper.Get<UserRole>(sql);

            int count = roles.Count();

            foreach (var item in roles)
            {
                var menuName = item.MenuName;//.ToLower();//转换为小写

                IList<Permission> pp = GetPermission(item.Privilege);

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

        }

        public static IList<Permission> GetPermission(string p)
        {
            IList<Permission> pp = new List<Permission>();

            if (!string.IsNullOrEmpty(p))
            {
                var s = p.Split('#'); int i;
                foreach (var item in s)
                {
                    if (int.TryParse(item, out i))
                        if (!pp.Contains((Permission)i))
                            pp.Add((Permission)i);
                }
            }
            return pp;
        }
    }

    public class AppSettings
    {
        static string section = "AppSettings";

        private readonly string ConnString;

        public AppSettings(IConfiguration config)
        {
            ConnString = config.GetSection(section)["connString"];
        }
    }

        public class Helper
        {
            public static string ConnectionString { get; } = //ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ConnectionString;
                 "server=localhost;port=3307;user id=root;pwd=mysql; database=sports; pooling=true;Charset=utf8";
            public static DbProviderFactory Factory { get; } = // DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ProviderName);
                new MySql.Data.MySqlClient.MySqlClientFactory();

            public static IDbConnection GetConn()
            {
                IDbConnection conn = Factory.CreateConnection();
                conn.ConnectionString = ConnectionString;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                return conn;
            }

            public static IEnumerable<T> Get<T>(string sql, object param = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
            {
                using (var conn = GetConn())
                {
                    return conn.Query<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
                }
            }

            public static T FirstOrDefault<T>(string sql, object param = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
            {
                using (var conn = GetConn())
                {
                    return conn.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
                }
            }
        }

        public class UserRole
        {
            public int RoleId { get; set; }
            public int MenuId { get; set; }
            public int Power { get; set; }
            public string Privilege { get; set; }
            public string RoleName { get; set; }
            public byte IsAdmin { get; set; }
            public string RoleRemark { get; set; }
            public string MenuName { get; set; }
            public string DisplayName { get; set; }
            public int ParentId { get; set; }
            public string Url { get; set; }
            public int Sort { get; set; }
            public string Icon { get; set; }
            public byte Visible { get; set; }
            public string Permission { get; set; }
            public string MenuRemark { get; set; }
        }

        public class Privilege
        {
            public string MenuName { get; set; }
            public int Power { get; set; }
            public IEnumerable<Permission> Permission { get; set; }
        }

        public class Role
        {
            public int Id { get; set; }
            public string RoleName { get; set; }
            public byte IsAdmin { get; set; }
            public int Created { get; set; }
            public DateTime Updated { get; set; }
            public int CreateUserId { get; set; }
            public int UpdateUserId { get; set; }
            public string Remark { get; set; }
        }

        public class Menu
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public int ParentId { get; set; }
            public string Url { get; set; }
            public int Sort { get; set; }
            public string Icon { get; set; }
            public byte Visible { get; set; }
            public string Permission { get; set; }
            public string Remark { get; set; }
        }


        public enum Permission
        {
            Not = 0,
            Add = 1,
            Delete = 2,
            Update = 4,
            Select = 8,
            Print = 16,
            All = 1024
        }
}