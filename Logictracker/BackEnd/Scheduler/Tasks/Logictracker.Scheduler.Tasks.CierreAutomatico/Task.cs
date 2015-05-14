using System;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Scheduler.Tasks.CierreAutomatico
{
    public class Task : BaseTask
    {
        private const string ComponentName = "Cierre Automático de Tickets Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas)
            {
                if (empresa.CierreDistribucionAutomatico)
                {
                    STrace.Trace(ComponentName, string.Format("Cierre Automático habilitado para: {0}", empresa.RazonSocial));
                    var viajes = DaoFactory.ViajeDistribucionDAO.GetList(new[] { empresa.Id },
                                                                         new[] { -1 }, // LINEAS
                                                                         new[] { -1 }, // TRANSPORTISTAS
                                                                         new[] { -1 }, // DEPARTAMENTOS
                                                                         new[] { -1 }, // CENTROS DE COSTO
                                                                         new[] { -1 }, // SUB CENTROS DE COSTO
                                                                         new[] { -1 }, // VEHICULOS
                                                                         new[] { (int)ViajeDistribucion.Estados.EnCurso },
                                                                         DateTime.UtcNow.AddDays(-1),
                                                                         DateTime.UtcNow);

                    STrace.Trace(ComponentName, string.Format("Cantidad de tickets en curso: {0}", viajes.Count()));

                    foreach (var viaje in viajes)
                    {
                        var evento = new CloseEvent(DateTime.UtcNow);
                        var ciclo = new CicloLogisticoDistribucion(viaje, DaoFactory, new MessageSaver(DaoFactory));
                        ciclo.ProcessEvent(evento);
                        STrace.Trace(ComponentName, string.Format("Viaje Cerrado - Código: {0} - Id: {1} ", viaje.Codigo, viaje.Id));
                    }
                }
            }

            STrace.Trace(ComponentName, "Fin de la tarea");
        }
    }
}
