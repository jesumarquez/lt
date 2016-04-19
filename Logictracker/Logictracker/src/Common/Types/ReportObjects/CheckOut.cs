using System;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class CheckOut
    {
        public class Modo
        {
            public const int CheckIn = 1;
            public const int CheckOut = 2;
            public const int Acumulado = 3;
            public const int AcumuladoPorc = 4;
        }

        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }

        public CheckOut(DateTime date, int cantidad)
        {
            Fecha = date;
            Cantidad = cantidad;
        }
    }
}
