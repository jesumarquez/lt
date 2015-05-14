#region Usings

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Urbetrack.Backbone;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.Comm.Core.Transport;
using Urbetrack.Comm.Core.Transport.XBeeRLP;
using Urbetrack.Messaging;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Hacking;
using Urbetrack.Torino;

#endregion

namespace Urbetrack.Comm.Core.TransactionUsers
{
	public class ServerTU : TransactionUser
	{
		private static void BeSpineOnline(Device d)
		{
			if (d.RemoteDeviceState.State == DeviceState.States.CONNECTED)
			{
				d.RemoteDeviceState.State = d.Type == DeviceTypes.Types.URB_v0_5
												? DeviceState.States.ONLINE
												: DeviceState.States.SYNCING;
			}
		}

		public override bool NuevaSolicitud(PDU pdu, Transaccion tr, Transporte t)
		{
			var d = Devices.I().FindById(pdu.IdDispositivo);

			if (d == null)
			{
				if (pdu.CH == 1) return true;
				STrace.Debug(typeof(ServerTU).FullName, String.Format("Device Not Found! Id={0} CH={1} CL={2}", pdu.IdDispositivo, pdu.CH, pdu.CL));
				return false;
			}

			if (d.DisabledUntil > DateTime.Now)
			{
				STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("Device disabled until={0} CH={1} CL={2}", d.DisabledUntil, pdu.CH, pdu.CL));
				return false;
			}

			switch (pdu.CH)
			{
				case (byte)Codes.HighCommand.RemoteShell:
					{
						var rsh = pdu as RemoteShell;
						if (rsh != null) STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("RemoteShell: '{0}'", rsh.CommandLine));
						return true;
					}
				case (byte)Codes.HighCommand.MsgPosicion:
					{
						var pos = pdu as Posicion;
						if (pos == null)
						{
							STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("Invalid Packet CH={0} CL={1}", pdu.CH, pdu.CL));
							return false;
						}
						if (AutoReport != null)
						{
							var auto_report_failure = false;
							try
							{
								return AutoReport(this, pos);
							}
							catch (Exception e)
							{
								STrace.Exception(typeof(ServerTU).FullName, e, d.Id);
								auto_report_failure = true;
								return false;
							}
							finally
							{
								try
								{
									if (auto_report_failure)
									{
										STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("CrapReceivedCounter -> Device[{0}]: NEW_REQUEST/AutoReportFailure", d.LogId));
										d.RemoteDeviceState.CrapReceivedCounter++;
									}
									else
									{
										if (pos.Puntos.Count > 0)
										{
											d.RemoteDeviceState.LastKnownGPSPoint = pos.Puntos[pos.Puntos.Count - 1];
											d.RemoteDeviceState.LastReceivedTrackingData = DateTime.Now;
										}
										BeSpineOnline(d);

										if (d.XBeeSession.State == XBeeSession.SessionStates.ACTIVE)
										{
											d.XBeeSession.ReceivedPositions += pos.Puntos.Count;
											STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("Posiciones Recibidas XBEE: {0}/{1}", d.XBeeSession.ReceivedPositions, d.XBeeSession.ExpectedTrackingPositions));
											if ((d.XBeeSession.ExpectedTrackingPositions > 0 && d.XBeeSession.ReceivedPositions > d.XBeeSession.ExpectedTrackingPositions) || d.XBeeSession.Report.QueryState == XBeeReport.QueryStates.QUERY_STEPBY)
											{
												d.XBeeSession.Deactivate();
											}
											else
											{
												var dt = d.Transporte as TransporteXBEE;
												if (dt != null)
												{
													dt.DoReceiveReport(d);
												}
											}
										}
									}
								}
								catch (Exception e)
								{
									STrace.Exception(typeof(ServerTU).FullName, e, d.Id);
								}
							}
						}
						STrace.Debug(typeof(ServerTU).FullName, d.Id, "AutoReport HandlerMissing");
						return false;
					}
				case (byte)Codes.HighCommand.KeepAlive:
					STrace.Debug(typeof(ServerTU).FullName, d.Id, "KEEPALIVE");
					return true;
				case (byte)Codes.HighCommand.SystemReport:
					try
					{
						var sr = pdu as SystemReport;
						if (sr == null) return false;
						d.SetInfoParameter("system_resets", sr.SystemResets);
						d.SetInfoParameter("watchdog_resets", sr.WatchDogResets);
						d.SetInfoParameter("net_udp_recv_bytes", sr.NETWORK_UDP_ReceivedBytes);
						d.SetInfoParameter("net_udp_sent_bytes", sr.NETWORK_UDP_SentBytes);
						d.SetInfoParameter("net_udp_recv_dgams", sr.NETWORK_UDP_ReceivedDgrams);
						d.SetInfoParameter("net_udp_sent_dgams", sr.NETWORK_UDP_SentDgrams);
						BeSpineOnline(d);
						return true;
					}
					catch (Exception e)
					{
						STrace.Exception(typeof(ServerTU).FullName, e, d.Id);
						return false;
					}
				case (byte)Codes.HighCommand.MsgEvento:
					{
						try
						{
							if (NuevoEvento(d, pdu, tr))
							{
								if (d.XBeeSession.State == XBeeSession.SessionStates.ACTIVE)
								{
									d.XBeeSession.ReceivedEvents++;
									var dt = d.Transporte as TransporteXBEE;
									if (dt != null)
									{
										dt.DoReceiveReport(d);
									}
								}

								BeSpineOnline(d);
								return true;
							}
							throw new Exception();
						}
						catch (Exception)
						{
							STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("CrapReceivedCounter -> Device[{0}]: NEW_REQUEST/EventFailure", d.LogId));
							d.RemoteDeviceState.CrapReceivedCounter++;
						}
						return false;
					}
				default:
					tr.CLdeRespuesta = 0x01; // codigo interino de MensajeDesconocido.
					return true;
			}
		}

		private bool NuevoEvento(Device d, PDU pdu, Transaccion tr)
		{
			d.RemoteDeviceState.LastReceivedEventData = DateTime.Now;

			switch (pdu.CL)
			{
				case 0x20:
				case 0x00:
					{
						//RFID Detectado.
						var rfid = pdu as RFIDDetectado;
						if (rfid == null) return false;
						return RFIDDetected != null && RFIDDetected(this, rfid);
					}
				case 0x11:
				case 0x10:
				case 0x02:
				case 0x01:
					{
						var exv = pdu as ExcesoVelocidad;
						if (exv == null) return false;
						return ExcesoVelocidad != null && ExcesoVelocidad(this, exv);
					}
				case 0xFE:
				case 0xFF:
					{
						//Evento Generico
						var evt = pdu as Evento;
						if (evt == null) return false;
						d.DeviceEvent(evt);
						// ignoro los eventos de dispositivo online enviados por el dispositivo.
						if (evt.CodigoEvento == (short)MessageIdentifier.DeviceOnLine_internal) return true;
						return Evento != null && Evento(this, evt);
					}
				default:
					tr.CLdeRespuesta = 0x01; // codigo interino de MensajeDesconocido.
					return true;
			}
		}

		public override PDU RequieroRespuesta(PDU pdu, Transaccion tr, Transporte t)
		{
			if (pdu.CH == (byte)Codes.HighCommand.LoginRequest)
			{
				var lrq = pdu as LoginRequest;
				if (lrq == null) throw new NullReferenceException("No se pudo reconvertir la pdu en LRQ.");
				var d = Devices.I().FindByImei(lrq.IMEI);
				if (d == null)
				{
					STrace.Debug(typeof(ServerTU).FullName, String.Format("Dispositivo no registrado, imei={0}", lrq.IMEI));
					var lr = new LoginRechazado
								 {
									 IdDispositivo = 0x0000,
									 Seq = pdu.Seq
								 };
					return lr;
				}
				if (d.DisabledUntil > DateTime.Now)
				{
					STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("Dispositivo deshabilitado password='{0}'", Hacker.Device.CheckPasswords ? "OK" : "(hack: no comparados)"));
					return null;
				}
				STrace.Debug(typeof(ServerTU).FullName, d.Id, String.Format("Login aceptado imei='{0}' password='{1}'", lrq.IMEI, Hacker.Device.CheckPasswords ? "OK" : "(hack: no comparados)"));
				d.Touch(lrq.Destino);
				d.Type = lrq.DetectedDeviceType;
				d.Transporte = t;
				// actualizamos la base de datos.
				d.SetServerParameter("known_firmware_signature", lrq.Firmware);
				d.RemoteDeviceState.FirmwareVersion = lrq.Firmware;

				if (!String.IsNullOrEmpty(lrq.XbeeHardware))
					d.SetServerParameter("known_xbee_hardware_version", lrq.XbeeHardware);

				if (!String.IsNullOrEmpty(lrq.XbeeFirmware))
				{
					d.SetServerParameter("known_xbee_firmware_version", lrq.XbeeFirmware);
					d.RemoteDeviceState.XBeeFirmware = lrq.XbeeFirmware;
				}

				if (lrq.ConfigRevision == 0x7FFF || lrq.QTreeRevision == 0x7FFFFFFF ||
					lrq.MessagesRevision == 0x7FFFFFFF)
				{
					d.StorageMediaFailure = true;
					d.SetServerParameter("known_config_revision", 0);
					d.SetServerParameter("known_qtree_revision", 0);
					d.SetServerParameter("known_messages_revision", 0);
					d.SetServerParameter("known_secure_identifier", "media_error");
					d.RemoteDeviceState.QTreeRevision = 0;
				}
				else
				{
					d.SetServerParameter("known_config_revision", lrq.ConfigRevision);
					d.SetServerParameter("known_qtree_revision", lrq.QTreeRevision);
					d.SetServerParameter("known_messages_revision", lrq.MessagesRevision);
					d.SetServerParameter("known_secure_identifier", lrq.SecureId);
					d.RemoteDeviceState.QTreeRevision = lrq.QTreeRevision;
					d.StorageMediaFailure = false;
				}

				// definimos si cambia a online o a onnet.
				d.RemoteDeviceState.LastLoginGPSPoint = (d.SupportsFixAtLogin ? lrq.GPSPoint : null);
				d.RemoteDeviceState.LastLogin = DateTime.Now;

				// Deteccion de Reconexion no Sincronizada:
				//   el dispo se reconecto antes que el gtw se de cuenta que lo perdio,
				//   se detienen funciones en curso, ya que apuntan a la ip anterior
				//   y como fallaran, se producira un falso OFFLINE.-
				if (d.State != DeviceTypes.States.OFFLINE)
				{
					d.CancelAllTasks();
				}
				d.State = lrq.Transporte.DeviceGoesONNET ? DeviceTypes.States.ONNET : DeviceTypes.States.ONLINE;

				d.RemoteDeviceState.StorageMediaFailure = d.StorageMediaFailure;
				d.RemoteDeviceState.TransientDeviceNetworkPath = d.Destino.ToString();
				// al cambiar el estado mando al SPINE lo que sea que haya cambiado.
				d.RemoteDeviceState.State = DeviceState.States.CONNECTED;
				// automatizamos asincronicamente la actualizacion del dispositivo.
				var dummy = new byte[2];
				Devices.I().Cmd.Push("CONFIGURE:" + d.Id_short, dummy);

				//Devices.i().cmd.Push("RSH:" + d.Id + ":sms 1168175235 id=" + d.Id + " codigo=" + d.LegacyCode + " interno=" + d.Vehicle, dummy);Devices.i().cmd.Push("RSH:" + d.Id + ":sms 1168175235 id=" + d.Id + " codigo=" + d.LegacyCode + " interno=" + d.Vehicle, dummy);
				//Devices.i().cmd.Push("RSH:" + d.Id + ":simat AT+CIMI", dummy);
				//Devices.i().cmd.Push("RSH:" + d.Id + ":simat AT+CNUM", dummy);
				//Devices.i().cmd.Push("RSH:" + d.Id + ":simat AT+CLCC", dummy);

				// configuramos los terminos de la sesion que se acepta.

				var max_pdu_samples = (byte)d.GetServerParameterInt("max_pud_samples", Device.DefaultMaxPDUSamples);
				var flush_timeout = (byte)d.GetServerParameterInt("flush_timeout", Device.DefaultFlushTimeout);

				short retrieve_flags;
				if (lrq.Transporte.DeviceGoesONNET)
				{
					retrieve_flags = (short)d.GetServerParameterInt("retrieve_flags_xbee", Device.DefaultXbeeRetrieveFlags);
					max_pdu_samples = (byte)d.GetServerParameterInt("max_pud_samples_xbee", Device.DefaultXbeeMaxPDUSamples);
				}
				else
				{
					retrieve_flags = (short)d.GetServerParameterInt("retrieve_flags", Device.DefaultRetrieveFlags);
				}

				// validamos que las mediciones pedidas entren en una PDU.
				// HARDCODE: 24 se toma como el largo de un FIX.
				if (lrq.Transporte.MTU / 24 < max_pdu_samples)
				{
					max_pdu_samples = (byte)(lrq.Transporte.MTU / 24);
				}

				// finalmente, aceptamos el dispositivo.
				var la = new LoginAceptado
							 {
								 IdDispositivo = d.Id_short,
								 IdAsignado = d.Id_short,
								 Seq = pdu.Seq,
								 MaxPDUSamples = max_pdu_samples,
								 FlushTimeout = flush_timeout,
								 RetrieveFlags = retrieve_flags,
								 CL = d.LoginAcceptedQueryType,
								 IdAnterior = lrq.IdDispositivo,
							 };
				pdu.IdDispositivo = d.Id_short; //creo que aca me jodiste :P
				return la;
			}
			return null;
		}

		public override void RespuestaEntregada(PDU pdu, Transaccion tr, Transporte t)
		{
			throw new NotImplementedException();
		}

		public override void RespuestaNoEntregada(PDU pdu, Transaccion tr, Transporte t)
		{
			throw new NotImplementedException();
		}

		public override PDU RequieroACK(PDU pdu, Transaccion tr, Transporte t)
		{
			throw new NotImplementedException();
		}

		public override void SolicitudNoEnviada(PDU source, Transaccion tr, Transporte t)
		{
			var d = Devices.I().FindById(source.IdDispositivo);
			if (d != null) d.SolicitudNoEnviada(source);
			else STrace.Debug(typeof(ServerTU).FullName, String.Format("dispositivo no existe Id={0}", source.IdDispositivo));
		}

		public override void SolicitudEntregada(PDU response, PDU source, Transaccion tr, Transporte t)
		{
			var d = Devices.I().FindById(source.IdDispositivo);
			if (d != null) d.SolicitudEntregada(response, source);
			else STrace.Debug(typeof(ServerTU).FullName, String.Format("dispositivo no existe Id={0}", source.IdDispositivo));
		}

		public override void SolicitudCancelada(PDU source, Transaccion tr, Transporte t)
		{
			var d = Devices.I().FindById(source.IdDispositivo);
			if (d != null) d.SolicitudCancelada(source);
			else STrace.Debug(typeof(ServerTU).FullName, String.Format("dispositivo no existe Id={0}", source.IdDispositivo));
		}

		#region Evento AutoReport

		public delegate bool AutoReportHandler(object sender, Posicion autoreport);

		public event AutoReportHandler AutoReport;

		#endregion Evento AutoReport

		#region Evento RFID Detectado en Dispositivo

		public delegate bool RFIDDetectedHandler(object sender, RFIDDetectado pdu);

		public event RFIDDetectedHandler RFIDDetected;

		#endregion Evento RFID Detectado en Dispositivo

		#region Evento Exceso de Velocidad

		public delegate bool ExcesoVelocidadHandler(object sender, ExcesoVelocidad pdu);

		public event ExcesoVelocidadHandler ExcesoVelocidad;

		#endregion Evento Exceso de Velocidad

		#region Evento Generico

		public delegate bool EventoHandler(object sender, Evento pdu);

		public event EventoHandler Evento;

		#endregion Evento Generico

		#region Primitivas para FOTA

		public void FlashOverTheAir(short devid, string file)
		{
			var d = Devices.I().FindById(devid);
			var skip = (d.FotaCanceledAtPage <= (d.DataPageWindowSize * 3)
							? 0
							: d.FotaCanceledAtPage - (d.DataPageWindowSize * 3));
			d.AutoEvento(MessageIdentifier.FotaStart, skip); // FOTA INICIADO.
			d.FotaCanceledAtPage = 0;
			FlashOverTheAir(devid, file, skip);
		}

		private static void FlashOverTheAir(short devid, string file, int skip_pages)
		{
			var d = Devices.I().FindById(devid);

			if (!File.Exists(file))
			{
				STrace.Debug(typeof(ServerTU).FullName, String.Format("DEVICE[{0}]/FOTA: el archivo '{1}' no existe o no hay permisos.", d.LogId, file));
			}

			if (skip_pages != 0)
			{
				STrace.Debug(typeof(ServerTU).FullName, String.Format("DEVICE[{0}]/FOTA: reinciando fota desde pagina={1}", d.LogId, skip_pages));
			}

			short pagina = 0;

			using (var fs = File.OpenRead(file))
			{
				var b = new byte[512];
				while (fs.Read(b, 0, 512) > 0)
				{
					if (pagina < skip_pages)
					{
						pagina++;
						continue;
					}
					var pdu = new DataPage
								  {
									  CL = (byte)(d.SupportExtendedDataPage ? 0x02 : 0x00),
									  IdDispositivo = devid,
									  Transporte = d.Transporte,
									  Destino = d.Destino,
									  Pagina = pagina++,
									  Buffer = new byte[512]
								  };
					Array.Copy(b, pdu.Buffer, 512);
					d.FotaEnqueue(pdu);
				}
			}

			using (var fs = File.OpenRead(file))
			{
				var md5Hasher = MD5.Create();
				var b = md5Hasher.ComputeHash(fs);
				if (b.GetLength(0) != 16) throw new Exception("el hash md5 no retorna 16 bytes.");
				var pdu = new DataPage
							  {
								  CL = (byte)(d.SupportExtendedDataPage ? 0x04 : 0x01),
								  IdDispositivo = devid,
								  Transporte = d.Transporte,
								  Destino = d.Destino,
								  Pagina = pagina,
								  Buffer = b
							  };
				d.FotaEnqueue(pdu);
				if (d.fota_q != null) d.TotalDePaginasFota = (short)d.fota_q.Count;
			}

			if (d.SupportsFota)
			{
				var lapse = Hacker.Network.DATA_PAGE_INITIAL_LAPSE;
				for (var i = 0; i < d.DataPageWindowSize; ++i)
				{
					d.RunFotaQueue();
					Thread.Sleep(lapse);
					lapse = Hacker.Network.DATA_PAGE_LAPSE;
				}
			}
			else
			{
				d.RunFotaQueue();
			}
		}

		#endregion Primitivas para FOTA

		#region Rutinas del Acarreo

		public void Command(short devid, Transporte t, byte tipo, byte[] datos)
		{
			var d = Devices.I().FindById(devid);
			if (d == null)
			{
				throw new NullReferenceException(String.Format("COMMAND: El dispositivo no existe dev_id={0}.", devid));
			}
			var req = new Command(tipo)
						  {
							  IdDispositivo = devid,
							  Datos = datos,
							  Destino = d.Destino
						  };
			var mrc = new MRC(req, t, this);
			// Seq, automatico aqui.
			t.NuevaTransaccion(mrc, req);
			mrc.Start();
		}

		#endregion Rutinas del Acarreo

		// ReSharper disable RedundantDefaultFieldInitializer
		public int FotaQueueDelay = 0; //500;
		// ReSharper restore RedundantDefaultFieldInitializer

		#region Propiedades del Objeto

		// ReSharper disable UnusedPrivateMember
		// ReSharper disable UnusedMember.Local
		private byte ProximaSecuencia
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedPrivateMember
		{
			get
			{
				if (proximaSecuencia < 0x7F)
				{
					proximaSecuencia = 0x80;
					return proximaSecuencia;
				}
				return proximaSecuencia++;
			}
		}

		public override byte LimiteSuperiorSeq
		{
			get { return 0xFF; }
		}

		public override byte LimiteInferiorSeq
		{
			get { return 0x80; }
		}

		#endregion Propiedades del Objeto

		private byte proximaSecuencia = 0x80;

		public void RaiseEvento(Evento evt)
		{
			if (Evento != null)
			{
				Evento(this, evt);
			}
		}
	}
}