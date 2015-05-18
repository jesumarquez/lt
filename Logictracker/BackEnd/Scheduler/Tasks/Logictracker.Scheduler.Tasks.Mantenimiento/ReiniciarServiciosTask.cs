using System;
using System.ServiceProcess;
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
                    STrace.Trace(GetType().FullName, "Servicio a reiniciar: " + service.ServiceName);
                    service.Refresh();

                    var detenido = false;
                    var iniciado = false;
                    var retry = 0;
                    if (service.CanStop)
                    {
                        while (!detenido && retry < 5)
                        {
                            retry++;
                            STrace.Trace(GetType().FullName, "Deteniendo servicio: " + service.ServiceName);
                            service.Stop();
                            Thread.Sleep(5000);
                            service.Refresh();
                            if (service.Status == ServiceControllerStatus.Stopped)
                            {
                                STrace.Trace(GetType().FullName, "Servicio detenido: " + service.ServiceName);
                                detenido = true;
                                retry = 0;
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
                            }
                        }

                        service.Close();
                    }
                }
                
                var ts = te.getTimeElapsed().TotalSeconds;
                STrace.Trace(GetType().FullName, string.Format("Servicios reiniciados en {0} segundos.", ts));
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
