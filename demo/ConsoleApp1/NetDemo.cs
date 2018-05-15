using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hawk;
using Hawk.Common;
using System.Web;

namespace ConsoleApp1
{
    public class NetDemo
    {
        static void Main2()
        {
            var url = "http://localhost:3639/Net/Default5.aspx";

            var data = "nihao=你好&a=hello&b=189343&love=爱你";

            var header = new List<KeyValuePair<string, string>>();
            header.Add(new KeyValuePair<string, string>("a_h", HttpUtility.UrlEncode("能说什么呢")));
            header.Add(new KeyValuePair<string, string>("ba_h", "nothing"));

            var s = NetHelper.SendRequest(url + "?get=没什么好说的", data, true, header: header);

            Console.WriteLine(s);

            Console.WriteLine("=================================\r\npost方法");
            var content = new List<KeyValuePair<string, string>>();
            content.Add(new KeyValuePair<string, string>("phs", "能说什么呢"));
            content.Add(new KeyValuePair<string, string>("pah", "nothing"));

            s = content.JoinEx("&", x => x.Key + "=" + x.Value);

            Console.WriteLine(s);
            s = NetHelper.Post(url + "?post=我是post", content, header);
            Console.WriteLine(s);
        }
    }
}
