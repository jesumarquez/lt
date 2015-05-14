using System;
using System.Globalization;
using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import
{
    internal static class StringExtensions
    {
        public static string AsString(this IData data, int key, int length)
        {
            var text = data[key];
            if (text == null) return null;
            return text.Length > length ? text.Substring(0, length) : text;
        }
        public static bool? AsBool(this IData data, int key)
        {
            var text = data[key];
            if (text == null) return false;
            bool b;
            if (bool.TryParse(text, out b)) return b;
            return null;
        }
        public static double? AsDouble(this IData data, int key)
        {
            var text = data[key];
            if (text == null) return null;
            text = text.Replace(',', '.');
            double d;
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public static float? AsSingle(this IData data, int key)
        {
            var text = data[key];
            if (text == null) return null;
            text = text.Replace(',', '.');
            float d;
            if (float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public static int? AsInt32(this IData data, int key)
        {
            var text = data[key];
            if (text == null) return null;
            int d;
            if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public static short? AsInt16(this IData data, int key)
        {
            var text = data[key];
            if (text == null) return null;
            short d;
            if (short.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public static byte? AsByte(this IData data, int key)
        {
            var text = data[key];
            if (text == null) return null;
            byte d;
            if (byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public static DateTime? AsDateTime(this IData data, int key)
        {
            return AsDateTime(data, key, 0);
        }
        public static DateTime? AsDateTime(this IData data, int key, int gmt)
        {
            var text = data[key];
            if (text == null) return null;
            DateTime d;
            if (DateTime.TryParse(text, new CultureInfo("es-AR"), DateTimeStyles.None, out d)) return d.AddHours(-gmt);
            if (text.Length == 8)
            {
                int year, month, day;

                if (int.TryParse(text.Substring(0, 4), out year)
                 && int.TryParse(text.Substring(4, 2), out month)
                 && int.TryParse(text.Substring(6, 2), out day))
                {
                    d = new DateTime(year, month, day);
                    return d.AddHours(-gmt);
                }
            }
            return null;
        }

              

        internal static string Truncate(this string text, int length)
        {
            if (text == null) return string.Empty;
            return text.Length > length ? text.Substring(0, length) : text;
        }
        internal static bool AsBool(this string text)
        {
            if (text == null) return false;
            bool comb;
            bool.TryParse(text, out comb);
            return comb;
        }
        internal static DateTime? AsDateTime(this string text)
        {
            return AsDateTime(text, 0);
        }
        internal static DateTime? AsDateTime(this string text, int gmt)
        {
            if (text == null) return null;
            DateTime dt;
            if (DateTime.TryParse(text, new CultureInfo("es-AR"), DateTimeStyles.None, out dt))
                return dt.AddHours(-gmt);
            if (text.Length == 8)
            {
                int year, month, day;
                
                if (int.TryParse(text.Substring(0, 4), out year) 
                 && int.TryParse(text.Substring(4, 2), out month)
                 && int.TryParse(text.Substring(6, 2), out day))
                {
                    dt = new DateTime(year, month, day);
                    return dt.AddHours(-gmt);
                }
            }

            return null;
        }
        internal static int? AsInt(this string text)
        {
            if (text == null) return null;
            int orden;
            if(int.TryParse(text, out orden)) return orden;
            return null;
        }
        internal static double? AsDouble(this string text)
        {
            if (text == null) return null;
            double val;
            if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                return val;
            return null;
        }
    }
}
