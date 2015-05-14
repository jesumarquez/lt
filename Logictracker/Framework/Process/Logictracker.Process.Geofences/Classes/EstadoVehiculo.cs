using System;
using System.Linq;
using Logictracker.Cache;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject;
using Logictracker.Utils;
using System.Collections.Generic;

namespace Logictracker.Process.Geofences.Classes
{
    public class EstadoVehiculo
    {
        public Coche Vehiculo { get; set; }
        public GPSPoint Posicion { get; set; }
        public Geocerca ZonaManejo { get; set; }
        public List<EstadoGeocerca> GeocercasDentro { get; set; }
        public List<GeocercaEvent> Eventos { get; set; }

        public EstadoVehiculo(Coche vehiculo)
        {
            Vehiculo = vehiculo;
            GeocercasDentro = new List<EstadoGeocerca>();
            Eventos = new List<GeocercaEvent>();
        }

        public static EstadoVehiculo Load(Coche vehiculo, DAOFactory daoFactory)
        {
            var te = new TimeElapsed();
            var dentro1 = Retrieve(vehiculo, "dentro");
            if (te.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("EstadoVehiculo.Load/Retrieve ({0} secs)", te.getTimeElapsed().TotalSeconds.ToString()));
            if (dentro1 == null) return null;


            te.Restart();            
            var posicion = daoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(vehiculo);
            if (te.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("EstadoVehiculo.Load/GetLastOnlineVehiclePosition ({0} secs)", te.getTimeElapsed().TotalSeconds.ToString()));

            if (posicion == null) return null;

            te.Restart();
            var estado = new EstadoVehiculo(vehiculo)
            {
                Posicion = new GPSPoint(posicion.FechaMensaje, 
                                        (float) posicion.Latitud, 
                                        (float) posicion.Longitud,
                                        posicion.Velocidad, 
                                        GPSPoint.SourceProviders.Unknown, 
                                        0)
            };
            if (te.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("EstadoVehiculo.Load/New EstadoVehiculo ({0} secs)", te.getTimeElapsed().TotalSeconds.ToString()));
            
            te.Restart();
            var zonamanejo = (int)(Retrieve(vehiculo, "zonamanejo")?? 0);
            if (te.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("EstadoVehiculo.Load/RetrieveZonaManejo ({0} secs)", te.getTimeElapsed().TotalSeconds.ToString()));
            te.Restart();
            estado.ZonaManejo = zonamanejo > 0 ? daoFactory.ReferenciaGeograficaDAO.FindGeocerca(zonamanejo) : null;
            if (te.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("EstadoVehiculo.Load/FindGeocercas ({0} secs)", te.getTimeElapsed().TotalSeconds.ToString()));
            var dentro = (int[])dentro1;

            te.Restart();
            estado.GeocercasDentro = dentro.Select(x => GetEstadoGeocercaDentro(vehiculo, daoFactory.ReferenciaGeograficaDAO.FindGeocerca(x))).ToList();
            if (te.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("EstadoVehiculo.Load/GetEstadoGeocercaDentro ({0} secs)", te.getTimeElapsed().TotalSeconds.ToString()));
            return estado;
        }
        public static EstadoGeocerca GetEstadoGeocerca(Coche vehiculo, Geocerca geocerca)
        {
            var key = string.Format("device[{0}].geocercas", vehiculo.Dispositivo.Id);
            var dentro1 = LogicCache.Retrieve<int[]>(key);
            if (dentro1 == null) return null;

            var dentro = dentro1;
            return dentro.Contains(geocerca.Id) ? GetEstadoGeocercaDentro(vehiculo, geocerca) : null;
        }
        private static EstadoGeocerca GetEstadoGeocercaDentro(Coche vehiculo, Geocerca geocerca)
        {
            return new EstadoGeocerca
            {
                EnExcesoVelocidad = (bool)(Retrieve(vehiculo, geocerca.Id, "enexceso") ?? false),
                Estado = EstadosGeocerca.Dentro,
                Geocerca = geocerca,
                PosicionInicioExceso = Retrieve(vehiculo, geocerca.Id, "posicionexceso") as GPSPoint,
                VelocidadMaxima = (int)(Retrieve(vehiculo, geocerca.Id, "velomaxima") ?? 0),
                VelocidadPico = (int)(Retrieve(vehiculo, geocerca.Id, "velopico") ?? 0)
            };
        }
        public void Save()
        {
            Store("zonamanejo", ZonaManejo != null ? ZonaManejo.Id : 0);
            Store("dentro", GeocercasDentro.Select(x=>x.Geocerca.Id).ToArray());
            foreach(var geocerca in GeocercasDentro)
            {
                Store(geocerca.Geocerca.Id, "enexceso", geocerca.EnExcesoVelocidad);
                Store(geocerca.Geocerca.Id, "posicionexceso", geocerca.PosicionInicioExceso);
                Store(geocerca.Geocerca.Id, "velomaxima", geocerca.VelocidadMaxima);
                Store(geocerca.Geocerca.Id, "velopico", geocerca.VelocidadPico);
            }
        }
        private void Store(string variable, object value)
        { 
            const string prefix = "[EV({0})][{1}]";
            LogicCache.Store(string.Format(prefix, Vehiculo.Dispositivo.Id, variable), value); 
        }
        private static object Retrieve(Coche vehiculo, string variable)
        {
            const string prefix = "[EV({0})][{1}]";
            return LogicCache.Retrieve<object>(string.Format(prefix, vehiculo.Dispositivo.Id, variable));
        }
        private void Store(int geocerca, string variable, object value)
        {
            const string prefix = "[EV({0}:{1})][{2}]";
            LogicCache.Store(string.Format(prefix, Vehiculo.Dispositivo.Id, geocerca, variable), value);
        }
        private static object Retrieve(Coche vehiculo, int geocerca, string variable)
        {
            const string prefix = "[EV({0}:{1})][{2}]";
            return LogicCache.Retrieve<object>(string.Format(prefix, vehiculo.Dispositivo.Id,geocerca, variable));
        }
    }
}
