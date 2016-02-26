using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using System;
using System.Linq;

namespace Logictracker.Scheduler.Tasks.GeneracionTareas
{
    public class ControlDescansoTask : BaseTask
    {
        private const string ComponentName = "Control de Descanso Task";

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

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(e => e.ControlaDescanso);

            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));

            try
            {
                foreach (var empresa in empresas)
	            {
                    STrace.Trace(GetType().FullName, string.Format("Procesando Empresa: {0}({1})", empresa.RazonSocial, empresa.Id));

		            var vehiculos = DaoFactory.CocheDAO.FindList(new[]{empresa.Id}, new[] { -1 })
                                                       .Where(c => c.Dispositivo != null);

                    var vehiculosPendientes = vehiculos.Count();
                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                    foreach (var vehiculo in vehiculos)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}({1})", vehiculo.Interno, vehiculo.Id));

                        var detenciones = DaoFactory.LogMensajeDAO.GetByVehicleAndCode(vehiculo.Id, MessageCode.StoppedEvent.GetMessageCode(), Desde, Hasta, 1).Where(e => e.IdPuntoDeInteres.HasValue);

                        var mayoresa3horas = detenciones.Where(d => d.Duracion > new TimeSpan(3,0,0).TotalSeconds);
                        var mayoresa10min = detenciones.Where(d => d.Duracion < new TimeSpan(3,0,0).TotalSeconds && d.Duracion > new TimeSpan(0,10,0).TotalSeconds);
                        STrace.Trace(GetType().FullName, string.Format("Detenciones a procesar: {0}", detenciones.Count()));

                        var i = 1;
                        var enInfraccion = false;
                        foreach (var detencion in mayoresa3horas)
                        {
                            var anterior = DaoFactory.LogMensajeDAO.GetLastByVehicleAndCode(vehiculo.Id, MessageCode.StoppedEvent.GetMessageCode(), detencion.Fecha, detencion.Fecha.AddDays(-1), 1);
                            if (anterior != null)
                            {
                                var tiempoEntreDetenciones = detencion.Fecha.Subtract(anterior.FechaFin.Value);
                                if (tiempoEntreDetenciones.Hours > 8)
                                {

                                }
                            }
                        }
                    
                        STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", --vehiculosPendientes));
                    }
	            }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");
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
