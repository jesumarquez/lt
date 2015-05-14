#region Usings

using System.Collections.Generic;
using System.Reflection;

#endregion

namespace XbeeCore
{
    public class Stack
    {
        // total historico de transacciones
        internal static int _created_transactions;
        // total activo de transacciones
        internal static int _active_transactions;
        // total historico de transacciones con ACK
        internal static int _completed_transactions;
        // total historico de transacciones sin ACK ni NACK
        internal static int _timedout_transactions;
        // total historico de retransmisioes (NACKs)
        internal static int _nacks;
        // total historico del error frecuente
        internal static int _write_unautorized_access_exceptions;
        // total historico de otro error frecuente 
        internal static int _write_io_exceptions;
        // total historico de errores de escritura en general
        internal static int _write_unknow_exceptions;
        // total historico del error frecuente
        internal static int _read_unautorized_access_exceptions;
        // total historico de otro error frecuente 
        internal static int _read_io_exceptions;
        // total historico de errores de lectura en general
        internal static int _read_unknow_exceptions;
        // total historico de aplicaciones de la guarda de no lectura.
        //internal static int _read_timedout_guard;
        // total activo en segundos desde que no recibe datos
        internal static int _last_read_seconds;
        // total historico de errores del COM en general
        internal static int _uart_unknow_exceptions;
        // total historico de checksums
        internal static int _checksum_errors;
        // total historico de checksums
        internal static int _decoder_unexpected_x7E;
        // total historico de bytes leidos por puerto serie
        internal static int _readed_bytes;
        // total historico de bytes escritos por puerto serie
        internal static int _writed_bytes;
        // total historico de bytes ignorados por puerto serie
        internal static int _padding_bytes;
        // total historico de bytes leidos por puerto serie
        internal static int _acks_too_late;
        // total historico de veces que fue necesario reinicializar la UART
        internal static int _uart_resets;
        // total historico de errores de operacion invalida.
        internal static int _invalid_operation_exception;
        // total historico de excepciones de timeout
        internal static int _timeout_exceptions;
        // transiente de aplicaciones de la guarda de no lectura.
        //internal static int _transient_read_timedout_guard;

        public static void Clear()
        {
            var instance = new Stack();
            var type = instance.GetType();
            foreach (var m in type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField |
                                                        BindingFlags.Instance))
            {
                if (m.MemberType != MemberTypes.Field && m.MemberType != MemberTypes.Property) continue;
                object[] zero = {0};
                type.InvokeMember(m.Name,
                                  BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField |
                                  BindingFlags.Instance, null, instance, zero);
            }
        }

        public static Dictionary<string, string> Update()
        {
            var outdata = new Dictionary<string, string>();
            var instance = new Stack();
            var type = instance.GetType();
            foreach (var m in type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField |
                                                        BindingFlags.Instance))
            {
                if (m.MemberType != MemberTypes.Field && m.MemberType != MemberTypes.Property) continue;
                var valor = (int) type.InvokeMember(m.Name,
                                                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField |
                                                    BindingFlags.Instance, null, instance, null);
                outdata.Add(m.Name.Substring(1).Replace('_', ' '), valor.ToString());
            }
            return outdata;
        }
         
    }
}
