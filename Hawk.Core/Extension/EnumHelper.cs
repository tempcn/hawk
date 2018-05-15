using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Hawk
{
    /// <summary>枚举类型助手类</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class EnumHelper
    {
        /// <summary>
        /// 获取枚举字段的注释
        /// </summary>
        /// <returns></returns>
        public static string Description(this Enum value)
        {
            if (value == null) return null;
            var type = value.GetType();

#if !NET40
            var item = type.GetField(value.ToString(), BindingFlags.Public | BindingFlags.Static);
            if (item == null) return null;
            var att = item.GetCustomAttribute<DescriptionAttribute>(false);
            if (att != null && !string.IsNullOrEmpty(att.Description))
                return att.Description;
#else
            var fields = type.GetFields();
            if (fields == null || fields.Length == 0) return null;

            for (int i = 0; i < fields.Length; i++)
            {
                var attr = (DescriptionAttribute[])fields[i]
                     .GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attr != null && attr.Length > 0)
                    return attr[0].Description;
            }
#endif
            return null;
        }

        public static string ToStringEx(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }
    }
}
