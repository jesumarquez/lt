using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects.Datamart;

namespace Logictracker.Scheduler.Tasks.Mantenimiento.Util
{
    public class Datamarter
    {
	    private PeriodData Data { get; set; }

        public Datamarter(PeriodData data)
        {
            Data = data;
        }

        public List<Datamart> CreateNoReportDatamartRecords(DateTime inicio, DateTime fin, double km)
        {
            var listado = new List<Datamart>();
            var fechas = Data.FechasDeCorte.Where(f => f > inicio && f < fin);
            var fraccionKm = km / (fechas.Count() + 1);

            var ini = inicio;
            foreach(var fecha in fechas)
            {
                listado.Add(CreateDatamartRecord(ini, fecha, fraccionKm, null));
                ini = fecha;
            }
            listado.Add(CreateDatamartRecord(ini, fin, fraccionKm, null));

            return listado;
        }
		public Datamart CreateDatamartRecord(DateTime desde, DateTime hasta, double km, String estadoMotor, String vehicleStatus, 
            int avgSpeed, int minSpeed, int maxSpeed, Zona zona)
        {
            var sinRep = vehicleStatus == Datamart.Estadosvehiculo.SinReportar;
            var datamart = CreateDatamartRecord(desde, hasta, km, zona);
            datamart.EngineStatus = estadoMotor;
            datamart.Kilometers = km;
            datamart.MovementHours = !sinRep && km > 0 ? hasta.Subtract(desde).TotalHours : 0;
            datamart.StoppedHours = !sinRep && km == 0 ? hasta.Subtract(desde).TotalHours : 0;
            datamart.NoReportHours = sinRep ? hasta.Subtract(desde).TotalHours : 0;
            datamart.AverageSpeed = avgSpeed;
            datamart.MinSpeed = minSpeed == int.MaxValue ? 0: minSpeed;
            datamart.MaxSpeed = maxSpeed;
            datamart.VehicleStatus = sinRep ? Datamart.Estadosvehiculo.SinReportar
                : km > 0 ? Datamart.Estadosvehiculo.EnMovimiento
                : Datamart.Estadosvehiculo.Detenido;
            datamart.HorasMarcha = estadoMotor == Datamart.EstadosMotor.Encendido ? hasta.Subtract(desde).TotalHours : 0;
            return datamart;
        }

        private Datamart CreateDatamartRecord(DateTime desde, DateTime hasta, double km, Zona zona)
        {
            var excesos = Data.GetExcesos(desde, hasta);
            int idCiclo;
            var tipoCiclo = Data.GetTipoCiclo(desde, out idCiclo);

            return new Datamart
            {
                Begin = desde,
                Employee = Data.GetChofer(desde),
                End = hasta,
                EngineStatus = Datamart.EstadosMotor.Indefinido, //falta
                GeograficRefference = Data.GetGeoReference(desde),
                Vehicle = Data.Vehiculo,
                Kilometers = km,
                MovementHours = 0, //falta
                StoppedHours = 0, //falta
                NoReportHours = hasta.Subtract(desde).TotalHours,//falta
                HorasMarcha = 0, //falta
                Consumo = Data.GetConsumo(hasta),
                Infractions = excesos.Excesos,
                InfractionMinutes = excesos.SegundosExceso / 60.0,
                AverageSpeed = 0, //falta
                MinSpeed = 0, //falta
                MaxSpeed = 0, //falta
                VehicleStatus = Datamart.Estadosvehiculo.SinReportar, //falta
                Shift = Data.GetTurno(desde),
                TipoCiclo = tipoCiclo,
                IdCiclo = idCiclo,
                EnTimeTracking = Data.GetTimeTracking(desde),
                Zona = zona
            };
        }
    }
}
