using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class IngresosPorEquipoVo
    {
        public const int IndexKeyIdEquipo = 0;

        public const int IndexNombreEquipo = 0;
        public const int IndexTotalCargado = 1;
        public const int IndexCantIngresos = 2;
        public const int IndexUltiMedicion = 3;

        [GridMapping(Index = IndexNombreEquipo, ResourceName = "Entities", VariableName = "PARENTI19", AllowGroup = false, InitialSortExpression = true)]
        public string NombreEquipo { get; set; }

        [GridMapping(Index = IndexTotalCargado, ResourceName = "Labels", VariableName = "TOTAL_CARGADO", DataFormatString = "{0:2} lit", AllowGroup = false)]
        public double TotalCargado { get; set; }

        [GridMapping(Index = IndexCantIngresos, ResourceName = "Labels", VariableName = "MEDICIONES_TOMADAS", AllowGroup = false)]
        public int CantIngresos { get; set; }

        [GridMapping(Index = IndexUltiMedicion, ResourceName = "Labels", VariableName = "ULTIMA_MEDICION", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime UltiMedicion { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdEquipo { get; set; }


        public IngresosPorEquipoVo(IngresosPorEquipo c)
        {
            NombreEquipo = c.NombreEquipo;
            TotalCargado = c.TotalCargado;
            CantIngresos = c.CantIngresos;
            UltiMedicion = c.UltiMedicion;
            IdEquipo = c.IDEquipo;
        }
    }
}
