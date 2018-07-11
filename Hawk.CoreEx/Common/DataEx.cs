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
        static ConnectionStringSettings _ConnSet = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1];

        //#if NET40 || NET45 || NET452
        public static string ConnectionString { get; set; } = _ConnSet.ConnectionString; //ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ConnectionString;

        public static string ProviderName { get; set; } = _ConnSet.ProviderName;
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
                return DbProviderFactories.GetFactory(ProviderName); //(ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1].ProviderName);
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

        public static bool ExecuteTrans(string sql)
        {
            bool exec = false;
            using (var conn = GetConn())
            {
                var trans = conn.BeginTransaction();
                try
                {
                    conn.Execute(sql, transaction: trans);
                    trans.Commit();
                    exec = true;
                }
                catch
                {
                    trans.Rollback();
                    exec = false;
                }
            }
            return exec;
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
            var sql = $"SELECT COUNT(1) FROM {table} {where}";
            using (var conn = GetConn())
            {
                return conn.ExecuteScalar<int>(sql);
            }
        }

        /// <summary>
        /// 执行存储过程,获取返回数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proc"></param>
        /// <param name="outParam"></param>
        /// <param name="args">存储过程参数名和对应值</param>
        /// <param name="prefix">SqlServer[@],MySql[?],Oracle[:]</param>
        /// <returns></returns>
        public static T GetStored<T>(string proc, string outParam, IEnumerable<KeyValuePair<string, object>> args, string prefix = "?")
        {
            var param = new DynamicParameters();
            foreach (var item in args)
            {
                param.Add(prefix + item.Key, item.Value);
            }
            //param.Add(prefix + outParam, null, DbType.Int32, ParameterDirection.Output);
            using (var conn = GetConn())
            {
                int s = conn.Execute(proc, param, commandType: CommandType.StoredProcedure);
                if (s > 0)
                    return param.Get<T>(prefix + outParam);
                return default(T);
            }
        }

        /// <summary>
        /// 分页存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count">返回的总数量</param>
        /// <param name="args">参数</param>
        /// <param name="proc">存储过程名</param>
        /// <param name="outParam">返回参数名</param>
        /// <param name="prefix">SqlServer[@],MySql[?],Oracle[:]</param>
        /// <returns></returns>
        public static IEnumerable<T> GetPageResult<T>(out int count, IEnumerable<KeyValuePair<string, object>> args,
            string proc, string outParam, string prefix = "?")
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
