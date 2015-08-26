using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class ProgramacionReporte : IAuditable, IHasEmpresa, IHasLinea
    {
        #region IAuditable Members

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual string Report { get; set; }
        public virtual string Vehicles { get; set; }
        public virtual char Periodicity { get; set; }
        public virtual bool Active{ get; set; }
        public virtual string Mail { get; set; }
        public virtual string Drivers { get; set; }
        public virtual string MessageTypes { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual short Format { get; set; }
        public virtual string ReportName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool InCicle { get; set; }
        public virtual int OvercomeKilometers { get; set; }
        public virtual bool ShowCorners { get; set; }
        public virtual string Geofences { get; set; }
        public virtual bool CalculateKm { get; set; }
        public virtual double GeofenceTime { get; set; }
        public virtual string Documents { get; set; }
        public virtual string Odometers { get; set; }
        public virtual bool ShowDetours { get; set; }
        //public virtual TimeSpan DetentionTime { get; set; }
        //public virtual double Distance { get; set; }
        //public virtual TimeSpan UnreportedTime { get; set; }

        public static class Reportes
        {
            public const string ReporteEventos = "EventsReport";
            public const string VerificadorVehiculos = "VerificadorVehiculos";
            public const string ActividadVehicular = "VehicleActivityReport";
            public const string KilometrosAcumulados = "AccumulatedKilometersReport";
            public const string InfraccionesVehiculo = "VehicleInfractionsReport";
            public const string InfraccionesConductor = "DriversInfractionsReport";
            public const string EventosGeocercas = "GeofenceEventsReport";
            public const string VencimientoDocumentos = "DocumentsExpirationReport";
            public const string TiempoAcumulado = "MobilesTimeReport";
            public const string ReporteOdometros = "OdometersReport";
            public const string TrasladosViaje = "TransfersPerTripReport";
            public const string EstadoEntregas = "DeliverStatusReport";
            public const string ResumenRutas = "SummaryRoutes";
        }

        public static class FormatoReporte
        {
            public const short Excel = 0;
            public const short Html = 1;
            public const short HtmlAttached = 2;
            public const short Csv = 3;
            public const short CsvAttached = 4;
        }
    }
}
