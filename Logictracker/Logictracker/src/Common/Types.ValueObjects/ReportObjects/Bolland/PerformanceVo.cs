using System;

namespace Logictracker.Types.ValueObjects.ReportObjects.Bolland
{
    [Serializable]
    public class PerformanceVo
    {
        public static class Index
        {
            public const int Interno = 0;
            public const int Patente = 1;
            public const int Kilometros = 2;
            public const int VelocidadMaxima = 3;
            public const int Infracciones = 4;
            public const int Aceleraciones = 5;
            public const int Desaceleraciones = 6;
            public const int Desconexiones  = 7;
            public const int ScoreVelocidad  = 8;
            public const int ScoreInfracciones  = 9;
            public const int ScoreAceleraciones  = 10;
            public const int ScoreDesaceleraciones  = 11;
            public const int ScoreDesconexiones  = 12;
            public const int ScoreTotal = 13;
        }

        [GridMapping(Index = Index.Interno, ResourceName = "Labels", VariableName = "INTERNO", IncludeInSearch = true)]
        public string Interno { get; set; }

        [GridMapping(Index = Index.Patente, ResourceName = "Labels", VariableName = "PATENTE", IncludeInSearch = true)]
        public string Patente { get; set; }

        [GridMapping(Index = Index.Kilometros, ResourceName = "Labels", VariableName = "KM", DataFormatString = "{0:0}")]
        public double Kilometros { get; set; }

        [GridMapping(Index = Index.VelocidadMaxima, ResourceName = "Labels", VariableName = "VELOCIDAD_MAXIMA")]
        public int VelocidadMaxima { get; set; }

        [GridMapping(Index = Index.Infracciones, ResourceName = "Labels", VariableName = "INFRACCIONES")]
        public int Infracciones { get; set; }

        [GridMapping(Index = Index.Aceleraciones, ResourceName = "Labels", VariableName = "ACELERACIONES")]
        public int Aceleraciones { get; set; }

        [GridMapping(Index = Index.Desaceleraciones, ResourceName = "Labels", VariableName = "DESACELERACIONES")]
        public int Desaceleraciones { get; set; }

        [GridMapping(Index = Index.Desconexiones, ResourceName = "Labels", VariableName = "DESCONEXIONES")]
        public int Desconexiones { get; set; }

        [GridMapping(Index = Index.ScoreVelocidad, ResourceName = "Labels", VariableName = "SCORE_VELOCIDAD", DataFormatString = "{0:0}")]
        public double ScoreVelocidad { get; set; }

        [GridMapping(Index = Index.ScoreInfracciones, ResourceName = "Labels", VariableName = "SCORE_INFRACCIONES", DataFormatString = "{0:0}")]
        public double ScoreInfracciones { get; set; }

        [GridMapping(Index = Index.ScoreAceleraciones, ResourceName = "Labels", VariableName = "SCORE_ACELERACIONES", DataFormatString = "{0:0}")]
        public double ScoreAceleraciones { get; set; }

        [GridMapping(Index = Index.ScoreDesaceleraciones, ResourceName = "Labels", VariableName = "SCORE_DESACELERACIONES", DataFormatString = "{0:0}")]
        public double ScoreDesaceleraciones { get; set; }

        [GridMapping(Index = Index.ScoreDesconexiones, ResourceName = "Labels", VariableName = "SCORE_DESCONEXIONES", DataFormatString = "{0:0}")]
        public double ScoreDesconexiones { get; set; }

        [GridMapping(Index = Index.ScoreTotal, ResourceName = "Labels", VariableName = "SCORE_TOTAL", DataFormatString = "{0:0}")]
        public double ScoreTotal { get; set; }

        [GridMapping(IsDataKey = true)]
        public int Id { get; set; }
    }
}
