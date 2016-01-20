using System;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Tracker.Application.Integration;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Process.CicloLogistico
{
    public static class EventFactory
    {

        public static IEvent GetInitEvent()
        {
            return GetInitEvent(DateTime.UtcNow);
        }
        public static IEvent GetInitEvent(DateTime date)
        {
            return new InitEvent(date);
        }

        public static IEvent GetCloseEvent()
        {
            return GetCloseEvent(DateTime.UtcNow);
        }
        public static IEvent GetCloseEvent(DateTime date)
        {
            return new CloseEvent(date);
        }
        public static IEvent GetCloseEvent(bool informDevice)
        {
            return new CloseEvent(DateTime.UtcNow, informDevice);
        }
        public static IEvent GetCloseEvent(DateTime date, bool informDevice)
        {
            return new CloseEvent(date, informDevice);
        }

        public static IEvent GetEvent(DAOFactory daoFactory, GPSPoint inicio, string codigo, Int32? idPuntoDeInteres, IMessage message, Coche vehiculo, Empleado chofer)
        {   
            var extra = (message as IExtraData);
            var extraData = extra != null && extra.Data.Count > 1 ? extra.Data[1] : -1;
            var extraData2 = extra != null && extra.Data.Count > 2 ? extra.Data[2] : -1;
            var extraData3 = extra != null && extra.Data.Count > 3 ? extra.Data[3] : -1;
            return GetEvent(daoFactory, inicio, codigo, idPuntoDeInteres, extraData, extraData2, extraData3, vehiculo, chofer);
        }

        public static IEvent GetEvent(DAOFactory daoFactory, GPSPoint inicio, string codigo, Int32? idPuntoDeInteres, int extraData, Coche vehiculo, Empleado chofer)
        {
            return GetEvent(daoFactory, inicio, codigo, idPuntoDeInteres, extraData, -1, -1, vehiculo, chofer);
        }

        public static IEvent GetEvent(DAOFactory daoFactory, GPSPoint inicio, string codigo, Int32? idPuntoDeInteres, int extraData, int extraData2, Coche vehiculo,  Empleado chofer)
        {
            return GetEvent(daoFactory, inicio, codigo, idPuntoDeInteres, extraData, extraData2, -1, vehiculo, chofer);
        }

        public static IEvent GetEvent(DAOFactory daoFactory, GPSPoint inicio, string codigo, Int32? idPuntoDeInteres, Int64 extraData, Int64 extraData2, Int64 extraData3, Coche vehiculo, Empleado chofer)
        {
            IEvent evento = null;
            int cod;
            if (int.TryParse(codigo, out cod) && cod > 0 && cod < 20)
            {
                evento = new ManualEvent(inicio.Date, inicio.Lat, inicio.Lon, codigo);
            }
            else if (codigo == MessageCode.InsideGeoRefference.GetMessageCode() && idPuntoDeInteres.HasValue)
            {
                evento = new GeofenceEvent(inicio.Date, idPuntoDeInteres.Value, GeofenceEvent.EventoGeofence.Entrada, inicio.Lat, inicio.Lon, chofer);
            }
            else if (codigo == MessageCode.OutsideGeoRefference.GetMessageCode() && idPuntoDeInteres.HasValue)
            {
                evento = new GeofenceEvent(inicio.Date, idPuntoDeInteres.Value, GeofenceEvent.EventoGeofence.Salida, inicio.Lat, inicio.Lon, chofer);
            }
            else if (codigo == MessageCode.TolvaDeactivated.GetMessageCode())
            {
                evento = new TolvaEvent(inicio.Date, TolvaEvent.EstadoTolva.Off, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.TolvaActivated.GetMessageCode())
            {
                evento = new TolvaEvent(inicio.Date, TolvaEvent.EstadoTolva.On, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.MixerStopped.GetMessageCode())
            {
                evento = new TrompoEvent(inicio.Date, TrompoEvent.SentidoTrompo.Detenido, TrompoEvent.VelocidadTrompo.Undefined, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.MixerClockwiseFast.GetMessageCode())
            {
                evento = new TrompoEvent(inicio.Date, TrompoEvent.SentidoTrompo.HorarioDerecha, TrompoEvent.VelocidadTrompo.Fast, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.MixerClockwiseSlow.GetMessageCode())
            {
                evento = new TrompoEvent(inicio.Date, TrompoEvent.SentidoTrompo.HorarioDerecha, TrompoEvent.VelocidadTrompo.Slow, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.MixerClockwise.GetMessageCode())
            {
                evento = new TrompoEvent(inicio.Date, TrompoEvent.SentidoTrompo.HorarioDerecha, TrompoEvent.VelocidadTrompo.Undefined, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.MixerCounterClockwiseFast.GetMessageCode())
            {
                evento = new TrompoEvent(inicio.Date, TrompoEvent.SentidoTrompo.AntihorarioIzquierda, TrompoEvent.VelocidadTrompo.Fast, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.MixerCounterClockwiseSlow.GetMessageCode())
            {
                evento = new TrompoEvent(inicio.Date, TrompoEvent.SentidoTrompo.AntihorarioIzquierda, TrompoEvent.VelocidadTrompo.Slow, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.MixerCounterClockwise.GetMessageCode())
            {
                evento = new TrompoEvent(inicio.Date, TrompoEvent.SentidoTrompo.AntihorarioIzquierda, TrompoEvent.VelocidadTrompo.Undefined, inicio.Lat, inicio.Lon);
            }
            else if (codigo == MessageCode.GarminTextMessageCannedResponse.GetMessageCode())
            {
                // extraData = ID Device
                // extraData2 = ID Entrega / ID Ruta
                // extraData3 = Codigo Mensaje
                STrace.Debug(typeof(EventFactory).FullName, Convert.ToInt32(extraData), "extraData=" + extraData + " extraData2=" + extraData2 + " extraData3=" + extraData3);
                var veh = daoFactory.CocheDAO.FindMobileByDevice(Convert.ToInt32(extraData));
                
                if (veh != null && veh.Empresa.IntegrationServiceEnabled)
                {
                    var intService = new IntegrationService(daoFactory);

                    if (veh.Empresa.IntegrationServiceCodigoMensajeAceptacion == extraData3.ToString())
                    {
                        var distribucion = daoFactory.ViajeDistribucionDAO.FindById(Convert.ToInt32(extraData2));                        
                        
                        if (distribucion != null)
                        {
                            var enCurso = daoFactory.ViajeDistribucionDAO.FindEnCurso(veh);
                            if (enCurso == null)
                            {
                                intService.ResponseAsigno(distribucion, true);
                                var eventoInicio = new InitEvent(inicio.Date);
                                var ciclo = new CicloLogisticoDistribucion(distribucion, daoFactory, new MessageSaver(daoFactory));
                                ciclo.ProcessEvent(eventoInicio);
                            }
                            else
                            {
                                intService.ResponsePreasigno(distribucion, true);
                            }
                        }

                        return null;
                    }
                    else if (veh.Empresa.IntegrationServiceCodigoMensajeRechazo == extraData3.ToString())
                    {
                        var distribucion = daoFactory.ViajeDistribucionDAO.FindById(Convert.ToInt32(extraData2));
                        distribucion.Vehiculo = null;
                        daoFactory.ViajeDistribucionDAO.SaveOrUpdate(distribucion);

                        var enCurso = daoFactory.ViajeDistribucionDAO.FindEnCurso(veh);
                        if (enCurso == null)
                            intService.ResponseAsigno(distribucion, false);
                        else
                            intService.ResponsePreasigno(distribucion, false);
                        
                        return null;
                    }
                }
                
                if (extraData2 == 0) return null;
                var entrega = daoFactory.EntregaDistribucionDAO.FindById(Convert.ToInt32(extraData2));
                var mensajeVo = daoFactory.MensajeDAO.GetByCodigo(extraData3.ToString("#0"), veh.Empresa, veh.Linea);
                try
                {
                    var mensaje = daoFactory.MensajeDAO.FindById(mensajeVo.Id);
                    try
                    {
                        var descriptiva = " - " + entrega.Viaje.Codigo + " - " + entrega.Descripcion;

                        var ms = new MessageSaver(daoFactory);
                        var log = ms.Save(null, Convert.ToString(extraData3), veh.Dispositivo, veh, chofer, inicio.Date, inicio, descriptiva, entrega.Viaje, entrega);

                        try
                        {
                            entrega.MensajeConfirmacion = log as LogMensaje;
                            daoFactory.EntregaDistribucionDAO.SaveOrUpdate(entrega);
                        }
                        catch (Exception){ }

                        if (mensaje.TipoMensaje.DeConfirmacion)
                            evento = new GarminEvent(inicio.Date, extraData2, inicio.Lat, inicio.Lon, EntregaDistribucion.Estados.Completado, chofer);
                        else if (mensaje.TipoMensaje.DeRechazo) 
                            evento = new GarminEvent(inicio.Date, extraData2, inicio.Lat, inicio.Lon, EntregaDistribucion.Estados.NoCompletado, chofer);
                        else
                            STrace.Error(typeof (CicloLogisticoFactory).FullName, Convert.ToInt32(extraData), "Respuesta de mensaje de Canned Response inválida sin tipo de mensaje adecuado para la ocasión. (" + extraData2 + "-" + extraData + ")");
                    }
                    catch (Exception e)
                    {
                        STrace.Exception(typeof (EventFactory).FullName, e,
                                         "E#2 Vehicle=" + veh.Id + " entrega=" +
                                         (entrega == null ? "null" : entrega.Id.ToString("#0")) + " mensajeVo=" +
                                         mensajeVo.Id + " mensaje=" +
                                         (mensaje == null ? "null" : mensaje.Id.ToString("#0")));
                    }
                } 
                catch (Exception e)
                {
                    STrace.Exception(typeof (EventFactory).FullName, e,
                                     "E#1 Vehicle=" + veh.Id + " entrega=" +
                                     (entrega == null ? "null" : entrega.Id.ToString("#0")) + " mensajeVo=" +
                                     (mensajeVo == null ? "null" : mensajeVo.Id.ToString("#0")));
                }
            }
            else if (codigo == MessageCode.ValidacionRuteo.GetMessageCode())
            {
                var deviceId = inicio.DeviceId;
                var vehicle = daoFactory.CocheDAO.FindMobileByDevice(deviceId);
                if (vehicle != null)
                {
                    var ruta = daoFactory.ViajeDistribucionDAO.FindEnCurso(vehicle);
                    if (ruta != null)
                        evento = new RouteEvent(inicio.Date, ruta.Id, inicio.Lat, inicio.Lon, RouteEvent.Estados.Enviado);
                }
            }
            else if (codigo == MessageCode.GarminETAReceived.GetMessageCode())
            {
                if (vehiculo.HasActiveStop())
                {
                    var activeStop = vehiculo.GetActiveStop();
                    if (activeStop > 0)
                    {
                        var edDAO = daoFactory.EntregaDistribucionDAO;
                        var vdDAO = daoFactory.ViajeDistribucionDAO;
                        var detalle = edDAO.FindById(activeStop);

                        if (detalle != null)
                        {
                            var utcnow = DateTime.UtcNow;
                            var fechora = vehiculo.EtaEstimated();
                            var minutos = fechora != null && fechora > utcnow ? fechora.Value.Subtract(utcnow).TotalMinutes : 0;
                            var texto = String.Format(CultureManager.GetLabel("GARMIN_STOP_ETA_ARRIVAL"), minutos, fechora);

                            if (minutos > 0)
                            {
                                using (var transaction = SmartTransaction.BeginTransaction())
                                {
                                    try
                                    {
                                        try
                                        {
                                            new MessageSaver(daoFactory).Save(null, codigo, vehiculo.Dispositivo, vehiculo, chofer, DateTime.UtcNow, null, texto,
                                                detalle.Viaje, detalle);
                                        }
                                        catch (Exception ex)
                                        {
                                            STrace.Exception(typeof (EventFactory).FullName, ex, vehiculo.Dispositivo.Id,
                                                String.Format("Exception doing MessageSaver.Save({0})", texto));
                                            throw ex;
                                        }
                                        try
                                        {
                                            detalle.GarminETA = fechora;
                                            detalle.GarminETAInformedAt = utcnow;
                                            vdDAO.SaveOrUpdate(detalle.Viaje);
                                        }
                                        catch (Exception ex)
                                        {
                                            STrace.Exception(typeof (EventFactory).FullName, ex, vehiculo.Dispositivo.Id,
                                                String.Format("Exception doing MessageSaver.Save({0})", texto));
                                            throw ex;
                                        }
                                        transaction.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        throw ex;
                                    }
                                }
                            }
                        }
                        else
                        {
                            STrace.Error(typeof(EventFactory).FullName, vehiculo.Dispositivo.Id, String.Format("Processing GarminETAReceived cannot be processed because EntregaDistribucion #{0} is null.", activeStop));                                                   
                        }
                    }
                    else
                    {
                        STrace.Error(typeof(EventFactory).FullName, vehiculo.Dispositivo.Id, "Processing GarminETAReceived cannot be processed because ActiveStop # is not valid.");                        
                    }
                }
            }
            else
            {
                var stopstatus = TranslateStopStatus(codigo);
                if (stopstatus != -1)
                {
                    evento = new GarminEvent(inicio.Date, extraData, inicio.Lat, inicio.Lon, stopstatus, chofer);
                }
            }

            return evento;
        }

        private static short TranslateStopStatus(string codigo)
        {
            if (codigo == MessageCode.GarminStopStatusActive.GetMessageCode())
                return GarminEvent.StopStatus.Active;
            if (codigo == MessageCode.GarminStopStatusDone.GetMessageCode())
                return GarminEvent.StopStatus.Done;
            if (codigo == MessageCode.GarminStopStatusUnreadInactive.GetMessageCode())
                return GarminEvent.StopStatus.UnreadInactive;
            if (codigo == MessageCode.GarminStopStatusReadInactive.GetMessageCode())
                return GarminEvent.StopStatus.ReadInactive;
            if (codigo == MessageCode.GarminStopStatusDeleted.GetMessageCode())
                return GarminEvent.StopStatus.Deleted;
            
            return -1;
        }

        public static IEvent GetEvent(DAOFactory daoFactory, LogMensaje mensaje)
        {
            var inicio = new GPSPoint(mensaje.Fecha, (float)mensaje.Latitud, (float)mensaje.Longitud);
            var codigo = mensaje.Mensaje.Codigo;
            var idPuntoDeInteres = mensaje.IdPuntoDeInteres;
            const int extraData = 0;

            return GetEvent(daoFactory, inicio, codigo, idPuntoDeInteres, extraData, -1, -1, null, mensaje.Chofer);
        }
    }
}
