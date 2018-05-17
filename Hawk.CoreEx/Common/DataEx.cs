using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Reflection;

//#if NET40 || NET45 || NET452 || NET46
using System.Configuration;
//#endif
using Dapper;

namespace Hawk.Common
{
    public class DataEx//<T> where T:class
    {
//#if NET40 || NET45 || NET452
        static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ConnectionString;
        //#else
        //            public static string ConnectionString { get;set; } 
        //#endif

#if NETSTANDARD2_0 || NETCOREAPP2_0 // !(NET40 || NET45 || NET452)
        public static string AssemblyName { get; set; }
        public static string FullName { get; set; }
        internal class Delay
        {
            internal static readonly DbProviderFactory _instance =
               (DbProviderFactory)(Assembly.Load(AssemblyName).CreateInstance(FullName));
        }
#endif
        static DbProviderFactory Factory
        {
            get
            {
#if NET452
                return DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ProviderName);
#else
                //return RefectHelper.CreateInstance<T>(AssemblyName, FullName) as DbProviderFactory;
                return Delay._instance;
#endif
            }
        }       

        // public static IDbConnection Connection { get; } = Factory.CreateConnection();

        public static IDbConnection GetConn()
        {
            IDbConnection conn = Factory.CreateConnection();
            conn.ConnectionString = ConnectionString;

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            return conn;
        }

        public static int Execute(string sql, object param = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConn())
            {
                return conn.Execute(sql, param, commandTimeout: commandTimeout, commandType: commandType);
            }
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

        public static T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConn())
            {
                return conn.ExecuteScalar<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
            }
        }

        public static int GetCount(string table, string where = null)
        {
            var sql = $"select count(1) from {table} {where}";
            using (var conn = GetConn())
            {
                return conn.ExecuteScalar<int>(sql);
            }
        }

        /// <summary>
        /// 分页存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="args"></param>
        /// <param name="proc"></param>
        /// <param name="outParam"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetPageResult<T>(out int count, IEnumerable<KeyValuePair<string, object>> args,
            string proc = "page_result", string outParam = "p_recordcount", string prefix = "?")
        {
            var param = new DynamicParameters();
            foreach (var item in args)
            {
                //param.Add("?p_table", table);
                //param.Add("?p_columns", columns);
                //param.Add("?p_page", page);
                //param.Add("?p_size", size);
                //param.Add("?p_order", order);
                //param.Add("?p_order_type", orderType);
                //param.Add("?p_where", where);
                //param.Add("?p_recordcount", null, DbType.Int32, ParameterDirection.Output);
                param.Add(prefix + item.Key, item.Value);
            }
            param.Add(prefix + outParam, null, DbType.Int32, ParameterDirection.Output);

            using (var conn = GetConn())
            {
                var s = conn.Query<T>(proc, param: param, commandType: CommandType.StoredProcedure);
                count = param.Get<int>(prefix + outParam);
                return s;
            }
        }  

        public static DbDataReader ExecuteReader(string cmdText)
        {
            return ExecuteReader(cmdText, CommandType.Text, null);
        }

        public static DbDataReader ExecuteReader(string cmdText, CommandType cmdType, params DbParameter[] cmdParms)
        {
            DbConnection conn = Factory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            DbCommand cmd = conn.CreateCommand();// Factory.CreateCommand();

            PrepareCommand(cmd, conn, cmdText, cmdType, cmdParms);

            DbDataReader dataReader;

            try
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
            }
            catch { dataReader = null; }
            return dataReader;
        }

        static void PrepareCommand(DbCommand cmd, DbConnection conn, string cmdText, CommandType cmdType, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (DbParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
}
