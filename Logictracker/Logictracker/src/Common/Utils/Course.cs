#region Usings

using System;

#endregion

namespace Logictracker.Utils
{
    [Serializable]
    public class Course
    {
        private ushort packed;

        public Course(short data)
        {
            packed = (ushort) data;
        }

		public Course(float data)
		{
			Pack(data);
		}

		//debe ser public para la serializacion
		public Course(double course)
		{
			Pack((float) course);
		}
        public Course()
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
            return Unpack().ToString();
        }
    }
}