using System;
using System.Collections.Generic;
using System.Text;

namespace avl2cmd
{
    public class CMDPDU
    {
        public static String encode_estado(int vehiculo, int estado, int tiempo)
        {
            return String.Format("S0{0}{1}0{2}\r", vehiculo.ToString().PadLeft(4, '0'), estado.ToString().PadLeft(2, '0'), tiempo.ToString().PadLeft(3, '0'));
        }

        public static String encode_ack()
        {
            char a = (char)0x06;
            return String.Format("{0}0\r",a);
        }

        public static String encode_nack()
        {
            char a = (char)0x15;
            return String.Format("{0}0\r", a);
        }
    }
}
