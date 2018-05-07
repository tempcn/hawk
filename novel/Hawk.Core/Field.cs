using System;

namespace Hawk
{
    public static class Field
    {
        public const string ID_CARD_PATTERN = @"(^\d{15}$)|(^\d{17}([0-9]|[X|x])$)";

        public const string TEL_PATTERN = @"^0(\d{2}[-]?\d{8}|\d{3}[-]?(\d{7}|\d{8}))$";

        public const string DATE_FULL_STRING = "yyyyMMddHHmmssfff";

        public const string DATE_STRING = "yyyy-MM-dd HH:mm:ss";

        public const string DATE_SHORT_STRING = "yyyy-MM-dd";
    }
}
