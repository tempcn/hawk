using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Hawk.Common
{
    public class JsonSerializeHelper
    {      
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFile"></param>
        /// <returns></returns>
        public static T Load<T>(string jsonFile)
        {
            using (var sr = new StreamReader(jsonFile, Encoding.UTF8))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonReader reader = new JsonTextReader(sr);
                return serializer.Deserialize<T>(reader);
            }
        }

        public static bool Save(object obj, string jsonFile)
        {
            using (var sw = new StreamWriter(jsonFile, false, Encoding.UTF8))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonWriter writer = new JsonTextWriter(sw);
                serializer.Serialize(writer, obj);
            }
            return true;
        }
    }
}
