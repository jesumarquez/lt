using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public int IdEmpresa { get; set; }
        public string Empleado { get; set; }
        public int IdEmpleado { get; set; }
        public string Transportista { get; set; }
        public int IdTransportista { get; set; }
        public string PuntoEntrega { get; set; }
        public int IdPuntoEntrega { get; set; }
        public string CodigoPedido { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string InicioVentana { get; set; }
        public string FinVentana { get; set; }
        public bool Selected { get; set; }
    }
}
