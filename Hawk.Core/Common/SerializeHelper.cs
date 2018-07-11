using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Hawk.Common
{
    public class SerializeHelper
    {
        static IDictionary<int, XmlSerializer> xmlDict = new Dictionary<int, XmlSerializer>();

        static XmlSerializer GetSerializer(Type t)
        {
            int typeHash = t.GetHashCode();
            if (!xmlDict.ContainsKey(typeHash))
                xmlDict[typeHash] = new XmlSerializer(t);
            //xmlDict.Add(typeHash, new XmlSerializer(t));
            return xmlDict[typeHash];
        }

        //public static string Serializer(object obj)
        //{
        //    var xsn = new XmlSerializerNamespaces();
        //    xsn.Add(string.Empty, string.Empty);            
        //    XmlSerializer xs = new XmlSerializer(obj.GetType());
        //    StringWriter sw = new StringWriter();

        //    xs.Serialize(sw, obj,xsn);
        //    return sw.ToString();
        //}

        /// <summary>
        /// 序列化成Xml字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>xml字符串</returns>
        public static string Serialize(object obj)
        {
            string result = "";
            XmlSerializer xs = GetSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xtw = null;
            StreamReader sr = null;
            var xsn = new XmlSerializerNamespaces();//根节点忽略命名空间
            xsn.Add(string.Empty, string.Empty);
            try
            {
                xtw = new XmlTextWriter(ms, Encoding.UTF8)
                {
                    Formatting = Formatting.Indented
                };
                xs.Serialize(xtw, obj, xsn);
                //xs.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(ms);
                result = sr.ReadToEnd();
            }
#if DEBUG
            catch (Exception ex)
            {
             throw ex;
            }
#else
            catch { }
#endif
            finally
            {
                if (xtw != null)
                    xtw.Close();
                if (sr != null)
                    sr.Close();
                ms.Close();
            }
            return result;
        }

        //public static object Des(Type t,string s)
        //{
        //    byte[] b = Encoding.UTF8.GetBytes(s);
        //    try
        //    {
        //        XmlSerializer xs = GetSerializer(t);
        //        return xs.Deserialize(new MemoryStream(b));
        //    }
        //    catch { throw; }
        //}

        /// <summary>
        /// Xml字符串反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string s) where T : class, new()
        {
            T t = new T();
            byte[] b = Encoding.UTF8.GetBytes(s);
            try
            {
                XmlSerializer xs = GetSerializer(t.GetType());
                t = (T)xs.Deserialize(new MemoryStream(b));
            }
#if DEBUG
            catch (Exception ex)
            {
             throw ex;
            }
#else
            catch { }
#endif
            return t;
        }

        /// <summary>
        /// 把Xml文件反序列化为对象
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        public static T Load<T>(string file) where T : class, new()
        {
            T t = new T();
            FileStream fs = null;
            try
            {
                // open the stream...
                fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = GetSerializer(t.GetType());// new XmlSerializer(type);
                t = serializer.Deserialize(fs) as T;
            }
#if DEBUG
            catch (Exception ex)
            {
             throw ex;
            }
#else
            catch { }
#endif
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return t;
        }

        /// <summary>
        /// 把对象序列化成Xml文件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="file">文件路径</param>
        public static bool Save(object obj, string file)
        {
            bool success = false;

            FileStream fs = null;
            // serialize it...
            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);
            try
            {
                fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = GetSerializer(obj.GetType());// new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj, xsn);
                success = true;
            }
#if DEBUG
            catch (Exception ex)
            {
             throw ex;
            }
#else
            catch { success = false; }
#endif
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return success;
        }
    }
}
