using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Utils;
using Logictracker.Messaging;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class DatamartDistribucionTask : BaseTask
    {
        private DateTime StartDate
        {
            get
            {
                var startDate = GetDateTime("Desde");
                return startDate.HasValue ? startDate.Value : DateTime.Today.AddDays(-1);
            }
        }
        private DateTime EndDate
        {
            get
            {
                var endDate = GetDateTime("Hasta");
                return endDate.HasValue ? endDate.Value : StartDate.AddHours(24);
            }
        }
        private bool Regenera
        {
            get
            {
                var regenera = GetBoolean("Regenera");
                return regenera.HasValue && regenera.Value;
            }
        }
        private IEnumerable<int> Empresas
        {
            get
            {
                var empresas = GetListOfInt("Empresas");
                return empresas.Any() ? empresas : new List<int> { -1 };
            }
        }

        protected override void OnExecute(Timer timer)
        {
            var regenera = Regenera;

            STrace.Trace(GetType().FullName, string.Format("Procesando distribuciones. Desde: {0} - Hasta: {1} - Regenera: {2} - Empresas: {3}", StartDate, EndDate, regenera, Empresas));

            var inicio = DateTime.UtcNow;

            try
            {
                var distribuciones = GetDistribuciones();
                var distribucionesPendientes = distribuciones.Count;
                STrace.Trace(GetType().FullName, string.Format("Distribuciones a procesar: {0}", distribucionesPendientes));

                foreach (var distribucion in distribuciones)
                {
                    try
                    {
                        ProcesarDistribucion(distribucion.Id, regenera);
                        STrace.Trace(GetType().FullName, string.Format("Distribuciones a procesar: {0}", --distribucionesPendientes));
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(GetType().FullName, ex);

                        var procesado = false;
                        var retry = 0;
                        while (!procesado && retry < 5)
                        {
                            retry++;
                            try
                            {
                                ProcesarDistribucion(distribucion.Id, regenera);
                                STrace.Trace(GetType().FullName, "Distribuci�n " + distribucion.Id + " procesada exitosamente luego de " + retry + " intentos.");
                                STrace.Trace(GetType().FullName, string.Format("Distribuciones a procesar: {0}", --distribucionesPendientes));
                                procesado = true;
                                var parametros = new[] { "Distribuci�n " + distribucion.Id + " procesada exitosamente luego de " + retry + " intentos.", distribucion.Id.ToString("#0"), distribucion.Inicio.ToString("dd/MM/yyyy HH:mm") };
                                SendMail(parametros);
                            }
                            catch (Exception e)
                            {
                                STrace.Exception(GetType().FullName, e);
                            }
                        }
                        if (retry == 5 && !procesado)
                        {
                            var parametros = new[] { "No se pudieron generar registros de Datamart para la distribuci�n " + distribucion.Id, distribucion.Id.ToString("#0"), distribucion.Inicio.ToString("dd/MM/yyyy HH:mm") };
                            SendMail(parametros);
                            STrace.Error(GetType().FullName, "No se pudieron generar registros de Datamart para la distribuci�n " + distribucion.Id);
                        }
                    }
                }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");

                var fin = DateTime.UtcNow;
                var duracion = fin.Subtract(inicio).TotalMinutes;

                var param = new[] { "Datamart Entregas", inicio.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), fin.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), duracion + " minutos" };
                SendSuccessMail(param);

                DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duracion, DataMartsLog.Moludos.DatamartEntregas, "Datamart finalizado exitosamente");
            }
            catch (Exception exc)
            {
                AddError(exc);
            }
            finally
            {
                ClearData();
            }
        }

        private IList<ViajeDistribucion> GetDistribuciones()
        {
            var ids = GetListOfInt("Ids");
            if (ids != null && ids.Count > 0)
                return DaoFactory.ViajeDistribucionDAO.FindByIds(ids);
            
            return DaoFactory.ViajeDistribucionDAO.GetListForDatamart(Empresas, StartDate, EndDate);
        }

        private void ProcesarDistribucion(int idDistribucion, bool regenera)
        {
            using (var transaction = SmartTransaction.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var distribucion = DaoFactory.ViajeDistribucionDAO.FindById(idDistribucion);

                    if (!regenera && HasBeenProcessed(distribucion))
                    {
                        STrace.Trace(GetType().FullName, string.Format("Distribuci�n ya procesada: {0}", distribucion.Id));
                    }
                    else
                    {
                        DaoFactory.DatamartDistribucionDAO.DeleteRecords(distribucion.Id);

                        ProcessDistribucionPorGps(distribucion);

                        STrace.Trace(GetType().FullName, string.Format("Distribuci�n procesada: {0}", distribucion.Id));
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    ClearData();
                }
            }
        }

        private void ProcessDistribucionPorGps(ViajeDistribucion distribucion)
        {
            var entregas = distribucion.Detalles;

            EntregaDistribucion anterior = null;

            if (distribucion.Tipo == ViajeDistribucion.Tipos.Desordenado)
                entregas = distribucion.GetEntregasPorOrdenReal();

            var orden = 0;
            foreach (var entrega in entregas)
            {
                var te = new TimeElapsed();
                var tiempoRecorrido = new TimeSpan(0);
                var kms = 0.0;
                if (anterior != null &&
                    (entrega.Estado.Equals(EntregaDistribucion.Estados.Completado)
                    || entrega.Estado.Equals(EntregaDistribucion.Estados.Visitado)))
                {
                    if (entrega.EntradaOManualExclusiva.HasValue && anterior.SalidaOManualExclusiva.HasValue)
                    {
                        tiempoRecorrido = entrega.EntradaOManualExclusiva.Value.Subtract(anterior.SalidaOManualExclusiva.Value);
                        if (tiempoRecorrido.TotalMinutes < 0) tiempoRecorrido = new TimeSpan(0);
                    }

                    if (entrega.Viaje.Vehiculo != null && anterior.FechaMin < entrega.FechaMin && entrega.FechaMin < DateTime.MaxValue)
                    { 
                        var dm = DaoFactory.DatamartDAO.GetMobilesKilometers(anterior.FechaMin, entrega.FechaMin, new List<int> {entrega.Viaje.Vehiculo.Id}).FirstOrDefault();
                        kms = dm != null ? dm.Kilometers : 0.0;
                        //kms = DaoFactory.CocheDAO.GetDistance(entrega.Viaje.Vehiculo.Id, anterior.FechaMin, entrega.FechaMin);
                    }
                }

                var tiempoEntrega = entrega.Salida.HasValue && entrega.Entrada.HasValue
                                        ? entrega.Salida.Value.Subtract(entrega.Entrada.Value)
                                        : new TimeSpan();
                var desvio = entrega.Entrada.HasValue 
                                ? entrega.Entrada.Value.Subtract(entrega.Programado)
                                : new TimeSpan();

                var confirmaciones = new List<string> { MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode(),
                                                        MessageCode.EstadoLogisticoCumplidoManualNoRealizado.GetMessageCode(),
                                                        MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode()};
                var eventos = entrega.EventosDistri.Where(e => confirmaciones.Contains(e.LogMensaje.Mensaje.Codigo));
                var evento = eventos.Any() ? eventos.OrderBy(e => e.Fecha).FirstOrDefault() : null;
                var distancia = evento != null ? Distancias.Loxodromica(evento.LogMensaje.Latitud, evento.LogMensaje.Longitud, entrega.ReferenciaGeografica.Latitude, entrega.ReferenciaGeografica.Longitude) : (double?)null;

                var registro = new DatamartDistribucion
                                   {
                                       Empresa = entrega.Viaje.Empresa,
                                       Linea = entrega.Linea,
                                       Vehiculo = entrega.Viaje.Vehiculo,
                                       CentroDeCostos = entrega.Viaje.CentroDeCostos,
                                       Viaje = entrega.Viaje,
                                       Detalle = entrega,
                                       Fecha = entrega.Viaje.Inicio,
                                       Ruta = entrega.Viaje.Codigo.Trim(),
                                       Entrega = entrega.Descripcion.Trim().Length > 50 ? entrega.Descripcion.Trim().Substring(0, 50) : entrega.Descripcion.Trim(),
                                       IdEstado = entrega.Estado,
                                       Estado = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado)),
                                       Km = kms,
                                       Recorrido = tiempoRecorrido.TotalMinutes,
                                       TiempoEntrega = tiempoEntrega.TotalSeconds > 0 ? tiempoEntrega.TotalMinutes : 0.0,
                                       Entrada = entrega.Entrada,
                                       Salida = entrega.Salida,
                                       Manual = entrega.Manual,
                                       Programado = entrega.Programado,
                                       Desvio = desvio.TotalMinutes,
                                       Orden = orden,
                                       Importe = 0.0f,
                                       PuntoEntrega = entrega.PuntoEntrega,
                                       Confirmacion = entrega.MensajeConfirmacion != null 
                                                        ? entrega.MensajeConfirmacion.Mensaje.Descripcion
                                                        : string.Empty,
                                       Cliente = entrega.PuntoEntrega != null && entrega.PuntoEntrega.Cliente != null 
                                                    ? entrega.PuntoEntrega.Cliente.Descripcion : string.Empty,
                                       Distancia = distancia
                                   };

                DaoFactory.DatamartDistribucionDAO.Save(registro);
                
                orden++;

                if (entrega.Estado.Equals(EntregaDistribucion.Estados.Completado)
                 || entrega.Estado.Equals(EntregaDistribucion.Estados.Visitado))
                    anterior = entrega;
            }
        }

        private bool HasBeenProcessed(ViajeDistribucion distribucion)
        {
            var records = DaoFactory.DatamartDistribucionDAO.GetRecords(distribucion.Id);
            return records.Count() == distribucion.EntregasTotalCount;
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }

        private void SendMail(string[] parametros)
        {
            var configFile = Config.Mailing.DatamartErrorMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuraci�n de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = "Datamart Error: Id=" + parametros[1] + " Fecha=" + parametros[2];
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }

        private void SendSuccessMail(IEnumerable<string> parametros)
        {
            var configFile = Config.Mailing.DatamartSuccessMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuraci�n de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = "Datamart finalizado exitosamente";
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }
    }
}
