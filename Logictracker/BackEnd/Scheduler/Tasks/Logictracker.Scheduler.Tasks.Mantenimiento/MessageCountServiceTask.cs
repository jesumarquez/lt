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
        private DateTime StartDate
        {
            get
            {
                var startDate = GetDateTime("Desde");
                return startDate.HasValue ? startDate.Value : DateTime.Today.AddDays(-1).AddHours(3);
            }
        }
        private DateTime EndDate
        {
            get
            {
                var endDate = GetDateTime("Hasta");
                return endDate.HasValue ? endDate.Value : StartDate.AddHours(24);
            }
        }

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(GetType().FullName, "Iniciando MessageCount...");

            var empresas = DaoFactory.EmpresaDAO.GetList().Where(emp => emp.MessageCountEnabled);

            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));
            if (!empresas.Any()) return;

            var te = new TimeElapsed();

            var desde = StartDate;
            var hasta = EndDate;

            try
            {
                foreach (var empresa in empresas)
                {
                    var maxCant = empresa.MessageCountValue;
                    var vehiculos = DaoFactory.CocheDAO.FindList(new[] {empresa.Id}, new[] { -1 })
                                                       .Where(c => c.Dispositivo != null);
                    var vehiculosPendientes = vehiculos.Count();
                    STrace.Trace(GetType().FullName, string.Format("Veh�culos a procesar: {0}", vehiculosPendientes));

                    foreach (var vehiculo in vehiculos)
                    {
                        STrace.Trace(GetType().FullName, string.Format("Procesando veh�culo: {0}", vehiculo.Id));

                        var msgs = DaoFactory.LogMensajeDAO.Count(vehiculo.Id, desde, hasta);
                        var msgsAdmin = DaoFactory.LogMensajeAdminDAO.Count(vehiculo.Id, desde, hasta);
                        var total = msgs + msgsAdmin;

                        if (total > maxCant)
                        {
                            STrace.Trace(GetType().FullName, string.Format("Veh�culo {0} con {1} mensajes", vehiculo.Id, total));

                            var parametros = new[]
                                 {
                                     vehiculo.Empresa != null ? vehiculo.Empresa.RazonSocial : string.Empty,
                                     vehiculo.Linea != null ? vehiculo.Linea.Descripcion : string.Empty,
                                     vehiculo.Interno,
                                     vehiculo.Patente + " - (Id Dispositivo: " + vehiculo.Dispositivo.Id + ")",
                                     desde.ToString("dd/MM/yyyy HH:mm"),
                                     hasta.ToString("dd/MM/yyyy HH:mm"),
                                     total.ToString("#0") + " - (Mensajes Admin: " + msgsAdmin.ToString("#0") + ")"
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

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuraci�n de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = "Exceso de mensajes por m�vil: " + parametros[2] + " (" + parametros[0] + ")";
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
