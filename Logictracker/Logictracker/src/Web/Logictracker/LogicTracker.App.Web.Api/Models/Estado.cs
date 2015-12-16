using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class Estado
    {       
        public int id { get; set; }

        public string transportista { get; set; }

        public string enhorario { get; set; }

        public string cliente { get; set; }
        
        public DateTime fechahora { get; set; }

        public string empleado { get; set; }

        public string estado { get; set; }

        public string observacion { get; set; }
    }
}