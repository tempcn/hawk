using System;
using System.Collections.Generic;
using System.Text;
using Hawk;
using Hawk.Common;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine(dt.ToDateString());

            //string s = "亵仙,鏖仙,芸仙";
            string s1 = "䨲颥";

            Console.WriteLine(s1);
            Console.WriteLine(Spell.GetInitial(s1));
            Console.WriteLine(Spell.Get(s1));
            Console.WriteLine(Spell.Get(s1,"#"));

            Console.WriteLine(Spell.SpellMusicCode.Length);
            Console.WriteLine(Spell.SpellCodeIndex.Length);
            Console.WriteLine(Spell.Chinese.Length);
            Console.WriteLine(Spell.SpellCodeIndex.GetLength(0));
            Console.WriteLine(Spell.SpellCodeIndex.GetLength(1));

            string py = "jiong";
            Console.WriteLine(Spell.GetChineseTxt(py));

            //Console.WriteLine(Spell.GetFirst(s1));
            //Console.WriteLine(Spell.GetFirstOne(s1));
            //Console.WriteLine(Spell.GetInitials(s1));
            //Console.WriteLine(Spell.MakeSpellCode(s1, SpellOptions.FirstLetterOnly));

            //Console.WriteLine("======================");

            //Console.WriteLine(Spell.Get(s1));
            //Console.WriteLine(Spell.GetPinyin(s1));
            //Console.WriteLine(Spell.MakeSpellCode(s1, SpellOptions.EnableUnicodeLetter));

            //Console.WriteLine("======================");



  

            //Console.WriteLine(Hawk.Common.PyCode.codes.Length);
            //Console.WriteLine(PyHash.hashes[0].Length);

            Xdf xdf = new Xdf();

            Console.WriteLine(xdf.Inti);
            Console.WriteLine(xdf.Strings);
            Console.WriteLine(xdf.DateTimed);
            Console.WriteLine(xdf.DateTimeOffsetd);
            Console.WriteLine(xdf.TimeSpant);
            Console.WriteLine(xdf.Floats);
            Console.WriteLine(xdf.Doubled);

            xdf.DateTimed = DateTime.MinValue;
            xdf.DateTimeOffsetd = DateTimeOffset.MinValue;
            xdf.TimeSpant = TimeSpan.MinValue;         
            Console.WriteLine(xdf.DateTimed);
            Console.WriteLine(xdf.DateTimeOffsetd);
            Console.WriteLine(xdf.TimeSpant);

            xdf.DateTimed = DateTime.MaxValue;
            xdf.DateTimeOffsetd = DateTimeOffset.MaxValue;
            xdf.TimeSpant = TimeSpan.MaxValue;
            Console.WriteLine(xdf.DateTimed);
            Console.WriteLine(xdf.DateTimeOffsetd);
            Console.WriteLine(xdf.TimeSpant);

            xdf.TimeSpant = TimeSpan.Zero;
            Console.WriteLine(xdf.TimeSpant);
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
