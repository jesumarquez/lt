using System;
using System.Collections.Generic;
using System.Linq;
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
        public virtual char Periodicity { get; set; }
        public virtual bool Active{ get; set; }
        public virtual string Mail { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual FormatoReporte Format { get; set; }
        public virtual string ReportName { get; set; }
        public virtual string Description { get; set; }

        public virtual IList<ParametroReportesProg> Parametros { get; set; }

        public ProgramacionReporte()
        {
            Parametros=new List<ParametroReportesProg>();
            Format = FormatoReporte.Excel;
        }

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

            public static int GetDropDownListIndex(string reporte)
            {
                switch (reporte)
                {
                    case EstadoEntregas:
                        return 1;
                    default:
                        return 0;
                }
            }
        }        

        public enum FormatoReporte
        {
            Excel,
            Html,
            HtmlAttached,
            Csv,
            CsvAttached
        }

        public virtual void AddParameterList(IList<int> idList, ParameterType parameterType)
        {
            foreach (var selectedId in idList)
            {
                if (selectedId!=0)
                    Parametros.Add(new ParametroReportesProg
                    {
                        EntityId = selectedId,
                        ParameterType = parameterType,
                        ProgramacionReporte = this
                    });
            }
        }

        public virtual List<int> GetParameters(ParameterType parameterType)
        {
            return (from parametro in Parametros where parametro.ParameterType.Equals(parameterType) select parametro.EntityId).ToList();
        }
    }
}
