using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class KilometrosDiariosVo
    {   
        public const int IndexVehiculo = 0;
        public const int IndexFecha = 1;
        public const int IndexKmTotales = 2;
        public const int IndexKmEnRuta = 3;

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IsInitialGroup = true, AllowGroup = true, InitialSortExpression = true, InitialGroupIndex = 0)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", InitialSortExpression = true, DataFormatString = "{0:d}")]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexKmTotales, ResourceName = "Labels", VariableName = "KM_TOTALES", DataFormatString = "{0: 0.00}")]
        public double KmTotales { get; set; }

        [GridMapping(Index = IndexKmEnRuta, ResourceName = "Labels", VariableName = "KM_EN_RUTA", DataFormatString = "{0: 0.00}")]
        public double KmEnRuta { get; set; }

        public KilometrosDiariosVo(KilometrosDiarios kilometrosDiarios)
        {
            Fecha = kilometrosDiarios.Fecha;
            Vehiculo = kilometrosDiarios.Vehiculo;
            KmTotales = kilometrosDiarios.KmTotales;
            KmEnRuta = kilometrosDiarios.KmEnRuta;
        }
    }
}
