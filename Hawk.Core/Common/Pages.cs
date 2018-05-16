namespace Hawk.Common
{
    public class Pages
    {
        private int pageSize;
        /// <summary>
        /// 页面数量
        /// </summary>
        public int PageSize
        {
            //get { return pageSize; }
            //set { pageSize = value; }

            get
            {
                return pageSize < 1 ? 10 : pageSize;
            }
            set { pageSize = value; }
        }
        ///// <summary>
        ///// 页面数量
        ///// </summary>
        //public int PageSize { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageTotal { get { return (Count + PageSize - 1) / PageSize; } }
        private int index;
        /// <summary>
        /// 当前页
        /// </summary>
        public int Index { get { return index < 1 ? 1 : index; } set { index = value; } }
        /// <summary>
        /// 上一页
        /// </summary>
        public int Prev { get { if (Index > 1) return Index - 1; else return 0; } }
        /// <summary>
        /// 下一页
        /// </summary>
        public int Next { get { if (Index < PageTotal) return Index + 1; else return PageTotal; } }
        private int group;
        /// <summary>
        /// 显示多少导航索引
        /// </summary>
        public int Group
        {
            get { if (group < 2) return 10; return group; }
            set { group = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int[] List
        {
            get
            {
                if (Count > PageSize && Index <= PageTotal)
                {
                    if (Index == 1)
                    {
                        return ShowFirst();
                    }
                    else if (Index >= PageTotal)
                        return ShowLast();
                    else
                        return ShowMiddle();
                }
                return new int[0];
            }
        }

        /// <summary>
        /// 第一页显示
        /// </summary>
        /// <returns></returns>
        int[] ShowFirst()
        {
            int len;
            if (PageTotal > Group)
            {
                len = Group;
            }
            else
            {
                len = PageTotal;
            }
            int[] s = new int[len];
            for (int i = 0; i < len; i++)
            {
                s[i] = i + 1;
            }
            return s;
        }

        /// <summary>
        /// 最后一页显示
        /// </summary>
        /// <returns></returns>
        int[] ShowLast()
        {
            int recordPage = PageTotal / Group,
                begin, end;
            //int[] s;
            if (PageTotal > Group)
            {
                //尾页索引不能和自定义导航数量除尽
                if (Index % Group != 0)
                {
                    begin = recordPage * Group + 1;
                    end = PageTotal;
                }
                else
                {
                    begin = (recordPage - 1) * Group + 1;
                    end = PageTotal;
                }
            }
            else
            {
                begin = 1;
                end = PageTotal;
            }
            int[] s = new int[end + 1 - begin];
            for (int i = begin; i <= end; i++)
            {
                s[i - begin] = i;
            }

            return s;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int[] ShowMiddle()
        {
            int begin, end;

            #region 中间的索引,页数大于自定义的导航
            if (PageTotal > Group)
            {
                //分多少次显示导航索引
                int recordPage = PageTotal / Group;
                //当前索引所在的导航索引
                int currentPage = Index / Group;

                if (currentPage == 0 || Index == Group)
                {
                    begin = 1; end = Group;
                }
                //尾页索引不能和自定义导航数量除尽
                else if (currentPage == recordPage)
                {
                    if (Index % Group != 0)
                    {
                        begin = currentPage * Group + 1; end = PageTotal;
                    }
                    else
                    {
                        begin = (currentPage - 1) * Group + 1; end = Index;
                    }
                }
                else
                {
                    if (Index % Group != 0)
                    {
                        begin = currentPage * Group + 1; end = (currentPage + 1) * Group;
                    }
                    else
                    {
                        begin = (currentPage - 1) * Group + 1; end = currentPage * Group;
                    }
                }
            }
            #endregion
            #region 中间的索引,页数小于自定义的导航
            else
            {
                begin = 1; end = PageTotal;
            }
            #endregion

            int[] s = new int[end + 1 - begin];
            for (int i = begin; i <= end; i++)
            {
                s[i - begin] = i;
            }
            return s;
        }
    }
}
