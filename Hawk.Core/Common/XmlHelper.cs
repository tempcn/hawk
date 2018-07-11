using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Hawk.Common
{
    public static class XmlHelper
    {
        /// <summary>
        /// Xml字符串序列化为实体对象
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToXmlEntity(this string xml, Type type)
        {
            object result = null;
            if (!string.IsNullOrEmpty(xml) && type.IsPublic)
            {
                var serial = new XmlSerializer(type);

                StringReader reader = null;
                XmlReader xr = null;

                try
                {
                    reader = new StringReader(xml);
                    //XmlReaderSettings settings = new XmlReaderSettings();
                    //settings.IgnoreComments = true;
                    //xr = XmlReader.Create(reader, settings);
                    xr = XmlReader.Create(reader);
                    result = serial.Deserialize(xr);
                }
                catch { }
                //catch (Exception ex)
                //{
                //     Console.WriteLine(ex.Message);
                //}
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (xr != null)
                        xr.Close();
                }
            }
            return result;
            //object result = null;
            //if(!string.IsNullOrEmpty(xml)&& !type.IsPublic)
            //{
            //    var serial = new XmlSerializer(type);
            //    using (var reader = new StringReader(xml))
            //    using (var xr = new XmlTextReader(reader))
            //    {
            //        result = serial.Deserialize(xr);
            //    }
            //}
            //return result;
        }

        /// <summary>
        /// Xml字符串序列化为实体对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static TEntity ToXmlEntity<TEntity>(this string xml) where TEntity : class
        {
            return ToXmlEntity(xml, typeof(TEntity)) as TEntity;
        }

        /// <summary>数据流转为Xml实体对象</summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="stream">数据流</param>
        /// <param name="encode">编码</param>
        /// <returns>Xml实体对象</returns>
        public static TEntity ToXmlEntity<TEntity>(this Stream stream, Encoding encode = null) where TEntity : class
        {
            return ToXmlEntity(stream, typeof(TEntity), encode) as TEntity;
        }

        /// <summary>数据流转为Xml实体对象</summary>
        /// <param name="stream">数据流</param>
        /// <param name="type">实体类型</param>
        /// <param name="encode">编码</param>
        /// <returns>Xml实体对象</returns>
        public static object ToXmlEntity(this Stream stream, Type type, Encoding encode = null)
        {
            object result = null;
            if (stream != null && type.IsPublic)
            {
                var serial = new XmlSerializer(type);

                StreamReader reader = null;
                XmlReader xr = null;
                //if (encode == null) encode = Encoding.UTF8;
                try
                {
                    reader = new StreamReader(stream, encode ?? Encoding.UTF8);
                    xr = XmlReader.Create(reader);
                    result = serial.Deserialize(xr);
                }
                catch { }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (xr != null)
                        xr.Close();
                }
            }
            return result;
        }

        /// <summary>Xml文件转为Xml实体对象</summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="file">Xml文件</param>
        /// <param name="encode">编码</param>
        /// <returns>Xml实体对象</returns>
        public static TEntity ToXmlFileEntity<TEntity>(this string file, Encoding encode = null) where TEntity : class
        {
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    return ToXmlEntity<TEntity>(stream, encode);
                }
            }
            return null;
        }

        /// <summary>
        /// 一个泛型集合对象转换为Xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public static string ToXml<T>(this IDictionary<string, T> dict, string rootName = null)
        {
            if (string.IsNullOrEmpty(rootName)) rootName = "xml";
            var doc = new XmlDocument();
            var root = doc.CreateElement(rootName);
            doc.AppendChild(root);
            if (dict != null && dict.Count > 0)
            {
                foreach (var item in dict)
                {
                    var element = doc.CreateElement(item.Key);
                    element.InnerText = item.Value.ToString();
                    root.AppendChild(element);
                }
            }
            return doc.OuterXml;
        }

        /// <summary>
        /// 一个泛型集合对象转换为Xml字符串(有加CDATA标识)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public static string ToXmlCDATA<T>(this IDictionary<string, T> dict, string rootName = null)
        {
            if (string.IsNullOrEmpty(rootName)) rootName =  "xml";
            var sbXml = new StringBuilder();
            sbXml.Append("<").Append(rootName).Append(">");
            if (dict != null && dict.Count > 0)
            {
                foreach (var item in dict)
                {
                    sbXml.Append("<").Append(item.Key).Append(">");
                    if (typeof(T) == typeof(int) || typeof(T) == typeof(double) || typeof(T) == typeof(float))
                        sbXml.Append(item.Value.ToString());
                    else
                        sbXml.Append("<![CDATA[").Append(item.Value.ToString()).Append("]]>");
                    sbXml.Append("</").Append(item.Key).Append(">");
                }
            }
            sbXml.Append("</").Append(rootName).Append(">");
            return sbXml.ToString();
        }

        /// <summary>
        /// 一个Xml字符串转换为泛型集合
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IDictionary<string, string> FromXmlDict(this string xml)
        {
            if (string.IsNullOrEmpty(xml)) return null;

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var root = xmlDoc.DocumentElement;
            var dict = new Dictionary<string, string>();
            if (root.ChildNodes != null && root.ChildNodes.Count > 0)
            {
                foreach (XmlNode item in root.ChildNodes)
                {
                    if (item.ChildNodes != null && (item.ChildNodes.Count > 1 ||
                        item.ChildNodes.Count == 1 && !(item.FirstChild is XmlText) && !(item.FirstChild is XmlCDataSection)))
                    {
                        dict[item.Name] = item.InnerXml;
                    }
                    else
                    {
                        dict[item.Name] = item.InnerText;
                    }
                }
            }
            return dict;
        }
    }
}
