#region Usings

using System;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Dispatcher.Handlers.Common;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Utils;
using Logictracker.Tracker.Application.Integration;

#endregion

namespace Logictracker.Dispatcher.Handlers
{
    [FrameworkElement(XName = "TextEventsHandler", IsContainer = false)]
    public class TextEvents : DeviceBaseHandler<TextEvent>
    {
        #region Protected Methods

        protected override HandleResults OnDeviceHandleMessage(TextEvent message)
        {
            var employee = GetChofer(message.UserIdentifier);

			var pos = message.GeoPoint;
			if (pos == null)
			{
				var pp = DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(Coche);
				pos = GPSPoint.Factory(message.GetDateTime(), pp == null ? (float)0.0 : (float)pp.Latitud, pp == null ? (float)0.0 : (float)pp.Longitud);
			}

        	MessageSaver.Save(message, MessageCode.TextEvent.GetMessageCode(), Dispositivo, Coche, employee, pos.Date, pos, message.Text);

            if (Coche.Empresa.IntegrationServiceEnabled)
            {
                var text = message.Text.Trim().ToUpperInvariant();
                var ruta = DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche);
                if (ruta != null && text.Contains(Coche.Empresa.IntegrationServicePrefixConfirmation))
                {   
                    var sosTicket = DaoFactory.SosTicketDAO.FindByCodigo(ruta.Codigo);
                    if (sosTicket != null)
                    {
                        text = text.Replace(Coche.Empresa.IntegrationServicePrefixConfirmation, string.Empty);
                        var intService = new IntegrationService(DaoFactory);
                        if (sosTicket.Patente.ToUpperInvariant().Contains(text))
                            intService.ConfirmaPatente(sosTicket, true);
                        else
                            intService.ConfirmaPatente(sosTicket, false);
                    }
                }
            }

            if (Coche.Empresa.AsignoDistribucionPorMensaje)
            {
                var text = message.Text.Trim().ToUpperInvariant();
                STrace.Trace("AsignoDistribucionPorMensaje", string.Format("Text: {0}", text));
                
                var prefijo = Coche.Empresa.AsignoDistribucionPrefijoMensaje.Trim().ToUpperInvariant();
                if (text.Contains(prefijo))
                {
                    STrace.Trace("AsignoDistribucionPorMensaje", string.Format("Empresa: {0} - Coche: {1} - Msj: {2}", Coche.Empresa.Id, Coche.Id, text));

                    var mensaje = MessageSender.Create(Dispositivo, new MessageSaver(DaoFactory))
                                               .AddCommand(MessageSender.Comandos.SubmitTextMessage);

                    var viajeEnCurso = DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche);
                    if (viajeEnCurso == null)
                    {
                        var codigoRuta = DateTime.Today.ToString("yyMMdd") + "|" + text.Replace(prefijo, string.Empty).Trim();
                        var viaje = DaoFactory.ViajeDistribucionDAO.FindByCodigo(Coche.Empresa.Id, Coche.Linea.Id, codigoRuta);
                        if (viaje != null)
                        {
                            if (viaje.Estado == ViajeDistribucion.Estados.Pendiente)
                            {
                                viaje.Vehiculo = Coche;
                                DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);

                                var ciclo = new CicloLogisticoDistribucion(viaje, DaoFactory, new MessageSaver(DaoFactory));
                                var evento = new InitEvent(DateTime.UtcNow);
                                ciclo.ProcessEvent(evento);

                                mensaje = mensaje.AddMessageText("RUTA " + codigoRuta + " INICIADA");
                                STrace.Trace("AsignoDistribucionPorMensaje", string.Format("RUTA {0} INICIADA", codigoRuta));
                            }
                            else
                            {
                                mensaje = mensaje.AddMessageText("NO ES POSIBLE INICIAR LA RUTA " + codigoRuta);
                                STrace.Trace("AsignoDistribucionPorMensaje", string.Format("NO ES POSIBLE INICIAR LA RUTA {0}", codigoRuta));
                            }
                        }
                        else
                        {
                            mensaje = mensaje.AddMessageText("RUTA " + codigoRuta + " NO ENCONTRADA");
                            STrace.Trace("AsignoDistribucionPorMensaje", string.Format("RUTA {0} NO ENCONTRADA", codigoRuta));
                        }
                    }
                    else
                    {
                        mensaje = mensaje.AddMessageText("USTED YA TIENE ASIGNADA UNA RUTA INICIADA");
                        STrace.Trace("AsignoDistribucionPorMensaje", string.Format("USTED YA TIENE ASIGNADA UNA RUTA INICIADA"));
                    }
                    mensaje.Send();
                }
            }

            return HandleResults.Success;
        }

        #endregion
    }
}
