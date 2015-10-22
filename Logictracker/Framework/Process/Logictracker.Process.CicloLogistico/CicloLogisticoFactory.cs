using System;
using System.Collections.Generic;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using Logictracker.AVL.Messages;

namespace Logictracker.Process.CicloLogistico
{
    public static class CicloLogisticoFactory
    {
        /// <summary>
        /// List of automatic service message codes.
        /// </summary>
        private static readonly List<string> CodigosAutomaticos = new List<string>
                                                                {
                                                                    MessageCode.TareaRealizada.GetMessageCode(),
                                                                    MessageCode.TareaNoRealizada.GetMessageCode(),
                                                                    MessageCode.InsideGeoRefference.GetMessageCode(),
                                                                    MessageCode.OutsideGeoRefference.GetMessageCode(),
                                                                    MessageCode.TolvaActivated.GetMessageCode(),
                                                                    MessageCode.TolvaDeactivated.GetMessageCode(),
                                                                    MessageCode.MixerStopped.GetMessageCode(),
                                                                    MessageCode.MixerClockwise.GetMessageCode(),
                                                                    MessageCode.MixerClockwiseFast.GetMessageCode(),
                                                                    MessageCode.MixerClockwiseSlow.GetMessageCode(),
                                                                    MessageCode.MixerCounterClockwise.GetMessageCode(),
                                                                    MessageCode.MixerCounterClockwiseFast.GetMessageCode(),
                                                                    MessageCode.MixerCounterClockwiseSlow.GetMessageCode(),
                                                                    MessageCode.GarminStopStatusActive.GetMessageCode(),
                                                                    MessageCode.GarminStopStatusDeleted.GetMessageCode(),
                                                                    MessageCode.GarminStopStatusDone.GetMessageCode(),
                                                                    MessageCode.GarminStopStatusReadInactive.GetMessageCode(),
                                                                    MessageCode.GarminStopStatusUnreadInactive.GetMessageCode(),
                                                                    MessageCode.GarminTextMessageCannedResponse.GetMessageCode(),
                                                                    MessageCode.RouteStatusCancelled.GetMessageCode(),
                                                                };

        public static ICicloLogistico GetCiclo(Coche coche, IMessageSaver messageSaver)
        {
            var daoFactory = new DAOFactory();
            return GetCiclo(coche, messageSaver, daoFactory);
        }

        private static ICicloLogistico GetCiclo(Coche coche, IMessageSaver messageSaver, DAOFactory daoFactory)
        {
            if (coche.Dispositivo == null) return null;

            var distribucion = daoFactory.ViajeDistribucionDAO.FindEnCurso(coche);

            if (distribucion != null) return new CicloLogisticoDistribucion(distribucion, daoFactory, messageSaver);

            var ticket = daoFactory.TicketDAO.FindEnCurso(coche.Dispositivo);

            if (ticket != null) return new CicloLogisticoHormigon(ticket, daoFactory, messageSaver);

            return null;
        }

        public static bool IsAutomaticCode(string code)
        {
            int cod;
            var manual = int.TryParse(code, out cod) && cod > 0 && cod < 20;
            return manual || CodigosAutomaticos.Contains(code);
        }

        public static void Process(DAOFactory daoFactory, string codigo, Coche vehiculo, GPSPoint point, IMessage message, bool ignoreMessages, Empleado chofer)
        {
            try
            {
                // Si no es uno de los codigos que cambian estados automáticos, salgo directamente;))
                if (!IsAutomaticCode(codigo)) return;

                var evento = EventFactory.GetEvent(daoFactory, point, codigo, null, message, vehiculo, chofer);
                if (evento == null) return;

                Process(vehiculo, evento, ignoreMessages);
            }
            catch (Exception ex)
            {
                STrace.Exception(typeof(CicloLogisticoFactory).FullName, ex, "Code: " + (codigo ?? "null") + " vehicle: " + (vehiculo == null ? "null" : vehiculo.Id.ToString("#0")) + " message:" + (message == null ? "null" : message.ToString()) + " gpspoint:" + (point == null ? "null" : point.ToString()));
            }
        }
        public static void Process(Coche vehiculo, GPSPoint position, bool ignoreMessages)
        {
            try
            {
                var evento = new PositionEvent(position.Date, position.Lat, position.Lon);
                Process(vehiculo, evento, ignoreMessages);
            }
            catch (Exception ex)
            {
                STrace.Exception(typeof(CicloLogisticoFactory).FullName, ex);
            }
        }
        public static void Process(Coche vehiculo, IEvent data, bool ignoreMessages)
        {
            try
            {
                if(vehiculo.Dispositivo == null) return;

                var daoFactory = new DAOFactory();
                var messageSaver = ignoreMessages ? null : new MessageSaver(daoFactory);

                ICicloLogistico ciclo = null;
                var garmin = data as GarminEvent;
                if (garmin != null && garmin.DetailId > 0)
                {
                    var detailId = Convert.ToInt32(garmin.DetailId);

                    var entrega = daoFactory.EntregaDistribucionDAO.FindById(detailId);
                    if (entrega == null)
                    {
                        STrace.Error(typeof(CicloLogisticoFactory).FullName, detailId, "No se ha encontrado una distribucion para el vehiculo (" + garmin.DetailId.ToString("#0") + ")");
                    }
                    else
                    {
                        if (entrega.Viaje == null)
                        {
                            STrace.Error(typeof(CicloLogisticoFactory).FullName, detailId, "No se ha encontrado el ticket para la entrega (" + entrega.Id + ")");
                        }
                        else
                        {
                            var ticket = entrega.Viaje;
                            ciclo = new CicloLogisticoDistribucion(ticket, daoFactory, messageSaver);
                        }
                    }
                }
                else
                {
                    ciclo = GetCiclo(vehiculo, messageSaver, daoFactory);    
                }
                
                if (ciclo == null)
                {
                    return;
                }
                ciclo.ProcessEvent(data);
            }
            catch (Exception ex)
            {
                STrace.Exception(typeof(CicloLogisticoFactory).FullName, ex);
            }
        }

        public static void ProcessEstadoLogistico(Coche vehiculo, Event evento)
        {
            try
            {
                if (vehiculo.Dispositivo == null) return;

                var daoFactory = new DAOFactory();
                var distribucion = daoFactory.ViajeDistribucionDAO.FindEnCurso(vehiculo);

                if (distribucion != null)
                {
                    var messageSaver = new MessageSaver(daoFactory);
                    CicloLogisticoDistribucion ciclo = null;
                    ciclo = new CicloLogisticoDistribucion(distribucion, daoFactory, messageSaver);

                    if (ciclo != null) ciclo.ProcessEstadoLogistico(evento);
                }
            }
            catch (Exception ex)
            {
                STrace.Exception(typeof(CicloLogisticoFactory).FullName, ex);
            }
        }
    }
}
