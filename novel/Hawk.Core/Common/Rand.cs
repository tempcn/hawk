using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Hawk.Common
{
    public class Rand
    {
        //static RandomNumberGenerator _rnd;

        //private Rand() { }

        //static Rand()
        //{
        //    _rnd = new RNGCryptoServiceProvider();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min">返回的随机数的下界（随机数可取该下界值）</param>
        /// <param name="max">返回的随机数的上界（随机数不能取该上界值）</param>
        /// <returns></returns>
        public static int RandomNum(int min = int.MinValue, int max = int.MaxValue)
        {
            //var buf = new Byte[4];
            //_rnd.GetBytes(buf);
            //var n = BitConverter.ToInt32(buf, 0);

            var n = Guid.NewGuid().GetHashCode();

            if (min == int.MinValue && max == int.MaxValue) return n;
            if (min == 0 && max == int.MaxValue) return Math.Abs(n);
            if (min == int.MinValue && max == 0) return -Math.Abs(n);

            var num = max - min;
            //return (Int32)(num * Math.Abs(n) / ((Int64)UInt32.MaxValue + 1) + min);
            return (int)((num * (uint)n >> 32) + min);
        }

        /// <summary>
        /// 定义随机出现的字符串
        /// </summary>
        /// <param name="length">字符的长度</param>
        /// <param name="letter">是否包含字母</param>
        /// <param name="ignoreCase">是否包含大写字母</param>
        /// <returns></returns>
        public static string RandomCode(int length, bool letter = true, bool ignoreCase = false)
        {
            var s = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var ch = ' ';
                int n = 0;
                if (!letter)
                {
                    n = RandomNum(0, 10);
                    ch = (char)('0' + n);
                }
                else
                {
                    if (ignoreCase)
                    {
                        n = RandomNum(0, 62);//数字,大小字母
                        if (n < 10)
                            ch = (char)('0' + n);
                        else if (n < 36)
                            ch = (char)('A' + n - 10);
                        else
                            ch = (char)('a' + n - 36);
                    }
                    else
                    {
                        n = RandomNum(0, 36);
                        if (n < 10)
                            ch = (char)('0' + n);
                        else
                            ch = (char)('a' + n - 10);
                    }
                }
                s.Append(ch);
            }

            return s.ToString();
        }

        /// <summary>
        /// 随机把某个数分成一个数组
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num">数组个数</param>
        /// <param name="min">数组最小数(最后一位不一定小于它)</param>
        /// <returns></returns>
        public static int[] RandomInt(int s, int num, int min)
        {
            int[] rad = new int[num];

            int max = s;
            int t = 0;
            for (int k = 0; k < num; k++)
            {
                if (max > min)
                {
                    if (k == num - 1)
                        rad[k] = max;
                    else
                    {
                        byte[] buffer = Guid.NewGuid().ToByteArray();
                        int seed = BitConverter.ToInt32(buffer, 0);
                        Random random = new Random(seed);
                        rad[k] = random.Next(min, max + 1);
                        max = max - rad[k];
                    }
                }
                else
                {
                    if (t == 0)
                    {
                        t++;
                        rad[k] = max;
                    }
                    else
                        rad[k] = 0;
                }
            }
            return rad;
        }

        /// <summary>
        /// 定义随机出现的字符串
        /// </summary>
        /// <param name="length">字符的长度(小于或等于字符集合的长度)</param>
        /// <param name="num">字符集合</param>
        /// <returns></returns>
        public static int[] RandomInt(int length, int[] num)
        {
            int[] c = new int[length];
            Random random = new Random();
            int end = num.Length;
            int[] tempArray = null;// new string[end];

            tempArray = (int[])num.Clone();

            for (int i = 0; i < length; i++)
            {
                int n = random.Next(0, end);
                //sb.Append(tempArray[num]);
                c[i] = tempArray[n];
                tempArray[n] = tempArray[end - 1];
                end--;
            }
            return c;
        }

        static readonly char[] CHAR_ARRAY = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y' };

        /// <summary>
        /// 定义随机出现的字符串,忽略某些易混淆的字符
        /// </summary>
        /// <param name="length">字符的长度</param>
        /// <param name="removeObscure">忽略某些易混淆的字符</param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            var code = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int seed = Guid.NewGuid().GetHashCode();
                Random random = new Random(seed);
                int t = random.Next(CHAR_ARRAY.Length);

                code.Append(CHAR_ARRAY[t]);
            }
            return code.ToString();

            //char[] c = RandomChar(length, strArray);

            //return CharToString(c);
        }

        ///// <summary>
        ///// 随机获取不超过10位的数字组合
        ///// </summary>
        ///// <param name="length">最长10位</param>
        ///// <returns></returns>
        //public static string RandomNum(int length)
        //{
        //    return RandomChar(length, false, false);
        //}

        ///// <summary>
        ///// 定义随机出现的字符串
        ///// </summary>
        ///// <param name="length">字符的长度</param>
        ///// <param name="letter">是否包含字母</param>
        ///// <param name="ignoreCase">是否包含大写字母</param>
        ///// <returns></returns>
        //public static string RandomChar(int length, bool letter = true, bool ignoreCase = false)
        //{
        //    //string[] nums = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        //    //string[] lower = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        //    //string[] upper = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        //    //stringp[ strArray = new string[] { "0","1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        //    int len = 10;
        //    if (letter)
        //    {
        //        len = 36;
        //        if (ignoreCase)
        //            len = 62;
        //    }

        //    char[] strArray = new char[len];

        //    for (char i = '0'; i <= '9'; i++)  //ascii=> 0 48          
        //        strArray[i - 48] = i;

        //    if (letter)
        //    {
        //        for (char i = 'a'; i <= 'z'; i++)//ascii=> a 97 z 122               
        //            strArray[i - 87] = i;//97-10
        //        if (ignoreCase)
        //            for (char i = 'A'; i <= 'Z'; i++)//ascii=> A 65 Z 90
        //                strArray[i - 29] = i;//65-36
        //    }
        //    char[] c = RandomChar(length, strArray);

        //    return CharToString(c);

        //    #region  获取小写字母数组
        //    //string[] lowers = new string[26];
        //    //char zero = 'a';
        //    //for (char a = 'a'; a <= 'z'; i++)
        //    //{
        //    //    int index = ((int)a) - ((int)zero);
        //    //    lowers[index] = a.ToString();
        //    //}
        //    #endregion

        //    #region old code
        //    //StringBuilder builder = new StringBuilder(36);

        //    //for (char i = '0'; i <= '9'; i++)
        //    //{
        //    //    if (i == '9')
        //    //        builder.Append(i);
        //    //    else
        //    //        builder.Append(i).Append(",");
        //    //}
        //    //if (letter)
        //    //{
        //    //    builder.Append(",");
        //    //    for (char a = 'a'; a <= 'z'; a++)
        //    //    {
        //    //        if (a == 'z')
        //    //            builder.Append(a);
        //    //        else
        //    //            builder.Append(a).Append(",");
        //    //    }
        //    //    if (ignoreCase)
        //    //    {
        //    //        builder.Append(",");
        //    //        for (char A = 'A'; A <= 'Z'; A++)
        //    //        {
        //    //            if (A == 'Z')
        //    //                builder.Append(A);
        //    //            else
        //    //                builder.Append(A).Append(",");
        //    //        }
        //    //    }
        //    //}
        //    //string[] resultArray = builder.ToString().Split(',');
        //    #endregion

        //    //return RandomChar(length, strArray);
        //}

        /// <summary>
        /// 定义随机出现的字符串
        /// </summary>
        /// <param name="length">字符的长度(小于或等于字符集合的长度)</param>
        /// <param name="strArray">字符集合</param>
        /// <returns></returns>
        public static char[] RandomChar(int length, char[] strArray)
        {
            //string str = "A,S,D,F,G,H,J,K,L,Z,X,C,V,B,N,M,Q,W,E,R,T,Y,U,I,I,O,P";
            // string[] strArray = array.Split(split);//str.Split(',');

            #region 随机数	 
            //StringBuilder code = new StringBuilder(length);
            //for (int i = 0; i < length; i++)
            //{
            //    byte[] buffer = System.Guid.NewGuid().ToByteArray();
            //    int seed = BitConverter.ToInt32(buffer, 0);
            //    Random random = new Random(seed);//new Random((int)(DateTime.Now.Ticks / 1000L) + i);
            //    int t = random.Next(strArray.Length);

            //    code.Append(strArray[t]);
            //}
            //return code.ToString();
            #endregion

            #region   随机不重复数
            char[] c = new char[length];
            //StringBuilder sb = new StringBuilder(length);
            int send = Guid.NewGuid().GetHashCode();
            Random random = new Random(send);
            int end = strArray.Length;
            char[] tempArray = null;// new string[end];

            //for (int i = 0; i <=end; i++)           
            //    tempArray[i] = strArray[i];          

            //throw 对象必须是基元数组
            //Buffer.BlockCopy(strArray, 0, tempArray, 0, end);

            tempArray = (char[])strArray.Clone();
            //tempArray = strArray;
            for (int i = 0; i < length; i++)
            {
                int num = random.Next(0, end);
                //sb.Append(tempArray[num]);
                c[i] = tempArray[num];
                tempArray[num] = tempArray[end - 1];
                end--;
            }
            return c;// sb.ToString();
            #endregion
        }

        /// <summary>
        /// 定义随机出现的字符串
        /// </summary>
        /// <param name="length">字符的长度(小于或等于字符集合的长度)</param>
        /// <param name="strArray">字符集合</param>
        /// <returns></returns>
        public static string[] RandomArray(int length, string[] strArray)
        {
            string[] c = new string[length];
            int send = Guid.NewGuid().GetHashCode();
            Random random = new Random(send);
            int end = strArray.Length;
            string[] tempArray = null;// new string[end];

            tempArray = (string[])strArray.Clone();

            for (int i = 0; i < length; i++)
            {
                int num = random.Next(0, end);
                //sb.Append(tempArray[num]);
                c[i] = tempArray[num];
                tempArray[num] = tempArray[end - 1];
                end--;
            }
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sum"></param>
        /// <returns></returns>
        public static string GetSimplifiedChineseMath(out int sum)
        {
            string[] sc = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖", "拾", "佰", "仟", "万", "亿" };
            string[] op = { "+", "-", "x", "÷", "%" };

            Random random = new Random();

            int a = random.Next(0, 11);
            int b = random.Next(1, 11);

            string x = op[random.Next(0, 5)];

            switch (x)
            {
                case "+":
                    sum = a + b;
                    break;
                case "-":
                    sum = a - b;
                    break;
                case "x":
                    //a = random.Next(1, 10);
                    //b = random.Next(1, 10);
                    sum = a * b;
                    break;
                case "÷"://取商
                    //a = random.Next(10, 100);
                    //b = random.Next(1, 10);
                    sum = a / b;
                    break;
                case "%"://取余
                    //a = random.Next(1, 10);
                    //b = random.Next(1, 10);
                    sum = a % b;
                    break;
                default:
                    sum = 0;
                    break;
            }

            string str = sc[a] + x + sc[b] + "=";

            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetMathString(out int sum)
        {
            Random random = new Random();

            int a = random.Next(10, 100);
            int b = random.Next(10, 100);

            string[] op = { "+", "-", "x", "÷", "%" };

            string x = op[random.Next(0, 5)];

            switch (x)
            {
                case "+":
                    sum = a + b;
                    break;
                case "-":
                    sum = a - b;
                    break;
                case "x":
                    a = random.Next(1, 10);
                    b = random.Next(1, 10);
                    sum = a * b;
                    break;
                case "÷"://取商
                    a = random.Next(10, 100);
                    b = random.Next(1, 10);
                    sum = a / b;
                    break;
                case "%"://取余
                    a = random.Next(1, 10);
                    b = random.Next(1, 10);
                    sum = a % b;
                    break;
                default:
                    sum = 0;
                    break;
            }

            string str = a + x + b + "=";
            if (b < 0)
                str = a + x + "(" + b + ")=";
            return str;
        }

        /// <summary>
        /// 随机获取范围为0x4e00--0x9fa5
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetSimplifiedChinese(int length)
        {
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int val = random.Next(0x4e00, 0x9fa5);//random.Next((int)char.MinValue, (int)char.MaxValue);
                result.Append(((char)val).ToString());
            }
            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strLen"></param>
        /// <returns></returns>
        public static string GetGBKString(int strLen)
        {
            StringBuilder sb = new StringBuilder(strLen);

            string[] strArray = GetGBKArray(strLen);
            //for (int i = 0; i < strArray.Length; i++)
            //    sb.Append(strArray[i]);
            //return sb.ToString();

            return StringToString(strArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strLen"></param>
        /// <returns></returns>
        public static string[] GetGBKArray(int strLen)
        {
            string[] strArray = new string[strLen];
            Encoding encoding = Encoding.GetEncoding("gbk");

            byte[][] bytes = CreateRegionCode(strLen);
            for (int i = 0; i < bytes.Length; i++)
                strArray[i] = encoding.GetString(bytes[i]);

            return strArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strLen"></param>
        /// <returns></returns>
        public static byte[][] CreateRegionCode(int strLen)
        {
            //定义存储汉字编码的数组元素
            byte[] rBase = new byte[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf };

            Random random = new Random();

            byte[][] bytes = new byte[strLen][];

            //每循环一次产生一个含两个元素的十六进制字节数组，并将其放入object数组中 
            //每个汉字有四个区位码组成 
            //区位码第1位和区位码第2位作为字节数组第一个元素 
            //区位码第3位和区位码第4位作为字节数组第二个元素 

            for (int i = 0; i < strLen; i++)
            {
                //区位码第1位 
                int r1 = random.Next(11, 14);//范围在B,C,D之间如果为D,第2位不能是7以后的十六进制数
                byte bR1 = rBase[r1];

                //区位码第2位 
                random = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i);//更换随机数发生器的种子避免产生重复值 

                int r2;
                if (r1 == 13)//为D            
                    r2 = random.Next(0, 7);
                else
                    r2 = random.Next(0, 16);
                byte bR2 = rBase[r2];

                //区位码第3位 
                random = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
                int r3 = random.Next(10, 16);//random.Next(0, 16);
                byte bR3 = rBase[r3];

                //区位码第4位,如果第3位为A,不能是0;为F,不能是F 
                random = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
                int r4;
                switch (r3)
                {
                    //为A
                    case 10: r4 = random.Next(1, 16); break;
                    //为F
                    case 15: r4 = random.Next(0, 15); break;
                    default: r4 = random.Next(0, 16); break;
                }
                byte bR4 = rBase[r4];

                bytes[i] = new byte[] { (byte)(bR1 * 0x10 + bR2), (byte)(bR3 * 0x10 + bR4) };
            }
            return bytes;
        }

        public static string CharToString(char[] c)
        {
            StringBuilder sb = new StringBuilder(c.Length);
            for (int i = 0; i < c.Length; i++)
                sb.Append(c[i]);
            return sb.ToString();
        }

        public static string StringToString(string[] s)
        {
            StringBuilder sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
                sb.Append(s[i]);
            return sb.ToString();
        }

        public static string IntToString(int[] num)
        {
            StringBuilder sb = new StringBuilder(num.Length);
            for (int i = 0; i < num.Length; i++)
                sb.Append(num[i]);
            return sb.ToString();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="strLen"></param>
        ///// <returns></returns>
        //public static string[] GetGBKString(int strLen)
        //{
        //    Encoding encoding = Encoding.GetEncoding("gb2312");
        //    object[] obj = CreateRegionCode(strLen);
        //    StringBuilder builder = new StringBuilder();
        //    string[] strArray = new string[strLen];
        //    for (int i = 0; i < obj.Length; i++)
        //        strArray[i] = encoding.GetStringData((byte[])obj[i]);
        //    return strArray;
        //}
        //         ////获取GB2312编码页（表） 
        // //Encoding gb = Encoding.GetEncoding("gb2312");

        // ////调用函数产生4个随机中文汉字编码 
        // //object[] bytes = CreateRegionCode(4);

        // ////根据汉字编码的字节数组解码出中文汉字 
        // //string str1 = gb.GetStringData((byte[])Convert.ChangeType(bytes[0], typeof(byte[])));
        // //string str2 = gb.GetStringData((byte[])Convert.ChangeType(bytes[1], typeof(byte[])));
        // //string str3 = gb.GetStringData((byte[])Convert.ChangeType(bytes[2], typeof(byte[])));
        // //string str4 = gb.GetStringData((byte[])Convert.ChangeType(bytes[3], typeof(byte[])));
        // //string[] str = new string[] { str1, str2, str3, str4 };

        // //Session["checkCode"] = str1 + str2 + str3 + str4;       

        // Encoding encoding = Encoding.GetEncoding("gb2312");
        // object[] buf = CreateRegionCode(5);
        // StringBuilder sb = new StringBuilder();
        // string[] str = new string[5];
        // for (int i = 0; i < buf.Length; i++)
        // {
        //     str[i] = encoding.GetStringData((byte[])buf[i]);
        //     //sb.Append(encoding.GetStringData((byte[])buf[i]));
        // }
        //// Console.WriteLine(sb.ToString());
        // CreateImage(str);

        ///// <summary>
        ///// 此函数在汉字编码范围内随机创建含两个元素的十六进制字节数组,
        ///// 每个字节数组代表一个汉字，并将四个字节数组存储在object数组中。
        ///// </summary>
        ///// <param name="strLen">代表需要产生的汉字个数</param>
        ///// <returns></returns>
        //[Obsolete]
        //public static object[] CreateRegionCode(int strLen)
        //{
        //    //定义一个字符串数组储存汉字编码的组成元素 
        //    string[] rBase = new string[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

        //    Random random = new Random();

        //    //定义一个object数组用来 
        //    object[] bytes = new object[strLen];

        //    /*每循环一次产生一个含两个元素的十六进制字节数组，并将其放入object数组中 
        //     每个汉字有四个区位码组成 
        //     区位码第1位和区位码第2位作为字节数组第一个元素 
        //     区位码第3位和区位码第4位作为字节数组第二个元素 
        //    */
        //    for (int i = 0; i < strLen; i++)
        //    {
        //        //区位码第1位 
        //        int r1 = random.Next(11, 14);//范围在B,C,D之间如果为D,第2位不能是7以后的十六进制数
        //        string str_r1 = rBase[r1].Trim();

        //        //区位码第2位 
        //        random = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i);//更换随机数发生器的种子避免产生重复值 
        //        int r2;
        //        if (r1 == 13)//为D            
        //            r2 = random.Next(0, 7);
        //        else
        //            r2 = random.Next(0, 16);
        //        string str_r2 = rBase[r2].Trim();

        //        //区位码第3位 
        //        random = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
        //        int r3 = random.Next(10, 16);//random.Next(0, 16);
        //        string str_r3 = rBase[r3].Trim();

        //        //区位码第4位,如果第3位为A,不能是0;为F,不能是F 
        //        random = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
        //        int r4;
        //        switch (r3)
        //        {
        //            //为A
        //            case 10: r4 = random.Next(1, 16); break;
        //            //为F
        //            case 15: r4 = random.Next(0, 15); break;
        //            default: r4 = random.Next(0, 16); break;
        //        }
        //        string str_r4 = rBase[r4].Trim();

        //        //定义两个字节变量存储产生的随机汉字区位码 
        //        byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
        //        byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
        //        //将两个字节变量存储在字节数组中 
        //        byte[] buffer = new byte[] { byte1, byte2 };

        //        //将产生的一个汉字的字节数组放入byte数组中 
        //        bytes.SetValue(buffer, i);
        //    }
        //    return bytes;
        //}
    }
}
