using System;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ReporteInOutVo
    {
        public const int IndexTipo = 0;
        public const int IndexIn = 1;
        public const int IndexOut = 2;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexTipo, ResourceName = "Entities", VariableName = "PARENTI10")]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexIn, ResourceName = "Labels", VariableName = "IN")]
        public int In { get; set; }

        [GridMapping(Index = IndexOut, ResourceName = "Labels", VariableName = "OUT")]
        public int Out { get; set; }

        public ReporteInOutVo(string tipo, int vehiclesIn, int vehiclesOut)
        {
            Tipo = tipo;
            In = vehiclesIn;
            Out = vehiclesOut;
        }
    }
}
