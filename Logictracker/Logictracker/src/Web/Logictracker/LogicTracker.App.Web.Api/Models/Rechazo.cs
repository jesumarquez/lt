using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class Rechazo
    {
        public int id { get; set; }

        public DateTime fechahora { get; set; }

        public string motivo { get; set; }

        public string estado { get; set; }

        public string bultos { get; set; }

        public string codentrega { get; set; }

        public string vendedor { get; set; }

        public string supventa { get; set; }

        public string supruta { get; set; }

        public string territorio { get; set; }

        public Estado[] estados { get; set; }

        public string nombre { get; set; }

        public string usuariomobile { get; set; }
    }
}