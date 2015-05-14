using System;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Utils;
using Logictracker.WindowsServices;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class ReiniciarServiciosTask : BaseTask
    {
        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(GetType().FullName, "Iniciando reinicio de servicios...");

            var te = new TimeElapsed();
            
            try
            {
                var winServices = WinService.GetLogictrackerServices();

                foreach (var service in winServices)
                {
                    STrace.Trace(GetType().FullName, "Servicio a procesar: " + service.ServiceName);
                    WinService.RestartService(service);
                    Thread.Sleep(2000);
                }
                
                var ts = te.getTimeElapsed().TotalSeconds;
                STrace.Trace(GetType().FullName, string.Format("Servicios reiniciados en {0} segundos.", ts));
            }
            catch (Exception exc)
            {
                AddError(exc);
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
