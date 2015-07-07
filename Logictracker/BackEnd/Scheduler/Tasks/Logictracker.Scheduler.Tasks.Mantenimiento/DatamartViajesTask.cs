using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ReportObjects.Datamart;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class DatamartViajeTask : BaseTask
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
                return empresas.Any() ? empresas : new List<int> {-1};
            }
        }

        protected override void OnExecute(Timer timer)
        {
            var regenera = Regenera;

            STrace.Trace(GetType().FullName, string.Format("Procesando viajes. Desde: {0} - Hasta: {1} - Regenera: {2} - Empresas: {3}", StartDate, EndDate, regenera, Empresas));

            var inicio = DateTime.UtcNow;

            try
            {
                var viajes = GetViajes();
                var viajesPendientes = viajes.Count;
                STrace.Trace(GetType().FullName, string.Format("Viajes a procesar: {0}", viajesPendientes));

                foreach (var viaje in viajes)
                {
                    try
                    {
                        ProcesarViaje(viaje.Id, regenera);
                        STrace.Trace(GetType().FullName, string.Format("Viajes a procesar: {0}", --viajesPendientes));
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
                                ProcesarViaje(viaje.Id, regenera);
                                STrace.Trace(GetType().FullName, string.Format("Viajes a procesar: {0}", --viajesPendientes));
                                procesado = true;
                                var parametros = new[] { "Viaje " + viaje.Id + " procesado exitosamente luego de " + retry + " intentos.", viaje.Id.ToString("#0"), viaje.Inicio.ToString("dd/MM/yyyy HH:mm") };
                                SendMail(parametros);
                                STrace.Trace(GetType().FullName, "Viaje " + viaje.Id + " procesado exitosamente luego de " + retry + " intentos.");
                            }
                            catch (Exception e)
                            {
                                STrace.Exception(GetType().FullName, e);
                            }
                        }
                        if (retry == 5 && !procesado)
                        {
                            var parametros = new[] { "No se pudieron generar registros de Datamart para el viaje " + viaje.Id, viaje.Id.ToString("#0"), viaje.Inicio.ToString("dd/MM/yyyy HH:mm") };
                            SendMail(parametros);
                            STrace.Error(GetType().FullName, "No se pudieron generar registros de Datamart para el viaje " + viaje.Id);
                        }
                    }
                }

                STrace.Trace(GetType().FullName, "Tarea finalizada.");

                var fin = DateTime.UtcNow;
                var duracion = fin.Subtract(inicio).TotalMinutes;

                var param = new[] { "Datamart Viajes", inicio.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), fin.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), duracion + " minutos" };
                SendSuccessMail(param);

                DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duracion, DataMartsLog.Moludos.DatamartRutas, "Datamart finalizado exitosamente");
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

        private IList<ViajeDistribucion> GetViajes()
        {
            var ids = GetListOfInt("Ids");
            if (ids != null && ids.Count > 0)
                return DaoFactory.ViajeDistribucionDAO.FindByIds(ids).Where(v => v.InicioReal.HasValue).ToList();

            return DaoFactory.ViajeDistribucionDAO.GetListForDatamart(Empresas, StartDate, EndDate)
                                                  .Where(v => v.InicioReal.HasValue).ToList();
        }

        private void ProcesarViaje(int idViaje, bool regenera)
        {
            using (var transaction = SmartTransaction.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var viaje = DaoFactory.ViajeDistribucionDAO.FindById(idViaje);

                    if (!regenera && HasBeenProcessed(idViaje))
                    {
                        STrace.Trace(GetType().FullName, string.Format("Viaje ya procesado: {0}", idViaje));
                    }
                    else
                    {
                        DaoFactory.DatamartViajeDAO.DeleteRecords(idViaje);
                        ProcessViaje(viaje);
                        STrace.Trace(GetType().FullName, string.Format("Viaje procesado: {0}", idViaje));
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

        private void ProcessViaje(ViajeDistribucion viaje)
        {
            var kmProductivos = 0.0;
            var kmImproductivos = 0.0;
            var detalles = viaje.Detalles.Where(d => d.Linea == null);

            DateTime? ultima = null;
            DateTime? primera = null;

            var primeraEntrada = detalles.Min(e => e.Entrada);
            var primeraSalida = detalles.Min(e => e.Salida);
            var ultimaEntrada = detalles.Max(e => e.Entrada);
            var ultimaSalida = detalles.Max(e => e.Salida);

            if (primeraSalida.HasValue && primeraEntrada.HasValue)
                primera = primeraEntrada <= primeraSalida ? primeraEntrada : primeraSalida;
            else if (primeraEntrada.HasValue) primera = primeraEntrada;
            else if (primeraSalida.HasValue) primera = primeraSalida;

            if (ultimaSalida.HasValue && ultimaEntrada.HasValue)
                ultima = ultimaSalida >= ultimaEntrada ? ultimaSalida : ultimaEntrada;
            else if (ultimaSalida.HasValue) ultima = ultimaSalida;
            else if (ultimaEntrada.HasValue) ultima = ultimaEntrada;

            if (primera.HasValue && ultima.HasValue)
            {
                if (primera == ultima) // TODOS KMS IMPRODUCTIVOS
                {
                    var dm = DaoFactory.DatamartDAO.GetMobilesKilometers(viaje.InicioReal.Value, viaje.Fin, new List<int> { viaje.Vehiculo.Id }).FirstOrDefault();
                    kmImproductivos += dm != null ? dm.Kilometers : 0.0;
                }
                else
                {
                    // PRIMER TRAMO IMPRODUCTIVO
                    var dm = DaoFactory.DatamartDAO.GetMobilesKilometers(viaje.InicioReal.Value, primera.Value, new List<int> { viaje.Vehiculo.Id }).FirstOrDefault();
                    kmImproductivos += dm != null ? dm.Kilometers : 0.0;

                    // SEGUNDO TRAMO PRODUCTIVO
                    dm = DaoFactory.DatamartDAO.GetMobilesKilometers(primera.Value, ultima.Value, new List<int> { viaje.Vehiculo.Id }).FirstOrDefault();
                    kmProductivos += dm != null ? dm.Kilometers : 0.0;

                    // TERCER TRAMO IMPRODUCTIVO
                    dm = DaoFactory.DatamartDAO.GetMobilesKilometers(ultima.Value, viaje.Fin, new List<int> { viaje.Vehiculo.Id }).FirstOrDefault();
                    kmImproductivos += dm != null ? dm.Kilometers : 0.0;
                }
            }
            else if (!primera.HasValue && !ultima.HasValue) // TODOS KMS IMPRODUCTIVOS
            { 
                var dm = DaoFactory.DatamartDAO.GetMobilesKilometers(viaje.InicioReal.Value, viaje.Fin, new List<int> { viaje.Vehiculo.Id }).FirstOrDefault();
                kmImproductivos += dm != null ? dm.Kilometers : 0.0;
            }
            
            var kmProgramados = viaje.Detalles.Sum(d => d.KmCalculado);
            var kmTotales = kmProductivos + kmImproductivos;
            if (kmTotales <= 0) STrace.Error(GetType().FullName, string.Format("Viaje con kilometros en 0 - Id: {0}", viaje.Id));

            var costoViaje = viaje.Vehiculo.CocheOperacion != null
                                 ? viaje.Vehiculo.CocheOperacion.CostoKmUltimoMes * kmTotales
                                 : 0.0;

            var completadas = detalles.Count(d => d.Estado == EntregaDistribucion.Estados.Completado);
            var noCompletadas = detalles.Count(d => d.Estado == EntregaDistribucion.Estados.NoCompletado);
            var visitadas = detalles.Count(d => d.Estado == EntregaDistribucion.Estados.Visitado);
            var sinVisitar = detalles.Count(d => d.Estado == EntregaDistribucion.Estados.SinVisitar);

            var entregas = detalles.Where(d => d.Entrada.HasValue && d.Salida.HasValue)
                                   .Select(d => d.Salida.Value.Subtract(d.Entrada.Value));

            var totalEntregas = entregas.Any() ? entregas.Sum(e => e.TotalHours) : 0.0;
            var entregaMaxima = entregas.Any() ? entregas.Max(d => d.TotalHours) : 0.0;
            var entregaMinima = entregas.Any() ? entregas.Min(d => d.TotalHours) : 0.0;
            var entregaPromedio = entregas.Any() ? totalEntregas / (double)entregas.Count() : 0.0;

            var sumDm = DaoFactory.DatamartDAO.GetSummarizedDatamart(viaje.InicioReal.Value, viaje.Fin, viaje.Vehiculo.Id);
            var hsDetenido = sumDm.HsDetenido;
            var velocidadMax = sumDm.MaxSpeed;
            var velocidadPromedio = sumDm.AverageSpeed;

            var dmViaje = new DatamartViaje
                              {
                                  Empresa = viaje.Empresa,
                                  Linea = viaje.Linea,
                                  Vehiculo = viaje.Vehiculo,
                                  Viaje = viaje,
                                  Inicio = viaje.InicioReal.Value,
                                  Fin = viaje.Fin,
                                  Duracion = viaje.Fin.Subtract(viaje.InicioReal.Value).TotalMinutes,
                                  KmImproductivos = kmImproductivos,
                                  KmProductivos = kmProductivos,
                                  KmProgramados = kmProgramados.Value,
                                  KmTotales = kmTotales,
                                  EntregaMaxima = entregaMaxima,
                                  EntregaMinima = entregaMinima,
                                  EntregaPromedio = entregaPromedio,
                                  VelocidadMaxima = velocidadMax,
                                  VelocidadPromedio = velocidadPromedio,
                                  EntregasTotales = detalles.Count(),
                                  EntregasCompletadas = completadas,
                                  EntregasNoCompletadas = noCompletadas,
                                  EntregasNoVisitadas = sinVisitar,
                                  EntregasVisitadas = visitadas,
                                  HorasDetenido = hsDetenido,
                                  HorasEnEntrega = totalEntregas,
                                  Costo = costoViaje
                              };

            DaoFactory.DatamartViajeDAO.SaveOrUpdate(dmViaje);
        }

        private bool HasBeenProcessed(int idViaje)
        {
            var records = DaoFactory.DatamartViajeDAO.GetRecords(idViaje);
            return records.Count != 0;
        }

        private void ClearData()
        {
            ClearSessions();
            GC.Collect();
        }

        private void SendMail(string[] parametros)
        {
            var configFile = Config.Mailing.DatamartErrorMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = "Datamart Error: Id=" + parametros[1] + " Fecha=" + parametros[2];

            SendMailToAllDestinations(sender, parametros.ToList());
        }

        private void SendSuccessMail(string[] parametros)
        {
            var configFile = Config.Mailing.DatamartSuccessMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);
            sender.Config.Subject = "Datamart finalizado exitosamente";

            SendMailToAllDestinations(sender, parametros.ToList());
        }
    }
}
