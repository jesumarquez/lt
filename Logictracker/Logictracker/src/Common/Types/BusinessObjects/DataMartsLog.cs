using System;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class DataMartsLog : IAuditable
    {
        public static class Moludos
        {
            public const short DatamartRecorridos = 0;
            public const short DatamartEntregas = 1;
            public const short DatamartRutas = 2;
            public const short DatamartTramos = 3;
            public const short DatamartEstadoVehiculos = 4; 
            public const short VencimientoDocumentos = 5;
            public const short ExcesoVelocidadSitrack = 6;
            public const short VehicleData = 7;
            public const short ReportScheduler = 8;
            public const short GeneracionInversa = 9;
            public const short InformeViajeDescarga = 10;

            public static string GetString(short modulo)
            {
                switch (modulo)
                {
                    case DatamartRecorridos: return "Datamart Recorridos";
                    case DatamartEntregas: return "Datamart Entregas";
                    case DatamartRutas: return "Datamart Rutas";
                    case DatamartTramos: return "Datamart Tramos";
                    case DatamartEstadoVehiculos: return "Datamart Estado Vehículos";
                    case VencimientoDocumentos: return "Vencimiento de documentos";
                    case ExcesoVelocidadSitrack: return "Excesos de Velocidad Sitrack";
                    case VehicleData: return "Vehicle Data";
                    case ReportScheduler: return "Report Scheduler";
                    case GeneracionInversa: return "Generación Inversa";
                    case InformeViajeDescarga: return "Informe Viaje-Descarga";
                    default: return string.Empty;
                }
            }
        }

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        
        public virtual DateTime? FechaInicio { get; set; }
        public virtual DateTime? FechaFin { get; set; }        
        public virtual short Modulo { get; set; }
        public virtual string Mensaje   { get; set; }
        public virtual double Duracion { get; set; }
    }
}