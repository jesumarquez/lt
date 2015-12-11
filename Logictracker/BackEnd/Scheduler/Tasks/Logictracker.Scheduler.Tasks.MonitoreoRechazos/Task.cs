using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.Rechazos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Scheduler.Tasks.MonitoreoRechazos
{
    public class Task : BaseTask
    {
        private const string ComponentName = "Monitoreo Rechazos Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas)
            {
                if (empresa.MonitoreoRechazos)
                {
                    STrace.Trace(ComponentName, string.Format("Monitoreo Rechazos habilitado para: {0}", empresa.RazonSocial));

                    var activos = DaoFactory.TicketRechazoDAO.GetActivos(empresa.Id);
                    foreach (var rechazo in activos)
                    {                        
                        switch (rechazo.UltimoEstado)
                        {
                            case TicketRechazo.Estado.Notificado1:
                                if (rechazo.FechaHoraEstado.AddMinutes(15) < DateTime.UtcNow)
                                {
                                    rechazo.ChangeEstado(TicketRechazo.Estado.Notificado2, "Informe a Supervisor de Ventas", rechazo.SupervisorVenta);
                                    DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                                }
                                break;
                            case TicketRechazo.Estado.Notificado2:
                                if (rechazo.FechaHoraEstado.AddMinutes(15) < DateTime.UtcNow)
                                {
                                    rechazo.ChangeEstado(TicketRechazo.Estado.Notificado3, "Informe a Jefe de Ventas", rechazo.SupervisorRuta);
                                    DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                                }
                                break;
                            case TicketRechazo.Estado.Notificado3:
                                if (rechazo.FechaHoraEstado.AddHours(6) < DateTime.UtcNow)
                                {
                                    rechazo.ChangeEstado(TicketRechazo.Estado.NoResuelta, "Rechazo no resuelto", rechazo.SupervisorRuta);
                                    DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
