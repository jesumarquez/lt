using System;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class DatamartEstadoVehiculoTask : BaseTask
    {
        protected override void OnExecute(Timer timer)
        {
            var empresas = DaoFactory.EmpresaDAO.GetList().Where(emp => emp.CambiaEstado);
            
            STrace.Trace(GetType().FullName, string.Format("Procesando empresas. Cantidad: {0}", empresas.Count()));
            if (!empresas.Any()) return;

            try
            {
                var vehiculos = DaoFactory.CocheDAO.FindList(empresas.Select(emp => emp.Id), new[] {-1})
                                                   .Where(c => c.Dispositivo != null
                                                            && c.Interno.Trim() != "(Generico)");
                var vehiculosPendientes = vehiculos.Count();
                STrace.Trace(GetType().FullName, string.Format("Vehículos a procesar: {0}", vehiculosPendientes));

                foreach (var vehiculo in vehiculos)
                {
                    STrace.Trace(GetType().FullName, string.Format("Procesando vehículo: {0}", vehiculo.Id));

                    var lastPos = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(vehiculo);
                    if (lastPos == null || lastPos.FechaMensaje < DateTime.UtcNow.AddDays(-vehiculo.Empresa.CambiaEstadoDias)) 
                    {
                        var fecha = lastPos != null ? lastPos.FechaMensaje.ToString("dd/MM/yyyy HH:mm") : "NULL";
                        STrace.Trace(GetType().FullName, string.Format("Vehículo inactivo. Fecha: {0}", fecha));
                        if (vehiculo.Estado == Coche.Estados.Activo)
                        {
                            UpdateEstado(vehiculo, Coche.Estados.Revisar, fecha);
                            STrace.Trace(GetType().FullName, string.Format("Cambio de estado ACTIVO => REVISAR. ID: {0}", vehiculo.Id));
                        }
                    }
                    else
                    {
                        STrace.Trace(GetType().FullName, string.Format("Vehículo OK. Fecha: {0}", lastPos.FechaMensaje.ToString("dd/MM/yyyy HH:mm")));
                        if (vehiculo.Estado == Coche.Estados.Revisar)
                        {
                            UpdateEstado(vehiculo, Coche.Estados.Activo, lastPos.FechaMensaje.ToDisplayDateTime().ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm"));
                            STrace.Trace(GetType().FullName, string.Format("Cambio de estado REVISAR => ACTIVO. ID: {0}", vehiculo.Id));
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
                case Coche.Estados.Revisar: status = "REVISAR"; break;
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
            sender.Config.Subject = "Cambio de estado: " + parametros[4] + " " + parametros[0];
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
    }
}
