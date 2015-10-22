using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Scheduler.Tasks.Logiclink2.Strategies;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;
using System.Linq;
using System.Text;
using Logictracker.Services.Helpers;

namespace Logictracker.Scheduler.Tasks.Logiclink2
{
    public class Task : BaseTask
    {
        private Dictionary<int, List<int>> _empresasLineas = new Dictionary<int, List<int>>();
        private const string Component = "Logiclink2";

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

            var archivoPendiente = DaoFactory.LogicLinkFileDAO.GetNextPendiente(IdEmpresa);
            if (archivoPendiente != null)
            {
                STrace.Trace(Component, "Archivo a procesar: " + archivoPendiente.FilePath);

                var te = new TimeElapsed();
                ProcessArchivo(archivoPendiente);
                STrace.Trace(Component, "Archivo procesado en: " + te.getTimeElapsed().TotalSeconds + " segundos.");
            }
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
                            _empresasLineas = DistribucionFemsa.Parse(archivo, out rutas, out entregas);
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionQuilmes:
                            _empresasLineas = DistribucionQuilmes.Parse(archivo, out rutas, out entregas, out observaciones);
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            if (observaciones != string.Empty) result = result + " (" + observaciones + ")";
                            break;
                        case LogicLinkFile.Estrategias.DistribucionMusimundo:
                            _empresasLineas = DistribucionMusimundo.Parse(archivo, out rutas, out entregas, out observaciones);
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            if (observaciones != string.Empty) result = result + " (" + observaciones + ")";
                            break;
                        case LogicLinkFile.Estrategias.DistribucionBrinks:
                            //EmpresasLineas = DistribucionBrinks.Parse(archivo, out rutas, out entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionSos:
                            viaje = DistribucionSos.Parse(archivo, out rutas, out entregas);
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionReginaldLee:
                            _empresasLineas = DistribucionReginaldLee.Parse(archivo, out rutas, out entregas);
                            result = string.Format("Archivo procesado exitosamente. Rutas: {0} - Entregas: {1}", rutas, entregas);
                            break;
                        case LogicLinkFile.Estrategias.DistribucionCCU:
                            var extension = GetFileName(archivo.FilePath);
                            switch (extension)
	                        {
                                case "Rutas.xlsx":
                                    _empresasLineas = DistribucionCCU.ParseRutas(archivo, out rutas, out entregas, out observaciones);
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
                                    AddReferenciasGeograficas(detalle.ReferenciaGeografica);
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

                    if (_empresasLineas.Count > 0)
                    {
                        DaoFactory.ReferenciaGeograficaDAO.UpdateGeocercas(_empresasLineas);
                        _empresasLineas.Clear();
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

        private void AddReferenciasGeograficas(ReferenciaGeografica rg)
        {
            if (rg == null)
                STrace.Error(Component, "AddReferenciasGeograficas: rg is null");
            else if (rg.Empresa == null)
                STrace.Error(Component, "AddReferenciasGeograficas: rg.Empresa is null");
            else
            {
                if (!_empresasLineas.ContainsKey(rg.Empresa.Id))
                    _empresasLineas.Add(rg.Empresa.Id, new List<int> { -1 });

                if (rg.Linea != null)
                {
                    if (!_empresasLineas[rg.Empresa.Id].Contains(rg.Linea.Id))
                        _empresasLineas[rg.Empresa.Id].Add(rg.Linea.Id);
                }
                else
                {
                    var todaslaslineas = DaoFactory.LineaDAO.GetList(new[] { rg.Empresa.Id });
                    foreach (var linea in todaslaslineas)
                    {
                        if (!_empresasLineas.ContainsKey(linea.Id))
                            _empresasLineas[rg.Empresa.Id].Add(linea.Id);
                    }
                }
            }
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
