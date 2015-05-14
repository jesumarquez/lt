using System;
using Logictracker.Utils;

namespace Logictracker.Process.CicloLogistico
{
    internal class Message
    {
        public readonly string MessageCode;
        public readonly string Text;
        public readonly GPSPoint Position;
        public readonly DateTime Fecha;
        public Message(string messageCode, string text, GPSPoint position, DateTime fecha)
        {
            MessageCode = messageCode;
            Text = text;
            Position = position;
            Fecha = fecha;
        }
    }
}
