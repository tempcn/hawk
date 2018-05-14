using System;
using System.Text.RegularExpressions;

namespace Hawk.Common
{
    public static class Regular
    {
        static bool IsRegular(string s, string pattern)
         => string.IsNullOrEmpty(s) ? false : Regex.IsMatch(s, pattern, RegexOptions.None);

        /// <summary>
        /// 字符串是否符合Base64编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsBase64(this string s)
            => IsRegular(s, "^[a-zA-Z0-9+/]+[=]{0,2}$");

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmail(this string s)
             => IsRegular(s, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

        /// <summary>
        ///  检测是否符合移动电话格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMobile(this string s)
                => IsRegular(s, @"^0?(13|14|15|17|18)\d{9}$");

        /// <summary>
        /// 是否全是字母数字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(this string s)
             => IsRegular(s, "^\\w+$");

        /// <summary>
        /// 检测是否全部为中文,范围:[\u4e00-\u9fa5]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsSimplifiedChinese(this string s)
             => IsRegular(s, @"^[\u4e00-\u9fa5]+$");

        /// <summary>
        /// 自然数(大于等于0的整数)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNatural(this string s)
            => IsRegular(s, @"^(0|([1-9]\d*)){1}$");

        /// <summary>
        /// 符合正小数格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDec(this string s)
               => IsRegular(s, @"^(0|([1-9]\d*))(\.\d+)?$");
        //{
        //    if (!string.IsNullOrEmpty(s))
        //    {
        //        //^(0|([1-9]\d*))(\.\d{1,2})?$  小数点后2位
        //        return Regex.IsMatch(s, @"^(0|([1-9]\d*))(\.\d+)?$", RegexOptions.None);
        //    }
        //    return false;
        //}

        /// <summary>
        /// 整数
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInteger(this string s)
            => IsRegular(s, @"^(-)?(0|([1-9]\d*))$");

        /// <summary>
        /// 是否正确的ipv4地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIpv4(this string s)
              => IsRegular(s, @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$");

        /// <summary>
        /// 符合固定电话格式(区号与号码可以用-分开)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsTel(this string s)
            => IsRegular(s, @"^0(\d{2}[-]?\d{8}|\d{3}[-]?(\d{7}|\d{8}))$");

        /// <summary>
        /// 判断字符串是否符合十六进数制格式
        /// </summary>
        /// <returns></returns>
        public static bool IsHex(this string s)
            => IsRegular(s, "^[a-fA-F0-9]+$");

        /// <summary>
        /// 检测相对于Sql语句是否安全
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsSafeSqlStr(this string s, string pattern = @"[-|%|@|\*|!|\']")
            => string.IsNullOrEmpty(s) ? false : !Regex.IsMatch(s, pattern, RegexOptions.None);

        /// <summary>
        /// 格式:97851AB3-53C1-4C3F-98E6-384B53372C2C
        /// </summary>
        /// <param name="flag">是否带分割符</param>
        /// <returns></returns>
        public static bool IsGuid(this string s, bool flag = false)
            => IsRegular(s, flag ? @"^[a-f0-9A-F]{8}[\-]{1}([a-f0-9A-F]{4}[\-]{1}){3}[a-f0-9A-F]{12}$":
                "^[a-fA-F0-9]{32}$");
        //        //\w字母数字包括下划线
        //        //^[\w]{8}[\-]{1}([\w]{4}[\-]{1}){3}[\w]{12}$ 

        /// <summary>
        /// 纯字母数字组成,首字母
        /// </summary>
        /// <param name="minL">最少位取值</param>
        /// <param name="maxL">最多位(如果小于最少位,则不取值)</param>
        /// <returns></returns>
        public static bool IsName(this string s, int minL = 2, int maxL = 0)
        {
            var pattern = "^[a-zA-Z][0-9a-zA-Z]{";
            if (minL < 2)
                minL = 2;
            //pattern += (minL - 1).ToString();
            if (maxL <= minL)
                pattern = string.Concat(pattern, (minL - 1).ToString(), ",}$");
            else
            {
                pattern = string.Concat(pattern, (minL - 1).ToString(), ",", (maxL - 1).ToString(), "}$");
            }
            return IsRegular(s, pattern);
        }
          //  => IsRegular(s, string.Concat("^[a-zA-Z][0-9a-zA-Z]{", minL, ",", maxL, "}$"));
        //{
        //    if (!string.IsNullOrEmpty(s))
        //    {
        //        //\w字母数字包括下划线
        //        string pattern = string.Concat("^[a-zA-Z][0-9a-zA-Z]{", minL, ",", maxL, "}$");
        //        //string pattern = string.Concat("^([a-z]|[A-Z])+[0-9a-zA-Z_]{5,17}$";//6-18位
        //        return Regex.IsMatch(s, pattern, RegexOptions.None);
        //    }
        //    return false;
        //}        

        /// <summary>
        /// 未验证
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIpv6(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var pattern = @"^([\da-fA-F]{1,4}:){7}[\da-fA-F]{1,4}$";

                pattern = @"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$";

                return Regex.IsMatch(s, pattern, RegexOptions.Compiled);
            }
            return false;
        }

        /// <summary>
        /// 验证15或18位身份证号码
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsIdentityCard(this string s, out string num)
        {
            bool flag = false;
            num = "";
            if (!string.IsNullOrEmpty(s))
            {
                // string pattern = @"(^\d{15}$)|(^\d{17}([0-9]|[X|x])$)";//15或18位//@"^\d{17}([0-9]|[X|x])$"; 
                if (Regex.IsMatch(s, @"(^\d{ 15}$)| (^\d{ 17} ([0 - 9] |[X | x])$)"))
                {
                    s = s.ToUpper();
                    int[] arrInt = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                    string[] arrCh = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
                    int nTemp = 0, i;
                    DateTime date = DateTime.Now;
                    switch (s.Length)
                    {
                        case 15:
                            if (DateTime.TryParse("19" + s.Substring(6, 2) + "-" + s.Substring(8, 2) + "-" + s.Substring(10, 2), out date))
                            {
                                if (date < DateTime.Now)
                                {
                                    flag = true;
                                    num = s.Substring(0, 6) + "19" + s.Substring(6, s.Length - 6);
                                    for (i = 0; i < 17; i++)
                                        nTemp += int.Parse(num.Substring(i, 1)) * arrInt[i];
                                    num += arrCh[nTemp % 11];
                                }
                            }
                            break;
                        case 18:
                            if (DateTime.TryParse(s.Substring(6, 4) + "-" + s.Substring(10, 2) + "-" + s.Substring(12, 2), out date))
                                if (date < DateTime.Now)
                                {
                                    num = s;
                                    for (i = 0; i < 17; i++)
                                        nTemp += int.Parse(num.Substring(i, 1)) * arrInt[i];

                                    string valNum = arrCh[nTemp % 11];
                                    if (valNum == num.Substring(17, 1))
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        num = s.Substring(0, 17) + valNum;
                                    }
                                }
                            break;
                    }
                }
            }
            return flag;
        }

        public static string Capture(this string s, int len, string padding = "...")
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Length > len)
                {
                    if (!string.IsNullOrEmpty(padding))
                    {
                        if (len > padding.Length)
                            len = len - padding.Length;
                    }
                    return string.Concat(s.Substring(0, len), padding);
                }
                return s;
            }
            return string.Empty;
        }

        public static string ClearHtml(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] pattern = { "&#[^>]*;", "</?marquee[^>]*>", "</?object[^>]*>", "</?param[^>]*>", "</?embed[^>]*>", "</?table[^>]*>",
                          "</?tr[^>]*>","</?th[^>]*>","</?p[^>]*>","</?a[^>]*>","</?img[^>]*>","</?tbody[^>]*>","</?li[^>]*>", "</?span[^>]*>",
                          "</?div[^>]*>","</?td[^>]*>","(javascript|jscript|vbscript|vbs):","on(mouse|exit|error|click|key)",
                           "<\\?xml[^>]*>","<\\/?[a-z]+:[^>]*>","</?font[^>]*>","</?b[^>]*>","</?u[^>]*>","</?i[^>]*>","</?strong[^>]*>"
                           ," "                           ,@"<script[^>]*?>.*?</script>"   ,"</?script[^>]*>" // 有了上面的表达式不需要下面的表达式
                           ,@"<(.[^>]*)>",@"([\r\n])[\s]+",@"-->", @"&(quot|#34);",@"&(amp|#38);", @"&(lt|#60);",
                           @"&(gt|#62);",@"&(nbsp|#160);", @"&(iexcl|#161);",@"&(cent|#162);", @"&(pound|#163);",@"&(copy|#169);",@"&#(\d+);"
            };
                //保留script中的内容
                //string[] pattern = { "&#[^>]*;", "</?marquee[^>]*>", "</?object[^>]*>", "</?param[^>]*>", "</?embed[^>]*>", "</?table[^>]*>",
                //          "</?tr[^>]*>","</?th[^>]*>","</?p[^>]*>","</?a[^>]*>","</?img[^>]*>","</?tbody[^>]*>","</?li[^>]*>", "</?span[^>]*>",
                //          "</?div[^>]*>","</?td[^>]*>", "</?script[^>]*>","(javascript|jscript|vbscript|vbs):","on(mouse|exit|error|click|key)",
                //           "<\\?xml[^>]*>","<\\/?[a-z]+:[^>]*>","</?font[^>]*>","</?b[^>]*>","</?u[^>]*>","</?i[^>]*>","</?strong[^>]*>"

                //           ," ",@"<script[^>]*?>.*?</script>",@"<(.[^>]*)>",@"([\r\n])[\s]+",@"-->", @"&(quot|#34);",@"&(amp|#38);", @"&(lt|#60);",
                //           @"&(gt|#62);",@"&(nbsp|#160);", @"&(iexcl|#161);",@"&(cent|#162);", @"&(pound|#163);",@"&(copy|#169);",@"&#(\d+);"   };


                //    //保留p和br标签,其他不要
                //    string[] pattern = { "&#[^>]*;", "</?marquee[^>]*>", "</?object[^>]*>", "</?param[^>]*>", "</?embed[^>]*>", "</?table[^>]*>",
                //              "</?tr[^>]*>","</?th[^>]*>","</?a[^>]*>","</?img[^>]*>","</?tbody[^>]*>","</?li[^>]*>", "</?span[^>]*>",
                //              "</?div[^>]*>","</?td[^>]*>","(javascript|jscript|vbscript|vbs):","on(mouse|exit|error|click|key)",
                //               "<\\?xml[^>]*>","<\\/?[a-z]+:[^>]*>","</?font[^>]*>","</?u[^>]*>","</?i[^>]*>","</?strong[^>]*>"
                //               ," " ,@"<script[^>]*?>.*?</script>"   ,"</?script[^>]*>" 
                //               ,@"([\r\n])[\s]+",@"-->", @"&(quot|#34);",@"&(amp|#38);", @"&(lt|#60);",
                //               @"&(gt|#62);",@"&(nbsp|#160);", @"&(iexcl|#161);",@"&(cent|#162);", @"&(pound|#163);",@"&(copy|#169);",@"&#(\d+);"
                //};

                for (int i = 0; i < pattern.Length; i++)
                    s = Regex.Replace(s, pattern[i], "", RegexOptions.IgnoreCase);
            }
            return s;
        }
    }
}

