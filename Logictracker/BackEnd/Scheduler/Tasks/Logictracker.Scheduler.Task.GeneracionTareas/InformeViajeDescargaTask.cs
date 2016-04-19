using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Services.Helpers;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Scheduler.Tasks.GeneracionTareas
{
    public class InformeViajeDescargaTask : BaseTask
    {
        private const string ComponentName = "Generación de Informe Viaje-Descarga Task";

        private DateTime Desde
        {
            get
            {
                var startDate = GetDateTime("Desde");
                return startDate.HasValue ? startDate.Value : DateTime.UtcNow.AddHours(-3).Date.AddDays(-1).AddHours(3);
            }
        }
        private DateTime Hasta
        {
            get
            {
                var endDate = GetDateTime("Hasta");
                return endDate.HasValue ? endDate.Value : Desde.AddHours(24);
            }
        }

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var inicio = DateTime.UtcNow;

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(e => e.GeneraInformeViajeRecarga);

            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));

            try
            {
                foreach (var empresa in empresas)
	            {
                    var vehiculos = DaoFactory.CocheDAO.FindList(new[] { empresa.Id }, new[] { -1 }).Where(v => v.Linea != null);

                    var vehiculosPendientes = vehiculos.Count();
                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                    var eventosTotales = DaoFactory.LogMensajeDAO.GetByVehiclesAndCodes(vehiculos.Select(v => v.Id).ToArray(), new[] { MessageCode.InsideGeoRefference.GetMessageCode(), MessageCode.OutsideGeoRefference.GetMessageCode() }, Desde, Hasta, 1);

                    foreach (var vehiculo in vehiculos)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}", vehiculo.Id));

                        var eventos = eventosTotales.Where(ev => ev.Coche.Id == vehiculo.Id 
                                                              && ev.IdPuntoDeInteres.HasValue 
                                                              && ev.IdPuntoDeInteres == vehiculo.Linea.ReferenciaGeografica.Id);
                        var entradas = eventos.Where(e => e.Mensaje.Codigo == MessageCode.InsideGeoRefference.GetMessageCode());
                        var salidas = eventos.Where(e => e.Mensaje.Codigo == MessageCode.OutsideGeoRefference.GetMessageCode());

                        var i = 1;
                        var viaje = 1;
                        foreach (var salida in salidas)
                        {
                            STrace.Trace(GetType().FullName, string.Format("Procesando salida: {0}/{1}", i, salidas.Count()));

                            var vuelta = entradas.Where(e => e.Fecha > salida.Fecha).FirstOrDefault();
                            if (vuelta != null)
                            {
                                var duracion = vuelta.Fecha.Subtract(salida.Fecha).TotalHours;
                                var minutosDeViaje = duracion * 60;
                                if (minutosDeViaje < vehiculo.Empresa.MinutosMinimosDeViaje)
                                    continue;

                                var informe = new InformeViajeRecarga
                                { 
                                    Empresa = vehiculo.Empresa,
                                    Linea = vehiculo.Linea,
                                    Vehiculo = vehiculo,
                                    Patente = vehiculo.Patente,
                                    Interno = vehiculo.Interno,
                                    Accion = "Viaje " + viaje++,
                                    Fecha = salida.Fecha.Date.AddHours(3),
                                    Inicio = salida.Fecha,
                                    Fin = vuelta.Fecha,
                                    Duracion = duracion
                                };

                                DaoFactory.InformeViajeRecargaDAO.SaveOrUpdate(informe);

                                var recarga = salidas.Where(s => s.Fecha > vuelta.Fecha).FirstOrDefault();
                                if (recarga != null)
                                {
                                    var rec = new InformeViajeRecarga
                                    {
                                        Empresa = vehiculo.Empresa,
                                        Linea = vehiculo.Linea,
                                        Vehiculo = vehiculo,
                                        Patente = vehiculo.Patente,
                                        Interno = vehiculo.Interno,
                                        Accion = "Recarga",
                                        Fecha = vuelta.Fecha.Date.AddHours(3),
                                        Inicio = vuelta.Fecha,
                                        Fin = recarga.Fecha,
                                        Duracion = recarga.Fecha.Subtract(vuelta.Fecha).TotalHours
                                    };

                                    DaoFactory.InformeViajeRecargaDAO.SaveOrUpdate(rec);
                                }
                            }

                            i++;
                        }
                    
                        STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", --vehiculosPendientes));                    
                    }
	            }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");

                var fin = DateTime.UtcNow;
                var duration = fin.Subtract(inicio).TotalMinutes;

                DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duration, DataMartsLog.Moludos.InformeViajeDescarga, "Generación inversa finalizada exitosamente");
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
