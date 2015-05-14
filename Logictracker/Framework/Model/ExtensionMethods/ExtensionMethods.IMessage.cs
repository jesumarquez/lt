#region Usings

using System;
using System.Text;

#endregion

namespace Urbetrack.Model
{
    public static partial class ExtensionMethods
    {
        ///<summary>
        ///</summary>
        ///<param name="msg"></param>
        ///<returns></returns>
        public static ulong NewUniqueID(this IMessage msg)
        {
            return (ulong) Guid.NewGuid().GetHashCode();
        }

        ///<summary>
        /// Obtiene el NodeCode del origen del mensaje.-
        ///</summary>
        ///<param name="message">Mensaje origen</param>
        ///<returns>Si esta presente, retorna el NodeCode sino retorna 0.</returns>
        public static int GetDeviceId(this IMessage message)
        {
            try
            {
                return message.NodeCode;
            }catch (Exception)
            {
                return 0;   
            }
        }

        /// <summary>
        /// Extension que obtiene el payload del frame en formato String.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static string AsString(this IFrame frame)
        {
            if (frame == null || frame.Payload == null) return null;
            return Encoding.ASCII.GetString(frame.Payload);
        }

        /// <summary>
        /// Obtiene el Identificador del Chofer del vehiculo.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetRiderId(this IMessage message)
        {
            try
            {
                if (message is IUserIdentified)
                    return (message as IUserIdentified).UserIdentifier;
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}