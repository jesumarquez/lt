using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.Messages;
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

                    STrace.Trace(ComponentName, string.Format("Activos: {0}", activos.Count()));

                    foreach (var rechazo in activos)
                    {                        
                        switch (rechazo.UltimoEstado)
                        {
                            case TicketRechazo.Estado.Notificado1:
                                var tiempo = DateTime.UtcNow.Subtract(rechazo.FechaHoraEstado).TotalMinutes;
                                STrace.Trace(ComponentName, string.Format("Último estado: Notificado 1 - Minutos: {0}", tiempo));                                

                                if (tiempo > 15)
                                {
                                    rechazo.ChangeEstado(TicketRechazo.Estado.Notificado2, "Informe a Supervisor de Ventas", rechazo.SupervisorVenta);
                                    DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);

                                    var coche = DaoFactory.CocheDAO.FindByChofer(rechazo.SupervisorVenta.Id);
                                    var mensajeVO = DaoFactory.MensajeDAO.GetByCodigo(TicketRechazo.GetCodigoMotivo(rechazo.Motivo), coche.Empresa, coche.Linea);
                                    var mensaje = DaoFactory.MensajeDAO.FindById(mensajeVO.Id);                                    
                                    if (coche != null && mensaje != null)
                                    {
                                        var lastPosition = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);

                                        var newEvent = new LogMensaje
                                        {
                                            Coche = coche,
                                            Chofer = rechazo.SupervisorVenta,
                                            CodigoMensaje = mensaje.Codigo,
                                            Dispositivo = coche.Dispositivo,
                                            Expiracion = DateTime.UtcNow.AddDays(1),
                                            Fecha = DateTime.UtcNow,
                                            FechaAlta = DateTime.UtcNow,
                                            FechaFin = DateTime.UtcNow,
                                            IdCoche = coche.Id,
                                            Latitud = lastPosition != null ? lastPosition.Latitud : 0,
                                            LatitudFin = lastPosition != null ? lastPosition.Latitud : 0,
                                            Longitud = lastPosition != null ? lastPosition.Longitud : 0,
                                            LongitudFin = lastPosition != null ? lastPosition.Longitud : 0,
                                            Mensaje = mensaje,
                                            Texto = "INFORME DE RECHAZO NRO " + rechazo.Id + ": " + mensaje.Descripcion + " -> " + rechazo.Entrega.Descripcion                                            
                                        };

                                        DaoFactory.LogMensajeDAO.Save(newEvent);
                                    }
                                }
                                break;
                            case TicketRechazo.Estado.Notificado2:
                                var time = DateTime.UtcNow.Subtract(rechazo.FechaHoraEstado).TotalMinutes;
                                STrace.Trace(ComponentName, string.Format("Último estado: Notificado 2 - Minutos: {0}", time));

                                if (time > 15)
                                {
                                    rechazo.ChangeEstado(TicketRechazo.Estado.Notificado3, "Informe a Jefe de Ventas", rechazo.SupervisorRuta);
                                    DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);

                                    var coche = DaoFactory.CocheDAO.FindByChofer(rechazo.SupervisorRuta.Id);
                                    var mensajeVO = DaoFactory.MensajeDAO.GetByCodigo(TicketRechazo.GetCodigoMotivo(rechazo.Motivo), coche.Empresa, coche.Linea);
                                    var mensaje = DaoFactory.MensajeDAO.FindById(mensajeVO.Id);
                                    if (coche != null && mensaje != null)
                                    {
                                        var lastPosition = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);

                                        var newEvent = new LogMensaje
                                        {
                                            Coche = coche,
                                            Chofer = rechazo.SupervisorRuta,
                                            CodigoMensaje = mensaje.Codigo,
                                            Dispositivo = coche.Dispositivo,
                                            Expiracion = DateTime.UtcNow.AddDays(1),
                                            Fecha = DateTime.UtcNow,
                                            FechaAlta = DateTime.UtcNow,
                                            FechaFin = DateTime.UtcNow,
                                            IdCoche = coche.Id,
                                            Latitud = lastPosition != null ? lastPosition.Latitud : 0,
                                            LatitudFin = lastPosition != null ? lastPosition.Latitud : 0,
                                            Longitud = lastPosition != null ? lastPosition.Longitud : 0,
                                            LongitudFin = lastPosition != null ? lastPosition.Longitud : 0,
                                            Mensaje = mensaje,
                                            Texto = "INFORME DE RECHAZO NRO " + rechazo.Id + ": " + mensaje.Descripcion + " -> " + rechazo.Entrega.Descripcion
                                        };

                                        DaoFactory.LogMensajeDAO.Save(newEvent);
                                    }
                                }
                                break;
                            case TicketRechazo.Estado.Notificado3:
                                var horas = DateTime.UtcNow.Subtract(rechazo.FechaHoraEstado).TotalHours;
                                STrace.Trace(ComponentName, string.Format("Último estado: Notificado 3 - Horas: {0}", horas));

                                if (horas > 6)
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
