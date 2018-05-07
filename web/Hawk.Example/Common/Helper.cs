using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Dapper;

namespace Hawk.Example.Common
{
    public class Helper
    {
        public static string ConnectionString { get; } = //ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ConnectionString;
             "server=localhost;port=3307;user id=root;pwd=mysql; database=sports; pooling=true;Charset=utf8";
        public static DbProviderFactory Factory { get; } = // DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ProviderName);
            new MySqlClientFactory();

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
}
