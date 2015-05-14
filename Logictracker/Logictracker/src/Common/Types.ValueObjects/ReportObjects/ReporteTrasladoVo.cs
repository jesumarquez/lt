using System;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ReporteTrasladoVo
    {
        public const int IndexTransportista = 0;
        public const int IndexVehiculo = 1;
        public const int IndexFecha = 2;
        public const int IndexViaje = 3;
        public const int IndexKmReales = 4;
        public const int IndexKmProductivo = 5;
        public const int IndexKmImproductivo = 6;
        public const int IndexKmProgramado = 7;
        public const int IndexDiferenciaKm = 8;
        public const int IndexPorcDiferenciaKm = 9;
        public const int IndexTiempoReal = 10;
        public const int IndexTiempoProgramado = 11;
        public const int IndexDiferenciaTiempo = 12;
        public const int IndexPorcDiferenciaTiempo = 13;

        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 0)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 1)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:dd/MM/yyyy}", InitialSortExpression = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexViaje, ResourceName = "Entities", VariableName = "OPETICK03", IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "{0:0}")]
        public string Viaje { get; set; }

        [GridMapping(Index = IndexKmReales, ResourceName = "Labels", VariableName = "KM_REALES", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public double KmReales { get; set; }

        [GridMapping(Index = IndexKmProductivo, ResourceName = "Labels", VariableName = "PRODUCTIVOS", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public double KmProductivos { get; set; }

        [GridMapping(Index = IndexKmImproductivo, ResourceName = "Labels", VariableName = "IMPRODUCTIVOS", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public double KmImproductivos { get; set; }

        [GridMapping(Index = IndexKmProgramado, ResourceName = "Labels", VariableName = "PROGRAMADO", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public double KmProgramado { get; set; }

        [GridMapping(Index = IndexDiferenciaKm, ResourceName = "Labels", VariableName = "DIFERENCIA_KM", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public double DiferenciaKm { get; set; }

        [GridMapping(Index = IndexPorcDiferenciaKm, ResourceName = "Labels", VariableName = "PORC_KM", DataFormatString = "{0:0%}")]
        public double PorcDiferenciaKm { get; set; }

        [GridMapping(Index = IndexTiempoReal, ResourceName = "Labels", VariableName = "TIEMPO_REAL", IsAggregate = true, AggregateType = GridAggregateType.Avg)]
        public TimeSpan TiempoReal { get; set; }

        [GridMapping(Index = IndexTiempoProgramado, ResourceName = "Labels", VariableName = "PROG", IsAggregate = true, AggregateType = GridAggregateType.Avg)]
        public TimeSpan TiempoProgramado { get; set; }

        [GridMapping(Index = IndexDiferenciaTiempo, ResourceName = "Labels", VariableName = "DIFERENCIA_MIN", IsAggregate = true, AggregateType = GridAggregateType.Avg)]
        public TimeSpan DiferenciaTiempo { get; set; }

        [GridMapping(Index = IndexPorcDiferenciaTiempo, ResourceName = "Labels", VariableName = "PORC_MIN", DataFormatString = "{0:0%}")]
        public double PorcDiferenciaTiempo { get; set; }

        public int Id { get; set; }

        public ReporteTrasladoVo(ViajeDistribucion viaje, double kmReales, double trasladoImproductivo, double trasladoProductivo, double kmProgramado)
        {
            Id = viaje.Id;
            Transportista = viaje.Vehiculo.Transportista != null
                                ? viaje.Vehiculo.Transportista.Descripcion
                                : "Sin Transportista";
            Vehiculo = viaje.Vehiculo.Interno;
            Fecha = viaje.Inicio.ToDisplayDateTime();
            Viaje = viaje.Codigo;
            KmImproductivos = trasladoImproductivo;
            KmProductivos = trasladoProductivo;
            KmReales = kmReales;
            KmProgramado = kmProgramado;
            
            DiferenciaKm = KmReales - KmProgramado;
            PorcDiferenciaKm = KmReales == 0.00 ? 0.00 : DiferenciaKm / KmReales;

            TiempoReal = viaje.Fin.Subtract(viaje.InicioReal.Value);

            var inicioProg = viaje.MinTiempoProgramado;
            var finProg = viaje.MaxTiempoProgramado;
            if (inicioProg != null &&
                finProg != null)
                TiempoProgramado = finProg.Value.Subtract(inicioProg.Value);
            else
                TiempoProgramado = new TimeSpan(0, 0, 0, 0);


            DiferenciaTiempo = TiempoReal - TiempoProgramado;
            PorcDiferenciaTiempo = TiempoReal.TotalSeconds > 0 ? DiferenciaTiempo.TotalSeconds / TiempoReal.TotalSeconds : 0.00;
        }
    }
}
