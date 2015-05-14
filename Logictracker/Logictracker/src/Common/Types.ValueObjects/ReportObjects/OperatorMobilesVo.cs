using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class OperatorMobilesVo
    {
        public const int IndexPatente = 0;
        public const int IndexInterno = 1;
        public const int IndexInfracciones = 2;
        public const int IndexKilometros = 3;
        public const int IndexMovimiento = 4;


        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", AllowGroup = false)]
        public string Patente { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Labels", VariableName = "INTERNO", AllowGroup = false, InitialSortExpression = true)]
        public string Interno { get; set; }

        [GridMapping(Index = IndexInfracciones, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public int Infracciones { get; set; }

        [GridMapping(Index = IndexKilometros, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:2} hs", AllowGroup = false)]
        public double Kilometros { get; set; }

        [GridMapping(Index = IndexMovimiento, ResourceName = "Labels", VariableName = "TIEMPO_CONDUCCION", AllowGroup = false)]
        public TimeSpan Movimiento { get; set; }

        public OperatorMobilesVo(OperatorMobiles operatorMobiles)
        {
            Patente = operatorMobiles.Patente;
            Interno = operatorMobiles.Interno;
            Infracciones = operatorMobiles.Infracciones;
            Kilometros = operatorMobiles.Kilometros;
            Movimiento = operatorMobiles.DrivingTime;
        }
    }
}
