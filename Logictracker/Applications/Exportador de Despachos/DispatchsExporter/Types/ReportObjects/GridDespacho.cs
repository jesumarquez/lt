using System;
using System.Drawing;

namespace DispatchsExporter.Types.ReportObjects
{
    public class GridDespacho
    {
        public int MobileID { get; set; }
        public int ID { get; set; }
        public string Interno { get; set; }
        public double Volumen { get; set; }
        public DateTime Fecha { get; set; }
        public string Patente { get; set; }
        public string CodigoCentroDeCostos { get; set; }
        public string DescriCentroDeCostos { get; set; }
        public Image Icon { get; set; }
        public String Operador { get; set; }
    }
}