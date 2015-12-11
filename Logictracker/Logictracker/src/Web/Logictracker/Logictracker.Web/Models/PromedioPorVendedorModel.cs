using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Web.Models
{
    public class PromedioPorVendedorModel
    {
        public string Usuario { get; set; }
        public string EstadoIngreso { get; set; }
        public string EstadoEgreso { get; set; }
        public int Intervino { get; set; }
        public float Promedio { get; set; }
    }
}
