using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.Geofences.Classes;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Process.CicloLogistico
{
    public abstract class CicloLogisticoBase: ICicloLogistico
    {
        protected DAOFactory DaoFactory { get; set; }
        protected IMessageSaver MessageSaver { get; set; }
        public bool Regeneracion { get; set; }

        protected abstract Coche Vehiculo { get; }
        protected abstract Empleado Empleado { get; }
        protected abstract DateTime MinDate { get; }
        protected abstract DateTime MaxDate { get; }

        public abstract string CurrentState { get; }
        public abstract int CurrentStateCompleted { get; }
        public abstract int TotalCompleted { get; }
        public abstract int Delay { get; }

        public abstract bool EnGeocerca { get; }
        public abstract DateTime? EnGeocercaDesde { get; }
        public abstract DateTime Iniciado { get; }
        public abstract string Interno { get; }
        public abstract string Codigo { get; }
        public abstract string Cliente { get; }
        public abstract string Telefono { get; }
        public abstract string PuntoEntrega { get; }

        protected CicloLogisticoBase(DAOFactory daoFactory, IMessageSaver messageSaver)
        {
            DaoFactory = daoFactory;
            MessageSaver = messageSaver;
        }

        #region Process
        public void ProcessEvent(IEvent data)
        {
            ProcessEvent(data, false);
        }
        protected void ProcessEvent(IEvent data, bool isChildEvent)
        {
            if (IgnoreEvent(data))
            {
                AutoCloseTicket();

                if (data.EventType == EventTypes.Garmin)
                    Process(data as GarminEvent);

                return;
            }

            try
            {
                switch (data.EventType)
                {
                    case EventTypes.Init: Process(data as InitEvent); break;
                    case EventTypes.Close: Process(data as CloseEvent); break;
                    case EventTypes.Manual: Process(data as ManualEvent); break;
                    case EventTypes.Geofence: Process(data as GeofenceEvent); break;
                    case EventTypes.Tolva: Process(data as TolvaEvent); break;
                    case EventTypes.Trompo: Process(data as TrompoEvent); break;
                    case EventTypes.Garmin: Process(data as GarminEvent); break;
                    case EventTypes.Mobile: Process(data as MobileEvent); break;
                    case EventTypes.Route: Process(data as RouteEvent); break;
                    case EventTypes.Position: Process(data as PositionEvent); break;
                }
            }
            catch (Exception ex)
            {
                STrace.Exception("CicloLogisticoBase: ProcessEvent", ex);
                throw;
            }
            finally
            {
                // Cierre automatico de tickets viejos
                if (!isChildEvent) AutoCloseTicket();
            }
        }
        protected virtual void Process(InitEvent data) {}
        protected virtual void Process(CloseEvent data) {}
        protected virtual void Process(ManualEvent data) {}
        protected virtual void Process(GeofenceEvent data) {}
        protected virtual void Process(TolvaEvent data) {}
        protected virtual void Process(TrompoEvent data) {}
        protected virtual void Process(GarminEvent data) { }
        protected virtual void Process(MobileEvent data) { }
        protected virtual void Process(RouteEvent data) { }
        protected virtual void Process(PositionEvent data) { }

        #endregion

        protected abstract bool IgnoreEvent(IEvent data);
        protected abstract void AutoCloseTicket();

        #region Geocercas

        /// <summary>
        /// Ids de todas las geocercas de las que se analiza entrada y salida
        /// </summary>
        protected abstract IEnumerable<int> Geocercas { get; }

        protected abstract IEnumerable<int> Puntos { get; }

        /// <summary>
        /// Construye una clave única para almacenar el estado de la geocerca en la cache
        /// </summary>
        /// <param name="geocerca"></param>
        /// <returns></returns>
        protected abstract string GetKeyGeocerca(int geocerca);

        /// <summary>
        /// Elimina el estado de todas las geocercas de este ticket de la cache
        /// </summary>
        protected void ClearGeocercasCache()
        {
            foreach (var key in Geocercas.Select(g => GetKeyGeocerca(g)).Where(key => LogicCache.KeyExists(typeof(EstadosEntrega), key)))
            {
                LogicCache.Delete(typeof(EstadosEntrega), key);
            }
        }

        /// <summary>
        /// Setea el estado inicial (dentro/fuera) de todas las geocercas del ticket a partir de la ultima posicion del vehiculo.
        /// </summary>
        protected void FirstPosition()
        {
            try
            {
                var position = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(Vehiculo);
                if (position == null) return;

                // Pongo todas las geocercas en Desconocido
                var geocercas = Geocercas.Select(geocerca => GetKeyGeocerca(geocerca));
                
                foreach (var key in geocercas)
                {
                    LogicCache.Store(typeof(EstadosEntrega), key, new EstadosEntrega(EstadosGeocerca.Desconocido), DateTime.UtcNow.AddHours(5));
                }

                // Proceso la primera posicion
                var positionEvent = new PositionEvent(position.FechaMensaje, position.Latitud, position.Longitud);
                ProcessGeocercas(positionEvent);
            }
            catch (Exception ex)
            {
                STrace.Exception(typeof(CicloLogisticoFactory).FullName, ex);
            }
        }

        /// <summary>
        /// Procesa una posicion y genera los eventos de entrada y salida de geocerca.
        /// </summary>
        /// <param name="data"></param>
        protected void ProcessGeocercas(PositionEvent data)
        {
            try
            {
                var geocercas = Puntos;
                var point = new GPSPoint(data.Date, (float)data.Latitud, (float)data.Longitud);

                foreach (var geocerca in geocercas)
                {
                    var key = GetKeyGeocerca(geocerca);
                    if (Regeneracion) key = "recalc_" + key;

                    var lastState = LogicCache.KeyExists(typeof(EstadosEntrega), key)
                        ? LogicCache.Retrieve<EstadosEntrega>(typeof(EstadosEntrega), key).Estado
                        : EstadosGeocerca.Desconocido;
                    
                    var geo = DaoFactory.ReferenciaGeograficaDAO.FindGeocerca(geocerca);
                    
                    var p = new PointF(point.Lon, point.Lat);
                    var inside = geo.IsInBounds(p) && geo.Contains(p.Y, p.X);

                    var newState = inside ? EstadosGeocerca.Dentro : EstadosGeocerca.Fuera;

                    if (lastState != newState)
                    {
                        var state = new EstadosEntrega(newState);
                        LogicCache.Store(typeof(EstadosEntrega), key, state, Regeneracion ? DateTime.UtcNow.AddMinutes(5) : DateTime.UtcNow.AddHours(5));
                        
                        IEvent evento = null;
                        if (lastState == EstadosGeocerca.Dentro && newState == EstadosGeocerca.Fuera)
                        {                            
                            evento = EventFactory.GetEvent(DaoFactory, point, MessageCode.OutsideGeoRefference.GetMessageCode(), geocerca, 0, null, Empleado);
                        }
                        else if (lastState == EstadosGeocerca.Fuera && newState == EstadosGeocerca.Dentro)
                        {
                            evento = EventFactory.GetEvent(DaoFactory, point, MessageCode.InsideGeoRefference.GetMessageCode(), geocerca, 0, null, Empleado);
                        }
                        if (evento == null) continue;
                        
                        ProcessEvent(evento, true);
                    }
                }
            }
            catch (Exception ex)
            {
                STrace.Exception(typeof(CicloLogisticoFactory).FullName, ex);
            }
        }
        #endregion

        #region Regeneration

        public void Regenerate()
        {
            Regenerate(MinDate, MaxDate);
        }
        public void Regenerate(DateTime desde, DateTime hasta)
        {
            Regeneracion = true;
            var maxMonths = Vehiculo.Empresa != null ? Vehiculo.Empresa.MesesConsultaPosiciones : 3;
            var logMensajes = DaoFactory.LogMensajeDAO.GetEvents(Vehiculo.Id, desde, hasta, maxMonths);

            foreach (var logMensaje in logMensajes)
            {
                if (logMensaje.Latitud == 0 || logMensaje.Longitud == 0)
                {
                    var pos = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(Vehiculo.Id, logMensaje.Fecha, maxMonths);
                    logMensaje.Latitud = pos.Latitud;
                    logMensaje.Longitud = pos.Longitud;
                    DaoFactory.LogMensajeDAO.SaveOrUpdate(logMensaje);
                }
                var evento = EventFactory.GetEvent(DaoFactory, logMensaje);
                if (evento == null) continue;
                ProcessEvent(evento);
            }

            var positions = DaoFactory.LogPosicionDAO.GetPositionsBetweenDates(Vehiculo.Id, desde, hasta, maxMonths);
            foreach (var logPosicion in positions)
            {
                var pos = new PositionEvent(logPosicion.FechaMensaje, logPosicion.Latitud, logPosicion.Longitud);
                ProcessEvent(pos);
            }
            Regeneracion = false;
        } 

        #endregion
        
        #region SaveMessage
        protected void SaveMessage(string messageCode, DateTime fecha)
        {
            SaveMessage(messageCode, string.Empty, null, fecha);
        }
        protected void SaveMessage(string messageCode, DateTime fecha, ViajeDistribucion viaje, EntregaDistribucion entrega)
        {
            SaveMessage(messageCode, string.Empty, null, fecha, viaje, entrega);
        }
        protected void SaveMessage(string messageCode, string text, DateTime fecha)
        {
            SaveMessage(messageCode, text, null, fecha);
        }
        protected void SaveMessage(string messageCode, string text, IEvent data)
        {
            var point = data.Latitud != 0 && data.Longitud != 0 
                ? new GPSPoint(data.Date, (float) data.Latitud, (float) data.Longitud)
                : null;
            SaveMessage(messageCode, text, point, data.Date);
        }
        protected void SaveMessage(string messageCode, string text, IEvent data, ViajeDistribucion viaje, EntregaDistribucion entrega)
        {
            var point = data.Latitud != 0 && data.Longitud != 0
                ? new GPSPoint(data.Date, (float)data.Latitud, (float)data.Longitud)
                : null;
            SaveMessage(messageCode, text, point, data.Date, viaje, entrega);
        }
        protected void SaveMessage(string messageCode, string text, GPSPoint position, DateTime fecha)
        {
            if (MessageSaver == null) return;
            MessageSaver.Save(null, messageCode, Vehiculo.Dispositivo, Vehiculo, Empleado, fecha, position, text);
        }
        protected void SaveMessage(string messageCode, string text, GPSPoint position, DateTime fecha, ViajeDistribucion viaje, EntregaDistribucion entrega)
        {
            if (MessageSaver == null) return;
            MessageSaver.Save(null, messageCode, Vehiculo.Dispositivo, Vehiculo, Empleado, fecha, position, text, viaje, entrega);
        } 
        #endregion
    }
}
