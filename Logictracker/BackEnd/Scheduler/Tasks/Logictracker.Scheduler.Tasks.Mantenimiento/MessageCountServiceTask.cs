using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class MessageCountServiceTask : BaseTask
    {
        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(GetType().FullName, "Iniciando MessageCount...");

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(emp => emp.MessageCountEnabled);

            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));
            if (!empresas.Any()) return;

            var te = new TimeElapsed();

            var desde = DateTime.UtcNow.Date.AddDays(-1).AddHours(3);
            var hasta = DateTime.UtcNow.Date.AddHours(3);

            try
            {
                foreach (var empresa in empresas)
                {
                    var maxCant = empresa.MessageCountValue;
                    var vehiculos = DaoFactory.CocheDAO.FindList(new[] {empresa.Id}, new[] { -1 })
                                                       .Where(c => c.Dispositivo != null);
                    var vehiculosPendientes = vehiculos.Count();
                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                    foreach (var vehiculo in vehiculos)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}", vehiculo.Id));

                        var msgs = DaoFactory.LogMensajeDAO.Count(vehiculo.Id, desde, hasta);

                        if (msgs > maxCant)
                        {
                            STrace.Trace(GetType().FullName, string.Format("Vehículo {0} con {1} mensajes", vehiculo.Id, msgs));

                            var parametros = new[]
                                 {
                                     vehiculo.Empresa != null ? vehiculo.Empresa.RazonSocial : string.Empty,
                                     vehiculo.Linea != null ? vehiculo.Linea.Descripcion : string.Empty,
                                     vehiculo.Interno,
                                     vehiculo.Patente,
                                     desde.ToString("dd/MM/yyyy HH:mm"),
                                     hasta.ToString("dd/MM/yyyy HH:mm"),
                                     msgs.ToString("#0")
                                 };
                            SendMail(parametros);
                        }
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

        private void SendMail(IList<string> parametros)
        {
            var configFile = Config.Mailing.MessageCountMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = "Exceso de mensajes por móvil: " + parametros[2] + " (" + parametros[0] + ")";
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
