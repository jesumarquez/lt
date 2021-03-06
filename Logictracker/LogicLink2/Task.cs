﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Cache;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Scheduler.Tasks.Logiclink2.Strategies;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Logiclink2
{
    public class Task : BaseTask
    {
        private const string Component = "Logiclink2";
        private bool _update;
        private DateTime _lastUpdate;

        private int IdEmpresa
        {
            get
            {
                var empresa = GetInt32("Empresa");
                return empresa.HasValue ? empresa.Value : -1;
            }
        }

        protected override void OnExecute(Timer timer)
        {
            if (IdEmpresa <= 0) return;

            var keyUpdate = Component + "_update_" + IdEmpresa;
            var keyLastUpdate = Component + "_lastUpdate_" + IdEmpresa;

            if (LogicCache.KeyExists(typeof(bool), keyUpdate))
                _update = (bool) LogicCache.Retrieve<object>(typeof(bool), keyUpdate);

            STrace.Trace(Component, string.Format("Init Update: {0}", _update ? "TRUE" : "FALSE"));

            if (LogicCache.KeyExists(typeof(DateTime), keyLastUpdate))
                _lastUpdate = (DateTime) LogicCache.Retrieve<object>(typeof(DateTime), keyLastUpdate);

            //var archivoPendiente = DaoFactory.LogicLinkFileDAO.GetNextPendiente(IdEmpresa);
            //if (archivoPendiente != null)
            //{
            //    STrace.Trace(Component, "Archivo a procesar: " + archivoPendiente.FilePath);
            //
            //    var te = new TimeElapsed();
            //    ProcessArchivo(archivoPendiente);
            //    STrace.Trace(Component, "Archivo procesado en: " + te.getTimeElapsed().TotalSeconds + " segundos.");
            //}

            var archivosPendientes = DaoFactory.LogicLinkFileDAO.GetPendientes(IdEmpresa).ToList();
            STrace.Trace(Component, "Archivos pendientes: " + archivosPendientes.Count());

            //var archivo = DaoFactory.LogicLinkFileDAO.FindById();
            //archivosPendientes.Add(archivo);

            foreach (var archivoPendiente in archivosPendientes)
            {
                STrace.Trace(Component, "Archivo a procesar: " + archivoPendiente.FilePath);
                
                var te = new TimeElapsed();
                ProcessArchivo(archivoPendiente);
                STrace.Trace(Component, "Archivo procesado en: " + te.getTimeElapsed().TotalSeconds + " segundos.");    
            }

            var empresa = DaoFactory.EmpresaDAO.FindById(IdEmpresa);
            var now = DateTime.UtcNow;
            var lastUpdateMinutes = now.Subtract(_lastUpdate).TotalMinutes;

            STrace.Trace(Component, string.Format("Last Update: {0} - {1} minutos", _lastUpdate.ToString("dd/MM/yyyy HH:mm:ss"), lastUpdateMinutes));
            STrace.Trace(Component, string.Format("Update: {0}", _update ? "TRUE" : "FALSE"));

            if (lastUpdateMinutes > empresa.LogiclinkMinutosUpdate && _update)
            {
                var lineas = new List<int>();
                var dict = new Dictionary<int, List<int>>();

                var todaslaslineas = DaoFactory.LineaDAO.GetList(new[] { IdEmpresa });
                lineas.Add(-1);
                lineas.AddRange(todaslaslineas.Select(l => l.Id));
                dict.Add(IdEmpresa, lineas);

                DaoFactory.ReferenciaGeograficaDAO.UpdateGeocercas(dict);
                _update = false;
                _lastUpdate = now;
            }

            STrace.Trace(Component, string.Format("Store Update: {0}", _update ? "TRUE" : "FALSE"));

            LogicCache.Store(typeof(bool), keyUpdate, _update);
            LogicCache.Store(typeof(DateTime), keyLastUpdate, _lastUpdate);
        }

        public void ProcessArchivo(LogicLinkFile archivo)
        {
            archivo.Status = LogicLinkFile.Estados.Procesando;
            DaoFactory.LogicLinkFileDAO.SaveOrUpdate(archivo);

            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    STrace.Trace(Component, "Procesando con Estrategia: " + archivo.Strategy);
                    var rutas = 0;
                    var entregas = 0;
                    var clientes = 0;
                    var asignaciones = 0;
                    var result = string.Empty;
                    var observaciones = string.Empty;
                    ViajeDistribucion viaje = null;

                    switch (archivo.Strategy)
                    {
                        case LogicLinkFile.Estrategias.DistribucionFemsa:
                            DistribucionFemsa.Parse(archivo, out rutas, out entregas);
                            _update = true;
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionQuilmes:
                            DistribucionQuilmes.Parse(archivo, out rutas, out entregas, out observaciones);
                            _update = true;
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            if (observaciones != string.Empty) result = result + " (" + observaciones + ")";
                            break;
                        case LogicLinkFile.Estrategias.DistribucionMusimundo:
                            DistribucionMusimundo.Parse(archivo, out rutas, out entregas, out observaciones);
                            _update = true;
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            if (observaciones != string.Empty) result = result + " (" + observaciones + ")";
                            break;
                        case LogicLinkFile.Estrategias.DistribucionBrinks:
                            //DistribucionBrinks.Parse(archivo, out rutas, out entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionSos:
                            viaje = DistribucionSos.Parse(archivo, out rutas, out entregas);
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionReginaldLee:
                            DistribucionReginaldLee.Parse(archivo, out rutas, out entregas);
                            _update = true;
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionCCU:
                            var extension = GetFileName(archivo.FilePath);
                            switch (extension)
	                        {
                                case "Rutas.xlsx":
                                    DistribucionCCU.ParseRutas(archivo, out rutas, out entregas, out observaciones);
                                    _update = true;
                                    result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                                    break;
                                case "Clientes.xlsx":
                                    DistribucionCCU.ParseClientes(archivo, out clientes);
                                    result = string.Format("Archivo procesado exitosamente. Clientes: {0}", clientes);
                                    break;
                                case "Asigno.xlsx":
                                    DistribucionCCU.ParseAsignaciones(archivo, out asignaciones, out observaciones);
                                    result = string.Format("Archivo procesado exitosamente. Asignaciones: {0}", asignaciones);
                                    break;
                                default:
                                    result = string.Format("Extensión '" + extension + "' no válida.");
                                    break;
	                        }
                            break;
                        case LogicLinkFile.Estrategias.PedidosPetrobras:
                            var pedidos = 0;
                            PedidosPetrobras.Parse(archivo, out pedidos, out observaciones);
                            result = string.Format("Archivo procesado exitosamente. Pedidos: {0}", pedidos);
                            if (observaciones != string.Empty) result = result + " (" + observaciones + ")";
                            break;
                        case LogicLinkFile.Estrategias.AsignacionClienteEmpleado:
                            var asignados = 0;
                            AsignacionClienteEmpleado.Parse(archivo, out asignados, out observaciones);
                            result = string.Format("Archivo procesado exitosamente. Asignados: {0}", asignados);
                            if (observaciones != string.Empty) result = result + " (" + observaciones + ")";
                            break;
                        case LogicLinkFile.Estrategias.AsignacionCodigoViaje:
                            var viajes = 0;
                            AsignacionCodigoViaje.Parse(archivo, out viajes, out observaciones);
                            result = string.Format("Archivo procesado exitosamente. Viajes: {0}", viajes);
                            if (observaciones != string.Empty) result = result + " (" + observaciones + ")";
                            break;
                    }

                    archivo.Status = LogicLinkFile.Estados.Procesado;
                    archivo.DateProcessed = DateTime.UtcNow;
                    archivo.Result = result;
                    DaoFactory.LogicLinkFileDAO.SaveOrUpdate(archivo);

                    transaction.Commit();

                    if (archivo.Strategy == LogicLinkFile.Estrategias.DistribucionSos)
                    {
                        if (viaje != null)
                        {
                            foreach (var detalle in viaje.Detalles)
                            {
                                if (detalle.PuntoEntrega != null && detalle.PuntoEntrega.ReferenciaGeografica != null)
                                    _update = true;
                            }

                            if (viaje.Vehiculo != null && viaje.Estado == ViajeDistribucion.Estados.Pendiente)
                            {
                                var tieneViajeEnCurso = DaoFactory.ViajeDistribucionDAO.FindEnCurso(viaje.Vehiculo) != null;
                                if (!tieneViajeEnCurso)
                                {
                                    //var ciclo = new CicloLogisticoDistribucion(viaje, DaoFactory,
                                    //    new MessageSaver(DaoFactory));
                                    //var evento = new InitEvent(DateTime.UtcNow);
                                    //ciclo.ProcessEvent(evento);
                                }
                                if (viaje.Vehiculo.Dispositivo != null)
                                {   
                                    var msg = new StringBuilder();
                                    var desde = viaje.Detalles[1].ReferenciaGeografica;
                                    var hasta = viaje.Detalles[2].ReferenciaGeografica;
                                    msg.Append("Viaje: " + viaje.Codigo);
                                    msg.Append("<br>Desde: " + GeocoderHelper.GetDescripcionEsquinaMasCercana(desde.Latitude, desde.Longitude));
                                    msg.Append("<br>Hasta: " + GeocoderHelper.GetDescripcionEsquinaMasCercana(hasta.Latitude, hasta.Longitude));

                                    var message = MessageSender.CreateSubmitTextMessage(viaje.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
                                    message.AddMessageText(msg.ToString());
                                    message.Send();

                                    //STrace.Trace(Component, "Ruta Aceptada: " + MessageCode.RutaAceptada.ToString());
                                    //STrace.Trace(Component, "Ruta Rechazada: " + MessageCode.RutaRechazada.ToString());

                                    //var msgText = "Acepta la asignacion del servicio <br><b>" + viaje.Codigo + "</b>?";
                                    //var replies = new[] { Convert.ToUInt32(MessageCode.RutaAceptada.ToString()), 
                                    //                      Convert.ToUInt32(MessageCode.RutaRechazada.ToString()) };
                                    //message = MessageSender.CreateSubmitCannedResponsesTextMessage(viaje.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
                                    //message.AddMessageText(msgText)
                                    //       .AddTextMessageId(Convert.ToUInt32(viaje.Id))
                                    //       .AddCannedResponses(replies)
                                    //       .AddAckEvent(MessageCode.GarminCannedMessageReceived.GetMessageCode());

                                    //message.Send();
                                }
                            }
                        }
                    }
                    
                    STrace.Trace(Component, result);
                }
                catch (Exception ex)
                {
                    AddError(ex);
                    transaction.Rollback();
                    ClearData();

                    archivo.Status = LogicLinkFile.Estados.Cancelado;
                    var result = "Archivo procesado erroneamente. Exception: " + ex.Message;
                    archivo.Result = result;
                    DaoFactory.LogicLinkFileDAO.SaveOrUpdate(archivo);

                    STrace.Trace(Component, result);
                }
                finally
                {
                    ClearData();
                }
            }
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }
        
        private string GetFileName(string filePath)
        {
            var filename = string.Empty;

            if (filePath.Contains('@'))
                filename = filePath.Split('@').Last();

            return filename;
        }
    }
}
