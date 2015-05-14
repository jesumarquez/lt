using System;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Scheduler.Tasks.InicioAutomatico
{
    public class Task : BaseTask
    {
        private const string ComponentName = "Inicio Automático de Tareas Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var desde = DateTime.UtcNow;
            var hasta = desde.AddMinutes(1);
            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas)
            {
                var centrosDeCosto = DaoFactory.CentroDeCostosDAO.GetList(new[] { empresa.Id }, new[] { -1 }, new[] { -1 });

                foreach (var centroDeCosto in centrosDeCosto)
                {
                    if (centroDeCosto.InicioAutomatico)
                    {
                        STrace.Trace(ComponentName, string.Format("Inicio Automático habilitado para: {0} - ({1})", centroDeCosto.Descripcion, empresa.RazonSocial));
                        var tareas = DaoFactory.ViajeDistribucionDAO.GetList(new[] { empresa.Id },
                                                                             new[] { -1 }, // LINEAS
                                                                             new[] { -1 }, // TRANSPORTISTAS
                                                                             new[] { -1 }, // DEPARTAMENTOS
                                                                             new[] { centroDeCosto.Id },
                                                                             new[] { -1 }, // SUBCENTROS DE COSTO
                                                                             new[] { -1 }, // VEHICULOS
                                                                             new[] { (int)ViajeDistribucion.Estados.Pendiente},
                                                                             desde,
                                                                             hasta)
                                                                    .Where(v => v.Vehiculo != null);

                        STrace.Trace(ComponentName, string.Format("Cantidad de tareas pendientes: {0}", tareas.Count()));

                        foreach (var viaje in tareas)
                        {
                            var evento = new InitEvent(DateTime.UtcNow);
                            var ciclo = new CicloLogisticoDistribucion(viaje, DaoFactory, new MessageSaver(DaoFactory));
                            ciclo.ProcessEvent(evento);
                            STrace.Trace(ComponentName, string.Format("Viaje Iniciado - Código: {0} - Id: {1} ", viaje.Codigo, viaje.Id));
                        }
                    }
                }
            }

            STrace.Trace(ComponentName, "Fin de la tarea");
        }
    }
}
