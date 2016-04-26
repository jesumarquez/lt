using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Scheduler.Tasks.Mantenimiento.Util;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    /// <summary>
    /// Task for maintaining and generating datamart data.
    /// </summary>
    public class DatamartGeneration : BaseVehicleTask
    {
        #region Private Const Properties

        /// <summary>
        /// Defines the time span that is considered to be a no report time span.
        /// </summary>
        private const Int32 NoReportMinutes = 5;

        #endregion

        #region Private Properties

        /// <summary>
        /// Three months for today.
        /// </summary>
        private readonly DateTime _threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
        /// <summary>
        /// Determines if regeneration is enabled for current datamart generation.
        /// </summary>
        private Boolean Regenerate
        {
            get
            {
                var regenerate = GetBoolean("Regenera");
                return !regenerate.HasValue || regenerate.Value;
            }
        }
        /// <summary>
        /// Gets the datamart end of generation period datetime.
        /// </summary>
        /// <returns></returns>
        private DateTime EndDate
        {
            get
            {
                var endDate = GetDateTime("Hasta");
                return endDate.HasValue ? endDate.Value : DateTime.UtcNow;
            }
        }
        /// <summary>
        /// Gets the datamart start of generation period datetime for the specified vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private DateTime GetStartDate(int vehicle)
        {
            var startDate = GetDateTime("Desde");

            return startDate != null ? startDate.Value : GetLastDatamartUpdate(vehicle);
        }

        #endregion

        #region Protected Methods

        protected override void OnExecute(Timer timer)
        {
            var inicio = DateTime.UtcNow;

            base.OnExecute(timer);

            RecompileStores();

            var today = EndDate;
            
            //DeleteOldDatamartRecords();

			STrace.Trace(GetType().FullName, "Processing vehicles.");

            SetVehicles();

            foreach (var vehicleId in Vehicles)
            {
                var vehicle = DaoFactory.CocheDAO.FindById(vehicleId);
                try
                {   
                    ProcessVehicle(vehicle, today);
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
                            ProcessVehicle(vehicle, today);
                            procesado = true;
                            var parametros = new[] { "Vehículo " + vehicle.Id + " procesado exitosamente luego de " + retry + " intentos.", vehicle.Id.ToString("#0"), today.ToString("dd/MM/yyyy HH:mm") };
                        }
                        catch (Exception e)
                        {
                            STrace.Exception(GetType().FullName, e);
                        }
                    }
                    if (retry == 5 && !procesado)
                    {
                        var parametros = new[] { "No se pudieron generar registros de Datamart para el vehículo: " + vehicle.Id, vehicle.Id.ToString("#0"), today.ToString("dd/MM/yyyy HH:mm") };
                        SendMail(parametros);
                        STrace.Error(GetType().FullName, "No se pudieron generar registros de Datamart para el vehículo: " + vehicle.Id);
                    }
                }
            }

            var fin = DateTime.UtcNow;
            var duracion = fin.Subtract(inicio).TotalMinutes;

            var param = new[] { "Datamart Recorridos", inicio.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), fin.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), duracion + " minutos" };
            SendSuccessMail(param);

            DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duracion, DataMartsLog.Moludos.DatamartRecorridos, "Datamart finalizado exitosamente");
        }

        private void RecompileStores()
        {
            DaoFactory.LogPosicionDAO.RecompileSP("sp_LogPosicionDAO_GetPositionsBetweenDates_3");
            DaoFactory.LogPosicionDAO.RecompileSP("sp_LogPosicionDAO_GetRegenerationStartDate_2");
            DaoFactory.LogPosicionDAO.RecompileSP("sp_LogPosicionDAO_GetRegenerationEndDate_2");
            DaoFactory.LogPosicionDAO.RecompileSP("sp_LogPosicionDAO_GetFirstPositionOlderThanDate_2");
            DaoFactory.LogPosicionDAO.RecompileSP("sp_LogPosicionDAO_GetFirstPositionNewerThanDate_2");
        }

        private void SetVehicles()
        {
            var distrito = GetInt32("Distrito");

            if (distrito.HasValue)
                Vehicles = DaoFactory.CocheDAO.FindAllActivos().Where(v => v.Empresa != null && v.Empresa.Id == distrito).Select(v => v.Id).ToList();
            
            VehiclesToProcess = Vehicles.Count;
        }

        #endregion

        #region Private Methods

        private void ProcessVehicle(Coche coche, DateTime today)
        {
            var lastUpdate = GetStartDate(coche.Id);

            if (today.AddDays(-7) > lastUpdate)
            {
                lastUpdate = today.AddDays(-7);
            }

			var dispo = coche.Dispositivo != null ? coche.Dispositivo.Id : 0;
            STrace.Trace(GetType().FullName, dispo, String.Format("Processing vehicle with id: {0}", coche.Id));
            
            if (!lastUpdate.Equals(DateTime.MinValue))
            {
                if (Regenerate) RegenerateOldPeriods(lastUpdate, coche, today);

                ProcessCurrentPeriods(lastUpdate, coche, today);
            }
			else
            {
                STrace.Trace(GetType().FullName, dispo, String.Format("No data to process for vehicle with id: {0}", coche.Id));
            }

			STrace.Trace(GetType().FullName, String.Format("Vehicles to process: {0}", --VehiclesToProcess));
        }

        private void ProcessCurrentPeriods(DateTime lastUpdate, Coche vehicle, DateTime today)
        {
            var dispo = vehicle.Dispositivo != null ? vehicle.Dispositivo.Id : 0;

            
            while (lastUpdate < today)
            {
                var lastDay = lastUpdate.AddDays(1) > today ? today : lastUpdate.AddDays(1);
                ProcessPeriod(vehicle, lastUpdate, lastDay);

				STrace.Trace(GetType().FullName, dispo, String.Format("Processed period from {0} to {1} for the vehicle with id: {2}", lastUpdate, lastDay, vehicle.Id));

                lastUpdate = lastUpdate.AddDays(1);
            }
        }

        private void RegenerateOldPeriods(DateTime lastUpdate, Coche vehicle, DateTime today)
        {
            var dispo = vehicle.Dispositivo != null ? vehicle.Dispositivo.Id : 0;
			
			var regenerate = GetDaysToRegenerate(vehicle, lastUpdate, today);

            if (regenerate.Count.Equals(0)) return;

            foreach (var period in regenerate)
            {
                ProcessPeriod(vehicle, period.Desde, period.Hasta);

				STrace.Trace(GetType().FullName, dispo, String.Format("Regenerated period from {0} to {1} for the vehicle with id: {2}", period.Desde, period.Hasta, vehicle.Id));
            }
        }

        private List<RegenerateDatamart> GetDaysToRegenerate(Coche vehicle, DateTime lastUpdate, DateTime today)
        {
            var dispo = vehicle.Dispositivo != null ? vehicle.Dispositivo.Id : 0;
			STrace.Trace(GetType().FullName, dispo, String.Format("Checking for periods to regenerate for the vehicle with id: {0}", vehicle.Id));

            var regenerate = DaoFactory.DatamartDAO.GetDaysToRegenerate(vehicle, lastUpdate, today, _threeMonthsAgo);
            
			if (regenerate.Count.Equals(0)) STrace.Trace(GetType().FullName, dispo, String.Format("No periods to regenerate for the vehicle with id: {0}", vehicle.Id));

            return regenerate;
        }

        private DateTime GetLastDatamartUpdate(int vehicle) { return DaoFactory.DatamartDAO.GetLastDatamartUpdate(vehicle); }

        private void DeleteOldDatamartRecords()
        {
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);

			STrace.Trace(GetType().FullName, "Deleting old datamart records.");

            DaoFactory.DatamartDAO.DeleteOldDatamartRecords(oneYearAgo);
        }

        private void ProcessPeriod(Coche vehicle, DateTime from, DateTime to)
        {
            using (var transaction = SmartTransaction.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Set environment data
                    var inicio = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0);
                    var fin = new DateTime(to.Year, to.Month, to.Day, to.Hour, 0, 0);

                    // Deletes all previously generated datamart records for the current vehicle and time span.
                    var t = new TimeElapsed();
                    DaoFactory.DatamartDAO.DeleteRecords(vehicle.Id, inicio, fin);
                    var ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Trace(GetType().FullName, string.Format("DeleteRecords en {0} segundos", ts));

                    var records = new List<Datamart>();
                    var registrosValidos = false;
                    
                    using (var data = new PeriodData(DaoFactory, vehicle, inicio, fin))
                    {
                        records = GenerateRecords(data);
                        registrosValidos = ValidateRecords(data, records);
                        if (!registrosValidos) STrace.Error(GetType().FullName, string.Format("Registros no válidos para el vehículo: {0}", vehicle.Id));
                    }                    

                    t.Restart();
                    foreach (var record in records) DaoFactory.DatamartDAO.Save(record);

                    ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Trace(GetType().FullName, string.Format("Save en {0} segundos", ts));

                    transaction.Commit();

                    if (!registrosValidos)
                    {
                        var parametros = new[] { "Se generaron registros de Datamart posiblemente inválidos para el vehículo: " + vehicle.Id, vehicle.Id.ToString("#0"), DateTime.Today.ToString("dd/MM/yyyy HH:mm") };
                        SendMail(parametros);
                        STrace.Error(GetType().FullName, "Se generaron registros de Datamart posiblemente inválidos para el vehículo: " + vehicle.Id);
                    }
                }
                catch (Exception ex)
                {
                    AddError(ex);
                    transaction.Rollback();
                }
                finally
                {
                    ClearData();
                    DoSleep();
                }
            }
        }

        private List<Datamart> GenerateRecords(PeriodData data)
        {
            var datamarter = new Datamarter(data);

            if (data.Posiciones.Count == 0)
            {
                var distancia = data.PosicionAnterior != null && data.PosicionSiguiente != null 
                    ? data.GetDistancia(data.PosicionAnterior, data.PosicionSiguiente, data.Inicio, data.Fin)
                    : 0;
                return datamarter.CreateNoReportDatamartRecords(data.Inicio, data.Fin, distancia);
            }

            var listado = new List<Datamart>();

            //if (data.PosicionAnterior == null && data.Posiciones[0].FechaMensaje > data.Inicio)
            //{
            //    listado.AddRange(datamarter.CreateNoReportDatamartRecords(data.Inicio, data.Posiciones[0].FechaMensaje, 0));
            //}

            var posIndex = 0;
            var inicio = data.Inicio;
            var lastPosition = data.PosicionAnterior ?? data.Posiciones[0];
            foreach (var fecha in data.FechasDeCorte)
            {
	            var minSpeed = int.MaxValue;
	            var maxSpeed = 0;
	            var sumSpeed = 0;
	            var lastHash = string.Empty;
	            var posCount = 0;
	            var km = 0.0;

                if (fecha < lastPosition.FechaMensaje)
                {
                    listado.AddRange(datamarter.CreateNoReportDatamartRecords(inicio, fecha, 0));
                    inicio = fecha;
                    continue;
                }
                
	            for (; posIndex < data.Posiciones.Count && data.Posiciones[posIndex].FechaMensaje < fecha; posIndex++)
	            {
		            var position = data.Posiciones[posIndex];
		            var hash = data.GetHash(position);
		            var timeBetween = position.FechaMensaje.Subtract(lastPosition.FechaMensaje);
		            if (inicio >= position.FechaMensaje || timeBetween == TimeSpan.Zero)
		            {
			            lastPosition = position;
			            lastHash = hash;
			            continue;
		            }

		            var distancia =  inicio > lastPosition.FechaMensaje
			                             ? data.GetDistancia(lastPosition, position, inicio, null)
			                             : data.GetDistancia(lastPosition, position, null, null);

		            var nextIsLast = !(posIndex + 1 < data.Posiciones.Count && data.Posiciones[posIndex + 1].FechaMensaje < fecha);
		            var cortar = hash != lastHash;
		            var noreport = timeBetween.TotalMinutes > NoReportMinutes;
		            var firstPos = posCount == 0;

		            if (!firstPos && (cortar || noreport || nextIsLast))
		            {
			            var estadoMotor = EstadoMotor.FromPosicion(lastPosition);
			            var estadoVehiculo = EstadoVehiculo.FromPosicion(lastPosition);
		                var zona = lastPosition.Zona;
			            var avgSpeed = sumSpeed / posCount;
			            var datamart = datamarter.CreateDatamartRecord(inicio, 
			                                                           noreport ? lastPosition.FechaMensaje : position.FechaMensaje, 
			                                                           km, estadoMotor, estadoVehiculo, avgSpeed, minSpeed, maxSpeed, zona);
			            listado.Add(datamart);

			            km = distancia;
			            if (noreport)
			            {
                            var datamarts = datamarter.CreateNoReportDatamartRecords(lastPosition.FechaMensaje, position.FechaMensaje, km);
				            listado.AddRange(datamarts);
				            km = 0;
			            }
                        
			            minSpeed = position.Velocidad;
			            maxSpeed = position.Velocidad;
			            sumSpeed = position.Velocidad;
			            inicio = position.FechaMensaje;
			            posCount = 1;
		            }
		            else
		            {
			            km += distancia;
			            sumSpeed += position.Velocidad;
			            if (position.Velocidad < minSpeed) minSpeed = position.Velocidad;
			            if (position.Velocidad > maxSpeed) maxSpeed = position.Velocidad;
			            posCount++;
		            }
                    
		            lastPosition = position;
		            lastHash = hash;
	            }
                
	            if (inicio < fecha)
	            {
		            var ultima = posIndex < data.Posiciones.Count - 1
			                         ? data.Posiciones[posIndex + 1]
			                         : data.PosicionSiguiente;
		            km +=  ultima == null ? 0 
			                   : (inicio > lastPosition.FechaMensaje
				                      ? data.GetDistancia(lastPosition, ultima, inicio, fecha)
				                      : data.GetDistancia(lastPosition, ultima, null, fecha));

		            if (posCount == 0)
		            {
                        var datamart2 = datamarter.CreateNoReportDatamartRecords(inicio, fecha, km);
			            listado.AddRange(datamart2);
		            }
		            else
		            {
			            var estadoMotor2 = EstadoMotor.FromPosicion(lastPosition);
			            var estadoVehiculo2 = EstadoVehiculo.FromPosicion(lastPosition);
		                var zona = lastPosition.Zona;
			            var avgSpeed2 = posCount > 0 ? sumSpeed / posCount : 0;
			            var datamart2 = datamarter.CreateDatamartRecord(inicio, fecha, km, estadoMotor2,
			                                                            estadoVehiculo2, avgSpeed2, minSpeed, maxSpeed, zona);
			            listado.Add(datamart2);
		            }
	            }
                
	            inicio = fecha;
            }

            return listado;
        }

        private bool ValidateRecords(PeriodData data, IEnumerable<Datamart> records)
        {
            var valido = true;
            var tuvoPosiciones = data.Posiciones.Count > 1;
            var tuvoMovimiento = data.Posiciones.Any(p => p.Velocidad > 0);
            var generoKilometros = records.Any(r => r.Kilometers > 0);

            if (tuvoPosiciones && tuvoMovimiento && !generoKilometros) valido = false;

            return valido;
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

        private void SendSuccessMail(IEnumerable<string> parametros)
        {
            var configFile = Config.Mailing.DatamartSuccessMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");

            var sender = new MailSender(configFile);

            sender.Config.Subject = "Datamart finalizado exitosamente";
            
            SendMailToAllDestinations(sender, parametros.ToList());
        }

        #endregion
    }
}
