using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Configuration;
using Newtonsoft.Json;

namespace Hawk.Common
{
    public class RedisHelper
    {
        RedisHelper() { }

        // static ConnectionMultiplexer Instance = Singleton<ConnectionMultiplexer>.Instance;

#if NET40 || NET45 ||NET452 || NET46
        public static string Config { get; set; } = ConfigurationManager.ConnectionStrings["redis"].ConnectionString;
#else
       public static string Config { get; set; }
#endif
        static readonly object _lock = new object();
        static ConnectionMultiplexer _instance;
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null || !_instance.IsConnected)
                    lock (_lock)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            Console.WriteLine("创建了一个对象");
                            _instance = ConnectionMultiplexer.Connect(Config);
                        }
                    }

                return _instance;
            }
        }

        public static string Get(string key, int db = -1)
            => Instance.GetDatabase(db).StringGet(key);

        public static async Task<string> GetAsync(string key, int db = -1)
            => await Instance.GetDatabase(db).StringGetAsync(key);

        public static bool Set(string key, RedisValue value, DateTimeOffset? expiry = null, int db = -1)
        {
            TimeSpan? ts = null;
            if (expiry.HasValue) ts = expiry.Value.ToUniversalTime() - DateTime.UtcNow;
            return Instance.GetDatabase(db).StringSet(key, value, ts);
        }

        public static async Task<bool> SetAsync(string key, RedisValue value, DateTimeOffset? expiry = null, int db = -1)
        {
            TimeSpan? ts = null;
            if (expiry.HasValue) ts = expiry.Value.ToUniversalTime() - DateTime.UtcNow;
            return await Instance.GetDatabase(db).StringSetAsync(key, value, ts);
        }

        public static T GetEx<T>(string key, int db = -1)
        {
            T t = default(T);
            var s = Get(key, db);
            if (!string.IsNullOrEmpty(s))
                t = JsonConvert.DeserializeObject<T>(s);
            return t;
        }

        public static bool SetEx(string key, object value, DateTimeOffset? expiry = null, int db = -1)
        {
            if (value == null) return false;
            TimeSpan? ts = null;
            if (expiry.HasValue) ts = expiry.Value.ToUniversalTime() - DateTime.UtcNow;
            return Instance.GetDatabase(db).StringSet(key, JsonConvert.SerializeObject(value), ts);
        }

        public static bool HashSet(string key, string field, RedisValue value, int db = -1)
            => Instance.GetDatabase(db).HashSet(key, field, value, When.Always, CommandFlags.None);

        public static async Task<bool> HashSetAsync(string key, string field, RedisValue value, int db = -1)
        => await Instance.GetDatabase(db).HashSetAsync(key, field, value, When.Always, CommandFlags.None);

        public static void HashSet(string key, HashEntry[] entry, int db = -1)
        => Instance.GetDatabase(db).HashSet(key, entry, CommandFlags.None);

        public static async Task HashSetAsync(string key, HashEntry[] entry, int db = -1)
        => await Instance.GetDatabase(db).HashSetAsync(key, entry, CommandFlags.None);

        public static string HashGet(string key, string field, int db = -1)
            => Instance.GetDatabase(db).HashGet(key, field);

        public static RedisValue[] HashGet(string key, RedisValue[] fields, int db = -1)
           => Instance.GetDatabase(db).HashGet(key, fields);

        public static async Task<RedisValue> HashGetAsync(string key, string field, int db = -1)
              => await Instance.GetDatabase(db).HashGetAsync(key, field);

        public static async Task<RedisValue[]> HashGetAsync(string key, RedisValue[] fields, int db = -1)
        => await Instance.GetDatabase(db).HashGetAsync(key, fields);

        public static HashEntry[] HashGetAll(string key, int db = -1)
      => Instance.GetDatabase(db).HashGetAll(key);

        public static async Task<HashEntry[]> HashGetAllAsync(string key, int db = -1)
     => await Instance.GetDatabase(db).HashGetAllAsync(key);

        public static bool KeyExpire(string key, TimeSpan? expiry, int db = -1)
        => Instance.GetDatabase(db).KeyExpire(key, expiry);


        public static bool KeyExpire(string key, DateTime? expiry, int db = -1)
        => Instance.GetDatabase(db).KeyExpire(key, expiry);

        public static bool KeyExists(string key, int db = -1)
            => string.IsNullOrEmpty(key) ? false : Instance.GetDatabase(db).KeyExists(key);

        //#if !NET40
        //        public static async Task<bool> KeyExistsAsync(string key, int db = -1)
        //       => await (string.IsNullOrEmpty(key) ? Task.FromResult(false) : Instance.GetDatabase(db).KeyExistsAsync(key));
        //#endif
        public static async Task<bool> KeyExistsAsync(string key, int db = -1)
      => await Instance.GetDatabase(db).KeyExistsAsync(key);

        public static bool HashExists(string key, RedisValue field, int db = -1)
          => string.IsNullOrEmpty(key) ? false : Instance.GetDatabase(db).HashExists(key, field);

        //#if !NET40
        //        public static async Task<bool> HashExistsAsync(string key, RedisValue field, int db = -1)
        //       => await (string.IsNullOrEmpty(key) ? Task.FromResult(false) : Instance.GetDatabase(db).HashExistsAsync(key, field));
        //#endif

        public static async Task<bool> HashExistsAsync(string key, RedisValue field, int db = -1)
=> await Instance.GetDatabase(db).HashExistsAsync(key, field);

        public static bool KeyDelete(string key, int db = -1)
    => string.IsNullOrEmpty(key) ? false : Instance.GetDatabase(db).KeyDelete(key);

        //#if !NET40
        //        public static async Task<bool> KeyDeleteAsync(string key, int db = -1)
        //       => await (string.IsNullOrEmpty(key) ? Task.FromResult(false) : Instance.GetDatabase(db).KeyDeleteAsync(key));
        //#endif

        public static async Task<bool> KeyDeleteAsync(string key, int db = -1)
       => await Instance.GetDatabase(db).KeyDeleteAsync(key);


        public static bool HashDelete(string key, RedisValue field, int db = -1)
          => string.IsNullOrEmpty(key) ? false : Instance.GetDatabase(db).HashDelete(key, field);

        //#if !NET40
        //        public static async Task<bool> HashDeleteAsync(string key, RedisValue field, int db = -1)
        //       => await (string.IsNullOrEmpty(key) ? Task.FromResult(false) : Instance.GetDatabase(db).HashDeleteAsync(key, field));
        //#endif
        public static async Task<bool> HashDeleteAsync(string key, RedisValue field, int db = -1)
       => await Instance.GetDatabase(db).HashDeleteAsync(key, field);

        public static RedisValue[] HashKeys(string key, int db = -1)
                => Instance.GetDatabase(db).HashKeys(key);

        public static async Task<RedisValue[]> HashKeysAsync(string key, int db = -1)
              => await Instance.GetDatabase(db).HashKeysAsync(key);

        /// <summary>
        /// 计算给定字符串中,被设置为1的比特位的数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"> -1表示最后一个字符， -2表示倒数第二个，以此类推</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static long BitCount(string key, long start = 0, long end = -1, int db = -1)
        => Instance.GetDatabase(db).StringBitCount(key, start, end);

        /// <summary>
        /// 计算给定字符串中,被设置为1的比特位的数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"> -1表示最后一个字符， -2表示倒数第二个，以此类推</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static async Task<long> BitCountAsync(string key, long start = 0, long end = -1, int db = -1)
        => await Instance.GetDatabase(db).StringBitCountAsync(key, start, end);

        /// <summary>
        /// 设置或清除指定偏移量上的位
        /// </summary>
        /// <param name="key"></param>
        /// <param name="offset"></param>
        /// <param name="bit"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool SetBit(string key, long offset, bool bit = false, int db = -1)
         => Instance.GetDatabase(db).StringSetBit(key, offset, bit);

        public static async Task<bool> SetBitAsync(string key, long offset, bool bit = false, int db = -1)
            => await Instance.GetDatabase(db).StringSetBitAsync(key, offset, bit);

        /// <summary>
        /// 获取指定偏移量上的位
        /// </summary>
        /// <param name="key"></param>
        /// <param name="offset"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool GetBit(string key, long offset, int db = -1)
         => Instance.GetDatabase(db).StringGetBit(key, offset);

        public static async Task<bool> GetBitAsync(string key, long offset, int db = -1)
            => await Instance.GetDatabase(db).StringGetBitAsync(key, offset);

        public static long Incrby(string key, long value, int db = -1)
         => Instance.GetDatabase(db).StringIncrement(key, value);

        public static double Incrby(string key, double value, int db = -1)
      => Instance.GetDatabase(db).StringIncrement(key, value);

        public static long Decrby(string key, long value, int db = -1)
       => Instance.GetDatabase(db).StringDecrement(key, value);

        public static double Decrby(string key, double value, int db = -1)
      => Instance.GetDatabase(db).StringDecrement(key, value);

        public static RedisResult ExecScript(string script, object param, int db = -1)
        {
            var prepared = LuaScript.Prepare(script);
            return Instance.GetDatabase(db).ScriptEvaluate(prepared, param);
        }

        public static RedisResult ExecScript(string script, RedisKey[] keys = null, RedisValue[] values = null, int db = -1)
        {
            return Instance.GetDatabase(db).ScriptEvaluate(script, keys, values);
        }

        public static RedisResult ExecScript(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, int db = -1)
        {
            return Instance.GetDatabase(db).ScriptEvaluate(hash, keys, values);
        }
    }
}
