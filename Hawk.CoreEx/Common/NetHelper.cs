using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Web;

namespace Hawk.Common
{
    public partial class NetHelper
    {
        public const string MEDIA_TYPE = "application/x-www-form-urlencoded";
        public const string MEDIA_JSON = "application/json;charset=utf-8";
        public const string MEDIA_MULTIPART = "multipart/form-data";
        public const string MEDIA_XML = "application/xml;charset=utf-8";

        static readonly HttpClient httpClient;

        static NetHelper()
        {
            ServicePointManager.DefaultConnectionLimit = 512;

            httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 1024 * 200;//200KB

            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; rv:46.0) Gecko/20100101 Firefox/46.0");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; )");
        }

        public static string Get(string url, IEnumerable<KeyValuePair<string, string>> head = null)
        {
            var req = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            if (head != null && head.Count() > 0)
                foreach (var item in head)
                    req.Headers.Add(item.Key, item.Value);

            return httpClient.SendAsync(req)?
                 .Result?
                 .Content?
                 .ReadAsStringAsync()?
                 .Result;
        }

        public static string Post(string url, IEnumerable<KeyValuePair<string, string>> content = null,
            IEnumerable<KeyValuePair<string, string>> head = null, Encoding encode = null, string mediaType = null)
        {
            var req = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };            
            if (content != null && content.Count() > 0)
            {
                string txt = content.JoinEx("&", x => x.Key + "=" + x.Value);

                //var httpContent = new MultipartFormDataContent("");         
                // req.Content = new FormUrlEncodedContent(content);//new StringContent(CreateLinkString(content));
                req.Content = new StringContent(txt, encode != null ? encode : Encoding.UTF8,
                    mediaType: mediaType != null ? mediaType : MEDIA_TYPE);
                // req.Content = new MultipartFormDataContent("");             
            }

            if (head != null && head.Count() > 0)
                foreach (var item in head)
                    req.Headers.Add(item.Key, item.Value);

            return httpClient.SendAsync(req)?
                 .Result?
                 .Content?
                 .ReadAsStringAsync()?
                 .Result;
        }
    }

    public partial class NetHelper
    {
        static string Read(StreamReader reader, int len = 512)
        {
            char[] read = new char[len];
            int count = reader.Read(read, 0, read.Length);
            var s = new StringBuilder();
            while (count > 0)
            {
                string str = new string(read, 0, count);
                s.Append(str);
                count = reader.Read(read, 0, 256);
            }
            return s.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="isPost"></param>
        /// <param name="encode"></param>
        /// <param name="mediaType"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string SendRequest(string url, string data = null, bool isPost = false, Encoding encode = null,
            string mediaType = MEDIA_TYPE, IEnumerable<KeyValuePair<string, string>> header = null)
        {
            string content = null;
            HttpWebRequest request = null;
            Stream inStream = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            encode = encode == null ? Encoding.UTF8 : encode;
            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                request.AllowAutoRedirect = true;
                request.Timeout = 30000;
                request.Method = isPost ? "POST" : "GET";
                request.KeepAlive = false;
                request.ContentType = mediaType;// "application/json";// "text/xml";// "application/x-www-form-urlencoded";
                request.ProtocolVersion = HttpVersion.Version10;
                if (header != null && header.Count() > 0)
                    foreach (var item in header)
                        request.Headers.Add(item.Key, item.Value);
                if (isPost && !string.IsNullOrEmpty(data))
                {
                    var b = encode.GetBytes(data);
                    request.ContentLength = b.Length;
                    using (var req = request.GetRequestStream())
                    {
                        req.Write(b, 0, b.Length);
                        req.Close();
                    }
                }
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    inStream = response.GetResponseStream();
                    reader = new StreamReader(inStream, encode);
                    content = Read(reader);
                }
            }

#if DEBUG 
            catch (WebException e)
            {
                content = e.Message;
            }
            catch (Exception e)
            {
                content = e.Message;
            }
#else 
            catch { }
#endif
            finally
            {
                if (inStream != null)
                    inStream.Close();
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();

                if (request != null)
                    request.Abort();
            }
            return content;
        }

        /// <summary>
        /// 多图上传
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string PostFile(string uri, string[] file, IEnumerable<KeyValuePair<string, string>> param)
        {
            //string uri = "http://localhost/api/test/much";
            //uri = "http://www.qiyuandai.cn/reg/check";

            Encoding encode = Encoding.UTF8;

            string boundary = string.Concat("----", DateTime.Now.Ticks.ToString("x"));
            //请求头
            string contentType = string.Concat("multipart/form-data; boundary=", boundary);
            string postData = string.Concat("--", boundary, "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n");
            string byteData = string.Concat("\r\n--", boundary, "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n");


            var head = new StringBuilder();
            if (param.Count() > 0)
            {
                foreach (var item in param)
                {
                    head.AppendFormat(postData, item.Key, item.Value);
                }
                //for (int i = 0; i < param.Count; i++)
                //{
                //    head.AppendFormat(postData, param[i].Key, param[i].Value);
                //}
                //head.Remove(head.Length - 2, 2);
            }
            //开始
            byte[] form_data = encode.GetBytes(head.ToString());
            //结尾
            byte[] foot_data = encode.GetBytes(string.Concat("\r\n--", boundary, "--\r\n"));


            HttpWebRequest request = System.Net.WebRequest.Create(uri) as System.Net.HttpWebRequest;
            request.Method = "POST";
            //request.Host = "114.125.25.68";
            //request.Referer = "http://www.baidu.com";
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0";
            request.ContentType = contentType;

            long leng = form_data.Length + foot_data.Length;
            //文件
            byte[][] file_data = new byte[file.Length][];

            FileStream[] fss = new FileStream[file.Length];

            for (int i = 0; i < file.Length; i++)
            {
                fss[i] = new System.IO.FileStream(file[i], System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //long len = fss[i].Length;

                string name = "upfile"; //请求参数名
                string fileName = HttpUtility.UrlEncode(Path.GetFileName(file[i]));
                string fileType = "image/jpeg";//            
                file_data[i] = encode.GetBytes(string.Format(byteData, name, fileName, fileType));

                leng += fss[i].Length + file_data[i].Length;
            }

            request.ContentLength = leng;
            Stream outStream = request.GetRequestStream();

            outStream.Write(form_data, 0, form_data.Length);

            for (int i = 0; i < file.Length; i++)
            {
                outStream.Write(file_data[i], 0, file_data[i].Length);
                //文件内容 
                byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fss[i].Length))];
                int bytesRead = 0;
                while ((bytesRead = fss[i].Read(buffer, 0, buffer.Length)) != 0)
                    outStream.Write(buffer, 0, bytesRead);
            }
            //结尾 
            outStream.Write(foot_data, 0, foot_data.Length);
            outStream.Close();

            System.Net.WebResponse response = null;
            System.IO.StreamReader reader = null;

            response = request.GetResponse() as System.Net.HttpWebResponse;
            reader = new System.IO.StreamReader(response.GetResponseStream(), encode);
            string s = reader.ReadToEnd().Trim();

            for (int k = 0; k < fss.Length; k++)
            {
                if (fss[k] != null)
                    fss[k].Close();
            }

            if (outStream != null)
                outStream.Close();
            if (reader != null)
                reader.Close();
            if (response != null)
                response.Close();

            return s;
        }
    }
}
