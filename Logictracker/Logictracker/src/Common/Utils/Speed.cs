#region Usings

using System;
using System.Globalization;

#endregion

namespace Logictracker.Utils
{
    [Serializable]
	public class Speed
    {
        private ushort packed;

        public Speed(short data)
        {
            packed = (ushort)data;
        }

        public Speed(float data)
        {
            Pack(data);
        }

		//debe ser public para la serializacion
		public Speed(double speed)
        {
            Pack((float) speed);
        }
        public Speed()
        {
            Pack(0);
        }

	    private void Pack(float data)
        {
			if (data == 0)
			{
				packed = 0;
				return;
			}

			var ipart = (ushort)((int)data & 0x1FF);
            var dpart = data - ipart;
            packed = (ushort)(0x7F & (int)((dpart / (1.0 / 127.0)) + 1));
            packed |= (ushort)(ipart << 7);
        }

        public float Unpack()
        {
            var ipart = (ushort)(0xFFF & (packed >> 7));
            var dpart = packed & 0x7F;
            return (float)((ipart * 1.0) + (dpart * (1.0 / 127.0)));
        }

        public override string ToString()
        {
            return Unpack().ToString(CultureInfo.InvariantCulture);
        }

		public static float KnotToKm(float speed)
		{
			return speed * (float)1.852;
		}

		public static float KmToKnot(float speed)
		{
			return speed / (float)1.852;
		}

		public static int CmPerSecondToKm(int speed)
		{
			return (speed * 36) / 1000;
		}
	}
}