using System;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Messaging
{
    public interface IMessageSaver
    {
        LogMensajeBase Save(string codigo, Coche coche, DateTime fecha, GPSPoint inicio, string texto);
        LogMensajeBase Save(string codigo, Coche coche, Empleado empleado, DateTime fecha, GPSPoint inicio, string texto);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto, Zona zonaManejo);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto, ViajeDistribucion viaje, EntregaDistribucion entrega);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int velPermitida, int velAlcanzada);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto, int idReferenciaGeografica);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int? velPermitida, int? velAlcanzada, int? idReferenciaGeografica);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int? velPermitida, int? velAlcanzada, int? idReferenciaGeografica, Zona zonaManejo);
        LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int? velPermitida, int? velAlcanzada, int? idReferenciaGeografica, Zona zonaManejo, ViajeDistribucion viaje, EntregaDistribucion entrega);
        void Discard(string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, DiscardReason discardReason);
    }
}
