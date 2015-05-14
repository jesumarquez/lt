#region Usings

using System;

#endregion

namespace Logictracker.Utils
{
    [Serializable]
    public class Altitude
    {
        private ushort packed;

        public Altitude(short data)
        {
            packed = (ushort) data;
        }

		public Altitude(float data)
		{
			Pack(data);
		}

		//debe ser public para la serializacion
		public Altitude()
		{
			Pack(0);
		}

		public void Pack(float data)
        {
            if (data == 0)
            {
                packed = 0;
                return;
            }
            var ipart = (ushort) ((int)data & 0xFFF);
            var dpart = data - ipart;
            packed = (ushort) (0xF & (int)((dpart /(1.0/8.0)) + 1));
            packed |= (ushort)(ipart << 4);
        }

        public float Unpack()
        {
            var ipart = (ushort)(0xFFF & (packed >> 4));
            var dpart = packed & 0xF;
            return (float)((ipart * 1.0) + (dpart * (1.0/8.0)));
        }

        public override string ToString()
        {
            return Unpack().ToString();
        }
    }
}