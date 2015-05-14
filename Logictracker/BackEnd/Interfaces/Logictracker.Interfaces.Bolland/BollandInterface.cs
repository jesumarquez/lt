using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Logictracker.AVL.Messages;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Layers.MessageQueue;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.IAgent;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace Logictracker.Interfaces.Bolland
{
	[FrameworkElement(XName = "BollandInterface", IsContainer = true)]
	public class BollandInterface : FrameworkElement, IService
	{
		#region Attributes

		[ElementAttribute(XName = "Dispatcher", IsSmartProperty = true, IsRequired = true)]
		public IDispatcherLayer Dispatcher
        {
            get { return (IDispatcherLayer)GetValue("Dispatcher"); }
			set { SetValue("Dispatcher", value); }
        }

		[ElementAttribute(XName = "Dir", IsSmartProperty = true, IsRequired = true)]
		public String Dir
		{
			get { return (String)GetValue("Dir"); }
			set { SetValue("Dir", value); }
		}

		[ElementAttribute(XName = "DirOk", IsSmartProperty = true, IsRequired = true)]
		public String DirOk
		{
			get { return (String)GetValue("DirOk"); }
			set { SetValue("DirOk", value); }
		}

		[ElementAttribute(XName = "DirErrores", IsSmartProperty = true, IsRequired = true)]
		public String DirErrores
		{
			get { return (String)GetValue("DirErrores"); }
			set { SetValue("DirErrores", value); }
		}

		[ElementAttribute(XName = "TimerSeconds", IsSmartProperty = true, IsRequired = true)]
		public int TimerSeconds
		{
			get { return (int)GetValue("TimerSeconds"); }
			set { SetValue("TimerSeconds", value); }
		}

		[ElementAttribute(XName = "MessageQueueMaxMessages", IsSmartProperty = true, IsRequired = true)]
		public int MessageQueueMaxMessages
		{
			get { return (int)GetValue("MessageQueueMaxMessages"); }
			set { SetValue("MessageQueueMaxMessages", value); }
		}

		[ElementAttribute(XName = "MessageQueue", IsSmartProperty = true, IsRequired = true)]
		public IMessageQueue MessageQueue
		{
			get { return (IMessageQueue)GetValue("MessageQueue"); }
			set { SetValue("MessageQueue", value); }
		}

		#endregion

        #region Public Methods

        public bool ServiceStart()
        {
			try
			{
				_seguir = true;
				_worker = new Thread(DoWork);
				_worker.Start();
				return true;
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e, "Exception during startup");
				return false;
			}
        }

		public bool ServiceStop()
        {
			try
			{
				_seguir = false;
				_worker.Interrupt();
				_worker.Abort();

				return true;
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e, "Exception during stop");
				return false;
			}
		}

        #endregion

        #region Private Members

		private void DoWork()
		{
			while (_seguir)
			{
				var files = Directory.GetFiles(Dir, "*.txt", SearchOption.TopDirectoryOnly);

				if ((MessageQueue.GetCount() < MessageQueueMaxMessages) && (files.Length > 0))
				{
					foreach (var filepath in files)
					{
						var file = Path.GetFileName(filepath);
						STrace.Debug(GetType().FullName, String.Format("Procesando: name={0} path={1}", file, filepath));
						if (!File.Exists(filepath))
						{
							STrace.Debug(GetType().FullName, String.Format("!Exists: {0}", filepath));
							continue;
						}
						if (file.StartsWith("G"))
						{
							try
							{
								ProcessGps(filepath);
								STrace.Debug(GetType().FullName, String.Format("GPS file terminado: {0}", file));
							}
							catch (Exception e)
							{
								STrace.Exception(GetType().FullName, e, String.Format("GPS file con error: {0}", file));
							}
						}
						else if (file.StartsWith("0"))
						{
							try
							{
								ProcessTaco(filepath);
								STrace.Debug(GetType().FullName, String.Format("Taco file terminado: {0}", file));
							}
							catch (Exception e)
							{
								STrace.Exception(GetType().FullName, e, String.Format("Taco file con error: {0}", file));
							}
						}
						else
						{
							STrace.Debug(GetType().FullName, String.Format("ignored file: {0}", file));
							File.Move(filepath, filepath.Replace(Dir, DirErrores).Replace(".txt", "_ignored.log"));
						}
					}
				}
				Thread.Sleep(TimerSeconds * 1000);
			}
		}

		private void ProcessGps(String file)
		{
			Empresa empresa = null;
			Dispositivo dev = null;
			var errorslist = new List<String>();
			var infoList = new List<String>();
			var moveAllFile = false;
			var lastdt = DateTime.MinValue;
			var latestdt = DateTime.MinValue;
			var repro = file.Contains("_r_");

			using (var sr = new StreamReader(file))
			{
				String line;
				while ((moveAllFile == false) && ((line = sr.ReadLine()) != null))
				{
					var datos = line.Split('@');
					try
					{
						switch (datos[0])
						{
							//default:
								//STrace.Trace(GetType().FullName, "Tipo de linea ignorada: {0}", line);
								//break;
							case "1": //info
								{
									infoList.Add(line);
									try
									{
										switch (datos[1])
										{
											//default:
												//STrace.Trace(GetType().FullName, "Tipo de linea de info ignorada: {0}", line);
												//break;
											case "3": //"Nombre de la Empresa"
												empresa = DaoFactory.EmpresaDAO.FindByCodigo(datos[2]);
												if (empresa == null)
												{
													throw new InvalidDataException(String.Format("No se encontro la empresa: {0}", datos[2]));
												}
												break;
											case "4": //"Fecha Y Hora de la Captura"
												//generar un evento?
												break;
											case "13": //"Numero de Movil"
												if (empresa == null)
												{
													throw new InvalidDataException("Primero declare la empresa!");
												}
												dev = DaoFactory.CocheDAO.FindByInterno(new List<int> {empresa.Id}, null, datos[2].Substring(0, datos[2].Length - 2)).Dispositivo;
												if (dev == null)
												{
													throw new InvalidDataException(String.Format("No se encontro el dispositivo o el movil: {0}", datos[2]));
												}
												lastdt = GetLastDt(dev);
												break;
										}
									}
									catch
									{
										moveAllFile = true;
										throw;
									}
									break;
								}

							case "2": //data
								{
									if (dev == null)
									{
										moveAllFile = true;
										throw new ArgumentNullException("", "Primero declare el movil!");
									}
									try
									{
										Debug.Assert(line.StartsWith("2@100@0@"));
										DispatcherDispatch(GPSPoint.Factory(
																DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss"),
																Convert.ToSingle(datos[5], CultureInfo.InvariantCulture),
																Convert.ToSingle(datos[6], CultureInfo.InvariantCulture),
																Convert.ToSingle(datos[7], CultureInfo.InvariantCulture)).ToPosition(dev.Id, 0), lastdt, repro, ref latestdt);
									}
									catch
									{
										errorslist.Add(line);
										throw;
									}
									break;
								}
						}
					}
					catch (Exception e)
					{
						STrace.Exception(GetType().FullName, e, dev.GetId());
					}
				}
				sr.Close();
			}
			Finishit(file, infoList, moveAllFile, errorslist, latestdt, dev.Id);
		}

		private void ProcessTaco(String file)
		{
			Empresa empresa = null;
			Dispositivo dev = null;
			var errorslist = new List<String>();
			var infoList = new List<String>();
			var moveAllFile = false;
			var maxvel = 0;
			var maxrpm = 0;
			String chofer = null;
			var lastdt = DateTime.MinValue;
			var latestdt = DateTime.MinValue;
			var repro = file.Contains("_r_");

			using (var sr = new StreamReader(file))
			{
				String line;
				while ((moveAllFile == false) && ((line = sr.ReadLine()) != null))
				{
					var datos = line.Split('@');
					try
					{
						switch (datos[0])
						{
							//default:
								//STrace.Trace(GetType().FullName, "Tipo de linea ignorada: {0}", line);
								//break;
							case "1": //info
								{
									infoList.Add(line);
									try
									{
										switch (datos[1])
										{
											//default:
												//STrace.Trace(GetType().FullName, "Tipo de linea de info ignorada: {0}", line);
												//break;
											case "3": //"Nombre de la Empresa"
												empresa = DaoFactory.EmpresaDAO.FindByCodigo(datos[2]);
												if (empresa == null)
												{
													throw new InvalidDataException(String.Format("No se encontro la empresa: {0}", datos[2]));
												}
												break;
											case "4": //"Fecha Y Hora de la Captura"
												//generar un evento?
												break;
											case "6": //"Numero de Movil"
												if (empresa == null)
												{
													throw new InvalidDataException("Primero declare la empresa!");
												}
												dev = DaoFactory.CocheDAO.FindByInterno(new List<int> {empresa.Id}, null, datos[2]).Dispositivo;
												if (dev == null)
												{
													throw new InvalidDataException(String.Format("No se encontro el dispositivo o el movil: {0}", datos[2]));
												}
												lastdt = GetLastDt(dev);
												break;
											case "11": //"Velocidad Maxima Programada"
												maxvel = Convert.ToInt32(datos[2], CultureInfo.InvariantCulture);
												break;
											case "12": //"RPM Maxima Programada"
												maxrpm = Convert.ToInt32(datos[2], CultureInfo.InvariantCulture);
												break;
										}
									}
									catch
									{
										moveAllFile = true;
										throw;
									}
									break;
								}

							case "2": //data
								{
									if (dev == null)
									{
										moveAllFile = true;
										throw new ArgumentNullException("", "Primero declare el movil!");
									}
									// usar el odometro para algo?
									try
									{
										switch (datos[1])
										{
												//default:
												//STrace.Trace(GetType().FullName, "Tipo de linea de datos ignorada: {0}", line);
												//break;
											case "1": //"Dato simple"
												{
													//2@1@0@20111118@144100@27@270     		"Evento;R;Fecha;Hora;Velocidad;Kilometros"  
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													//var code = (MessageIdentifier) Convert.ToInt16(datos[2], CultureInfo.InvariantCulture);
													//Dispatcher_Dispatch(code.FactoryEvent(dev.Id, 0, null, dt, null, null));
													////
													ReportOdometer(datos[6], datos[5], dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "2": //"Motor apagado"
												{
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													DispatcherDispatch(MessageIdentifier.EngineOffInternal.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													//
													ReportOdometer(datos[6], datos[5], dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "3": //"Motor encendido"
												{
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													DispatcherDispatch(MessageIdentifier.EngineOnInternal.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													//
													ReportOdometer(datos[6], datos[5], dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "4": //"Aceleracion Brusca"
												{
													//2@4@23418@20111202@135000@0@874         	"Evento;CHOFER;Fecha;Hora;R;Kilometros"
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifier.AccelerationEvent.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													//
													ReportOdometer(datos[6], null, dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "5": //"Desaceleracion Brusca"
												{
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifier.DesaccelerationEvent.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													//
													ReportOdometer(datos[6], null, dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "6": //"Inicio Infraccion"
												{
													//2@6@23418@20111203@203405@23418@880     	"Evento;CHOFER;Fecha;Hora;CHOFER;Kilometros"
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifier.SpeedingTicketInit.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													//
													ReportOdometer(datos[6], null, dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "7": //"Fin Infraccion"
												{
													//2@7@23418@20111203@203405@195@2         	"Evento;CHOFER;Fecha;Hora;Velocidad MAXIMA;minutos de infraccion"
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													var lista = new List<Int64>
														{
															Convert.ToInt32(datos[5], CultureInfo.InvariantCulture), //reached speed
															Convert.ToInt32(datos[6], CultureInfo.InvariantCulture)*60, //duration
															maxvel,
														};

													DispatcherDispatch(MessageIdentifier.SpeedingTicketEnd.FactoryEvent(dev.Id, 0, null, dt, chofer, lista), lastdt, repro, ref latestdt);
													break;
												}
											case "9": //"Desconexion"
												{
													//2@9@23418@20111119@203405@213000@20111119@0     "Evento;CHOFER;Fecha(reco);Hora;Hora(desco);Fecha;R"
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													var dt2 = DateTimeUtils.SafeParseFormat(datos[6] + datos[5].PadLeft(6, '0'), "yyyyMMddHHmmss");
													DispatcherDispatch(MessageIdentifier.SensorPowerDisconected.FactoryEvent(dev.Id, 0, null, dt2, chofer, null), lastdt, repro, ref latestdt);
													DispatcherDispatch(MessageIdentifier.SensorPowerReconected.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													break;
												}
											case "10": //"Inicio RPM Maxima"
												{
													//2@10@23418@20111119@203405@0@339        	"Evento;R;Fecha;Hora;Velocidad;R;Kilometros"
													//todo, este ejemplo tomado del mail no concuerda con su descripcion...
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													//var chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifier.RpmTicketInit.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													//
													ReportOdometer(datos[6], datos[5], dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "11": //"Final RPM Maxima"
												{
													//2@11@23418@20111119@203500@0@10300      	"Evento;R;Fecha;Hora;Velocidad;R;RPM MAXIMA"
													//todo, este ejemplo tomado del mail no concuerda con su descripcion...
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													//var chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifier.RpmTicketEnd.FactoryEvent(dev.Id, 0, null, dt, chofer, new List<Int64> { Convert.ToInt32(datos[6], CultureInfo.InvariantCulture), maxrpm }), lastdt, repro, ref latestdt);
													//
													ReportOdometer(null, datos[5], dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
											case "12": //"Inicio Chofer"
												{
													//2@12@23418@20111120@132302@0@23418      	"Evento;CHOFER;Fecha;Hora;R;CHOFER;"
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifierX.FactoryRfid(dev.Id, 0, null, dt, chofer, 0), lastdt, repro, ref latestdt);
													break;
												}
											case "13": //"Final Chofer"
												{
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifierX.FactoryRfid(dev.Id, 0, null, dt, chofer, 1), lastdt, repro, ref latestdt);
													chofer = null;
													break;
												}
											case "15": //"Servicio Tecnico"
												{
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													DispatcherDispatch(MessageIdentifier.TechnicalService.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													break;
												}
											case "30": //"Movimiento desde hasta y minutos de marcha"
												{
													//2@30@23418@20111119@81800@85200@34      	"Evento;CHOFER;Fecha;Hora INI;Hora FIN;Minutos de marcha"
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[5].PadLeft(6, '0'), "yyyyMMddHHmmss");
													var lista = new List<Int64>
														{
															(Int32) (DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss") - dt).TotalSeconds, //duration evento
															Convert.ToInt32(datos[6], CultureInfo.InvariantCulture)*60, //durationmarcha
														};
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifier.TraveledLeg.FactoryEvent(dev.Id, 0, null, dt, chofer, lista), lastdt, repro, ref latestdt);
													break;
												}
											case "31": //"Ultimo dato de detenido?"
												{
													//2@31@23418@20111119@85200@0@327         	"Evento;CHOFER;Fecha;Hora;R;Kilometros"
													var dt = DateTimeUtils.SafeParseFormat(datos[3] + datos[4].PadLeft(6, '0'), "yyyyMMddHHmmss");
													chofer = GetChoferPin(empresa, datos[2], dev.Id);
													DispatcherDispatch(MessageIdentifier.StoppedEvent.FactoryEvent(dev.Id, 0, null, dt, chofer, null), lastdt, repro, ref latestdt);
													//
													ReportOdometer(datos[6], null, dev, dt, chofer, lastdt, repro, ref latestdt);
													break;
												}
										}
									}
									catch
									{
										errorslist.Add(line);
										throw;
									}
									break;
								}
						}
					}
					catch (Exception e)
					{
						STrace.Exception(GetType().FullName, e, dev.GetId());
					}
				}
				sr.Close();
			}
			Finishit(file, infoList, moveAllFile, errorslist, latestdt, dev.Id);
		}

		private void DispatcherDispatch(IMessage msg, DateTime lastdt, Boolean repro, ref DateTime latestdt)
		{
			if (msg == null)
			{
				STrace.Debug(GetType().FullName, "Mensaje vacio");
				return;
			}
			if (repro || lastdt < msg.Tiempo)
			{
				latestdt = msg.Tiempo;
				Dispatcher.Dispatch(msg);
			}
			else
			{
				STrace.Debug(GetType().FullName, msg.DeviceId, String.Format("Igorando por tiempo menor a ultimo procesado, Dev={0}, lastdt={1} msg.Tiempo={2}", msg.DeviceId, lastdt, msg.Tiempo));
			}
		}

		private DateTime GetLastDt(Dispositivo dev)
		{
			DateTime dts;
			if (Lastdts.ContainsKey(dev.Id))
			{
				dts = Lastdts[dev.Id];
				STrace.Debug(GetType().FullName, dev.Id, String.Format("LastDateTimeProcessed Dict: {0}", dts));
			}
			else
			{
				var basedts = DaoFactory.DetalleDispositivoDAO.GetLastDateTimeProcessedValue(dev.Id);
				dts = DateTimeUtils.SafeParseFormat(basedts, "yyyyMMddHHmmss");
				Lastdts.Add(dev.Id, dts);
			}
			return dts;
		}

		private void Finishit(String file, IEnumerable<string> infoList, bool moveAllFile, List<String> errorslist, DateTime latestdt, int deviceId)
		{
			try
			{
				if (latestdt.Year > 2000)
				{
					Lastdts[deviceId] = latestdt;
					SetDetalleDispositivo(deviceId, "LastDateTimeProcessed", latestdt.ToString("yyyyMMddHHmmss"), "string");
				}

				if (moveAllFile)
				{
					File.Copy(file, GetFile2(file, Dir, DirErrores));
				}
				else if (errorslist.Count > 0)
				{
					using (var sw = new StreamWriter(GetFile2(file, Dir, DirErrores)))
					{
						foreach (var line in infoList)
						{
							sw.WriteLine(line);
						}
						foreach (var line in errorslist)
						{
							sw.WriteLine(line);
						}
					}
				}
				DaoFactory.SessionClear();
			}
			finally
			{
				File.Move(file, file.Replace(Dir, DirOk));
			}
		}

		private static String GetFile2(String file, String dir, String dirErrores)
		{
			file = file.Replace(dir, dirErrores);
			if (file.Contains("_r_"))
			{
				int pasada;
				Int32.TryParse(file.Substring(file.Length - 7, 3), out pasada);
				return String.Format("{0}_{1:D3}.txt", file.Substring(0, file.IndexOf("_r_")), (pasada + 1) % 100);
			}

			return file.Replace(".txt", "_r_001.txt");
		}

		private void ReportOdometer(String dato1, String dato2, Dispositivo dev, DateTime dt, String chofer, DateTime lastdt, Boolean repro, ref DateTime latestdt)
		{
			var str = String.Empty;
			if (dato1 != null) str += String.Format(CultureInfo.InvariantCulture, @"Tacometro_1_Odometro:{0},", dato1);
			if (dato2 != null) str += String.Format(CultureInfo.InvariantCulture, @"Tacometro_1_Velocidad:{0},", dato2);
			var odo = MessageIdentifier.TacometerData.FactoryEvent(MessageIdentifier.TelemetricData, dev.Id, 0, null, dt, chofer, null);
			odo.SensorsDataString = str;
			DispatcherDispatch(odo, lastdt, repro, ref latestdt);
		}

		private String GetChoferPin(Empresa empresa, String legajo, int deviceId)
		{
			var empleado = DaoFactory.EmpleadoDAO.FindByLegajo(empresa.Id, 0, legajo);
			if (empleado == null)
			{
				//STrace.Debug(typeof(BollandInterface).FullName, DeviceId, "No se encontro empleado, legajo: {0}", legajo);
				return null;
			}
			//todo: en lugar de usar una tarjeta usar el legajo...
			var tarjeta = empleado.Tarjeta;
			if (tarjeta == null)
			{
				STrace.Debug(typeof(BollandInterface).FullName, deviceId, String.Format("Empleado sin tarjeta, legajo: {0}", empleado.Legajo));
				return null;
			}

			
			return tarjeta.Pin;
		}

		private void SetDetalleDispositivo(int deviceId, string name, string value, string type)
		{
			var msg = new SetDetail(deviceId, 0);
			msg.SetUserSetting(SetDetail.Fields.Name, name);
			msg.SetUserSetting(SetDetail.Fields.Value, value);
			msg.SetUserSetting(SetDetail.Fields.TipoDato, type);
			msg.SetUserSetting(SetDetail.Fields.Consumidor, "S");
			msg.SetUserSetting(SetDetail.Fields.Editable, "false");
			msg.SetUserSetting(SetDetail.Fields.Metadata, "");
			msg.SetUserSetting(SetDetail.Fields.ValorInicial, "0");
			Dispatcher.Dispatch(msg);
		}

		private static readonly Dictionary<int, DateTime> Lastdts = new Dictionary<int, DateTime>();
		private Thread _worker;
		private bool _seguir = true;
		private DAOFactory _daoFactory;
		private DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } } 

		#endregion
	}
}
