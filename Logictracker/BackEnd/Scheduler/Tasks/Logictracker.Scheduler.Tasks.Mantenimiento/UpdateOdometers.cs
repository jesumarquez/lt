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
using Logictracker.Messaging;

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    public class UpdateOdometers : BaseVehicleTask
    {
        private DateTime EndDate
        {
            get
            {
                var endDate = GetDateTime("Hasta");
                return endDate.HasValue ? endDate.Value : DateTime.UtcNow.Date.AddHours(3).AddMinutes(-1);
            }
        }

        protected override void OnExecute(Timer timer)
        {
            var inicio = DateTime.UtcNow;

            base.OnExecute(timer);

            var today = EndDate;
            
			STrace.Trace(GetType().FullName, "Processing vehicles.");

            SetVehicles();

            foreach (var vehicleId in Vehicles)
            {
                try
                {
                    var vehicle = DaoFactory.CocheDAO.FindById(vehicleId);
                    ProcessVehicle(vehicle, today);
                }
                catch (Exception ex)
                {
                    STrace.Exception(GetType().FullName, ex);

                    var parametros = new[] { "No se pudieron actualizar odómetros para el vehículo: " + vehicleId, vehicleId.ToString("#0"), today.ToString("dd/MM/yyyy HH:mm") };
                    SendMail(parametros);
                    STrace.Error(GetType().FullName, "No se pudieron actualizar odómetros para el vehículo: " + vehicleId);
                }
                finally 
                {
                    ClearData();
                }
            }

            var fin = DateTime.UtcNow;
            var duracion = fin.Subtract(inicio).TotalMinutes;

            DaoFactory.DataMartsLogDAO.SaveNewLog(inicio, fin, duracion, DataMartsLog.Moludos.UpdateOdometers, "Actualización de odómetros finalizada exitosamente");
        }
        
        private void SetVehicles()
        {
            var distrito = GetInt32("Distrito");

            if (distrito.HasValue)
                Vehicles = DaoFactory.CocheDAO.FindAllActivos().Where(v => v.Empresa != null && v.Empresa.Id == distrito).Select(v => v.Id).ToList();
            
            VehiclesToProcess = Vehicles.Count;
        }

        private void ProcessVehicle(Coche coche, DateTime today)
        {
            var inicio = coche.LastOdometerUpdate;
            var fin = GetLastDatamartUpdate(coche.Id);

            if (today.AddDays(-7) > inicio)
                inicio = today.AddDays(-7);

            if (!inicio.Equals(DateTime.MinValue))
            {
                var dm = DaoFactory.DatamartDAO.GetSummarizedDatamart(inicio.Value, fin, coche.Id);
                var kilometros = dm.Kilometros;
                var horas = dm.HsMarcha;                
                var dias = fin.AddHours(-3).DayOfYear - inicio.Value.AddHours(-3).DayOfYear;
                var resetDailyOdometer = dias > 0;
                
                if (resetDailyOdometer) coche.DailyOdometer = kilometros;
                else coche.DailyOdometer += kilometros;

                coche.ApplicationOdometer += kilometros;
                coche.PartialOdometer += kilometros;
                coche.LastOdometerUpdate = fin;

                UpdateVehicleOdometers(coche, kilometros, horas, dias, fin);

                if (coche.TipoCoche.ControlaKilometraje) CheckDailyOdometer(coche, fin);

                DaoFactory.CocheDAO.Update(coche);
            }

			STrace.Trace(GetType().FullName, String.Format("Vehicles to process: {0}", --VehiclesToProcess));
        }

        private void UpdateVehicleOdometers(Coche coche, double kilometros, double horas, int dias, DateTime fin)
        {
            if (coche.Odometros.Count.Equals(0)) return;

            foreach (MovOdometroVehiculo odometro in coche.Odometros)
            {
                UpdateOdometerValues(coche, odometro, kilometros, horas, dias, fin);

                if (odometro.Vencido() && !odometro.UltimoDisparo.HasValue)
                {
                    odometro.UltimoDisparo = fin;
                    if (odometro.Odometro.EsIterativo) odometro.ResetOdometerValues();
                    var lastPosition = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(coche.Id, fin, 1);
                    MessageSaver.Save(MessageCode.OdometerExpired.GetMessageCode(), coche, fin, lastPosition.ToGpsPoint(), odometro.Odometro.Descripcion);
                }
                else if (odometro.SuperoSegundoAviso() && !odometro.FechaSegundoAviso.HasValue)
                {
                    odometro.FechaSegundoAviso = fin;
                    var lastPosition = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(coche.Id, fin, 1);
                    MessageSaver.Save(MessageCode.OdometerSecondWarning.GetMessageCode(), coche, fin, lastPosition.ToGpsPoint(), odometro.Odometro.Descripcion);
                }
                else if (odometro.SuperoPrimerAviso() && !odometro.FechaPrimerAviso.HasValue)
                {
                    odometro.FechaPrimerAviso = fin;
                    var lastPosition = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(coche.Id, fin, 1);
                    MessageSaver.Save(MessageCode.OdometerFirstWarning.GetMessageCode(), coche, fin, lastPosition.ToGpsPoint(), odometro.Odometro.Descripcion);
                }
            }
        }

        private void UpdateOdometerValues(Coche coche, MovOdometroVehiculo odometro, double kilometros, double horas, int dias, DateTime fin)
        {
            UpdateOdometerDays(odometro, dias);

            UpdateOdometerKilometers(coche, odometro, kilometros);

            UpdateOdometerHours(odometro, horas);

            odometro.UltimoUpdate = fin;
        }

        private void UpdateOdometerKilometers(Coche coche, MovOdometroVehiculo odometro, double kilometros)
        {
            if (odometro.Odometro.PorKm)
            {
                if (odometro.Odometro.EsIterativo) odometro.Kilometros += kilometros;
                else odometro.Kilometros = coche.TotalOdometer;
            }
            else odometro.Kilometros = 0;
        }

        private void UpdateOdometerDays(MovOdometroVehiculo odometro, int dias)
        {
            if (odometro.Odometro.PorTiempo) odometro.Dias += dias;
            else odometro.Dias = 0;
        }

        private void UpdateOdometerHours(MovOdometroVehiculo odometro, double horas)
        {
            if (odometro.Odometro.PorHoras) odometro.Horas += horas;
            else odometro.Horas = 0;
        }

        private void CheckDailyOdometer(Coche coche, DateTime fin)
        {
            var refference = coche.KilometrosDiarios.Equals(0) ? coche.TipoCoche.KilometrosDiarios : coche.KilometrosDiarios;

            if (refference.Equals(0)) return;

            if (coche.DailyOdometer < refference) return;

            if (coche.LastDailyOdometerRaise.HasValue && coche.LastDailyOdometerRaise.Value.AddHours(-3).DayOfYear == fin.AddHours(-3).DayOfYear) return;

            var text = String.Format(" (Referencia: {0}km)", (Int32)refference);

            var lastPosition = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(coche.Id, fin, 1);
            MessageSaver.Save(MessageCode.KilometersExceded.GetMessageCode(), coche, fin, lastPosition.ToGpsPoint(), text);

            coche.LastDailyOdometerRaise = fin;
        }

        private DateTime GetLastDatamartUpdate(int vehicle) { return DaoFactory.DatamartDAO.GetLastDatamartUpdate(vehicle); }

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
    }
}
