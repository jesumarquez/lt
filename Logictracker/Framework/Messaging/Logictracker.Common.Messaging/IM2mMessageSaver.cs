using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Utils;

namespace Logictracker.Messaging
{
    public interface IM2mMessageSaver
    {
        LogEvento Save(string codigo, Dispositivo dispositivo, Sensor sensor, SubEntidad subEntidad, DateTime inicio, DateTime fin, string texto);
        void Discard(string codigo, Dispositivo dispositivo, Sensor sensor, DateTime fecha, GPSPoint inicio, GPSPoint fin, DiscardReason discardReason);
    }
}
