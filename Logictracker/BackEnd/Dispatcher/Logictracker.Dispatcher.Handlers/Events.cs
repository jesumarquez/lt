using System;
using System.Globalization;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.Cache;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Dispatcher.Handlers.Common;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.Geofences;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Messages;
using Logictracker.Utils;
using System.Collections.Generic;

namespace Logictracker.Dispatcher.Handlers
{
	[FrameworkElement(XName = "EventsHandler", IsContainer = false)]
	public class Events : DeviceBaseHandler<Event>
	{
	    protected ExtraText ExtraText;
	    protected Zona ZonaManejo;

		#region Protected Methods

		protected override HandleResults OnDeviceHandleMessage(Event message)
		{
			//STrace.Debug(typeof(Events).FullName, message.DeviceId, "OnDeviceHandleMessage (EventsHandler)");

			AdjustRfidData(message);
            var code = GetGenericEventCode(message);
			STrace.Debug(typeof(Events).FullName, message.DeviceId, String.Format("code:{0} Subcode:{1}", code, message.GetData()));

            #region protect garmin on/off messages from be banned because of invalid date

            if ((code == MessageCode.GarminOn.GetMessageCode() || code == MessageCode.GarminOff.GetMessageCode()) && message.GeoPoint != null && FechaInvalida(message.GeoPoint.Date, DeviceParameters))
            {
                message.GeoPoint.Date = DateTime.UtcNow;
            }

            #endregion protect garmin on/off messages from be banned because of invalid date

            //if (IsGarbageMessage(code))
            //{
            //    return HandleResults.BreakSuccess;
            //}

		    if (IsInvalidMessage(code, message))
		    {
		        return HandleResults.BreakSuccess;
		    }

            ExtraText = new ExtraText(DaoFactory);

			if (MessageIdentifierX.IsEntityMessage(code))
			{
			    ProcessEntityEvent(message, code);
			}
			else if (Coche != null)
			{
			    var estado = GeocercaManager.CalcularEstadoVehiculo(Coche, message.GeoPoint, DaoFactory);
			    ZonaManejo = estado != null && estado.ZonaManejo != null && estado.ZonaManejo.ZonaManejo > 0
                    ? DaoFactory.ZonaDAO.FindById(estado.ZonaManejo.ZonaManejo) : null;
				ProcessEvent(code, message);
				ProcessPosition(message);
			}

			return HandleResults.Success;
		}

		#endregion

		#region Private Methods

		private static void ProcessPosition(IGeoPoint message)
		{
			var pos = message.GeoPoint;
			if (pos == null || (pos.Date < new DateTime(2010, 1, 1)) || (pos.Lat == 0) || (pos.Lon == 0)) return;
			new Positions().HandleMessage((Position)pos.ToPosition(message.DeviceId, 0));
		}
	
		private void ProcessEvent(string code, Event generico)
		{
			string applicationCode;
		    var esConfirmacionUbox = false;

            ControlarInicioDistribucionPorMensaje(code, generico);

            ControlarCierreDistribucionPorMensaje(code, generico);

		    if (MessageIdentifierX.IsRfidEvent(generico.Code)) applicationCode = ProcessRfidEvent(code, generico);
            else if (MessageIdentifierX.IsEstadoLogistico(generico.GetData())) applicationCode = ProcessEstadoLogistico(code, generico);
            else if (MessageIdentifierX.IsPanicEvent(code)) applicationCode = ProcessPanicEvent(generico, code);
            else if (code.Equals(MessageCode.SpeedingTicket.GetMessageCode())) applicationCode = ProcessVelocidadExcedidaGenericEvent(generico);
            else if (MessageIdentifierX.IsConfirmacionUbox(generico.GetData())) 
            {
                applicationCode = code;
                esConfirmacionUbox = true;
                // DEFINE EL PROCESAMIENTO CUANDO SABE SI TIENE UN PUNTO
            }            
            else applicationCode = ProcessGenericEvent(generico, code);
            
            if (DaoFactory.DetalleDispositivoDAO.GetProcesarCicloLogisticoFlagValue(generico.DeviceId))
			{
				GPSPoint point = null;
				if (generico.GeoPoint != null)
				{
					point = GPSPoint.Factory(generico.GeoPoint.Date, generico.GeoPoint.Lat, generico.GeoPoint.Lon);
				}
				else
				{
                    var maxMonths = Coche.Empresa != null ? Coche.Empresa.MesesConsultaPosiciones : 3;
                    var pos = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(Coche.Id, generico.Tiempo, maxMonths);
					if (pos != null) point = GPSPoint.Factory(generico.Tiempo, (float)pos.Latitud, (float)pos.Longitud);
				}

                if (point != null)
                {
                    if (esConfirmacionUbox) ProcessConfirmacionUbox(code, generico, point);
                    else
                    {
                        CicloLogisticoFactory.Process(DaoFactory, applicationCode, Coche, point, generico, false, GetChofer(generico.GetRiderId()));
                        CicloLogisticoFactory.ProcessEstadoLogistico(Coche, generico, applicationCode);
                    }
                }
                else if (esConfirmacionUbox) ProcessGenericEvent(generico, code);
			}
		}

        private void ControlarInicioDistribucionPorMensaje(string code, Event generico)
        {
            if (Coche.Empresa.InicioDistribucionPorMensaje &&
                code == Coche.Empresa.InicioDistribucionCodigoMensaje &&
                DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche) == null)
            {
                var distribucion = DaoFactory.ViajeDistribucionDAO.FindPendiente(new[] { Coche.Empresa.Id },
                                                                                 new[] { -1 }, new[] { Coche.Id },
                                                                                 DateTime.Today,
                                                                                 DateTime.Today.AddDays(1));
                if (distribucion != null)
                {
                    var evento = new InitEvent(generico.Tiempo);
                    var ciclo = new CicloLogisticoDistribucion(distribucion, DaoFactory, new MessageSaver(DaoFactory));
                    ciclo.ProcessEvent(evento);
                }
            }
        }

        private void ControlarCierreDistribucionPorMensaje(string code, Event generico)
        {
            if (Coche.Empresa.CierreDistribucionPorMensaje &&
                code == Coche.Empresa.CierreDistribucionCodigoMensaje)
            {
                var distribucion = DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche);
                if (distribucion != null)
                {
                    var evento = new CloseEvent(generico.Tiempo);
                    var ciclo = new CicloLogisticoDistribucion(distribucion, DaoFactory, new MessageSaver(DaoFactory));
                    ciclo.ProcessEvent(evento);
                }
            }
        }

	    private void ProcessEntityEvent(Event evento, string code)
		{
            try
            {
                STrace.Debug(typeof(Events).FullName, evento.DeviceId, String.Format("ProcessEntityEvent Datos: {0}, {1}", code, evento.SensorsDataString));

                var eventType = (MessageIdentifier)evento.Code;
                var eventSubcode = (MessageIdentifier)evento.GetData();

                if (eventType == MessageIdentifier.TelemetricData)
                {
                    foreach(var sensorCode in evento.SensorsDataDict.Keys)
                    {
                        var sensor = DaoFactory.SensorDAO.FindByCode(Dispositivo.Id, sensorCode);
                        if (sensor == null)
                        {
                            STrace.Error(typeof(Events).FullName, evento.DeviceId, String.Format("ProcessEntityEvent. No se encontró Sensor con código {0}", sensorCode));
                            continue;
                        }
                        var subEntidad = DaoFactory.SubEntidadDAO.FindBySensor(sensor.Id);
                        if (subEntidad == null)
                        {
                            STrace.Error(typeof(Events).FullName, evento.DeviceId, String.Format("ProcessEntityEvent. No se encontró Subentidad asociada al Sensor con Id {0}", sensor.Id));
                            continue;
                        }

                        var valorString = evento.SensorsDataDict[sensorCode];
                        double valorDouble;
                        var isNumeric = double.TryParse(valorString.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valorDouble);

                        var medicion = new Medicion
                        {
                            Dispositivo = Dispositivo,
                            FechaAlta = evento.TiempoAlta,
                            FechaMedicion = evento.GetDateTime(),
                            Sensor = sensor,
                            SubEntidad = subEntidad,
                            TipoMedicion = sensor.TipoMedicion,
                            Valor = valorString,
                            ValorDouble = valorDouble
                        };
                        DaoFactory.MedicionDAO.Save(medicion);

                        if (isNumeric)
                        {
                            var maximoExcedido = subEntidad.ControlaMaximo && medicion.ValorDouble > subEntidad.Maximo;
                            var minimoExcedido = subEntidad.ControlaMinimo && medicion.ValorDouble < subEntidad.Minimo;
                            var messageCode = maximoExcedido ? MessageCode.SensorUpperLimitExceeded.GetMessageCode()
                                            : minimoExcedido ? MessageCode.SensorLowerLimitExceeded.GetMessageCode()
                                            : null;

                            if (messageCode != null)
                            {
                                STrace.Debug(typeof(Events).FullName, evento.DeviceId, String.Format("Event Exceso: Msg: {0}, Sensor: {1}", messageCode, sensor.Codigo));

                                M2MMessageSaver.Save(messageCode, Dispositivo,
                                                     sensor, subEntidad, 
                                                     medicion.FechaMedicion, medicion.FechaMedicion,
                                                     " - " + medicion.Valor);
                            } 
                        }
                        
                        if (sensorCode.Contains("EngineState") &&
                           (eventSubcode == MessageIdentifier.EngineOn || eventSubcode == MessageIdentifier.EngineOff))
                        {
                            var msgCode = eventSubcode == MessageIdentifier.EngineOn
                                              ? MessageCode.EngineOn.GetMessageCode()
                                              : MessageCode.EngineOff.GetMessageCode();

                            STrace.Debug(typeof(Events).FullName, evento.DeviceId, String.Format("Event Engine: Msg: {0}, Sensor: {1}", msgCode, sensor.Codigo));

                            M2MMessageSaver.Save(msgCode, Dispositivo, sensor, subEntidad,
                                                 medicion.FechaMedicion, medicion.FechaMedicion,
                                                 string.Empty);
                        }
                    }
                }
                else if(eventType == MessageIdentifier.TelemetricEvent)
                {
                    var sensorCode = evento.SensorsDataDict.Keys.SafeFirstOrDefault();
					if (String.IsNullOrEmpty(sensorCode))
                    {
                        STrace.Error(typeof(Events).FullName, evento.DeviceId, String.Format("ProcessEntityEvent. No se encontro codigo de sensor. Datos: {0}, {1}", code, evento.SensorsDataString));
                        return;
                    }

                    var sensor = DaoFactory.SensorDAO.FindByCode(Dispositivo.Id, sensorCode);
                    if (sensor == null)
                    {
                        STrace.Error(typeof(Events).FullName, evento.DeviceId, String.Format("ProcessEntityEvent. No se encontro sensor con código {0}", sensorCode));
                        return;
                    }

                    var subEntidad = DaoFactory.SubEntidadDAO.FindBySensor(sensor.Id);
                    var messageCode = evento.Data[0].ToString(CultureInfo.InvariantCulture);
					M2MMessageSaver.Save(messageCode, Dispositivo, sensor, subEntidad, evento.GetDateTime(), evento.GetDateTime(), String.Empty);
                }
            }
			catch (Exception e)
			{
				STrace.Exception(typeof(Event).FullName, e, Dispositivo.Id);
			}
		}

		private string ProcessVelocidadExcedidaGenericEvent(IGeoPoint generico)
		{
            var text = ExtraText.GetVelocidadExcedidaExtraText(generico, Coche);
		    var chofer = GetChofer(generico.GetRiderId());
		    var fecha = generico.GetDateTime();
            var evento = MessageSaver.Save(generico, MessageCode.SpeedingTicket.GetMessageCode(), Dispositivo, Coche, chofer, fecha, generico.GeoPoint, text, ZonaManejo);
            
            var infraccion = new Infraccion
            {
                Vehiculo = Coche,
                Alcanzado = generico.GeoPoint.Speed.Unpack(),
                CodigoInfraccion = Infraccion.Codigos.ExcesoVelocidad,
                Empleado = evento.Chofer,
                Fecha = fecha,
                Latitud = generico.GeoPoint.Lat,
                Longitud = generico.GeoPoint.Lon,
                FechaFin = null,
                LatitudFin = 0,
                LongitudFin = 0,
                Permitido = 0,
                Zona = ZonaManejo,
                FechaAlta = DateTime.UtcNow
            };

            DaoFactory.InfraccionDAO.Save(infraccion);

		    return MessageCode.SpeedingTicket.GetMessageCode();
		}

        private string ProcessPanicEvent(Event generico, string code)
        {
            var text = ExtraText.GetExtraText(generico, code).Trim().ToUpperInvariant();
            var chofer = GetChofer(generico.GetRiderId());
            var fecha = generico.GetDateTime();
            var evento = MessageSaver.Save(generico, code, Dispositivo, Coche, chofer, fecha, generico.GeoPoint, text, ZonaManejo);

            var infraccion = new Infraccion
            {
                Vehiculo = Coche,
                Alcanzado = generico.GeoPoint.Speed.Unpack(),
                CodigoInfraccion = Infraccion.Codigos.Panico,
                Empleado = evento.Chofer,
                Fecha = fecha,
                Latitud = generico.GeoPoint.Lat,
                Longitud = generico.GeoPoint.Lon,
                FechaFin = null,
                LatitudFin = 0,
                LongitudFin = 0,
                Permitido = 0,
                Zona = ZonaManejo,
                FechaAlta = DateTime.UtcNow
            };

            DaoFactory.InfraccionDAO.Save(infraccion);

            return code;
        }

		private string ProcessGenericEvent(Event generico, string code)
		{
            var text = ExtraText.GetExtraText(generico, code).Trim().ToUpperInvariant();
            MessageSaver.Save(generico, code, Dispositivo, Coche, GetChofer(generico.GetRiderId()), generico.GetDateTime(), generico.GeoPoint, text, ZonaManejo);
		    return code;
		}

        private void ProcessConfirmacionUbox(string code, Event generico, GPSPoint point)
        {
            var extraText = string.Empty;
            var distribucion = DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche);
            if (distribucion != null)
            {
                var estado = GeocercaManager.CalcularEstadoVehiculo(Coche, point, DaoFactory);
                var detalles = distribucion.Detalles.Where(d => estado.GeocercasDentro.Select(g => g.Geocerca.Id).Contains(d.ReferenciaGeografica.Id));
                
                var conEntrada = detalles.Where(d => d.Entrada.HasValue && !d.Manual.HasValue)
                                         .OrderBy(d => d.Entrada.Value);
                
                detalles = conEntrada.Any() ? conEntrada : detalles.Where(d => !d.Manual.HasValue);

                var detalle = detalles.Any() ? detalles.First() : null;
                
                if (detalle != null)
                {
                    DaoFactory.Session.Refresh(detalle);
                
                    extraText = " (" + detalle.Orden + ") -> " + detalle.Viaje.Codigo + " - " + detalle.Descripcion;
                    detalle.Estado = generico.GetData().ToString("#0") == MessageCode.TareaRealizada.GetMessageCode() ? EntregaDistribucion.Estados.Completado : EntregaDistribucion.Estados.NoCompletado;
                    detalle.Manual = generico.GetDateTime();
                    DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
                }
            }
            
            var textoEvento = ExtraText.GetExtraText(generico, code) + extraText;
            var chofer = GetChofer(generico.GetRiderId());

            MessageSaver.Save(generico, code, Dispositivo, Coche, chofer, generico.GetDateTime(), generico.GeoPoint, textoEvento, ZonaManejo);
            MessageSaver.Save(MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode(), Coche, chofer, generico.GetDateTime(), generico.GeoPoint, textoEvento);
        }

		private string ProcessEstadoLogistico(string code, IGeoPoint generico)
		{
			if (code.Length == 1) code = code.PadLeft(2, '0');

            MessageSaver.Save(generico, code, Dispositivo, Coche, null, generico.GetDateTime(), generico.GeoPoint, string.Empty, ZonaManejo);

		    return code;
		}

		private string ProcessRfidEvent(string code, Event generico)
		{
			if (string.IsNullOrEmpty(code))
			{
				STrace.Debug(typeof(Events).FullName, generico.DeviceId, String.Format("RFID invalid message code detected. (Vehicle {0} - Code {1} - RFID Action {2})", Coche.Interno, generico.Code, generico.GetData()));
				return code;
			}

			var lastLogin = GetLastLogin(Coche);

			var driver = GetDriverForLogin(generico, lastLogin, code);
            var text = ExtraText.GetRfidExtraText(code.Equals(MessageCode.RfidDriverLogout.GetMessageCode()) && lastLogin != null && driver != null && driver.Tarjeta != null ? driver.Tarjeta.Pin : generico.GetRiderId(), driver);

            var login = MessageSaver.Save(generico, code, Dispositivo, Coche, driver, generico.GetDateTime(), generico.GeoPoint, text, ZonaManejo);

            if (login != null && code.Equals(MessageCode.RfidDriverLogin.GetMessageCode()))
            {
                var lastDate = lastLogin != null ? lastLogin.Fecha : DateTime.MinValue;
                if (lastDate <= login.Fecha) UpdateLastLogin(lastLogin, login);
            }

            // Cache Current Driver
            if (driver != null && code.Equals(MessageCode.RfidDriverLogin.GetMessageCode()))
            {
                DaoFactory.EmpleadoDAO.SetLoggedInDriver(Coche, driver.Id);
            }
            else if (driver != null && code.Equals(MessageCode.RfidDriverLogout.GetMessageCode()))
            {
                DaoFactory.EmpleadoDAO.SetLoggedInDriver(Coche, null);
            }

            // Cache Last Log
            if (driver != null)
            {
                var verif = new Empleado.VerificadorEmpleado { Empleado = driver, TipoFichada = Empleado.VerificadorEmpleado.TipoDeFichada.SinFichar };

                if (login != null)
                {
                    verif.Fecha = login.Fecha;

                    if (login.Coche != null)
                    {
                        var puerta = DaoFactory.PuertaAccesoDAO.FindByVehiculo(login.Coche.Id);

                        if (puerta != null)
                        {
                            verif.PuertaAcceso = puerta;
                            if (code.Equals(MessageCode.RfidEmployeeLogin.GetMessageCode())
                             || code.Equals(MessageCode.RfidDriverLogin.GetMessageCode()))
                            {
                                verif.TipoFichada = Empleado.VerificadorEmpleado.TipoDeFichada.Entrada;
                                if (puerta.ZonaAccesoEntrada != null) verif.ZonaAcceso = puerta.ZonaAccesoEntrada;
                            }
                            else if (code.Equals(MessageCode.RfidEmployeeLogout.GetMessageCode())
                                  || code.Equals(MessageCode.RfidDriverLogout.GetMessageCode()))
                            {
                                verif.TipoFichada = Empleado.VerificadorEmpleado.TipoDeFichada.Salida;
                                if (puerta.ZonaAccesoSalida != null) verif.ZonaAcceso = puerta.ZonaAccesoSalida;
                            }
                        }
                    }
                }

                DaoFactory.EmpleadoDAO.SetLastLog(driver, verif);
            }

		    // Eventos Acceso
            if (driver != null && 
                (code == MessageCode.RfidEmployeeLogin.GetMessageCode() 
                || code == MessageCode.RfidEmployeeLogout.GetMessageCode()
                || code == MessageCode.RfidDriverLogin.GetMessageCode()
                || code == MessageCode.RfidDriverLogout.GetMessageCode()))
            {
                var puerta = DaoFactory.PuertaAccesoDAO.FindByVehiculo(Coche.Id);
                if (puerta != null)
                {
                    var acceso = new EventoAcceso
                                     {
                                         Alta = DateTime.UtcNow,
                                         Empleado = driver,
                                         Entrada = code == MessageCode.RfidEmployeeLogin.GetMessageCode()
                                                || code == MessageCode.RfidDriverLogin.GetMessageCode(),
										 Fecha = generico.GetDateTime(),
                                         Puerta = puerta
                                     };

                    DaoFactory.EventoAccesoDAO.SaveOrUpdate(acceso);
                }
            }
		    return code;
		}

		private void DiscardEvent(string code, IGeoPoint message, DiscardReason discardReason)
		{
			MessageSaver.Discard(code, Dispositivo, Coche, GetChofer(message.GetRiderId()), message.GetDateTime(), message.GeoPoint, null, discardReason);
		}

		private bool IsInvalidMessage(string code, IGeoPoint message)
		{
			var discardReason = DiscardReason.None;

            if (Coche == null && !MessageIdentifierX.IsEntityMessage(code)) discardReason = DiscardReason.NoAssignedMobile;
			else if (message.GeoPoint != null && FechaInvalida(message.GeoPoint.Date, DeviceParameters)) discardReason = DiscardReason.InvalidDate;
			else if (message.GeoPoint != null && FueraDelGlobo(message.GeoPoint)) discardReason = DiscardReason.OutOfGlobe;
			else if (IsVelocidadInvalida(message)) discardReason = DiscardReason.InvalidSpeed;

			if (discardReason.Equals(DiscardReason.None)) return false;

			DiscardEvent(code, message, discardReason);

			return true;
		}

        private bool IsGarbageMessage(string code)
        {
            var isGarbage = false;

            if (Dispositivo != null)
            {
                var mensajes = DaoFactory.MensajeIgnoradoDAO.GetCodigosByDispositivo(Dispositivo.Id);

                if (mensajes.Contains(code)) isGarbage = true;
            }

            return isGarbage;
        }

		private bool IsVelocidadInvalida(IGeoPoint message)
		{
			if (message.GeoPoint == null) return false;

			var velocidadNegativa = message.GeoPoint.Velocidad < 0;
			var velocidadMuyGrande = message.GeoPoint.Velocidad > Coche.TipoCoche.MaximaVelocidadAlcanzable;

			return velocidadNegativa || velocidadMuyGrande;
		}

		private static string GetGenericEventCode(Event message)
		{
			var data = message.GetData();
			var dataS = data.ToString(CultureInfo.InvariantCulture);
			var identifier = message.Code;
			var identifierS = identifier.ToString(CultureInfo.InvariantCulture);
			var messageIdentifier = (MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), identifierS, true);
			if (messageIdentifier.Equals(MessageIdentifier.RfidDetected)) return GetRfidAction(data, message.GetRiderId());
			if (MessageIdentifierX.IsEstadoLogistico(message.GetData())) return dataS;
			return (identifierS == MessageCode.SpeedingTicket.GetMessageCode()) ? MessageCode.SpeedingTicket.GetMessageCode() : GetCode(identifier, data);
		}

		private static string GetCode(short identifier, Int64 data)
		{
			var messageIdentifier = (MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), identifier.ToString(CultureInfo.InvariantCulture), true);

			switch (messageIdentifier)
			{
				case MessageIdentifier.EngineOffInternal: return MessageCode.EngineOff.GetMessageCode();
				case MessageIdentifier.EngineOnInternal: return MessageCode.EngineOn.GetMessageCode();
				case MessageIdentifier.HardwareChangeInternal: return MessageCode.HardwareChange.GetMessageCode();
				case MessageIdentifier.DeviceShutdownInternal: return MessageCode.DeviceShutdown.GetMessageCode();
				case MessageIdentifier.DeviceOnLineInternal: return MessageCode.DeviceOnLine.GetMessageCode();
				case MessageIdentifier.DeviceOnNetInternal: return MessageCode.DeviceOnNet.GetMessageCode();
				case MessageIdentifier.DeviceOnProxyInternal: return MessageCode.DeviceOnProxy.GetMessageCode();
				case MessageIdentifier.DeviceOffLineInternal: return MessageCode.DeviceOffLine.GetMessageCode();
				case MessageIdentifier.FotaSuccess: return MessageCode.FotaSuccess.GetMessageCode();
				case MessageIdentifier.FotaFail: return MessageCode.FotaFail.GetMessageCode();
				case MessageIdentifier.FotaPause: return MessageCode.FotaPause.GetMessageCode();
				case MessageIdentifier.FotaStart: return MessageCode.FotaStart.GetMessageCode();
				case MessageIdentifier.QtreeStart: return MessageCode.QtreeStart.GetMessageCode();
				case MessageIdentifier.QtreePause: return MessageCode.QtreePause.GetMessageCode();
				case MessageIdentifier.QtreeSuccess: return MessageCode.QtreeSuccess.GetMessageCode();
				case MessageIdentifier.ConfigSuccess: return MessageCode.ConfigurationSuccess.GetMessageCode();
				case MessageIdentifier.ConfigFail: return MessageCode.ConfigurationFail.GetMessageCode();

                case MessageIdentifier.GarminOn: return MessageCode.GarminOn.GetMessageCode();
                case MessageIdentifier.GarminOff: return MessageCode.GarminOff.GetMessageCode();

                case MessageIdentifier.PermanenciaEnGeocercaExcedida: return MessageCode.PermanenciaEnGeocercaExcedida.GetMessageCode();
                case MessageIdentifier.PermanenciaEnGeocercaExcedidaEnCicloLogistico: return MessageCode.PermanenciaEnGeocercaExcedidaEnCicloLogistico.GetMessageCode();

				case MessageIdentifier.GenericMessage: return data.ToString(CultureInfo.InvariantCulture);

				default: return messageIdentifier.ToString("d");
			}
		}

        private static string GetRfidAction(Int64 data, string auxData)
		{
			switch (data)
			{
				case 0: return MessageCode.RfidDriverLogin.GetMessageCode();
				case 1: return MessageCode.RfidDriverLogout.GetMessageCode();
				case 2: return MessageCode.RfidDriverLogout.GetMessageCode(); // Logout por contacto.
				case 3: return MessageCode.RfidEmployeeLogin.GetMessageCode();
				case 4: return MessageCode.RfidEmployeeLogout.GetMessageCode();
				case 0xFF: return GetEmployeeRfidAction(auxData);
			}

			return string.Empty;
		}

		private static string GetEmployeeRfidAction(string rfid)
		{
			var key = string.Format("employeeRfid:{0}", rfid);

			if (LogicCache.KeyExists(typeof(string), key))
			{
				LogicCache.Delete(typeof(string), key);

				return MessageCode.RfidEmployeeLogout.GetMessageCode();
			}

			LogicCache.Store(typeof(string), key, rfid);

			return MessageCode.RfidEmployeeLogin.GetMessageCode();
		}

		private LogUltimoLogin GetNewLastLogin(LogUltimoLoginVo lastLogin, LogMensajeBase login)
		{
			return lastLogin == null ? new LogUltimoLogin(login) : UpdateLogin(lastLogin, login);
		}

		private Empleado GetDriverForLogin(IMessage generico, LogUltimoLoginVo lastLogin, IEquatable<string> code)
		{
		    return code.Equals(MessageCode.RfidDriverLogout.GetMessageCode()) && lastLogin != null && lastLogin.IdChofer > 0
		               ? DaoFactory.EmpleadoDAO.FindById(lastLogin.IdChofer)
		               : GetChoferForLogin(generico.GetRiderId());
		}

		private LogUltimoLoginVo GetLastLogin(Coche coche)
		{
			return DaoFactory.LogUltimoLoginDAO.GetLastVehicleRfidEvent(coche);
		}

		private void UpdateLastLogin(LogUltimoLoginVo lastLogin, LogMensajeBase login)
		{
			var log = GetNewLastLogin(lastLogin, login);

			if (lastLogin == null) DaoFactory.LogUltimoLoginDAO.Save(log);
			else DaoFactory.LogUltimoLoginDAO.Update(log);

			SetLastLogin(log);
		}

		private LogUltimoLogin UpdateLogin(LogUltimoLoginVo lastLogin, LogMensajeBase login)
		{
			var log = DaoFactory.LogUltimoLoginDAO.FindById(lastLogin.Id);

			log.Latitud = login.Latitud;
			log.Longitud = login.Longitud;
			log.Dispositivo = login.Dispositivo;
			log.Horario = login.Horario;
			log.DetalleHorario = login.DetalleHorario;
			log.Usuario = login.Usuario;
			log.Fecha = login.Fecha;
			log.Texto = login.Texto;
			log.Coche = login.Coche;
			log.Accion = login.Accion;
			log.Chofer = login.Chofer;
			log.Expiracion = login.Expiracion;
			log.Estado = login.Estado;
			log.Mensaje = login.Mensaje;
			log.FechaFin = login.FechaFin;
			log.LatitudFin = login.LatitudFin;
			log.LongitudFin = login.LongitudFin;
			log.VelocidadPermitida = login.VelocidadPermitida;
			log.VelocidadAlcanzada = login.VelocidadAlcanzada;
			log.IdPuntoDeInteres = login.IdPuntoDeInteres;

			return log;
		}

		private void SetLastLogin(LogMensajeBase lastLogin)
		{
			var lastLoginVo = new LogUltimoLoginVo(lastLogin);

			Coche.StoreLastLogin(lastLoginVo);

			Dispositivo.StoreLastLogin(lastLoginVo);
		}

		private void AdjustRfidData(Event message)
		{
			if (Coche == null) return;
			var mi = (MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), message.Code.ToString(CultureInfo.InvariantCulture), true);
			if (!mi.Equals(MessageIdentifier.RfidDetected) || !String.IsNullOrEmpty(message.UserIdentifier) || String.IsNullOrEmpty(message.SensorsDataString)) return;

			STrace.Debug(GetType().FullName, message.DeviceId, String.Format("Rfid from Legajo: {0}", message.SensorsDataString));
		    var leg = DaoFactory.EmpleadoDAO.FindByLegajo(Coche.Empresa == null ? -1 : Coche.Empresa.Id, Coche.Linea == null ? -1 : Coche.Linea.Id, message.SensorsDataString);
			if ((leg != null) && (leg.Tarjeta != null)) message.UserIdentifier = leg.Tarjeta.Pin;
		}

		#endregion
	}
}