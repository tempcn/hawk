using System;
using System.Text.RegularExpressions;

namespace Hawk.Common
{
    public static class Regular
    {
        /// <summary>
        /// 用户名格式,以字母开头.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="minL"></param>
        /// <param name="maxL"></param>
        /// <returns></returns>
        public static bool IsName(this string s, int minL = 5, int maxL = 17)
        {
            if (!string.IsNullOrEmpty(s))
            {
                //\w字母数字包括下划线
                string pattern = string.Concat("^[a-zA-Z][0-9a-zA-Z]{", minL, ",", maxL, "}$");
                //string pattern = string.Concat("^([a-z]|[A-Z])+[0-9a-zA-Z_]{5,17}$";//6-18位
                return Regex.IsMatch(s, pattern, RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 密码格式是否正确
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPassword(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                //\x27='
                //!"#$%&()*+,-./0-9:;<=>?@A-Z[\}^_`a-z{|}~
                string pattern = "^[\x21-\x26\x28-\x7E]{6,18}$";// "^[a-zA-Z0-9~!@#$%^&*()_+-=,.\"]{8,18}$";
                return Regex.IsMatch(s, pattern, RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmail(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Regex.IsMatch(s, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 是否正确的邮政编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPost(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Regex.IsMatch(s, @"^\d{6}$", RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        ///  检测是否符合移动电话格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMobile(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Regex.IsMatch(s, @"^0?(13|14|15|17|18)\d{9}$", RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 是否全是字母数字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(this string s)
        {
            if (!string.IsNullOrEmpty(s))
                return Regex.IsMatch(s, "^\\w+$");
            return false;
        }

        /// <summary>
        /// 检测是否全部为中文
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsSimplifiedChinese(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Regex.IsMatch(s, @"^[\u4e00-\u9fa5]+$", RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 自然数(大于等于0的整数)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNatural(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Regex.IsMatch(s, @"^(0|([1-9]\d*)){1}$", RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 符合正小数格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsDec(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                //^(0|([1-9]\d*))(\.\d{1,2})?$  小数点后2位
                return Regex.IsMatch(s, @"^(0|([1-9]\d*))(\.\d+)?$", RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 整数
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInteger(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string pattern = @"^(-)?(0|([1-9]\d*))$";
                return Regex.IsMatch(s, pattern, RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 是否正确的ipv4地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIpv4(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Regex.IsMatch(s, @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$", RegexOptions.None);
            }
            return false;
        }

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
        /// 判断字符串是否符合十六进数制格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsHex(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string pattern = "^[a-fA-F0-9]+$";
                //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.None);
                //return regex.IsMatch(str, 0);
                return Regex.IsMatch(s, pattern, RegexOptions.None);
            }
            return false;
        }

        /// <summary>
        /// 符合固定电话格式(区号与号码可以用-分开)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsTel(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return Regex.IsMatch(s, Field.TEL_PATTERN);
            }
            return false;
        }

        /// <summary>
        /// 检测相对于Sql语句是否安全
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsSafeSqlStr(this string s)
        {
            if (!string.IsNullOrEmpty(s))
                //return !Regex.IsMatch(s, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
                return !Regex.IsMatch(s, @"[-|%|@|\*|!|\']");
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
                if (Regex.IsMatch(s, Field.ID_CARD_PATTERN))
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

        /// <summary>
        /// 格式:97851AB3-53C1-4C3F-98E6-384B53372C2C
        /// </summary>
        /// <param name="s"></param>
        /// <param name="flag">是否带分割符</param>
        /// <returns></returns>
        public static bool IsGuid(this string s, bool flag = false)
        {
            if (!string.IsNullOrEmpty(s))
            {
                //\w字母数字包括下划线
                //^[\w]{8}[\-]{1}([\w]{4}[\-]{1}){3}[\w]{12}$ 
                string pattern = "^[a-fA-F0-9]{32}$";
                if (flag)
                    pattern = @"^[a-f0-9A-F]{8}[\-]{1}([a-f0-9A-F]{4}[\-]{1}){3}[a-f0-9A-F]{12}$";
                return Regex.IsMatch(s, pattern);
            }
            return false;
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

