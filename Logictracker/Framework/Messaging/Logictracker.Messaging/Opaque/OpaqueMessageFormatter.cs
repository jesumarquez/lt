#region Usings

using System;
using System.Globalization;
using System.IO;
using System.Messaging;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.MsmqMessaging.Opaque
{
    public class OpaqueMessageFormatter : IMessageFormatter
    {
        //private static int ReceivedMessageId;

        public OpaqueMessageFormatter(String readLabelReformatExpression)
        {
            ReadLabelReformatExpression = readLabelReformatExpression;
        }

        /// <summary>
        /// Permite reformatear el Label del mensaje de forma dinamica
        /// en el momento en que se lee de la cola.
        /// estas con las expresiones validas:
        /// 
        /// ${SendTime} : se reemplaza por Message.SentTime
        /// ${Label}    : se reemplaza por Message.Label (el valor original)
        /// ${StringBody}  : se reemplaza por Message.Body si este se puede convertir a String.
        /// 
        /// </summary>

        public String ReadLabelReformatExpression { get; set; }

        #region IMessageFormatter Members

        public object Clone()
        {
            return new OpaqueMessageFormatter(ReadLabelReformatExpression);
        }

        public bool CanRead(Message message)
        {
            return true;
        }

        public object Read(Message message)
        {
            var omsg = new OpaqueMessage
                           {
                               Label = LabelReformat(ReadLabelReformatExpression, message),
                               OpaqueBodyType = message.BodyType,
                               Length = Convert.ToInt32(message.BodyStream.Length),
                               Id = -1,
                               AppSpecific = message.AppSpecific
                           };
            omsg.OpaqueBody = new byte[omsg.Length];
            message.BodyStream.Read(omsg.OpaqueBody, 0, omsg.Length);
            return omsg;
        }

        public void Write(Message message, object obj)
        {
            var omsg = (OpaqueMessage) obj;
            if (omsg == null) throw new OpaqueMessageFormatterException("El objeto provisto no es del tipo OpaqueMessage.");
            message.Label = omsg.Label;
            message.BodyType = omsg.OpaqueBodyType;
            message.AppSpecific = omsg.AppSpecific;
            message.BodyStream = new MemoryStream(omsg.OpaqueBody);
        }

        #endregion

        public virtual String LabelReformat(String Format, Message message)
        {
            if (String.IsNullOrEmpty(Format))
            {
                //STrace.Trace(GetType().FullName,2,"OpaqueMessageQueueFormatter: Sin Reescritura de Label.");
                return message.Label;
            }
            // caso especial para evitar que interq reecriba el label
            // enviar este codigo en AppSpecific. (0x23570001) 
            // NOTA: 0x2357XXXX es la mascara que indica que el codigo es un codigo de Logictracker
            // el 0001 Indica dejar el label como esta.
            if ((message.AppSpecific & 0x7FFF0001) == 0x23570001) return message.Label;
            var work = Format;
            // reemplazo de datos relevantes.
            work = work.Replace("${Label}", message.Label);
            work = work.Replace("${SentTime}", message.SentTime.ToString(CultureInfo.InvariantCulture));
            work = work.Replace("${AppSpecific}", message.AppSpecific.ToString(CultureInfo.InvariantCulture));
            work = work.Replace("${MachineName}", Environment.MachineName);
            if (!work.Contains("${StringBody}"))
            {
                STrace.Debug(GetType().FullName, String.Format("OpaqueMessageQueueFormatter: Reescribiendo Label '{0}'", work));
                return work;
            }
            try
            {
                var strbody = (String) message.Body;
                if (!String.IsNullOrEmpty(strbody))
                {
                    work = work.Replace("${StringBody}", strbody);
                }
            } catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e, "OpaqueMessageFormatter.LabelReformat");
                return work;
            }
            STrace.Debug(GetType().FullName, String.Format("OpaqueMessageQueueFormatter: Reescribiendo Label '{0}' (W/STRBODY)", work));
            return work;
        }
    }
}