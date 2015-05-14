using System;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Scheduler.Tasks.ControlSalida
{
    public class Task : BaseTask
    {
        private const string ComponentName = "Control de Salida Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas)
            {
                if (empresa.ControlaInicioDistribucion)
                {
                    STrace.Trace(ComponentName, string.Format("Control de salida habilitado para: {0}", empresa.RazonSocial));
                    var tareas = DaoFactory.ViajeDistribucionDAO.GetList(new[] {empresa.Id},
                                                                         new[] {-1}, // LINEAS
                                                                         new[] {-1}, // TRANSPORTISTAS
                                                                         new[] {-1}, // DEPARTAMENTOS
                                                                         new[] {-1}, // CENTROS DE COSTO
                                                                         new[] {-1}, // SUB CENTROS DE COSTO
                                                                         new[] {-1}, // VEHICULOS
                                                                         new[] {(int) ViajeDistribucion.Estados.EnCurso},
                                                                         DateTime.Today,
                                                                         DateTime.Today.AddDays(1));
                    STrace.Trace(ComponentName, string.Format("Cantidad de tareas en curso: {0}", tareas.Count()));

                    foreach (var viaje in tareas)
                    {
                        if (DateTime.UtcNow > viaje.Inicio.AddMinutes(viaje.Umbral) 
                         && viaje.EntregasPendientesCount == viaje.EntregasTotalCount)
                        {
                            STrace.Trace(ComponentName, string.Format("Viaje Atrasado - Código: {0}", viaje.Codigo));
                            var maxMonths = viaje.Vehiculo.Empresa != null ? viaje.Vehiculo.Empresa.MesesConsultaPosiciones : 3;
                            var alarmas = DaoFactory.LogMensajeDAO.GetByVehicleAndCode(viaje.Vehiculo.Id, MessageCode.AtrasoTicket.GetMessageCode(), viaje.Inicio.AddMinutes(viaje.Umbral), DateTime.UtcNow, maxMonths);
                            if (!alarmas.Any())
                            {
                                var code = MessageCode.AtrasoTicket.GetMessageCode();
                                MessageSaver.Save(code, viaje.Vehiculo, viaje.Empleado, DateTime.UtcNow, null, " - Inicio " + viaje.Codigo);
                                STrace.Trace(ComponentName, "Evento generado");
                            }
                            else
                            {
                                STrace.Trace(ComponentName, "Ya se generó alarma");
                            }
                        }
                    }
                }
            }
            STrace.Trace(ComponentName, "Fin de la tarea");
        }
    }
}
