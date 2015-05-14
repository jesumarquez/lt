#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace XBeeTester
{
    class Stats
    {
        // total historico de paquetes de respuesta loopback.
        internal static int _loopedback_frames;
        // total historico de paquetes enviados y recibida la respuesta.
        internal static int _ping_frames_sent;
        // total historico de errores de envio
        internal static int _send_errors;
        // total historico de paquetes enviados y recibida la respuesta.
        internal static int _pong_frames_received;
        // total historico de paquetes recibidos
        internal static int _frames_received;
        // total historico de paquetes recibidos
        internal static int _frames_received_errors;
        // total historico de paquetes enviados
        internal static int _frames_sent;
        // total actual de paquetes en la cola de salida
        internal static int _tx_queue_size;
        // total de segundos de la prueba actual
        internal static int _0_test_in_seconds;
        // total de bytes de la prueba actual
        internal static int _0_test_in_bytes_sent;
        // total de bytes de la prueba actual
        internal static int _0_test_in_bytes_recv;
        // total de bytes de la prueba actual
        internal static int _0_efective_incomming_bps;
        internal static int _0_1_minute_avg_incomming_bps;







        public static void Clear()
        {
            var instance = new Stats();
            Type type = instance.GetType();
            foreach (var m in type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField |
                                                        BindingFlags.Instance))
            {
                if (m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                {
                    object[] zero = { 0 };
                    type.InvokeMember(m.Name,
                                                       BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField |
                                                        BindingFlags.Instance, null, instance, zero);
                }
            }
        }

        public static Dictionary<string, string> Update()
        {
            var outdata = new Dictionary<string, string>();
            var instance = new Stats();
            Type type = instance.GetType();
            foreach (var m in type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField |
                                                        BindingFlags.Instance))
            {
                if (m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                {
                    var valor = (int)type.InvokeMember(m.Name,
                                                       BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField |
                                                        BindingFlags.Instance, null, instance, null);
                    outdata.Add(m.Name.Substring(1).Replace('_', ' '), valor.ToString());
                }
            }
            return outdata;
        }
    }
}
