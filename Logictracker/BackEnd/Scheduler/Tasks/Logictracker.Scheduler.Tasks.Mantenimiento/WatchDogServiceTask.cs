using System;
using System.ServiceProcess;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Utils;
using Logictracker.WindowsServices;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class WatchDogServiceTask : BaseTask
    {
        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(GetType().FullName, "Iniciando WatchDog...");

            var serviceDescription = GetString("Service").Trim();

            if (serviceDescription.Equals(string.Empty)) return;

            var te = new TimeElapsed();
            
            try
            {
                var winServices = WinService.GetServicesToWatch(serviceDescription);

                foreach (var service in winServices)
                {
                    STrace.Trace(GetType().FullName, "Servicio a verificar: " + service.ServiceName);
                    service.Refresh();
                    
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        STrace.Trace(GetType().FullName, "Servicio detenido: " + service.ServiceName);

                        var iniciado = false;
                        var retry = 0;
                        while (!iniciado && retry < 5)
                        {
                            retry++;
                            STrace.Trace(GetType().FullName, "Iniciando servicio: " + service.ServiceName);
                            service.Start();
                            Thread.Sleep(5000);
                            service.Refresh();

                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                STrace.Trace(GetType().FullName, "Servicio iniciado: " + service.ServiceName);
                                iniciado = true;
                            }
                        }

                        service.Close();
                    }
                }
                
                var ts = te.getTimeElapsed().TotalSeconds;
                STrace.Trace(GetType().FullName, string.Format("Tarea finalizada en {0} segundos.", ts));
            }
            catch (Exception ex)
            {
                STrace.Exception(GetType().FullName, ex);
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
