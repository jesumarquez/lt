#region Usings

using System;
using System.Drawing;
using System.Globalization;

#endregion

namespace Logictracker.Types.BusinessObjects.Components
{
    [Serializable]
    public class RGBColor
    {
        public virtual short Red { get; set; }
        public virtual short Green { get; set; }
        public virtual short Blue { get; set; }

        public Color Color 
        { 
            get { return Color.FromArgb(Red, Green, Blue); }
            set 
            { 
                Red = value.R;
                Green = value.G;
                Blue = value.B;
            }
        }

        public virtual string HexValue
        {
            get
            {
                var RR = String.Format("{0:x}", Red);
                var GG = String.Format("{0:x}", Green);
                var BB = String.Format("{0:x}", Blue);
                var result = RR.Length == 2 ? RR : "0" + RR;

                result += GG.Length == 2 ? GG : "0" + GG;
                result += BB.Length == 2 ? BB : "0" + BB;

                return result;
            }
            set
            {
                if (value == null)
                {
                    Red = Green = Blue = 0;
                    return;
                }
                var RRGGBB = value;

                if (RRGGBB.StartsWith("#")) RRGGBB = RRGGBB.Substring(1, RRGGBB.Length - 1);

                if (value.Length != 6) return;

                Red = byte.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber);
                Green = byte.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber);
                Blue = byte.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber);
            }
        }
    }
}