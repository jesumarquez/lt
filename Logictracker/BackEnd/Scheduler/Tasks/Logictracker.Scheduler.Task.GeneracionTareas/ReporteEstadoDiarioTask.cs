using System;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Scheduler.Tasks.GeneracionTareas
{
    public class ReporteEstadoDiarioTask : BaseTask
    {
        private const string ComponentName = "Generación de Reporte Estado Diario Task";

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

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(e => e.GeneraReporteEstadoDiario);

            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));

            try
            {
                foreach (var empresa in empresas)
	            {
                    var vehiculos = DaoFactory.CocheDAO.FindList(new[] { empresa.Id }, new[] { -1 }).Where(v => v.Dispositivo != null);

                    var vehiculosPendientes = vehiculos.Count();
                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                    foreach (var vehiculo in vehiculos)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}", vehiculo.Id));

                        var dms = DaoFactory.DatamartDAO.GetBetweenDates(vehiculo.Id, Desde, Hasta);
                        var georefBaseId = vehiculo.Linea != null && vehiculo.Linea.ReferenciaGeografica != null ? vehiculo.Linea.ReferenciaGeografica.Id : 0;

                        var estadoDiario = new EstadoDiario();
                        estadoDiario.Empresa = vehiculo.Empresa;
                        estadoDiario.Vehiculo = vehiculo;
                        estadoDiario.Fecha = Desde;
                        estadoDiario.HorasBase = georefBaseId > 0 
                                                    ? dms.Where(dm => dm.GeograficRefference != null 
                                                                   && dm.GeograficRefference.Id == georefBaseId)
                                                         .Sum(dm => dm.End.Subtract(dm.Begin).TotalHours) 
                                                    : 0.0;
                        estadoDiario.HorasTaller = dms.Where(dm => dm.GeograficRefference != null 
                                                                && dm.GeograficRefference.TipoReferenciaGeografica.EsTaller)
                                                      .Sum(dm => dm.End.Subtract(dm.Begin).TotalHours);
                        estadoDiario.HorasDetenido = dms.Sum(dm => dm.StoppedHours);
                        estadoDiario.HorasEnMarcha = dms.Sum(dm => dm.HorasMarcha);
                        estadoDiario.HorasMovimiento = dms.Sum(dm => dm.MovementHours);
                        estadoDiario.HorasSinReportar = dms.Sum(dm => dm.NoReportHours);
                        estadoDiario.Kilometros = dms.Sum(dm => dm.Kilometers);
                        estadoDiario.HorasDetenidoSinGeocerca = dms.Where(dm => dm.GeograficRefference == null)
                                                                   .Sum(dm => dm.StoppedHours);
                        estadoDiario.HorasDetenidoEnGeocerca = dms.Where(dm => dm.GeograficRefference != null 
                                                                            && dm.GeograficRefference.Id != georefBaseId 
                                                                            && !dm.GeograficRefference.TipoReferenciaGeografica.EsTaller)
                                                                  .Sum(dm => dm.StoppedHours);

                        DaoFactory.EstadoDiarioDAO.SaveOrUpdate(estadoDiario);

                        STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", --vehiculosPendientes));                    
                    }
	            }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");

                var fin = DateTime.UtcNow;
                var duration = fin.Subtract(inicio).TotalMinutes;

                DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duration, DataMartsLog.Moludos.ReporteEstadoDiario, "Reporte Estado Diario generado exitosamente");
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
