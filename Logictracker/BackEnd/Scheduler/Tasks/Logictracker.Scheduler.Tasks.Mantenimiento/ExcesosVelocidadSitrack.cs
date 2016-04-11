using System;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Messages.Saver;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Messaging;
using Logictracker.Utils;
using Logictracker.Process.Geofences;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class ExcesosVelocidadSitrack : BaseTask
    {
        private const int IdTipoDispositivoSitrack = 49;

        protected override void OnExecute(Timer timer)
        {
            var start = DateTime.UtcNow;

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(emp => emp.GeneraInfraccionesSitrack);
            
            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));
            
            if (!empresas.Any()) return;

            try
            {
                var vehiculos = DaoFactory.CocheDAO.FindList(empresas.Select(emp => emp.Id), new[] { -1 })
                                                   .Where(c => c.Dispositivo != null
                                                            && c.Interno.Trim() != "(Generico)"
                                                            && c.Dispositivo.TipoDispositivo.Id == IdTipoDispositivoSitrack);

                var vehiculosPendientes = vehiculos.Count();
                STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));


                
                foreach (var vehiculo in vehiculos)
                {
                    STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}", vehiculo.Id));

                    var MensajesFin = DaoFactory.LogMensajeDAO.GetByVehicleAndCode(vehiculo.Id, "9009", DateTime.Today.AddDays(-1).AddHours(3), DateTime.Today.AddHours(3), 1);

                    STrace.Trace(GetType().FullName, string.Format("Infracciones a procesar: {0}", MensajesFin.Count));

                    var i = 1;

                    foreach (var msg in MensajesFin)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando infraccion: {0}/{1}", i, MensajesFin.Count));
                        var lastInicio = DaoFactory.LogMensajeDAO.GetLastByVehicleAndCodes(vehiculo.Id, new[] { "9008", "9009" }, msg.Fecha.AddDays(-1), msg.Fecha.AddSeconds(-1), 1);

                        
                        if (lastInicio != null)
                        {

                            if (lastInicio.Mensaje.Codigo == "9009")
                            {
                                continue;
                            }

                            var msgSever = new MessageSaver(DaoFactory);

                            var inicio = new GPSPoint();
                            inicio.Date = lastInicio.Fecha;
                            inicio.DeviceId = vehiculo.Dispositivo.Id;
                            inicio.Lat = (float)lastInicio.Latitud;
                            inicio.Lon = (float)lastInicio.Longitud;

                            var fin = new GPSPoint();
                            fin.Date = msg.Fecha;
                            fin.DeviceId = vehiculo.Dispositivo.Id;
                            fin.Lat = (float)msg.Latitud;
                            fin.Lon = (float)msg.Longitud;

                            var duracion = msg.Fecha.Subtract(lastInicio.Fecha).TotalSeconds;

                            if (duracion < 5)
                            {
                                continue;
                            }
                            
                            var chofer = vehiculo.Chofer;
                            var velocidadPermitida = Convert.ToInt32(80);
                            var posicionesEntreFechas = DaoFactory.LogPosicionDAO.GetPositionsBetweenDates(vehiculo.Id, inicio.Date, fin.Date);

                            var velocidadAlcanzada = 0;
                            foreach (var pos in posicionesEntreFechas)  
                            {
                                if (pos.Velocidad > velocidadAlcanzada)
                                {
                                    velocidadAlcanzada = pos.Velocidad;
                                }
                            }
                            
                            var texto = String.Format(": Permitida {0} - Alcanzada {1}", velocidadPermitida, velocidadAlcanzada);

                            var evento = MessageSaver.Save(null, MessageCode.SpeedingTicket.GetMessageCode(), vehiculo.Dispositivo, vehiculo,vehiculo.Chofer, inicio.Date, inicio, fin, texto, velocidadPermitida, velocidadAlcanzada, null, null);

                            var infraccion = new Infraccion
                            {
                                Vehiculo = vehiculo,
                                Alcanzado = velocidadAlcanzada,
                                CodigoInfraccion = Infraccion.Codigos.ExcesoVelocidad,
                                Empleado = evento.Chofer,
                                Fecha = inicio.Date,
                                Latitud = inicio.Lat,
                                Longitud = inicio.Lon,
                                FechaFin = msg.Fecha,
                                LatitudFin = msg.Latitud,
                                LongitudFin = msg.Longitud,
                                Permitido = velocidadPermitida,
                                Zona = null,
                                FechaAlta = DateTime.UtcNow
                            };

                            DaoFactory.InfraccionDAO.Save(infraccion);

                        }
                        
                        i++;
                    }

                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", --vehiculosPendientes));
                }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");

                var end = DateTime.UtcNow;
                var duration = end.Subtract(start).TotalMinutes;
                DaoFactory.DataMartsLogDAO.SaveNewLog(start, end, duration, DataMartsLog.Moludos.ExcesoVelocidadSitrack, "Exceso Velocidad Sitrack finalizado exitosamente");
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
            finally
            {
                ClearData();
            }
        }

        
        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
