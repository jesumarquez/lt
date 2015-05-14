using System;
using System.Linq;
using Logictracker.Cache;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Interfaces.AssistCargo;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

namespace Logictracker.Scheduler.Tasks.AssistCargo
{
    public class Task : BaseTask
    {
		private const String CacheKey = "AssistCargoLastSync";
		private const String CacheKeyLastDate = "AssistCargoLastSyncDate";

        #region Overrides of BaseTask

        protected override void OnExecute(Timer timer)
        {
            var service = new Service();

            ProcessCommands(service);

            var empresas = DaoFactory.EmpresaDAO.FindList()
                .Where(e => e.FrecuenciaReporte > 0);

            foreach (var empresa in empresas)
            {
                var coches = DaoFactory.CocheDAO.GetList(new[] {empresa.Id}, new[] {-1})
                    .Where(c => c.ReportaAssistCargo);
                foreach (var coche in coches)
                {
                    try
                    {
                        if (coche.KeyExists(CacheKey))
                        {
                            var lastSync = (DateTime) coche.Retrieve<object>(CacheKey);
                            STrace.Debug(GetType().FullName, String.Format("AssistCargo LastSync: {0}: {1} | Frecuencia: {2}", coche.Patente, lastSync, empresa.FrecuenciaReporte));
                            if (lastSync.AddMinutes(empresa.FrecuenciaReporte) > DateTime.UtcNow) continue;
                        }
                        else
                        {
                            STrace.Debug(GetType().FullName, String.Format("AssistCargo NotSync: {0} | Frecuencia: {1}", coche.Patente, empresa.FrecuenciaReporte));
                        }

                        var lastPosition = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);

                        if (coche.KeyExists(CacheKeyLastDate))
                        {
							var lastSyncDate = (String)coche.Retrieve<object>(CacheKeyLastDate);
                            if (lastPosition.FechaMensaje.ToString("yyyyMMddHHmmss") == lastSyncDate)
                            {
                                STrace.Debug(GetType().FullName, String.Format("AssistCargo: {0} | Ya se envio la ultima posicion ({1})", coche.Patente, lastSyncDate));
                                continue;
                            }
                        }
                        
                        var result = service.ReportPosition(coche, lastPosition);
                        STrace.Debug(GetType().FullName, String.Format("AssistCargo Event: {0} -> P: {1}", coche.Patente, result));
                        if (result >= 0)
                        {
                            coche.Store(CacheKey, DateTime.UtcNow);
                            coche.Store(CacheKeyLastDate, lastPosition.FechaMensaje.ToString("yyyyMMddHHmmss"));
                        }
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(GetType().FullName, ex);
                    }
                }
            }
        }


        #endregion

        private void ProcessCommands(Service service)
        {
            try
            {
                var command = service.ObtenerComando();
                STrace.Error(GetType().FullName, String.Format("AssistCargo.ProcessCommands: Obteniendo Comando para : {0}", (command == null?"NONE":command.Code)));

                if(command == null) return;
                command.Code = command.Code.Trim();

                var coche = DaoFactory.CocheDAO.FindByPatente(-1, command.Dominio);
                if (coche == null)
                {
                    service.EstadoComando(command.Id, "No se encontro un vehiculo con patente " + command.Dominio);
                    service.ComandoProcesado(command.Id, false);
                    STrace.Error(GetType().FullName, String.Format("AssistCargo.ProcessCommands: No se encontro un vehiculo con patente {0}", command.Dominio));
                    return;
                }
                if (coche.Dispositivo == null)
                {
                    service.EstadoComando(command.Id, "El vehiculo con patente " + command.Dominio +" no tiene un Dispositivo asignado");
                    service.ComandoProcesado(command.Id, false);
                    STrace.Error(GetType().FullName, String.Format("AssistCargo.ProcessCommands: El vehiculo con patente {0} no tiene un Dispositivo asignado", command.Dominio));
                    return;
                }
                if(!coche.ReportaAssistCargo)
                {
                    service.EstadoComando(command.Id, "El vehiculo con patente " + command.Dominio + " no tiene activada la interfaz con AssistCargo");
                    service.ComandoProcesado(command.Id, false);
                    STrace.Error(GetType().FullName, String.Format("AssistCargo.ProcessCommands: El vehiculo con patente {0} no tiene activada la interfaz con AssistCargo", command.Dominio));
                    return;
                }

                //var sender = new Sender(coche.Dispositivo.TipoDispositivo.ColaDeComandos);
                
                bool sent;
                if (command.Code == Config.AssistCargo.AssistCargoDisableFuelCode)
                {
                    //sent = sender.SendDisableFuel(coche.Dispositivo.Id, false, "AssistCargo", command.Id);

                    sent = MessageSender.CreateDisableFuel(coche.Dispositivo, new MessageSaver(DaoFactory))
                        .AddInmediately(false)
                        .AddTrackingId(command.Id)
                        .AddTrackingExtraData("AssistCargo")
                        .Send();
                }
                else if (command.Code == Config.AssistCargo.AssistCargoDisableFuelInmediatelyCode)
                {
                    //sent = sender.SendDisableFuel(coche.Dispositivo.Id, true, "AssistCargo", command.Id);

                    sent = MessageSender.CreateDisableFuel(coche.Dispositivo, new MessageSaver(DaoFactory))
                        .AddInmediately(true)
                        .AddTrackingId(command.Id)
                        .AddTrackingExtraData("AssistCargo")
                        .Send();
                }
                else if (command.Code == Config.AssistCargo.AssistCargoEnableFuelCode)
                {
                    //sent = sender.SendEnableFuel(coche.Dispositivo.Id, "AssistCargo", command.Id);

                    sent = MessageSender.CreateEnableFuel(coche.Dispositivo, new MessageSaver(DaoFactory))
                        .AddTrackingId(command.Id)
                        .AddTrackingExtraData("AssistCargo")
                        .Send();
                }
                else
                {
                    service.EstadoComando(command.Id, "No se encontro un comando con codigo '" + command.Code + "'.");
                    service.ComandoProcesado(command.Id, false);
                    STrace.Error(GetType().FullName, String.Format("AssistCargo.ProcessCommands: No se encontro un comando con codigo '{0}'. Disponibles: '{1}','{2}','{3}'", command.Code, Config.AssistCargo.AssistCargoDisableFuelCode, Config.AssistCargo.AssistCargoDisableFuelInmediatelyCode, Config.AssistCargo.AssistCargoEnableFuelCode));
                    return;
                }

                service.EstadoComando(command.Id, sent ? "Enviado al Gateway" : "No se pudo enviar al Gateway");

                if (!sent)
                {
                    service.ComandoProcesado(command.Id, false);
                    STrace.Error(GetType().FullName, String.Format("AssistCargo.ProcessCommands: El comando {0} ({1}) para el vehiculo con patente {2} no pudo ser procesado", command.Id, command.Code, command.Dominio));
                }
            }
            catch (Exception ex)
            {
				STrace.Exception(GetType().FullName, ex);
            }
        }
    }
}
