using System;
using System.Text;

namespace Hawk.Common
{
    public class Rand
    {
        //static RandomNumberGenerator _rnd;

        //private Rand() { }
#if NETSTANDARD2_0
        static Rand()
        {
            //_rnd = new RNGCryptoServiceProvider();        
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
#endif

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
        /// 简单的数字分组
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int[] Split(int s, int num)
        {
            int[] rad = new int[num];
            int sum = s - num;//先给每个数组中的数值留个1;

            for (int i = 0; i < num; i++)
            {
                if (i == num)
                {
                    rad[i] = sum + 1;
                }
                else
                {
                    int n = RandomNum(1, sum);
                    sum -= n;
                    rad[i] = n + 1;
                }
            }
            return rad;
        }

        public static int[] Split(int s, int num, int max, int min = 1)
        {
            int[] rad = new int[num];
            int sum = s - num;//先给每个数组中的数值留个1;
            //if (num * min > sum) min = 1;
            //if ((num - 1) * min + max > sum) max = sum - (num - 1) * min;
            if (max >= sum) max = sum + 1;

            for (int i = 0; i < num; i++)
            {
                int n = min;
                if (max < sum)
                    n = RandomNum(min, max);
                else
                    n = RandomNum(min, sum);
                sum -= n;
                rad[i] = n;
            }
            return rad;
        }

        /// <summary>
        /// 随机把某个数分成一个数组
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num">数组个数</param>
        /// <param name="min">数组最小数(最后一位不一定小于它)</param>
        /// <returns></returns>
        public static int[] SplitInt(int s, int num, int min)
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
                        //byte[] buffer = Guid.NewGuid().ToByteArray();
                        //int seed = BitConverter.ToInt32(buffer, 0);
                        //Random random = new Random(seed);
                        //rad[k] =  random.Next(min, max + 1);
                        rad[k] = RandomNum(min, max + 1);
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
                int t = random.Next( CHAR_ARRAY.Length);

                code.Append(CHAR_ARRAY[t]);
            }
            return code.ToString();

            //char[] c = RandomChar(length, strArray);

            //return CharToString(c);
        }        

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
    }
}
