using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class EstadoGarminTask : BaseTask
    {
        protected override void OnExecute(Timer timer)
        {
            var empresas = DaoFactory.EmpresaDAO.GetList().Where(emp => emp.CambiaEstadoGarmin);
            
            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));
            if (!empresas.Any()) return;

            try
            {
                var vehiculos = DaoFactory.CocheDAO.FindList(empresas.Select(emp => emp.Id), new[] {-1})
                                                   .Where(c => c.Dispositivo != null && c.Dispositivo.HasGarmin);
                var vehiculosPendientes = vehiculos.Count();
                STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                var codes = new[] {MessageCode.GarminOn.GetMessageCode(), MessageCode.EngineOn.GetMessageCode()};

                foreach (var vehiculo in vehiculos)
                {
                    STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}", vehiculo.Id));

                    var maxMonths = vehiculo.Empresa.MesesConsultaPosiciones;
                    var hasta = DateTime.UtcNow.Date;
                    var desde = hasta.AddDays(-vehiculo.Empresa.CambiaEstadoGarminDias);

                    var lastEngineOn = DaoFactory.LogMensajeDAO.GetLastByVehicleAndCode(vehiculo.Id, MessageCode.EngineOn.GetMessageCode(), desde, hasta, maxMonths);
                    var lastGarminOn = DaoFactory.LogMensajeDAO.GetLastByVehicleAndCode(vehiculo.Id, MessageCode.GarminOn.GetMessageCode(), desde, hasta, maxMonths);
                    
                    if (lastEngineOn != null && (lastGarminOn == null || lastGarminOn.Fecha < lastEngineOn.Fecha))
                    {
                        var fecha = lastEngineOn.Fecha.ToString("dd/MM/yyyy HH:mm");
                        STrace.Trace(GetType().FullName, string.Format("Garmin inactivo. Fecha: {0}", fecha));
                        if (vehiculo.Estado == Coche.Estados.Activo)
                        {
                            UpdateEstado(vehiculo, Coche.Estados.RevisarGarmin, fecha);
                            STrace.Trace(GetType().FullName, string.Format("Cambio de estado ACTIVO => REVISAR GARMIN. ID: {0}", vehiculo.Id));
                        }
                    }

                    STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", --vehiculosPendientes));
                }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
            finally
            {
                ClearData();
            }
        }

        private void UpdateEstado(Coche vehiculo, short estado, string fecha)
        {
            vehiculo.Estado = estado;
            DaoFactory.CocheDAO.SaveOrUpdate(vehiculo);
            var status = string.Empty;
            switch (estado)
            {
                case Coche.Estados.Activo: status = "ACTIVO"; break;
                case Coche.Estados.RevisarGarmin: status = "REVISAR GARMIN"; break;
            }

            var parametros = new[]
                                 {
                                     status,
                                     vehiculo.Empresa != null ? vehiculo.Empresa.RazonSocial : string.Empty,
                                     vehiculo.Linea != null ? vehiculo.Linea.Descripcion : string.Empty,
                                     vehiculo.Transportista != null ? vehiculo.Transportista.Descripcion : string.Empty,
                                     vehiculo.Interno + " (" + vehiculo.Patente + ")",
                                     fecha
                                 };
            SendMail(parametros);
        }

        private void SendMail(string[] parametros)
        {
            var configFile = Config.Mailing.CambioEstadoMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            var destinatarios = new List<string> { "gfernandez@logictracker.com" };
            
            sender.Config.Subject = "Cambio de estado: " + parametros[4] + " " + parametros[0];
            foreach (var destinatario in destinatarios)
            {
                sender.Config.ToAddress = destinatario;
                sender.SendMail(parametros);
                STrace.Trace(GetType().FullName, "Email sent to: " + destinatario);
            }
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
