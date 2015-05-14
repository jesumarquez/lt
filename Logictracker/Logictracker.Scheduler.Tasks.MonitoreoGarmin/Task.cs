using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

namespace Logictracker.Scheduler.Tasks.MonitoreoGarmin
{
    public class Task : BaseTask
    {
        private const string ComponentName = "Monitoreo Garmin Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");
            
            var inicioRuta = MessageCode.CicloLogisticoIniciado.GetMessageCode();
            var envioOk = MessageCode.ValidacionRuteo.GetMessageCode();

            var ahora = DateTime.UtcNow;
            var desde = DateTime.Today;
            var hasta = DateTime.UtcNow.AddMinutes(-10);

            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas)
            {
                if (empresa.MonitoreoGarmin)
                {
                    STrace.Trace(ComponentName, string.Format("Monitoreo Garmin habilitado para: {0}", empresa.RazonSocial));
                    var vehiculos = DaoFactory.CocheDAO.FindList(new[] {empresa.Id}, new[] {-1}).Select(c => c.Id).ToList();
                    
                    var maxMonths = empresa.MesesConsultaPosiciones;
                    var eventosInicio = DaoFactory.LogMensajeDAO.GetByVehiclesAndCode(vehiculos, inicioRuta, desde, hasta, maxMonths);

                    STrace.Trace(ComponentName, string.Format("Rutas iniciadas: {0}", eventosInicio.Count()));

                    foreach (var eventoInicio in eventosInicio)
                    {
                        var confirmaciones = DaoFactory.LogMensajeDAO.GetByVehicleAndCode(eventoInicio.Coche.Id, envioOk, eventoInicio.Fecha, ahora, maxMonths);
                        if (!confirmaciones.Any())
                        {
                            var parametros = new[]
                                                 {
                                                     eventoInicio.Coche.Id.ToString("#0"),
                                                     eventoInicio.Coche.Interno,
                                                     eventoInicio.Coche.Patente,
                                                     eventoInicio.Coche.Empresa.RazonSocial,
                                                     eventoInicio.Fecha.ToString("dd/MM/yyyy HH:mm:ss"),
                                                     ahora.ToString("dd/MM/yyyy HH:mm:ss")
                                                 };
                            SendMail(parametros);
                            STrace.Trace(ComponentName, string.Format("No hay confirmaciones de envío de ruta para el vehículo: {0}", eventoInicio.Coche.Id));
                        }

                    }
                }
            }
            STrace.Trace(ComponentName, "Fin de la tarea");
        }

        private void SendMail(string[] parametros)
        {
            var configFile = Config.Mailing.MonitoreoGarminMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            var destinatarios = new List<string> { "dev@logictracker.com" };

            sender.Config.Subject = "MonitoreoGarmin: Id=" + parametros[0] + " Fecha=" + parametros[4];
            foreach (var destinatario in destinatarios)
            {
                sender.Config.ToAddress = destinatario;
                sender.SendMail(parametros);
                STrace.Trace(GetType().FullName, "Email sent to: " + destinatario);
            }
        }
    }
}
