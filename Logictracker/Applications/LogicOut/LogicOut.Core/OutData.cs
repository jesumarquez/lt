using System.Globalization;
using System;

namespace LogicOut.Core.Export
{
    public partial class OutData
    {
        public string this[string key]
        {
            get
            {
                for (var i = 0; i < Keys.Count; i++)
                    if (Keys[i] == key) return Values[i];

                return null;
            }
        }
        public string AsString(string key, int length)
        {
            var text = this[key];
            if (text == null) return null;
            return text.Length > length ? text.Substring(0, length) : text;
        }
        public double? AsDouble(string key)
        {
            var text = this[key];
            if (text == null) return null;
            text = text.Replace(',', '.');
            double d;
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public float? AsSingle(string key)
        {
            var text = this[key];
            if (text == null) return null;
            text = text.Replace(',', '.');
            float d;
            if (float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public int? AsInt32(string key)
        {
            var text = this[key];
            if (text == null) return null;
            int d;
            if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public byte? AsByte(string key)
        {
            var text = this[key];
            if (text == null) return null;
            byte d;
            if (byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
            return null;
        }
        public DateTime? AsDateTime(string key)
        {
            var text = this[key];
            if (text == null) return null;
            DateTime d;
            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) return d;
            return null;
        }
    }
}
