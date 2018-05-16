//using System;
//using System.Text;

//namespace Hawk.Common
//{
//    /*
//     css：
//     <div class="page-wrap"><span class="btn">首页</span><span class="btn">上一页</span><span class="current">1</span><a href="/news/trade/2">2</a><a href="/news/trade/2" class="btn">下一页</a><a href="/news/trade/2" class="btn">尾页</a></div>

//     .page-wrap span, .page-wrap a { background-color: #dedede; display: inline-block; padding: 1px 8px; font-size: 12px; margin-left: 3px; color: #666666; }
//     .page-wrap span.current { background: #5289d4; border-color: #5289d4; color: #fff; font-weight: bold; }
//    .page-wrap a:hover { text-decoration: none; }
//     */
//    /// <summary>
//    /// 分页显示,url重写方式
//    /// </summary>
//    public class ShowPager
//    {
//        /// <summary>
//        /// 显示分页样式.如果使用url重写,则url参数只显示链接名称,
//        /// aspx参数为后缀名(如:.aspx,.html);否则url参数为网址
//        /// 全路径,aspx参数为string.Empty.默认使用url重写规则,
//        /// 网址后缀为aspx
//        /// </summary>
//        /// <param name="url">网络链接地址</param>
//        /// <param name="pageSize">每页数量</param>
//        /// <param name="pageIndex">索引页</param>
//        /// <param name="recordCount">总数量</param>
//        /// <param name="group">显示多少导航索引</param>
//        /// <param name="pageTotal">总页数</param>
//        /// <returns></returns>
//        public static string Show(string url, int pageSize, int pageIndex, int recordCount, int group, int pageTotal)
//        {
//            return Show(url, pageSize, pageIndex, recordCount, group, pageTotal, ".aspx");
//        }

//        /// <summary>
//        /// 显示分页样式.如果使用url重写,则url参数只显示链接名称,
//        /// aspx参数为后缀名(如:.aspx,.html);否则url参数为网址
//        /// 全路径,aspx参数为string.Empty.默认使用url重写规则,
//        /// 网址后缀为aspx
//        /// </summary>
//        /// <param name="url">网络链接地址</param>
//        /// <param name="pageSize">每页数量</param>
//        /// <param name="pageIndex">索引页</param>
//        /// <param name="recordCount">总数量</param>
//        /// <param name="group">显示多少导航索引</param> 
//        /// <param name="pageTotal">总页数</param>
//        /// <param name="aspx">网址后缀,例:.aspx,.html</param>
//        /// <returns></returns>
//        public static string Show(string url, int pageSize, int pageIndex, int recordCount, int group, int pageTotal, string aspx)
//        {
//            //int pageTotal = (total + pageSize - 1) / pageSize;//总页数// (total % pageSize != 0 ? (total / pageSize + 1) : (total / pageSize));//总页数
//            StringBuilder builder = new StringBuilder(100);
//            if (recordCount > pageSize && pageIndex <= pageTotal)
//            {
//                //builder.Append("<span class=\"none\">总：<span class=\"darkred\">").Append(recordCount).Append("</span></span>");
//                //builder.Append("第<span style=\"color:#ff0000;\">").Append(pageIndex).Append("</span> 页");
//                // 首页
//                if (pageIndex == 1)
//                {
//                    builder.Append(ShowFirst(url, aspx, pageTotal, group));

//                }
//                //尾页
//                else if (pageIndex >= pageTotal)
//                {
//                    builder.Append(ShowLast(url, aspx, pageTotal, group, pageIndex));
//                }
//                // 中间
//                else
//                {
//                    builder.Append("<a href=\"").Append(url).Append("1").Append(aspx).Append("\">首页</a>");
//                    builder.Append("<a href=\"").Append(url).Append((pageIndex - 1).ToString()).Append(aspx).Append("\">上一页</a>");

//                    builder.Append(ShowMiddle(url, aspx, pageTotal, group, pageIndex));

//                    builder.Append("<a href=\"").Append(url).Append((pageIndex + 1).ToString()).Append(aspx).Append("\">下一页</a>");
//                    builder.Append("<a href=\"").Append(url).Append(pageTotal).Append(aspx).Append("\">尾页</a>");
//                }
//                //builder.Append("<span class=\"none\">共<span class=\"darkred\">");
//                //builder.Append(pageTotal).Append("</span>页</span>");
//            }
//            return builder.ToString();
//        }

//        /// <summary>
//        /// 第一页显示
//        /// </summary>
//        /// <param name="url"></param>
//        /// <param name="aspx"></param>
//        /// <param name="pageTotal">总页数</param>
//        /// <param name="group">显示多少导航索引</param>
//        /// <returns></returns>
//        static string ShowFirst(string url, string aspx, int pageTotal, int group)
//        {
//            StringBuilder builder = new StringBuilder(100);

//            builder.Append("<span class=\"btn\">首页</span><span class=\"btn\">上一页</span><span class=\"current\">1</span>");

//            if (pageTotal > group)
//            {
//                //中间
//                for (int i = 2; i <= group; i++)
//                {
//                    builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                    //builder.Append("<a href=\"" + url + i.ToString() + aspx + "\">" + i.ToString() + "</a>");
//                    //builder.Append(string.Format("<a href=\"{0}{1}{2}\">{1}</a>", url, i, aspx));
//                }
//                //builder.Append("<a href=\"").Append(url).Append((group + 1).ToString()).Append(aspx).Append("\">>></a>");
//            }
//            else
//            {
//                for (int i = 2; i <= pageTotal; i++)
//                {
//                    builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                }
//            }
//            builder.Append("<a href=\"").Append(url).Append("2").Append(aspx).Append("\" class=\"btn\">下一页</a>");
//            builder.Append("<a href=\"").Append(url).Append(pageTotal).Append(aspx).Append("\" class=\"btn\">尾页</a>");

//            return builder.ToString();
//        }

//        /// <summary>
//        /// 最后一页显示
//        /// </summary>
//        /// <param name="url"></param>
//        /// <param name="aspx"></param>
//        /// <param name="pageTotal">总页数</param>
//        /// <param name="group"></param>
//        /// <param name="pageIndex"></param>
//        /// <returns></returns>
//        static string ShowLast(string url, string aspx, int pageTotal, int group, int pageIndex)
//        {
//            int recordPage = pageTotal / group;
//            StringBuilder builder = new StringBuilder(100);

//            builder.Append("<a href=\"").Append(url).Append("1").Append(aspx).Append("\">首页</a>");
//            builder.Append("<a href=\"").Append(url).Append((pageIndex - 1).ToString()).Append(aspx).Append("\">上一页</a>");

//            if (pageTotal > group)
//            {
//                //尾页索引不能和自定义导航数量除尽
//                if (pageIndex % group != 0)
//                {
//                    //builder.Append("<a href=\"").Append(url).Append((recordPage * group).ToString()).Append(aspx).Append("\"><<</a>");

//                    for (int i = recordPage * group + 1; i < pageTotal; i++)
//                    {
//                        builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                    }
//                }
//                else
//                {
//                    //builder.Append("<a href=\"").Append(url).Append(((recordPage - 1) * group).ToString()).Append(aspx).Append("\"><<</a>");

//                    for (int i = (recordPage - 1) * group + 1; i < pageTotal; i++)
//                    {
//                        builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                    }
//                }
//            }
//            else
//            {
//                for (int i = 1; i < pageTotal; i++)
//                {
//                    builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                }
//            }
//            builder.Append("<span class=\"current\">").Append(pageTotal).Append("</span>");
//            builder.Append("<span class=\"btn\">下一页</span><span class=\"btn\">尾页</span>");
//            return builder.ToString();
//        }

//        /// <summary>
//        /// 显示中间
//        /// </summary>
//        /// <param name="url"></param>
//        /// <param name="aspx"></param>
//        /// <param name="pageTotal"></param>
//        /// <param name="group"></param>
//        /// <param name="pageIndex"></param>
//        static string ShowMiddle(string url, string aspx, int pageTotal, int group, int pageIndex)
//        {
//            StringBuilder builder = new StringBuilder(100);
//            #region 中间的索引,页数大于自定义的导航
//            if (pageTotal > group)
//            {
//                //分多少次显示导航索引
//                int recordPage = pageTotal / group;
//                //当前索引所在的导航索引
//                int currentPage = pageIndex / group;

//                if (currentPage == 0 || pageIndex == group)
//                {
//                    for (int i = 1; i <= group; i++)
//                    {
//                        if (i == pageIndex)
//                        {
//                            builder.Append("<span class=\"current\">").Append(i).Append("</span>");
//                        }
//                        else
//                        {
//                            builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                        }
//                    }
//                    // builder.Append("<a href=\"").Append(url).Append((group + 1).ToString()).Append(aspx).Append("\">>></a>");//后面
//                }
//                //尾页索引不能和自定义导航数量除尽
//                else if (currentPage == recordPage)
//                {
//                    if (pageIndex % group != 0)
//                    {
//                        //builder.Append("<a href=\"").Append(url).Append((currentPage * group)).Append(aspx).Append("\"><<</a>");
//                        for (int i = currentPage * group + 1; i <= pageTotal; i++)
//                        {
//                            if (i == pageIndex)
//                            {
//                                builder.Append("<span class=\"current\">").Append(i).Append("</span>");
//                            }
//                            else
//                            {
//                                builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                            }
//                        }
//                    }
//                    else
//                    {
//                        //builder.Append("<a href=\"").Append(url).Append(((currentPage - 1) * group)).Append(aspx).Append("\"><<</a>");//前面
//                        for (int i = (currentPage - 1) * group + 1; i < pageIndex; i++)
//                        {
//                            builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                        }
//                        builder.Append("<span class=\"current\">").Append(pageIndex).Append("</span>");
//                        //builder.Append("<a href=\"").Append(url).Append((pageIndex + 1)).Append(aspx).Append("\">>></a>");//后面
//                    }
//                }
//                else
//                {
//                    if (pageIndex % group != 0)
//                    {
//                        //builder.Append("<a href=\"").Append(url).Append((currentPage * group)).Append(aspx).Append("\"><<</a>");//前面
//                        for (int i = currentPage * group + 1; i <= ((currentPage + 1) * group); i++)
//                        {
//                            if (i == pageIndex)
//                            {
//                                builder.Append("<span class=\"current\">").Append(i).Append("</span>");
//                            }
//                            else
//                            {
//                                builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                            }
//                        }
//                        //builder.Append("<a href=\"").Append(url).Append(((currentPage + 1) * group + 1)).Append(aspx).Append("\">>></a>");//后面
//                    }
//                    else
//                    {
//                        //builder.Append("<a href=\"").Append(url).Append(((currentPage - 1) * group)).Append(aspx).Append("\"><<</a>");//前面

//                        for (int i = (currentPage - 1) * group + 1; i <= currentPage * group; i++)
//                        {
//                            if (i == pageIndex)
//                            {
//                                builder.Append("<span class=\"current\">").Append(i).Append("</span>");
//                            }
//                            else
//                            {
//                                builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                            }
//                        }
//                        //builder.Append("<a href=\"").Append(url).Append((currentPage * group + 1)).Append(aspx).Append("\">>></a>");//后面
//                    }
//                }
//            }
//            #endregion
//            #region 中间的索引,页数小于自定义的导航
//            else
//            {
//                for (int i = 1; i <= pageTotal; i++)
//                {
//                    if (i == pageIndex)
//                    {
//                        builder.Append("<span class=\"current\">").Append(i).Append("</span>");
//                    }
//                    else
//                    {
//                        builder.Append("<a href=\"").Append(url).Append(i).Append(aspx).Append("\">").Append(i).Append("</a>");
//                    }
//                }
//            }
//            #endregion
//            return builder.ToString();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="pageSize">每页数量</param>
//        /// <param name="pageIndex">索引页</param>
//        /// <param name="recordCount">总数量</param>
//        /// <param name="group">显示多少导航索引</param> 
//        /// <param name="pageTotal">总页数</param>
//        /// <returns></returns>
//        public static string Show(int pageSize, int pageIndex, int recordCount, int group, int pageTotal)
//        {
//            StringBuilder builder = new StringBuilder();
//            if (recordCount > pageSize && pageIndex <= pageTotal)
//            {
//                // 首页
//                if (pageIndex == 1)
//                {
//                    builder.Append(ShowFirst(pageTotal, group));

//                }
//                //尾页
//                else if (pageIndex >= pageTotal)
//                {
//                    builder.Append(ShowLast(pageTotal, group, pageIndex));
//                }
//                // 中间
//                else
//                {
//                    builder.Append(ShowMiddle(pageTotal, group, pageIndex));
//                }
//            }
//            return builder.ToString();
//        }

//        /// <summary>
//        /// 第一页显示
//        /// </summary>
//        /// <param name="pageTotal">总页数</param>
//        /// <param name="group">显示多少导航索引</param>
//        /// <returns></returns>
//        static string ShowFirst(int pageTotal, int group)
//        {
//            StringBuilder builder = new StringBuilder();
//            if (pageTotal > group)
//            {
//                //中间
//                for (int i = 1; i <= group; i++)
//                {
//                    builder.Append(i);
//                    if (i != group)
//                        builder.Append(",");
//                }
//            }
//            else
//            {
//                for (int i = 1; i <= pageTotal; i++)
//                {
//                    builder.Append(i);
//                    if (i != pageTotal)
//                        builder.Append(",");
//                }
//            }
//            return builder.ToString();
//        }

//        /// <summary>
//        /// 最后一页显示
//        /// </summary>
//        /// <param name="pageTotal">总页数</param>
//        /// <param name="group"></param>
//        /// <param name="pageIndex"></param>
//        /// <returns></returns>
//        static string ShowLast(int pageTotal, int group, int pageIndex)
//        {
//            int recordPage = pageTotal / group;
//            StringBuilder builder = new StringBuilder();
//            if (pageTotal > group)
//            {
//                //尾页索引不能和自定义导航数量除尽
//                if (pageIndex % group != 0)
//                {
//                    for (int i = recordPage * group + 1; i <= pageTotal; i++)
//                    {
//                        builder.Append(i);
//                        if (i != pageTotal)
//                            builder.Append(",");
//                    }
//                }
//                else
//                {
//                    for (int i = (recordPage - 1) * group + 1; i <= pageTotal; i++)
//                    {
//                        builder.Append(i);
//                        if (i != pageTotal)
//                            builder.Append(",");
//                    }
//                }
//            }
//            else
//            {
//                for (int i = 1; i <= pageTotal; i++)
//                {
//                    builder.Append(i);
//                    if (i != pageTotal)
//                        builder.Append(",");
//                }
//            }
//            return builder.ToString();
//        }

//        /// <summary>
//        /// 显示中间
//        /// </summary>
//        /// <param name="pageTotal"></param>
//        /// <param name="group"></param>
//        /// <param name="pageIndex"></param>
//        static string ShowMiddle(int pageTotal, int group, int pageIndex)
//        {
//            StringBuilder builder = new StringBuilder();
//            #region 中间的索引,页数大于自定义的导航
//            if (pageTotal > group)
//            {
//                //分多少次显示导航索引
//                int recordPage = pageTotal / group;
//                //当前索引所在的导航索引
//                int currentPage = pageIndex / group;

//                if (currentPage == 0 || pageIndex == group)
//                {
//                    for (int i = 1; i <= group; i++)
//                    {
//                        builder.Append(i);
//                        if (i != group)
//                            builder.Append(",");
//                    }
//                }
//                //尾页索引不能和自定义导航数量除尽
//                else if (currentPage == recordPage)
//                {
//                    if (pageIndex % group != 0)
//                    {
//                        for (int i = currentPage * group + 1; i <= pageTotal; i++)
//                        {
//                            builder.Append(i);
//                            if (i != pageTotal)
//                                builder.Append(",");
//                        }
//                    }
//                    else
//                    {
//                        for (int i = (currentPage - 1) * group + 1; i <= pageIndex; i++)
//                        {
//                            builder.Append(i);
//                            if (i != pageIndex)
//                                builder.Append(",");
//                        }
//                    }
//                }
//                else
//                {
//                    if (pageIndex % group != 0)
//                    {
//                        for (int i = currentPage * group + 1; i <= ((currentPage + 1) * group); i++)
//                        {
//                            builder.Append(i);
//                            if (i != (currentPage + 1) * group)
//                                builder.Append(",");
//                        }
//                    }
//                    else
//                    {
//                        for (int i = (currentPage - 1) * group + 1; i <= currentPage * group; i++)
//                        {
//                            builder.Append(i);
//                            if (i != currentPage * group)
//                                builder.Append(",");
//                        }
//                    }
//                }
//            }
//            #endregion
//            #region 中间的索引,页数小于自定义的导航
//            else
//            {
//                for (int i = 1; i <= pageTotal; i++)
//                {
//                    builder.Append(i);
//                    if (i != pageTotal)
//                        builder.Append(",");
//                }
//            }
//            #endregion
//            return builder.ToString();
//        }
//    }
//}