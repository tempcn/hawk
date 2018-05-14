using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hawk.Common
{
    public class Comparer
    {
        public static void Sort(string[] s, bool direction = false)
        {
            if (direction)
                Array.Sort<string>(s, new StringComparer(direction));
            else
                Array.Sort<string>(s);
        }

        public static void Sort(int[] s, bool direction = false)
        {
            if (direction)
                Array.Sort<int>(s, new IntComparer(direction));
            else
                Array.Sort<int>(s);
        }

        public static void Sort(char[] s, bool direction = false)
        {
            if (direction)
                Array.Sort<char>(s, new CharComparer(direction));
            else
                Array.Sort<char>(s);
        }

        public static void Sort<T>(string[] key, T[] items, bool direction = false)
        {
            if (direction)
                Array.Sort<string, T>(key, items, new StringComparer(direction));
            else
                Array.Sort<string, T>(key, items);
        }

        public static void Sort<T>(int[] key, T[] items, bool direction = false)
        {
            if (direction)
                Array.Sort<int, T>(key, items, new IntComparer(direction));
            else
                Array.Sort<int, T>(key, items);
        }

        public static void Sort<T>(char[] key, T[] items, bool direction = false)
        {
            if (direction)
                Array.Sort<char, T>(key, items, new CharComparer(direction));
            else
                Array.Sort<char, T>(key, items);
        }
    }

    internal class CharComparer : IComparer<char>
    {
        private bool _direction = false;
        public int Compare(char x, char y)
        {
            if (!_direction)
                return x.CompareTo(y);
            return y.CompareTo(x);
        }
        /// <summary>
        /// 默认升序
        /// </summary>
        /// <param name="direction"></param>
        public CharComparer(bool direction = false)
        {
            _direction = direction;
        }
    }

    internal class StringComparer : IComparer<string>
    {
        private bool _direction = false;
        public int Compare(string x, string y)
        {
            if (!_direction)
                return x.CompareTo(y);
            return y.CompareTo(x);
        }
        /// <summary>
        /// 默认升序
        /// </summary>
        /// <param name="direction"></param>
        public StringComparer(bool direction = false)
        {
            _direction = direction;
        }
    }

    internal class IntComparer : IComparer<int>
    {
        private bool _direction = false;
        public int Compare(int x, int y)
        {
            if (!_direction)
                return x.CompareTo(y);
            return y.CompareTo(x);
        }
        /// <summary>
        /// 默认升序
        /// </summary>
        /// <param name="direction"></param>
        public IntComparer(bool direction = false)
        {
            _direction = direction;
        }
    }
}
