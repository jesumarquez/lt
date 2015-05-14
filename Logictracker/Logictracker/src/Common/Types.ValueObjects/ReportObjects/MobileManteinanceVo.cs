using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileManteinanceVo
    {
        public const int IndexTipoVehiculo = 0;
        public const int IndexInterno = 1;
        public const int IndexPatente = 2;
        public const int IndexReferencia = 3;
        public const int IndexHorasMarcha = 4;
        public const int IndexKilometros = 5;
        public const int IndexHorasPlanta = 6;
        public const int IndexHorasTaller = 7;

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true, IsInitialGroup = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, InitialSortExpression = true, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "Vehículos: {0}")]
        public string Interno { get; set; }

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", AllowGroup = false)]
        public string Patente { get; set; }

        [GridMapping(Index = IndexReferencia, ResourceName = "Labels", VariableName = "REFFERENCE", AllowGroup = true)]
        public string Referencia { get; set; }

        [GridMapping(Index = IndexHorasMarcha, ResourceName = "Labels", VariableName = "EN_MOVIMIENTO", AllowGroup = false)]
        public string HorasMarcha { get; set; }

        [GridMapping(Index = IndexKilometros, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:2} km", AllowGroup = false)]
        public double Kilometros { get; set; }

        [GridMapping(Index = IndexHorasPlanta, ResourceName = "Labels", VariableName = "HORAS_PLANTA", AllowGroup = false)]
        public string HorasPlanta { get; set; }

        [GridMapping(Index = IndexHorasTaller, ResourceName = "Labels", VariableName = "HORAS_TALLER", AllowGroup = false)]
        public string HorasTaller { get; set; }

        public MobileManteinanceVo(MobileMaintenance t)
        {
            TipoVehiculo = t.TipoVehiculo;
            Interno = t.Interno;
            Patente = t.Patente;
            Referencia = t.Referencia;
            Kilometros = t.Kilometros;
            HorasMarcha = t.HorasMarcha;
            HorasPlanta = t.HorasPlanta;
            HorasTaller = t.HorasTaller;
        }
    }
}
