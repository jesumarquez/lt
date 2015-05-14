using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects.Datamart;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class KmHsProductivosVo
    {
        public const int IndexTipoVehiculo = 0;
        public const int IndexVehiculo = 1;

        public const int IndexKmTotales = 2;
        public const int IndexHsTotales = 3;
        public const int IndexEnMovimiento = 4;
        public const int IndexDetenido = 5;
        public const int IndexRalenti = 6;
        public const int IndexSinReportar = 7;

        public const int IndexKmTurnos = 8;
        public const int IndexHsTotalesTurno = 9;
        public const int IndexEnMovimientoTurno = 10;
        public const int IndexDetenidoTurno = 11;
        public const int IndexRalentiTurno = 12;
        public const int IndexSinReportarTurno = 13;

        public const int IndexKmViajes = 14;
        public const int IndexHsTotalesViaje = 15;
        public const int IndexEnMovimientoViaje = 16;
        public const int IndexDetenidoViaje = 17;
        public const int IndexRalentiViaje = 18;
        public const int IndexSinReportarViaje = 19;

        public const int IndexKmTimeTracking = 20;
        public const int IndexHsTotalesTimeTracking = 21;
        public const int IndexEnMovimientoTimeTracking = 22;
        public const int IndexDetenidoTimeTracking = 23;
        public const int IndexRalentiTimeTracking = 24;
        public const int IndexSinReportarTimeTracking = 25;

        public const int IndexKmTickets = 26;
        public const int IndexHsTotalesTicket = 27;
        public const int IndexEnMovimientoTicket = 28;
        public const int IndexDetenidoTicket = 29;
        public const int IndexRalentiTicket = 30;
        public const int IndexSinReportarTicket = 31;

        public int Id { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true, IsInitialGroup = true, IncludeInSearch = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexKmTotales, ResourceName = "Labels", VariableName = "KM_TOTALES", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0.00}")]
        public double KmTotales { get; set; }

        [GridMapping(Index = IndexHsTotales, ResourceName = "Labels", VariableName = "HS_TOTALES", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan HsTotales { get; set; }

        [GridMapping(Index = IndexEnMovimiento, ResourceName = "Labels", VariableName = "EN_MOVIMIENTO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan EnMovimiento { get; set; }

        [GridMapping(Index = IndexDetenido, ResourceName = "Labels", VariableName = "DETENIDO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan Detenido { get; set; }

        [GridMapping(Index = IndexRalenti, ResourceName = "Labels", VariableName = "EN_RALENTI", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan Ralenti { get; set; }

        [GridMapping(Index = IndexSinReportar, ResourceName = "Labels", VariableName = "SIN_REPORTAR", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan SinReportar { get; set; }

        [GridMapping(Index = IndexKmTurnos, ResourceName = "Labels", VariableName = "KM_TURNOS", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0.00}")]
        public double KmTurnos { get; set; }

        [GridMapping(Index = IndexHsTotalesTurno, ResourceName = "Labels", VariableName = "HS_TURNO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan HsTotalesTurno { get; set; }

        [GridMapping(Index = IndexEnMovimientoTurno, ResourceName = "Labels", VariableName = "EN_MOVIMIENTO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan EnMovimientoTurno { get; set; }

        [GridMapping(Index = IndexDetenidoTurno, ResourceName = "Labels", VariableName = "DETENIDO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan DetenidoTurno { get; set; }

        [GridMapping(Index = IndexRalentiTurno, ResourceName = "Labels", VariableName = "EN_RALENTI", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan RalentiTurno { get; set; }

        [GridMapping(Index = IndexSinReportarTurno, ResourceName = "Labels", VariableName = "SIN_REPORTAR", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan SinReportarTurno { get; set; }

        [GridMapping(Index = IndexKmViajes, ResourceName = "Labels", VariableName = "KM_VIAJES", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0.00}")]
        public double KmViajes { get; set; }

        [GridMapping(Index = IndexHsTotalesViaje, ResourceName = "Labels", VariableName = "HS_EN_VIAJE", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan HsTotalesViaje { get; set; }

        [GridMapping(Index = IndexEnMovimientoViaje, ResourceName = "Labels", VariableName = "EN_MOVIMIENTO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan EnMovimientoViaje { get; set; }

        [GridMapping(Index = IndexDetenidoViaje, ResourceName = "Labels", VariableName = "DETENIDO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan DetenidoViaje { get; set; }

        [GridMapping(Index = IndexRalentiViaje, ResourceName = "Labels", VariableName = "EN_RALENTI", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan RalentiViaje { get; set; }

        [GridMapping(Index = IndexSinReportarViaje, ResourceName = "Labels", VariableName = "SIN_REPORTAR", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan SinReportarViaje { get; set; }

        [GridMapping(Index = IndexKmTimeTracking, ResourceName = "Labels", VariableName = "KM_TIME_TRACKING", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0.00}")]
        public double KmTimeTracking { get; set; }

        [GridMapping(Index = IndexHsTotalesTimeTracking, ResourceName = "Labels", VariableName = "HS_EN_TIME_TRACKING", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan HsTotalesTimeTracking { get; set; }

        [GridMapping(Index = IndexEnMovimientoTimeTracking, ResourceName = "Labels", VariableName = "EN_MOVIMIENTO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan EnMovimientoTimeTracking { get; set; }

        [GridMapping(Index = IndexDetenidoTimeTracking, ResourceName = "Labels", VariableName = "DETENIDO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan DetenidoTimeTracking { get; set; }

        [GridMapping(Index = IndexRalentiTimeTracking, ResourceName = "Labels", VariableName = "EN_RALENTI", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan RalentiTimeTracking { get; set; }

        [GridMapping(Index = IndexSinReportarTimeTracking, ResourceName = "Labels", VariableName = "SIN_REPORTAR", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan SinReportarTimeTracking { get; set; }

        [GridMapping(Index = IndexKmTickets, ResourceName = "Labels", VariableName = "KM_TICKETS", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0.00}")]
        public double KmTickets { get; set; }

        [GridMapping(Index = IndexHsTotalesTicket, ResourceName = "Labels", VariableName = "HS_EN_TICKET", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan HsTotalesTicket { get; set; }

        [GridMapping(Index = IndexEnMovimientoTicket, ResourceName = "Labels", VariableName = "EN_MOVIMIENTO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan EnMovimientoTicket { get; set; }

        [GridMapping(Index = IndexDetenidoTicket, ResourceName = "Labels", VariableName = "DETENIDO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan DetenidoTicket { get; set; }

        [GridMapping(Index = IndexRalentiTicket, ResourceName = "Labels", VariableName = "EN_RALENTI", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan RalentiTicket { get; set; }

        [GridMapping(Index = IndexSinReportarTicket, ResourceName = "Labels", VariableName = "SIN_REPORTAR", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan SinReportarTicket { get; set; }

        public KmHsProductivosVo(Coche coche, DateTime desde, DateTime hasta)
        {
            var dao = new DAOFactory();

            Id = coche.Id;
            TipoVehiculo = coche.TipoCoche.Descripcion;
            Vehiculo = coche.Interno;

            var hsTotales = hasta.Subtract(desde);
            var enMovimiento = new TimeSpan(0,0,0);
            var detenido = new TimeSpan(0,0,0);
            var ralenti = new TimeSpan(0,0,0);
            var sinReportar = new TimeSpan(0,0,0);

            var kmTurnos = 0.00;
            var hsTotalesTurno = new TimeSpan(0,0,0);
            var enMovimientoTurno = new TimeSpan(0,0,0);
            var detenidoTurno = new TimeSpan(0,0,0);
            var ralentiTurno = new TimeSpan(0,0,0);
            var sinReportarTurno = new TimeSpan(0,0,0);

            var kmViajes = 0.00;
            var hsTotalesViaje = new TimeSpan(0,0,0);
            var enMovimientoViaje = new TimeSpan(0,0,0);
            var detenidoViaje = new TimeSpan(0,0,0);
            var ralentiViaje = new TimeSpan(0,0,0);
            var sinReportarViaje = new TimeSpan(0,0,0);

            var kmTimeTracking = 0.00;
            var hsTotalesTimeTracking = new TimeSpan(0,0,0);
            var enMovimientoTimeTracking = new TimeSpan(0,0,0);
            var detenidoTimeTracking = new TimeSpan(0,0,0);
            var ralentiTimeTracking = new TimeSpan(0,0,0);
            var sinReportarTimeTracking = new TimeSpan(0,0,0);

            var kmTickets = 0.00;
            var hsTotalesTicket = new TimeSpan(0,0,0);
            var enMovimientoTicket = new TimeSpan(0,0,0);
            var detenidoTicket = new TimeSpan(0,0,0);
            var ralentiTicket = new TimeSpan(0,0,0);
            var sinReportarTicket = new TimeSpan(0, 0, 0);

            var data = dao.DatamartDAO.GetBetweenDates(coche.Id, desde, hasta);

            foreach (var datamart in data)
            {
                // TOTALES
                enMovimiento = enMovimiento.Add(TimeSpan.FromHours(datamart.MovementHours));
                detenido = detenido.Add(TimeSpan.FromHours(datamart.StoppedHours));
                sinReportar = sinReportar.Add(TimeSpan.FromHours(datamart.NoReportHours));

                if (datamart.EngineStatus == Datamart.EstadosMotor.Encendido)
                    ralenti = ralenti.Add(TimeSpan.FromHours(datamart.StoppedHours));

                // TICKETS
                if (datamart.IdCiclo != null && datamart.IdCiclo > 0)
                {
                    kmTickets += dao.CocheDAO.GetDistance(coche.Id, datamart.Begin, datamart.End);
                    hsTotalesTicket = hsTotalesTicket.Add(datamart.End.Subtract(datamart.Begin));

                    enMovimientoTicket = enMovimientoTicket.Add(TimeSpan.FromHours(datamart.MovementHours));
                    detenidoTicket = detenidoTicket.Add(TimeSpan.FromHours(datamart.StoppedHours));
                    sinReportarTicket = sinReportarTicket.Add(TimeSpan.FromHours(datamart.NoReportHours));

                    if (datamart.EngineStatus == Datamart.EstadosMotor.Encendido)
                        ralentiTicket = ralentiTicket.Add(TimeSpan.FromHours(datamart.StoppedHours));
                }

                // TIME TRACKING
                if (datamart.EnTimeTracking != null && datamart.EnTimeTracking == true)
                {
                    kmTimeTracking += dao.CocheDAO.GetDistance(coche.Id, datamart.Begin, datamart.End);
                    hsTotalesTimeTracking = hsTotalesTimeTracking.Add(datamart.End.Subtract(datamart.Begin));

                    enMovimientoTimeTracking = enMovimientoTimeTracking.Add(TimeSpan.FromHours(datamart.MovementHours));
                    detenidoTimeTracking = detenidoTimeTracking.Add(TimeSpan.FromHours(datamart.StoppedHours));
                    sinReportarTimeTracking = sinReportarTimeTracking.Add(TimeSpan.FromHours(datamart.NoReportHours));

                    if (datamart.EngineStatus == Datamart.EstadosMotor.Encendido)
                        ralentiTimeTracking = ralentiTimeTracking.Add(TimeSpan.FromHours(datamart.StoppedHours));
                }

                // VIAJES
                if (datamart.EngineStatus == Datamart.EstadosMotor.Encendido)
                {
                    kmViajes += dao.CocheDAO.GetDistance(coche.Id, datamart.Begin, datamart.End);
                    hsTotalesViaje = hsTotalesViaje.Add(datamart.End.Subtract(datamart.Begin));

                    enMovimientoViaje = enMovimientoViaje.Add(TimeSpan.FromHours(datamart.MovementHours));
                    detenidoViaje = detenidoViaje.Add(TimeSpan.FromHours(datamart.StoppedHours));
                    sinReportarViaje = sinReportarViaje.Add(TimeSpan.FromHours(datamart.NoReportHours));
                    ralentiViaje = ralentiViaje.Add(TimeSpan.FromHours(datamart.StoppedHours));
                }

                // TURNOS
                //if (datamart.EnTurno != null && datamart.EnTurno == true)
                {
                    kmTurnos += dao.CocheDAO.GetDistance(coche.Id, datamart.Begin, datamart.End);
                    hsTotalesTurno = hsTotalesTurno.Add(datamart.End.Subtract(datamart.Begin));

                    enMovimientoTurno = enMovimientoTurno.Add(TimeSpan.FromHours(datamart.MovementHours));
                    detenidoTurno = detenidoTurno.Add(TimeSpan.FromHours(datamart.StoppedHours));
                    sinReportarTurno = sinReportarTurno.Add(TimeSpan.FromHours(datamart.NoReportHours));

                    if (datamart.EngineStatus == Datamart.EstadosMotor.Encendido)
                        ralentiTurno = ralentiTurno.Add(TimeSpan.FromHours(datamart.StoppedHours));
                }
            }

            KmTotales = dao.CocheDAO.GetDistance(coche.Id, desde, hasta);
            HsTotales = hsTotales;
            EnMovimiento = enMovimiento;
            Detenido = detenido;
            Ralenti = ralenti;
            SinReportar = sinReportar;

            KmTurnos = kmTurnos;
            HsTotalesTurno = hsTotalesTurno;
            EnMovimientoTurno = enMovimientoTurno;
            DetenidoTurno = detenidoTurno;
            RalentiTurno = ralentiTurno;
            SinReportarTurno = sinReportarTurno;

            KmViajes = kmViajes;
            HsTotalesViaje = hsTotalesViaje;
            EnMovimientoViaje = enMovimientoViaje;
            DetenidoViaje = detenidoViaje;
            RalentiViaje = ralentiViaje;
            SinReportarViaje = sinReportarViaje;

            KmTimeTracking = kmTimeTracking;
            HsTotalesTimeTracking = hsTotalesTimeTracking;
            EnMovimientoTimeTracking = enMovimientoTimeTracking;
            DetenidoTimeTracking = detenidoTimeTracking;
            RalentiTimeTracking = ralentiTimeTracking;
            SinReportarTimeTracking = sinReportarTimeTracking;

            KmTickets = kmTickets;
            HsTotalesTicket = hsTotalesTicket;
            EnMovimientoTicket = enMovimientoTicket;
            DetenidoTicket = detenidoTicket;
            RalentiTicket = ralentiTicket;
            SinReportarTicket = sinReportarTicket;
        }
    }
}
