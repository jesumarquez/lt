#region Usings

using System;
using Logictracker.Types.BusinessObjects.Tickets;

#endregion

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class TicketReport
    {
        public TicketReport(Ticket tck)
        {
            Camion = (tck.Vehiculo != null) ? tck.Vehiculo.ToString() : string.Empty;
            Chofer = (tck.Empleado != null) ? tck.Empleado.Entidad.Descripcion : string.Empty;
            Ticket = tck.Codigo;
            Cliente = (tck.Cliente != null )?tck.Cliente.Descripcion : string.Empty;
            FechaInicio = tck.FechaTicket;
            FechaFin = tck.FechaFin;
            TiempoTotal = (tck.FechaFin.Value) - (tck.FechaTicket.Value); //Tiempo total del Ticket.  
            DescripcionProducto = tck.DescripcionProducto;
            EstadoLogistico = (tck.EstadoLogistico != null)? tck.EstadoLogistico.Descripcion : string.Empty;
            TicketID = tck.Id;
        }

        #region Private Properties

        #endregion

        #region Public Properties

        public string Camion { get; set; }

        public string Chofer { get; set; }

        public string Ticket { get; set; }

        public string Cliente { get; set; }

        public TimeSpan TiempoTotal { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public string DescripcionProducto { get; set; }

        public string EstadoLogistico { get; set; }

        public int TicketID { get; set; }

        #endregion
    }
}