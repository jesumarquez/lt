using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;

namespace Logictracker
{
    public static class SharedPositions
    {
        private static IEnumerable<LogUltimaPosicionVo> UpdatePositions(IEnumerable<Coche> coches)
        {
            var daoF = new DAOFactory();
            var posiciones = GetPositions(coches);

            foreach (var pos in posiciones)
            {
                var dispositivo = daoF.DispositivoDAO.FindById(pos.IdDispositivo);
                var ticket = daoF.TicketDAO.FindEnCurso(dispositivo);

                if (ticket == null || ticket.Estado == 0 || ticket.Detalles == null || ticket.Detalles.Count.Equals(0)) pos.IconoOnline = pos.IconoDefault;
                else
                {
                    var last = ticket.Detalles.OfType<DetalleTicket>().First();

                    foreach (DetalleTicket detalle in ticket.Detalles)
                    {
                        if (detalle.EstadoLogistico.Modo && detalle.Automatico.HasValue) last = detalle;
                        if (!detalle.EstadoLogistico.Modo && detalle.Manual.HasValue) last = detalle;
                    }

                    var auto = last.EstadoLogistico.Modo;

                    var diff = auto && last.Automatico.HasValue
                                   ? last.Programado.Value.Subtract(last.Automatico.Value)
                                   : !auto && last.Manual.HasValue
                                         ? last.Programado.Value.Subtract(last.Manual.Value)
                                         : TimeSpan.Zero;

                    if (diff.TotalMinutes < -5) pos.IconoOnline = pos.IconoAtraso;
                    else if (diff.TotalMinutes > 5) pos.IconoOnline = pos.IconoAdelanto;
                    else pos.IconoOnline = pos.IconoNormal;
                }
            }

            return posiciones.ToList();
        }

        private static IEnumerable<SharedPopup> UpdatePopUps(int empresa, ICollection<Coche> coches, DAOFactory daoF, int refreshTime)
        {
            if (coches.Count == 0) return new List<SharedPopup>(0);

            return daoF.LogMensajeDAO.GetMensajesToPopUp(empresa, coches.Select(c => c.Id).ToList(), 5, refreshTime).Select(popup=>new SharedPopup(popup)).ToList();
        }

        private static IEnumerable<SharedPopup> UpdatePopUps(int empresa, ICollection<Dispositivo> dispositivos, DAOFactory daoF, int refreshTime)
        {
            if (dispositivos.Count == 0) return new List<SharedPopup>(0);

            return daoF.LogMensajeDAO.GetMensajesDispositivosToPopUp(empresa, dispositivos.Select(c => c.Id).ToList(), 5, refreshTime).Select(popup => new SharedPopup(popup)).ToList();
        }

        private static IEnumerable<SharedPopupM2M> UpdatePopUpsM2M(ICollection<Dispositivo> dispositivos, DAOFactory daoF)
        {
            if (dispositivos.Count == 0) return new List<SharedPopupM2M>(0);

            return daoF.LogEventoDAO.GetEventosDispositivosToPopUp(dispositivos.Select(c => c.Id).ToList(), 5).Select(popup => new SharedPopupM2M(popup)).ToList();
        }

        private static IEnumerable<LogUltimaPosicionVo> GetPositions(IEnumerable<Coche> coches)
        {
            return new DAOFactory().LogPosicionDAO.GetLastVehiclesPositions(coches).Values.Where(position => position != null).ToList();
        }

        public static IEnumerable<LogUltimaPosicionVo> GetNewPositions(DateTime fecha, IList<Coche> coches)
        {
            var posiciones = UpdatePositions(coches);
            return from posicion in posiciones
                   let ultimo = fecha.Subtract(posicion.FechaMensaje)
                   let actual = DateTime.UtcNow.Subtract(posicion.FechaMensaje)
                   where posicion.FechaRecepcion > fecha
                     || (actual > TimeSpan.FromHours(48) && ultimo <= TimeSpan.FromHours(48))
                     || (actual > TimeSpan.FromMinutes(5) && ultimo <= TimeSpan.FromMinutes(5))
                   select posicion;
        }

        public static IEnumerable<LogUltimaPosicionVo> GetLastPositions(IList<Coche> coches)
        {
            return UpdatePositions(coches).ToList();
        }

        public static IEnumerable<SharedPopup> GetNewPopups(int empresa, IList<Coche> coches, DAOFactory daoF, int refreshTime)
        {
            return UpdatePopUps(empresa, coches, daoF, refreshTime);
        }

        public static IEnumerable<SharedPopup> GetNewPopups(int empresa, IList<Dispositivo> dispositivos, DAOFactory daoF, int refreshTime)
        {
            return UpdatePopUps(empresa, dispositivos, daoF, refreshTime);
        }

        public static IEnumerable<SharedPopupM2M> GetNewPopupsM2M(IList<Dispositivo> dispositivos, DAOFactory daoF)
        {
            return UpdatePopUpsM2M(dispositivos, daoF);
        }

        public static IEnumerable<LogUltimaPosicionVo> GetAllPositions(IEnumerable<int> idsLineas, DAOFactory daoF)
        {
            var coches = daoF.CocheDAO.GetList(new[]{-1}, idsLineas)
                .Where(v => v.Dispositivo != null);
            return UpdatePositions(coches);
        }

        public static IEnumerable<LogUltimaPosicionVo> GetLastPositionByInterno(string interno, DAOFactory daoF, int empresa, int linea, bool hideWithNoDevice)
        {
            var coches = daoF.CocheDAO.GetList(new[] {empresa}, new[] {linea});
            coches = coches.Where(c => c.Interno.ToLower().Contains(interno.ToLower())).ToList();

            if (hideWithNoDevice) coches = coches.Where(c => c.Dispositivo != null).ToList();

            return coches.Count <= 0 ? new List<LogUltimaPosicionVo>() : UpdatePositions(coches);
        }
    }

    [Serializable]
    public class SharedLinea
    {
        private readonly Dictionary<string, int> _internos;

        public SharedLinea(int id, DateTime lastDbCheck)
        {
            Id = id;
            LastDbCheck = lastDbCheck;
            _internos = new Dictionary<string, int>();
        }

        private int Id { get; set; }

        public DateTime LastDbCheck { get; set; }

        public Dictionary<string, int> Internos { get { return _internos; } }
    }

    [Serializable]
    public class SharedLastPosition
    {
        public DateTime Fecha;
        public string Icono;
        public readonly int Id;
        public string Interno;
        public double Latitud;
        public double Longitud;
        public DateTime Timestamp;
        public readonly int TipoVehiculo;
        public int Velocidad;
        public double Curso;

        public SharedLastPosition(int id, int tipovehiculo, string interno, double lat, double lon, string icono, DateTime fecha, DateTime timestamp, int velocidad, double curso)
        {
            Id = id;
            Interno = interno;
            Latitud = lat;
            Longitud = lon;
            Fecha = fecha;
            Timestamp = timestamp;
            TipoVehiculo = tipovehiculo;
            Icono = icono;
            Velocidad = velocidad;
            Curso = curso;
        }
    }

    [Serializable]
    public class SharedPopup
    {
        public readonly string Color;
        public DateTime DateTime;
        public DateTime DateTimeAlta;
        public readonly int Id;
        public readonly string Interno;
        public readonly string Text;
        public readonly int IdVehiculo;
        public readonly int IdTransportista;
        public readonly string CodigoMensaje;
        public readonly bool RequiereAtencion;
        public readonly int IdPerfil;

        public readonly string Sound;

        public SharedPopup(LogMensajeBase msg)
        {
            Id = msg.Id;
            Interno = msg.Coche.Interno;
            IdVehiculo = msg.Coche.Id;
            IdTransportista = msg.Coche.Transportista != null ? msg.Coche.Transportista.Id : -1;
            CodigoMensaje = msg.Mensaje.Codigo;
            DateTime = msg.Fecha;
            DateTimeAlta = msg.FechaAlta.Value;
            RequiereAtencion = msg.Accion.RequiereAtencion;
            IdPerfil = msg.Accion.PerfilHabilitado != null ? msg.Accion.PerfilHabilitado.Id : -1;

            Text = msg.Texto;

            if (msg.Accion != null)
            {
                Color = "#" + msg.Accion.RGB;

                if (msg.Accion.Sonido != null) Sound = msg.Accion.Sonido.URL;
            }
            else Color = "#CCCCCC";
        }
    }

    [Serializable]
    public class SharedPopupM2M
    {
        public readonly string Color;
        public DateTime DateTime;
        public readonly int Id;
        public readonly string Sensor;
        public readonly string SubEntidad;
        public readonly string Text;
        public readonly int IdSensor;
        public readonly int IdSubEntidad;
        public readonly string CodigoMensaje;

        public readonly string Sound;

        public SharedPopupM2M(LogEvento ev)
        {
            Id = ev.Id;
            Sensor = ev.Sensor.Descripcion;
            IdSensor = ev.Sensor.Id;
            SubEntidad = ev.SubEntidad.Descripcion;
            IdSubEntidad = ev.SubEntidad.Id;
            CodigoMensaje = ev.Mensaje.Codigo;
            DateTime = ev.Fecha;
            Text = ev.Texto;

            if (ev.Accion != null)
            {
                Color = "#" + ev.Accion.RGB;

                if (ev.Accion.Sonido != null) Sound = ev.Accion.Sonido.URL;
            }
            else Color = "#CCCCCC";
        }
    }
}