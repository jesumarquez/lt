using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.ValueObject.Messages;
using Logictracker.Utils;

namespace Logictracker.Messages.Saver
{
    public class M2mMessageSaver : MessageSaver, IM2mMessageSaver
    {
        public M2mMessageSaver(DAOFactory daoFctory): base(daoFctory)
        {
        }

        public void Discard(string codigo, Dispositivo dispositivo, Sensor sensor, DateTime fecha, GPSPoint inicio, GPSPoint fin, DiscardReason discardReason)
        {
            var mensaje = !String.IsNullOrEmpty(codigo) ? GetByCodigo(codigo, sensor) : null;
            DiscardEvent(mensaje, dispositivo, null, null, fecha, inicio, fin, discardReason, codigo);
        }

        public LogEvento Save(string codigo, Dispositivo dispositivo, Sensor sensor, SubEntidad subEntidad, DateTime inicio, DateTime fin, string texto)
        {
            var mensaje = !String.IsNullOrEmpty(codigo) ? GetByCodigo(codigo, sensor) : null;

            if (mensaje == null)
            {
                DiscardEvent(null, dispositivo, null, null, inicio, null, null, DiscardReason.NoMessageFound, codigo);
                return null;
            }

            var log = new LogEvento
                          {
                              Dispositivo = dispositivo,
                              Sensor = sensor,
                              SubEntidad = subEntidad,
                              Mensaje = DaoFactory.MensajeDAO.FindById(mensaje.Id),
                              Fecha = inicio,
                              FechaFin = fin,
                              Expiracion = DateTime.UtcNow.AddDays(1),
                              Estado = 0,
                              Texto = String.Concat(mensaje.Texto, ' ', texto)
                          };

            ProcessActions(log, subEntidad);

            return log;
        }

        private void ProcessActions(LogEvento log, SubEntidad subEntidad)
        {
            var appliesToAnyAction = false;

            var actions = DaoFactory.AccionDAO.FindByMensaje(log.Mensaje).Cast<Accion>().ToList();
            if (subEntidad.Empresa != null)
            {
                actions = actions.Where(a => a.Empresa == null || 
                                             a.Empresa.Id == -1 || 
                                             a.Empresa.Id == subEntidad.Empresa.Id)
                                 .ToList();
            }
            if (subEntidad.Linea != null)
            {
                actions = actions.Where(a => a.Linea == null ||
                                             a.Linea.Id == -1 ||
                                             a.Linea.Id == subEntidad.Linea.Id)
                                 .ToList();
            }
            
            var filteredActions = FilterActions(actions, log);

            foreach (var accion in filteredActions)
            {
                appliesToAnyAction = true;

                DaoFactory.RemoveFromSession(log);

                log.Id = 0;
                log.Accion = accion;

                if (accion.CambiaMensaje) log.Texto += string.Concat(" ", accion.MensajeACambiar);
                //if (accion.PideFoto) PedirFoto(log);
                if (accion.GrabaEnBase)
                {
                    DaoFactory.LogEventoDAO.Save(log);
                        DaoFactory.LogEventoDAO.Save(log);
                }
                if (accion.EsAlarmaDeMail) SendMail(log);
                if (accion.EsAlarmaSms) SendSms(log);
                if (accion.Habilita) HabilitarUsuario(log.Accion);
                if (accion.Inhabilita) InhabilitarUsuario(log.Accion);
                //if (accion.ReportarAssistCargo) ReportarAssistCargo(log, accion.CodigoAssistCargo);
            }

            if (!appliesToAnyAction)
            {
                DaoFactory.LogEventoDAO.Save(log);
                    DaoFactory.LogEventoDAO.Save(log);
            }
        }

        private static IEnumerable<Accion> FilterActions(ICollection<Accion> actions, LogEvento logEvento)
        {
            if (actions == null || actions.Count.Equals(0)) return new List<Accion>();

            return actions.Where(act => act.Mensaje != null
                                     && act.Mensaje.Id.Equals(logEvento.Mensaje.Id));
        }

        private MensajeVO GetByCodigo(String codigo, Sensor sensor)
        {
            var empresa = sensor != null && sensor.Dispositivo != null ? sensor.Dispositivo.Empresa ?? (sensor.Dispositivo.Linea != null ? sensor.Dispositivo.Linea.Empresa : null) : null;
            var linea = sensor != null && sensor.Dispositivo != null ? sensor.Dispositivo.Linea : null;

            return DaoFactory.MensajeDAO.GetByCodigo(codigo, empresa, linea);
        }

	    private static void SendSms(LogEvento log)
        {
            var parameters = log.SubEntidad != null
                                 ? new List<string>
                                       {
                                           log.SubEntidad.Linea != null? log.SubEntidad.Linea.Descripcion : log.SubEntidad.Empresa != null ? log.SubEntidad.Empresa.RazonSocial : "Sistema",
                                           log.SubEntidad.Descripcion,
                                           log.SubEntidad.ToLocalString(log.Fecha, false),
                                           log.Texto
                                       }
                                 : new List<string>
                                       {
                                           "Sistema",
                                           "(Ninguno)",
                                           string.Format("{0} {1}", log.Fecha.ToShortDateString(),log.Fecha.ToShortTimeString()),
                                           log.Texto
                                       };

            SendSms(log.Accion.DestinatariosSms, parameters);
        }

	    private static void SendMail(LogEvento log)
        {
            var link = string.Empty;

            var responsable = string.Empty;

            var parameters = log.SubEntidad != null
                                 ? new List<string>
                                       {
                                           log.SubEntidad.Linea != null ? log.SubEntidad.Linea.Descripcion : log.SubEntidad.Empresa != null ? log.SubEntidad.Empresa.RazonSocial : "Sistema",
                                           log.SubEntidad.Descripcion,
                                           responsable,
                                           log.SubEntidad.ToLocalString(log.Fecha, true),
                                           string.Empty,
                                           log.Texto,
                                           link
                                       }
                                 : new List<string>
                                       {
                                           "Sistema",
                                           "(Ninguno)",
                                           responsable,
                                           string.Format("{0} {1}", log.Fecha.ToShortDateString(),log.Fecha.ToShortTimeString()),
                                           string.Empty,
                                           log.Texto,
                                           link
                                       };

            SendMailToAllDestinations(log.Accion.DestinatariosMail, null, log.Accion.AsuntoMail, parameters);
        }
    }
}
