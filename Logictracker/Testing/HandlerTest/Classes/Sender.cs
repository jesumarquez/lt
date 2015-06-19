using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Layers.MessageQueue;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace HandlerTest.Classes
{
    public class Sender
    {
        private static ulong _messageId = 1;

        public static Position CreatePosition(Dispositivo dispositivo, DateTime date, double latitud, double longitud, int velocidad)
        {
            if (dispositivo == null) return null;
            var messgeId = GetNextMessageId();
            return new GPSPoint(date, (float)latitud, (float)longitud, velocidad, GPSPoint.SourceProviders.GpsProvider, 50.0f)
                .ToPosition(dispositivo.Id, messgeId) as Position;
        }
        public static Event CreateGenericEvent(MessageIdentifier messageIdentifier, Dispositivo dispositivo, DateTime date)
        {
            return CreateGenericEvent(messageIdentifier, dispositivo, date, null);
        }
        public static Event CreateGenericEvent(MessageIdentifier messageIdentifier, Dispositivo dispositivo, DateTime date, IEnumerable<Int64> extraData)
        {
            return messageIdentifier.FactoryEvent(MessageIdentifier.GenericMessage, dispositivo.Id, 0, null, date, null, extraData);
        }

        public static void Enqueue(string queue, object message , string queueType)
        {
            try
            {
                if (message == null) return;

                var umq = new IMessageQueue(queue) { QueueType = queueType };

                if (umq.LoadResources())
                {
                    umq.Send(message);
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private static ulong GetNextMessageId()
        {
            return _messageId++;
        }
    }
}
