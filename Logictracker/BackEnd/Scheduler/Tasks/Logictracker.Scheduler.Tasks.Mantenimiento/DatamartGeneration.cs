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

        /// <summary>
        /// Performs tasks main activities.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            base.OnExecute(timer);

            var today = EndDate;
            
            //DeleteOldDatamartRecords();

			STrace.Trace(GetType().FullName, "Processing vehicles.");

            var inicio = DateTime.UtcNow;

            foreach (var vehicle in Vehicles)
            {
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
                            var parametros = new[] { "Vehículo " + vehicle + " procesado exitosamente luego de " + retry + " intentos.", vehicle.ToString("#0"), today.ToString("dd/MM/yyyy HH:mm") };
                            SendMail(parametros);
                            STrace.Trace(GetType().FullName, "Vehículo " + vehicle + " procesado exitosamente luego de " + retry + " intentos.");
                        }
                        catch (Exception e)
                        {
                            STrace.Exception(GetType().FullName, e);
                        }
                    }
                    if (retry == 5 && !procesado)
                    {
                        var parametros = new[] { "No se pudieron generar registros de Datamart para el vehículo: " + vehicle, vehicle.ToString("#0"), today.ToString("dd/MM/yyyy HH:mm") };
                        SendMail(parametros);
                        STrace.Trace(GetType().FullName, "No se pudieron generar registros de Datamart para el vehículo: " + vehicle);
                    }
                }
            }

            var fin = DateTime.UtcNow;
            var duracion = fin.Subtract(inicio).TotalMinutes;

            var param = new[] { "Datamart Recorridos", inicio.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), fin.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss"), duracion + " minutos" };
            SendSuccessMail(param);

            DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duracion, DataMartsLog.Moludos.DatamartRecorridos, "Datamart finalizado exitosamente");
        }

        #endregion

        #region Private Methods

		/// <summary>
        /// Process the specified vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="today"></param>
        private void ProcessVehicle(int vehicle, DateTime today)
        {
            var lastUpdate = GetStartDate(vehicle);

			var coche = DaoFactory.CocheDAO.FindById(vehicle);
			var dispo = coche.Dispositivo != null ? coche.Dispositivo.Id : 0;
			STrace.Trace(GetType().FullName, dispo, String.Format("Processing vehicle with id: {0}", vehicle));
            
            if (!lastUpdate.Equals(DateTime.MinValue))
            {
                if (Regenerate) RegenerateOldPeriods(lastUpdate, vehicle, today);

                ProcessCurrentPeriods(lastUpdate, vehicle, today);
            }
			else
            {
	            STrace.Trace(GetType().FullName, dispo, String.Format("No data to process for vehicle with id: {0}", vehicle));
            }

			STrace.Trace(GetType().FullName, String.Format("Vehicles to process: {0}", --VehiclesToProcess));
        }

        /// <summary>
        /// Process the current periods of the specified vehicle.
        /// </summary>
        /// <param name="lastUpdate"></param>
        /// <param name="vehicle"></param>
        /// <param name="today"></param>
        private void ProcessCurrentPeriods(DateTime lastUpdate, int vehicle, DateTime today)
        {
			var coche = DaoFactory.CocheDAO.FindById(vehicle);
			var dispo = coche.Dispositivo != null ? coche.Dispositivo.Id : 0;
            while (lastUpdate < today)
            {
                var lastDay = lastUpdate.AddDays(1) > today ? today : lastUpdate.AddDays(1);
                ProcessPeriod(vehicle, lastUpdate, lastDay);

				STrace.Trace(GetType().FullName, dispo, String.Format("Processed period from {0} to {1} for the vehicle with id: {2}", lastUpdate, lastDay, vehicle));

                lastUpdate = lastUpdate.AddDays(1);
            }
        }

        /// <summary>
        /// Regenerates old period for the specified vehicle.
        /// </summary>
        /// <param name="lastUpdate"></param>
        /// <param name="vehicle"></param>
        /// <param name="today"></param>
        private void RegenerateOldPeriods(DateTime lastUpdate, int vehicle, DateTime today)
        {
			var coche = DaoFactory.CocheDAO.FindById(vehicle);
			var dispo = coche.Dispositivo != null ? coche.Dispositivo.Id : 0;
			
			var regenerate = GetDaysToRegenerate(vehicle, lastUpdate, today);

            if (regenerate.Count.Equals(0)) return;

            foreach (var period in regenerate)
            {
                ProcessPeriod(vehicle, period.Desde, period.Hasta);

				STrace.Trace(GetType().FullName, dispo, String.Format("Regenerated period from {0} to {1} for the vehicle with id: {2}", period.Desde, period.Hasta, vehicle));
            }
        }

        /// <summary>
        /// Gets the days that need to be regenerated for the current vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="lastUpdate"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        private List<RegenerateDatamart> GetDaysToRegenerate(int vehicle, DateTime lastUpdate, DateTime today)
        {
			var coche = DaoFactory.CocheDAO.FindById(vehicle);
			var dispo = coche.Dispositivo != null ? coche.Dispositivo.Id : 0;
			STrace.Trace(GetType().FullName, dispo, String.Format("Checking for periods to regenerate for the vehicle with id: {0}", vehicle));

            var regenerate = DaoFactory.DatamartDAO.GetDaysToRegenerate(vehicle, lastUpdate, today, _threeMonthsAgo);
            
			if (regenerate.Count.Equals(0)) STrace.Trace(GetType().FullName, dispo, String.Format("No periods to regenerate for the vehicle with id: {0}", vehicle));

            return regenerate;
        }

        /// <summary>
        /// Gets the last datamart update for the current vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private DateTime GetLastDatamartUpdate(int vehicle) { return DaoFactory.DatamartDAO.GetLastDatamartUpdate(vehicle); }

        /// <summary>
        /// Deletes old datamart records.
        /// </summary>
        private void DeleteOldDatamartRecords()
        {
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);

			STrace.Trace(GetType().FullName, "Deleting old datamart records.");

            DaoFactory.DatamartDAO.DeleteOldDatamartRecords(oneYearAgo);
        }

        /// <summary>
        /// Process the specified period for the givenn vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void ProcessPeriod(int vehicle, DateTime from, DateTime to)
        {
            using (var transaction = SmartTransaction.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    // Set environment data
                    var vehiculo = DaoFactory.CocheDAO.FindById(vehicle);
                    var inicio = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0);
                    var fin = new DateTime(to.Year, to.Month, to.Day, to.Hour, 0, 0);

                    // Deletes all previously generated datamart records for the current vehicle and time span.
                    DaoFactory.DatamartDAO.DeleteRecords(vehiculo.Id, inicio, fin);
                    List<Datamart> records;

                    using (var data = new PeriodData(DaoFactory, vehiculo, inicio, fin))
                    {
                        records = GenerateRecords(data);
                    }

                    foreach (var record in records) DaoFactory.DatamartDAO.Save(record);

                    transaction.Commit();
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

        /// <summary>
        /// Generate current period datamart records.
        /// </summary>
        private List<Datamart> GenerateRecords(PeriodData data)
        {
            var datamarter = new Datamarter(data);

            if(data.Posiciones.Count == 0)
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
	            var lastHash = String.Empty;
	            var posCount = 0;
	            var km = 0.0;

                if(fecha < lastPosition.FechaMensaje)
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

        /// <summary>
        /// Release all asigned resources.
        /// </summary>
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
