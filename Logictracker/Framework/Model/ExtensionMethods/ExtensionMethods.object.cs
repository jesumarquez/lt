#region Usings

using System;
using System.Net;
using System.Text;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Model
{
    /// <summary>
    /// Agrupacion de Extension Methods.
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Extension global que permite enrutar el trazado de objetos
        /// ITraceable.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void TRACE(this object obj, int source, string format, params object[] args)
        {
            if (obj is ITraceable && (obj as ITraceable).ContextTracer != null) (obj as ITraceable).ContextTracer.Append(String.Format(format, args));
        }

        public static string NODE(this object obj)
        {
            if (obj is INode) return String.Format("Node[{0:D6}]", (obj as INode).NodeCode);

            if (obj is IMessage) return String.Format("Node[{0:D6}]", (obj as IMessage).NodeCode);

            return "???unknow???";
        }

        public static string CONTEXT(this object obj, params object[] args)
        {
            var ctxt = obj.NODE();

            ctxt += string.Format(", {0:D1} args: ", args.GetLength(0));

            foreach (var o in args)
            {
                if (obj is INode) ctxt += String.Format("Node[{0:D6}]", (obj as INode).NodeCode);

                if (obj is IMessage) ctxt += String.Format("Message[{0,32}]<{1}>", (obj as IMessage).GetType().Name, (obj as IMessage).NODE());
                
                if (obj is IPEndPoint) ctxt += String.Format("EndPoint[{0,24}]", (obj as IPEndPoint));
                
                if (obj is IFrame) ctxt += String.Format("Frame<{0}>", Encoding.ASCII.GetString((obj as IFrame).Payload));
            }

            return ctxt;
        }

        public static void RECORD(this object obj, string type, string inp, string outp, params object[] args)
        {
            SimpleTrace.Trace(typeof(ExtensionMethods).FullName,"{0}:{1,10} '{2}' '{3}' context: {4}", obj.NODE(), type, inp, outp, obj.CONTEXT(args));
        }

        /// <summary>
        /// Numero aleatorio de 64 Bits
        /// </summary>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static UInt64 NextInt64(this Random rnd)
        {
            var buffer = new byte[sizeof(UInt64)];

            rnd.NextBytes(buffer);

            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Obtiene un identificador univoco de 128 bits, codificado en base
        /// 32 para hacerlo imprimible y sin caracteres significativos en 
        /// XML.
        /// </summary>
        /// <param name="rnd">cualquier objeto, puede ser null.</param>
        /// <returns></returns>
        public static string GenerateKey128b32(this object rnd) { return SafeConvert.ToBase32(Guid.NewGuid().ToByteArray()); }
    }
}