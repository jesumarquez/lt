using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Process.Geofences.Classes;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject;
using Logictracker.Utils;
using System.Drawing;
using System.Threading;

namespace Logictracker.Process.Geofences
{
    public static class GeocercaManager
    {
        private static readonly Dictionary<string, QtreeNode> Qtrees = new Dictionary<string, QtreeNode>();
        private static readonly Dictionary<int, object> LocksByVehicle = new Dictionary<int, object>();
        private static readonly Dictionary<string, object> LocksByEmpresaLinea = new Dictionary<string, object>();

        public static EstadoVehiculo GetEstadoVehiculo(Coche vehiculo, DAOFactory daoFactory)
        {
            if (vehiculo == null) return null;

            var ts = new TimeElapsed();
            var estadoVehiculo = EstadoVehiculo.Load(vehiculo, daoFactory);
            if (ts.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("EstadoVehiculo.Load ({0} secs)", ts.getTimeElapsed().TotalSeconds));

            if (estadoVehiculo == null)
            {
                var posicion = daoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(vehiculo);
                if (posicion == null) return null;
                var point = new GPSPoint(posicion.FechaMensaje, (float)posicion.Latitud, (float)posicion.Longitud, posicion.Velocidad, GPSPoint.SourceProviders.Unknown, 0);
                return CalcularEstadoVehiculo(vehiculo, point, daoFactory);
            }

            return estadoVehiculo;
        }
        public static EstadoGeocerca GetEstadoGeocerca(Coche vehiculo, int geocerca, DAOFactory daoFactory)
        {
            if (vehiculo == null) return null;

            var geo = daoFactory.ReferenciaGeograficaDAO.FindGeocerca(geocerca);
            return geo == null ? null : EstadoVehiculo.GetEstadoGeocerca(vehiculo, geo);
        }
        public static EstadoVehiculo CalcularEstadoVehiculo(Coche vehiculo, GPSPoint position, DAOFactory daoFactory)
        {
            if (vehiculo == null || position == null) return null;
            var estadoVehiculo = new EstadoVehiculo(vehiculo) { Posicion = position };
            var t = new TimeElapsed();
            lock (GetLock(vehiculo.Id))
            {
                if (t.getTimeElapsed().TotalSeconds > 0.5) STrace.Error("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("CalcularEstadoVehiculo/EntroAlLock ({0} secs)", t.getTimeElapsed().TotalSeconds));

                t.Restart();
                var qtree = GetQtree(vehiculo);
                if (t.getTimeElapsed().TotalSeconds > 0.5) STrace.Error("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("CalcularEstadoVehiculo/GetQTree ({0} secs)", t.getTimeElapsed().TotalSeconds));
                var geocercas = qtree != null && qtree.GetData(position.Lat, position.Lon) != null
                                    ? qtree.GetData(position.Lat, position.Lon) : new List<Geocerca>(0);

                t.Restart();
                if (vehiculo.Empresa.EvaluaSoloGeocercasViaje)
                {
                    var viajeActivo = daoFactory.ViajeDistribucionDAO.FindEnCurso(vehiculo);
                    if (viajeActivo != null)
                    {
                        var tiposGeocercaViaje = vehiculo.Empresa.TiposGeocercaViaje;
                        var idGeocercas = geocercas.Where(g => !tiposGeocercaViaje.Contains(g.TipoReferenciaGeograficaId)).Select(g => g.Id).ToList();

                        var idsEntregas = viajeActivo.Detalles.Select(d => d.ReferenciaGeografica.Id).Distinct().ToList();

                        idGeocercas.AddRange(idsEntregas);
                        idGeocercas = idGeocercas.Distinct().ToList();

                        var faltantes = idsEntregas.Where(id => !idGeocercas.Contains(id));
                        if (faltantes.Any())
                        {
                            if (vehiculo.Empresa.Id == 92) STrace.Error("ResetQtree", string.Format("Actual: {0} - Faltantes: {1}", geocercas.Count, faltantes.Count()));
                            foreach (var idGeocerca in faltantes)
                            {
                                try
                                {
                                    var geocerca = daoFactory.ReferenciaGeograficaDAO.FindGeocerca(idGeocerca);
                                    geocercas.Add(geocerca);
                                }
                                catch { }
                            }
                            if (vehiculo.Empresa.Id == 92) STrace.Error("ResetQtree", "Total: " + geocercas.Count);
                        }

                        geocercas = geocercas.Where(g => idGeocercas.Contains(g.Id)).ToList();
                        if (t.getTimeElapsed().TotalSeconds > 0.5) STrace.Error("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercasViaje: {0} segundos", t.getTimeElapsed().TotalSeconds));
                    }
                }

                t.Restart();
                foreach (var geocerca in geocercas)
                {
                    var estadoGeocerca = new EstadoGeocerca { Geocerca = geocerca };
                    var inside = IsInside(geocerca, position.Lat, position.Lon);
                    if (!inside) { continue; }

                    estadoGeocerca.Estado = EstadosGeocerca.Dentro;
                    if (geocerca.ControlaVelocidad)
                    {
                        estadoGeocerca.VelocidadMaxima = geocerca.GetVelocidadMaxima(vehiculo.Id);
                        estadoGeocerca.EnExcesoVelocidad = geocerca.ControlaVelocidad && position.Velocidad > estadoGeocerca.VelocidadMaxima;
                        estadoGeocerca.PosicionInicioExceso = estadoGeocerca.EnExcesoVelocidad ? position : null;
                        estadoGeocerca.VelocidadPico = estadoGeocerca.EnExcesoVelocidad ? position.Velocidad : 0;
                    }
                    if (geocerca.ZonaManejo > 0)
                    {
                        if (estadoVehiculo.ZonaManejo == null || estadoVehiculo.ZonaManejo.PrioridadZona > geocerca.PrioridadZona)
                        {
                            estadoVehiculo.ZonaManejo = geocerca;
                        }
                    }

                    estadoVehiculo.GeocercasDentro.Add(estadoGeocerca);
                }
                if (t.getTimeElapsed().TotalSeconds > 0.5) STrace.Error("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("CalcularEstadoVehiculo/ForEach ({0} secs)", t.getTimeElapsed().TotalSeconds));

                return estadoVehiculo;
            }
        }

        public static EstadoVehiculo Process(Coche vehiculo, GPSPoint position, DAOFactory daoFactory)
        {
            if (vehiculo == null || position == null) return null;

            var t = new TimeElapsed();
            var estadoAnterior = GetEstadoVehiculo(vehiculo, daoFactory);
            if (t.getTimeElapsed().TotalSeconds > 0.5) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/GetEstadoVehiculo ({0} secs)", t.getTimeElapsed().TotalSeconds));

            t.Restart();
            var estadoActual = CalcularEstadoVehiculo(vehiculo, position, daoFactory);
            if (t.getTimeElapsed().TotalSeconds > 0.5) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/CalcularEstadoVehiculo ({0} secs)", t.getTimeElapsed().TotalSeconds));

            t.Restart();
            var geocercasAnterior = new Dictionary<int, EstadoGeocerca>();
            var geocercasActual = new Dictionary<int, EstadoGeocerca>();

            if (estadoAnterior != null)
            {
                foreach (var estadoGeocerca in estadoAnterior.GeocercasDentro)
                {
                    if (!geocercasAnterior.ContainsKey(estadoGeocerca.Geocerca.Id))
                        geocercasAnterior.Add(estadoGeocerca.Geocerca.Id, estadoGeocerca);
                }
            }
            else geocercasAnterior = null;

            foreach (var estadoGeocerca in estadoActual.GeocercasDentro)
            {
                if (!geocercasActual.ContainsKey(estadoGeocerca.Geocerca.Id))
                    geocercasActual.Add(estadoGeocerca.Geocerca.Id, estadoGeocerca);
            }
            var geocercasTodas = geocercasAnterior != null ? geocercasAnterior.Keys.Union(geocercasActual.Keys) : geocercasActual.Keys;
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/Part #1 ({0} secs)", t.getTimeElapsed().TotalSeconds));

            foreach (var geocerca in geocercasTodas)
            {
                t.Restart();
                var anterior = geocercasAnterior != null && geocercasAnterior.ContainsKey(geocerca) ? geocercasAnterior[geocerca] : null;
                var actual = geocercasActual.ContainsKey(geocerca) ? geocercasActual[geocerca] : new EstadoGeocerca { Geocerca = anterior != null ? anterior.Geocerca : null, Estado = EstadosGeocerca.Fuera };
                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #1 ({0} secs)", t.getTimeElapsed().TotalSeconds));

                t.Restart();
                if (anterior != null && anterior.Geocerca != null && !anterior.Geocerca.IsVigente(position.Date)) continue;
                if (actual.Geocerca != null && !actual.Geocerca.IsVigente(position.Date)) continue;
                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #2 ({0} secs)", t.getTimeElapsed().TotalSeconds));

                t.Restart();
                var antesDentro = anterior != null;
                var dentro = actual.Estado == EstadosGeocerca.Dentro;

                var sale = antesDentro && !dentro;
                var entra = !antesDentro && dentro;

                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #3 ({0} secs)", t.getTimeElapsed().TotalSeconds));

                if (actual.Geocerca != null)
                {
                    t.Restart();
                    if (sale && actual.Geocerca.ControlaEntradaSalida)
                    {
                        estadoActual.Eventos.Add(new GeocercaEvent { Evento = GeocercaEventState.Sale, Estado = actual });
                    }
                    if (sale && (actual.Geocerca.EsInicio || actual.Geocerca.EsIntermedio || actual.Geocerca.EsFin))
                    {
                        estadoActual.Eventos.Add(new GeocercaEvent { Evento = GeocercaEventState.TimeTrackingSalida, Estado = actual });
                    }
                    if (entra && actual.Geocerca.ControlaEntradaSalida)
                    {
                        estadoActual.Eventos.Add(new GeocercaEvent { Evento = GeocercaEventState.Entra, Estado = actual });
                    }
                    if (entra && (actual.Geocerca.EsInicio || actual.Geocerca.EsIntermedio || actual.Geocerca.EsFin))
                    {
                        estadoActual.Eventos.Add(new GeocercaEvent { Evento = GeocercaEventState.TimeTrackingEntrada, Estado = actual });
                    }
                    if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #4 ({0} secs)", t.getTimeElapsed().TotalSeconds));

                    if (actual.Geocerca.ControlaVelocidad)
                    {
                        t.Restart();
                        var antesExcedido = anterior != null && anterior.EnExcesoVelocidad;
                        var excedido = actual.VelocidadMaxima > 0 && position.Velocidad > actual.VelocidadMaxima;
                        if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #5 ({0} secs)", t.getTimeElapsed().TotalSeconds));
                        t.Restart();
                        if (dentro && !antesExcedido && excedido)
                        {
                            actual.InicioExceso(position);
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #6 (InicioExceso) ({0} secs)", t.getTimeElapsed().TotalSeconds));
                        }
                        else if (excedido)
                        {
                            actual.UpdateVelocidadPico(position.Velocidad);
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #7 (Excedido) ({0} secs)", t.getTimeElapsed().TotalSeconds));
                        }
                        t.Restart();
                        if (antesExcedido && (!dentro || !excedido))
                        {
                            estadoActual.Eventos.Add(new GeocercaEvent { Evento = GeocercaEventState.ExcesoVelocidad, Estado = actual.Clone() });
                            actual.FinExceso();
                            if (t.getTimeElapsed().TotalSeconds > 1)
                                STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ForEach Part #8 (FinExceso) ({0} secs)", t.getTimeElapsed().TotalSeconds));
                        }
                    }

                    if (antesDentro && !sale)
                    {
                        // Controla Permanencia en Geocerca
                        if (actual.Geocerca.ControlaPermanencia)
                        {
                            var entrada = daoFactory.LogMensajeDAO.GetLastGeoRefferenceEventDate(vehiculo, MessageCode.InsideGeoRefference.GetMessageCode(), actual.Geocerca.Id);
                            if (entrada.HasValue)
                            {
                                t.Restart();
                                var ultimaAlarma = daoFactory.LogMensajeDAO.GetLastGeoRefferenceEventDate(vehiculo, MessageCode.PermanenciaEnGeocercaExcedida.GetMessageCode(), actual.Geocerca.Id, entrada.Value);
                                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.ProcessLogMensajeDAO.GetLastGeoRefferenceEventDate 1 ({0} secs)", t.getTimeElapsed().TotalSeconds));
                                if (!ultimaAlarma.HasValue)
                                {
                                    var tiempoActual = position.Date.Subtract(entrada.Value);
                                    if (tiempoActual.TotalMinutes > actual.Geocerca.MaximaPermanencia)
                                    {
                                        actual.PosicionInicioExceso = position;
                                        estadoActual.Eventos.Add(new GeocercaEvent { Evento = GeocercaEventState.ExcesoPermanencia, Estado = actual });
                                    }
                                }
                            }
                        }

                        // Controla Permanencia en Geocerca en Ciclo Logístico
                        if (actual.Geocerca.ControlaPermanenciaEntrega)
                        {
                            t.Restart();
                            var ticket = daoFactory.TicketDAO.FindEnCurso(vehiculo.Dispositivo);
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/TicketDAO.FindEnCurso ({0} secs)", t.getTimeElapsed().TotalSeconds));
                            t.Restart();
                            var distri = daoFactory.ViajeDistribucionDAO.FindEnCurso(vehiculo);
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.Process/ViajeDistribucionDAO.FindEnCurso ({0} secs)", t.getTimeElapsed().TotalSeconds));
                            if (ticket != null || distri != null)
                            {
                                var entrada = daoFactory.LogMensajeDAO.GetLastGeoRefferenceEventDate(vehiculo, MessageCode.InsideGeoRefference.GetMessageCode(), actual.Geocerca.Id);
                                if (entrada.HasValue)
                                {
                                    var inicio = entrada.Value;

                                    if (ticket != null && ticket.FechaTicket.HasValue && ticket.FechaTicket.Value > inicio)
                                        inicio = ticket.FechaTicket.Value;
                                    if (distri != null && distri.InicioReal.HasValue && distri.InicioReal.Value > inicio)
                                        inicio = distri.InicioReal.Value;

                                    t.Restart();
                                    var ultimaAlarma = daoFactory.LogMensajeDAO.GetLastGeoRefferenceEventDate(vehiculo, MessageCode.PermanenciaEnGeocercaExcedidaEnCicloLogistico.GetMessageCode(), actual.Geocerca.Id, entrada.Value);
                                    if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", vehiculo.Dispositivo.Id, String.Format("GeocercaManager.ProcessLogMensajeDAO.GetLastGeoRefferenceEventDate 2 ({0} secs)", t.getTimeElapsed().TotalSeconds));
                                    if (!ultimaAlarma.HasValue)
                                    {
                                        var tiempoActual = position.Date.Subtract(inicio);
                                        if (tiempoActual.TotalMinutes > actual.Geocerca.MaximaPermanenciaEntrega)
                                        {
                                            actual.PosicionInicioExceso = position;
                                            estadoActual.Eventos.Add(new GeocercaEvent { Evento = GeocercaEventState.ExcesoPermanenciaEntrega, Estado = actual });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            estadoActual.Save();
            return estadoActual;
        }

        private static object GetLock(int vehicle)
        {
            lock (LocksByVehicle)
            {
                if (!LocksByVehicle.ContainsKey(vehicle)) LocksByVehicle.Add(vehicle, new object());
                return LocksByVehicle[vehicle];
            }
        }

        private static object GetLock(int empresa, int linea)
        {
            lock (LocksByEmpresaLinea)
            {
                var key = empresa + "-" + linea;
                if (!LocksByEmpresaLinea.ContainsKey(key)) LocksByEmpresaLinea.Add(key, new object());
                return LocksByEmpresaLinea[key];
            }
        }

        private static bool IsInside(Geocerca geocerca, double latitud, double longitud)
        {
            var point = new PointF((float)longitud, (float)latitud);
            return geocerca.IsInBounds(point) && geocerca.Contains(latitud, longitud);
        }

        private static QtreeNode GetQtree(Coche vehiculo)
        {
            var empresa = vehiculo.Empresa != null ? vehiculo.Empresa.Id : -1;
            var linea = vehiculo.Linea != null ? vehiculo.Linea.Id : -1;

            return GetQtree(empresa, linea);
        }

        public static void GetQtreeTest(int empresa, int linea)
        {
            var t = new TimeElapsed();
            GetQtree(empresa, linea);
            var ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Trace("DispatcherLock", string.Format("GetQtree({0},{1}): {2} segundos", empresa, linea, ts));
        }

        private static QtreeNode GetQtree(int empresa, int linea)
        {
            var lastMod = ReferenciaGeograficaDAO.GetLastUpdate(empresa, linea);
            var key = GetQtreeKey(empresa, linea);

            var root = Qtrees.ContainsKey(key) ? Qtrees[key] : null;

            if (root != null && lastMod < root.LastUpdate)
            {
                return root;
            }
            
            if (Monitor.TryEnter(Qtrees))
            {
                try
                {
                    STrace.Error("ResetQtree", string.Format("qtree UPDATE ---> Empresa: {0} - Linea: {1}", empresa, linea));
               
                    using (var transaction = SmartTransaction.BeginTransaction())
                    {
                        var daoFactory = new DAOFactory();
                        var geocercas = daoFactory.ReferenciaGeograficaDAO.GetGeocercasFor(empresa, linea);
                        transaction.Commit();

                        var keyToRemove = string.Empty;
                        if (root != null) keyToRemove = key;
                        root = new QtreeNode();

                        foreach (var geocerca in geocercas)
                        {
                            root.AddValue(geocerca);
                        }

                        if (keyToRemove != string.Empty) Qtrees.Remove(keyToRemove);
                        Qtrees.Add(key, root);
                    }

                    STrace.Trace("DispatcherLock", string.Format("qtree NEW ---> {0} | {1}", empresa, linea));
                }
                finally
                {
                    Monitor.Exit(Qtrees);
                }
            }
            else
            {
                STrace.Trace("DispatcherLock", string.Format("qtree OLD ---> {0} | {1}", empresa, linea));
            }

            return root;

         
        }

        public static string GetQtreeKey(int empresa, int linea)
        {
            return string.Concat(empresa, ":", linea);
        }
    }
}
