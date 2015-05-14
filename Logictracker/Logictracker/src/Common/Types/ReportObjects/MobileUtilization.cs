namespace Logictracker.Types.ReportObjects
{
    public class MobileUtilization
    {
        public string Interno { get; set; }
        public string Centro { get; set; }
        public double HsTurno { get; set; }
        public double HsEsperadas { get; set; }
        public double PorcentajeEsperado { get; set;}
        public double HsTurnoReales { get; set; }
        public double PorcentajeTurno { get; set; }
        public double HsFueraTurno { get; set; }
        public double HsRealesFueraTurno { get; set; }
        public double PorcentajeFueraTurno { get; set; }

        public double PorcentajeProd { get; set; }
        public double PorcentajeTotal { get; set; }

        public int IdCentro { get; set; }
        public int IdVehiculo { get; set; }
    }
}
