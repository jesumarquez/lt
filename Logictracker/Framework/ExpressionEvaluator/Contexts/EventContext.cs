#region Usings

using System;

#endregion

namespace Logictracker.ExpressionEvaluator.Contexts
{
    public class EventContext
    {
        public int Duracion { get; set; }
        public int Exceso { get; set; }
        public bool TieneTicket { get; set; }
        public string Dispositivo { get; set;}
        public string Interno { get; set; }
        public string Legajo { get; set; }
        public string Texto { get; set; }
        public int VelocidadPermitida { get; set; }
        public int VelocidadAlcanzada { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
