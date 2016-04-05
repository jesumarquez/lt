using System;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class CheckOut
    {
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }

        public CheckOut(DateTime date, int cantidad)
        {
            Fecha = date;
            Cantidad = cantidad;
        }
    }
}
