using System;
using System.Collections.Generic;
using System.Text;
using Hawk;
using Hawk.Common;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApp1
{  
   class Program
    {
        static void WriteFile(string s,string fName="1")
        {
            string path = @"z:\char" + fName + ".txt";
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                sw.WriteLine(s);

        }

        static int ToHex(char c,Encoding encode)
        {
            var buffer = encode.GetBytes(c.ToString());
            int code = buffer[0];
            if(buffer.Length>1)
            {
                string tmp = "";
                for (int j = 0; j < buffer.Length; j++)                
                    tmp += buffer[j].ToString("X2");
                code = int.Parse(tmp, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return code;
        }

        static void Main8(string[] args)
        {
            int a = 0x4e, b = 0x6;

            var sm = Encoding.BigEndianUnicode.GetString(new byte[] { (byte)a, (byte)b },0,2);

            Console.WriteLine(sm);

            var py = Spell.Get('\u4e06');
            Console.WriteLine(py);
            //char cc = '𠛋';

            //Console.WriteLine(Spell.SpellCode[368]);
            //Console.WriteLine(Spell.SpellCode[70]);

            var sl = "丅";//"𠛋";
            var bb = Encoding.BigEndianUnicode.GetBytes(sl);
            for (int i = 0; i < bb.Length; i++)
            {
                Console.WriteLine(bb[i].ToString("X2"));
            }

            string fuhao = "āáǎàōóǒòêéěèīíǐìūúǔùǖǘǚǜü";

            for (int i = 0; i < fuhao.Length; i++)
            {
                Console.WriteLine("原字符:{0},unicode编码:{1}", fuhao[i], ToHex(fuhao[i], Encoding.Unicode).ToString("X2"));
            }       
        }

        static void Main2(string[] args)
        {
            //var rs = new StringBuilder();
            //for (int i = 0; i < Spell.Chinese.Length; i++)
            //{
            //    rs.Append("/*").Append((i + 1).ToString().PadLeft(4).PadRight(6)).Append("*/\"");
            //    rs.Append(Spell.Chinese[i]);
            //    rs.Append("\"");
            //    if (i != Spell.Chinese.Length - 1)
            //        rs.Append(",");
            //    rs.AppendLine();
            //}

            //WriteFile(rs.ToString(), "ccc");

            var getC = GetChinese();
            var getSC = new StringBuilder();
            int j = 0;
            foreach (var item in getC)
            {
                j++;
                if (getSC.Length > 0)
                    getSC.AppendLine(",");
                getSC.Append("/*").Append((j).ToString().PadLeft(4).PadRight(6)).Append("*/\"");
                getSC.Append(item.Key.PadRight(6));
                getSC.Append(":").Append(item.Value.JoinEx());
                getSC.Append("\"");
            }

            WriteFile(getSC.ToString(), "ccc2");

            int k = 0; int white = Spell.Chinese.Length * 7;
            for (int i = 0; i < Spell.Chinese.Length; i++)
            {
                k += Spell.Chinese[i].Length;
            }
            Console.WriteLine("汉字总数:" + k);

            IEnumerable<string> strNum = null;// new string[] { "nihao", "xdk", "hkk", "22" };

            strNum = new string[] { "kkkxxx" };

            strNum = strNum.AddEx("xdf5");
            strNum = strNum.AddEx("nihao2");
            Console.WriteLine(strNum.JoinEx("#"));

            IEnumerable<EnModl> skk = null;
            skk = skk.AddEx(new EnModl() { Id = 100, Name = "xdf" });
            skk = skk.AddEx(new EnModl() { Id = 1000, Name = "king" });
            var ls = skk.JoinEx(",", x => x.Id + "#" + x.Name);
            var ls2 = skk.JoinEx();
            Console.WriteLine(ls);
            Console.WriteLine(ls2);
            Console.WriteLine(skk.Count());
        }

        static IDictionary<string, IList<string>> GetExistsPinYin()
        {
            IDictionary<string, IList<string>> dict = new Dictionary<string, IList<string>>();

            for (int i = 0; i < Spell.SpellCode.Length; i++)
            {
                dict[Spell.SpellCode[i]] = new List<string>();
            }
            return dict;
        }

        static IDictionary<string, IList<string>> GetChinese()
        {
            IDictionary<string, IList<string>> dict = GetExistsPinYin();
            for (int i = 0x4e; i <= 0x9f; i++)
            {
                for (int j = 0; j <= 0xff; j++)
                {
                    var hz = Encoding.BigEndianUnicode.GetString(new byte[] { (byte)i, (byte)j }, 0, 2);
                    var spell = Spell.Get(hz);
                    if (spell != hz)//查找到拼音
                    {
                        if(dict.ContainsKey(spell))
                        {
                            IList<string> item2 = dict[spell];
                            item2.Add(hz);
                            dict[spell] = item2;
                        }
                        else
                        {
                            IList<string> item = new List<string>();
                            item.Add(hz);
                            dict[spell] = item;
                        }
                    }
                }
            }
            return dict;
           // return dict.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
        }

        static void Main77(string[] args)
        {
            IDictionary<string, int> pinyin = new Dictionary<string, int>();
            for (int i = 0; i < 415; i++)
            {
                string key = Spell.SpellCode[i];
                pinyin[key] = i + 1;
            }
            int k = 0, m = 0;
            var scode = new StringBuilder();
            for (int i = 0x4e; i <= 0x9f; i++)
            {
                scode.Append("/*").Append(i.ToString("X2"));
                scode.Append("*/{");
                for (int j = 0; j <= 0xff; j++)
                {
                    m++;
                    if (i == 0x9f && j > 0xa5)
                        scode.Append("0");
                    else
                    {
                        var hz = Encoding.BigEndianUnicode.GetString(new byte[] { (byte)i, (byte)j }, 0, 2);
                        var spell = Spell.Get(hz);
                        if (spell != hz)//查找到拼音
                        {
                            if (pinyin.ContainsKey(spell))
                            {
                                k++;
                                scode.Append(pinyin[spell]);
                            }
                            else
                                scode.Append("0");
                        }
                        else
                        {
                            scode.Append("0");
                        }
                    }
                    if (j != 0xff)
                        scode.Append(",");
                }
                scode.Append("}");
                if (i != 0x9f)
                    scode.Append(",");
                scode.AppendLine();
            }

            Console.WriteLine("总拼音索引数是:{0}", m);//20992
            Console.WriteLine("可转换为拼音的有:{0}", k);//20738
            WriteFile(scode.ToString(), "_new");
        }

        static void Main888(string[] args)
        {
            //Encoding encode = Encoding.UTF8;
            //encode = Encoding.GetEncoding("gb2312");

            DateTime dt = DateTime.Now;
            Console.WriteLine(dt.ToStringEx());

            Console.WriteLine(Spell.Get('给'));

            var sb = new StringBuilder(); int k = 0;
            var sb2 = new StringBuilder();
            for (char i = '\u4e00'; i <= '\u9fa5'; i++)
            {
                //var gb2312 = ToHex(i, encode).ToString("X2");
                var spell = Spell.Get(i); // PinYin.GetPinyin(i);  //Spell.Get(i);
                if (spell != i.ToString())
                {
                    k++;
                    sb.Append(i.ToString()).Append(",").Append(spell);
                    sb.AppendLine();
                }
                else
                    sb2.Append(i.ToString());

                // sb.Append(i).Append(",").Append(spell).Append(",").Append(ToHex(i, encode).ToString("X2"));
                //sb.Append(i).Append(",").Append(spell);
                //sb.AppendLine();
            }
            Console.WriteLine("总字符数是:{0}", ('\u9fa5' - '\u4e00' + 1).ToString());//20902
            Console.WriteLine("可转换为拼音的有:{0}", k);//20745
                                                 // Console.WriteLine("没拼音字符是:" + sb2.ToString());//157

            WriteFile(sb.ToString() + sb2.ToString(), "new_uncode52");


            ////string s = "亵仙,鏖仙,芸仙";
            //string s1 = "玥啊ㄗㄘ朱镕基䨲颥>? &*№☆★";
            //s1 = "其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示其实把代码复制出来之后，感觉也不是那么难懂，都是一些基本的指示";

            // Console.WriteLine(PinYin.Get(s1));
            // Console.WriteLine(PinYin.GetPinyin(s1));         


            //Console.WriteLine(Hawk.Common.PyCode.codes.Length);
            //Console.WriteLine(PyHash.hashes[0].Length);

            //Xdf xdf = new Xdf();

            //Console.WriteLine(xdf.Inti);
            //Console.WriteLine(xdf.Strings);
            //Console.WriteLine(xdf.DateTimed);
            //Console.WriteLine(xdf.DateTimeOffsetd);
            //Console.WriteLine(xdf.TimeSpant);
            //Console.WriteLine(xdf.Floats);
            //Console.WriteLine(xdf.Doubled);

            //xdf.DateTimed = DateTime.MinValue;
            //xdf.DateTimeOffsetd = DateTimeOffset.MinValue;
            //xdf.TimeSpant = TimeSpan.MinValue;         
            //Console.WriteLine(xdf.DateTimed);
            //Console.WriteLine(xdf.DateTimeOffsetd);
            //Console.WriteLine(xdf.TimeSpant);

            //xdf.DateTimed = DateTime.MaxValue;
            //xdf.DateTimeOffsetd = DateTimeOffset.MaxValue;
            //xdf.TimeSpant = TimeSpan.MaxValue;
            //Console.WriteLine(xdf.DateTimed);
            //Console.WriteLine(xdf.DateTimeOffsetd);
            //Console.WriteLine(xdf.TimeSpant);

            //xdf.TimeSpant = TimeSpan.Zero;
            //Console.WriteLine(xdf.TimeSpant);
        }
    }

    public class EnModl
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Xdf
    {
        public int Inti { get; set; }
        public float Floats { get; set; }
        public double  Doubled { get; set; }
        public string Strings { get; set; }
        public DateTime DateTimed { get; set; }
        public DateTimeOffset DateTimeOffsetd { get; set; }
        public TimeSpan TimeSpant { get; set; }
    }
}
