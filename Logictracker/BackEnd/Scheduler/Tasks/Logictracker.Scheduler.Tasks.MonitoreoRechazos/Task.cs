using System;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Scheduler.Tasks.MonitoreoRechazos
{
    public class Task : BaseTask
    {
        private const string ComponentName = "Monitoreo Rechazos Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas.Where(empresa => empresa.MonitoreoRechazos))
            {
                STrace.Trace(ComponentName, string.Format("Monitoreo Rechazos habilitado para: {0}", empresa.RazonSocial));

                var activos = DaoFactory.TicketRechazoDAO.GetActivos(empresa.Id);

                STrace.Trace(ComponentName, string.Format("Activos: {0}", activos.Count()));

                foreach (var rechazo in activos)
                {
                    var tiempo = DateTime.UtcNow.Subtract(rechazo.FechaHoraEstado).TotalMinutes;
                    switch (rechazo.UltimoEstado)
                    {
                        case TicketRechazo.Estado.Notificado1:
                            STrace.Trace(ComponentName, string.Format("Último estado: Notificado 1 - Minutos: {0}", tiempo));

                            if (tiempo > 15 && rechazo.SupervisorVenta != null)
                            {
                                rechazo.ChangeEstado(TicketRechazo.Estado.Notificado2, "Informe a Supervisor de Ventas", rechazo.SupervisorVenta);
                                DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);

                                var coche = DaoFactory.CocheDAO.FindByChofer(rechazo.SupervisorVenta.Id);
                                if (coche != null)
                                {
                                    var mensajeVo = DaoFactory.MensajeDAO.GetByCodigo(TicketRechazo.GetCodigoMotivo(rechazo.Motivo), coche.Empresa, coche.Linea);
                                    var mensaje = DaoFactory.MensajeDAO.FindById(mensajeVo.Id);
                                    EnviaMensaje(coche, mensaje, rechazo, rechazo.SupervisorVenta);
                                }
                            }
                            break;
                        case TicketRechazo.Estado.Notificado2:
                            STrace.Trace(ComponentName, string.Format("Último estado: Notificado 2 - Minutos: {0}", tiempo));

                            if (tiempo > 15 && rechazo.SupervisorRuta != null)
                            {
                                rechazo.ChangeEstado(TicketRechazo.Estado.Notificado3, "Informe a Jefe de Ventas", rechazo.SupervisorRuta);
                                DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);

                                var coche = DaoFactory.CocheDAO.FindByChofer(rechazo.SupervisorRuta.Id);
                                if (coche != null)
                                {
                                    var mensajeVo = DaoFactory.MensajeDAO.GetByCodigo(TicketRechazo.GetCodigoMotivo(rechazo.Motivo), coche.Empresa, coche.Linea);
                                    var mensaje = DaoFactory.MensajeDAO.FindById(mensajeVo.Id);
                                    EnviaMensaje(coche, mensaje, rechazo, rechazo.SupervisorRuta);
                                }
                            }
                            break;
                        case TicketRechazo.Estado.Notificado3:
                            STrace.Trace(ComponentName, string.Format("Último estado: Notificado 3 - Horas: {0}", tiempo / 60));

                            if (tiempo > 360 && rechazo.SupervisorRuta != null)
                            {
                                rechazo.ChangeEstado(TicketRechazo.Estado.NoResuelta, "Rechazo no resuelto", rechazo.SupervisorRuta);
                                DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                            }
                            break;
                    }
                }
            }

            STrace.Trace(ComponentName, "Fin de la tarea");
        }

        private void EnviaMensaje(Coche coche, Mensaje mensaje, TicketRechazo rechazo, Empleado empleado)
        {
            if (coche == null || mensaje == null) return;

            var lastPosition = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);

            var newEvent = new LogMensaje
            {
                Coche = coche,
                Chofer = empleado,
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
                Texto =
                    "INFORME DE RECHAZO NRO " + rechazo.Id + ": " + mensaje.Descripcion + " -> " +
                    rechazo.Entrega.Descripcion
            };

            DaoFactory.LogMensajeDAO.Save(newEvent);
        }
    }
}
