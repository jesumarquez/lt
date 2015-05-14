using System.Collections.Generic;

namespace Urbetrack.Mobile.Toolkit
{
    public static class Format
    {
        public static int IdentSize { get; set; }
        public static char IdentCharacter { get; set; }

        static Format()
        {
            IdentSize = 8;
            IdentCharacter = ' ';
        }

        public static string Ident(string text, int level)
        {
            return Ident(text, level, 0);
        }

        public static string Ident(string text, int level, int extra)
        {
            var result = new string(IdentCharacter, IdentSize * level + extra);
            return result + text;
        }

        public static string Join(IEnumerable<string> data, string separator)
        {
            var result = "";
            foreach(var line in data)
            {
                result += line;
                result += separator;
            }
            return result;
        }

        /*
        public static bool Contains(this string src, string data)
        {
            if (String.IsNullOrEmpty(data)) return false;
            if (String.IsNullOrEmpty(data)) return false;
            while(!src.StartsWith(data))
            {
                if (src.Length > 0)
                {
                    src.Remove(0, 1);
                    continue;
                }
                return false;
            }
            return true;
        }
         */
    }
}