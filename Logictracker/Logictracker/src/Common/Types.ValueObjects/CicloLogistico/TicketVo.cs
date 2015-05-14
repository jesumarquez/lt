using System;
using System.Linq;
using Iesi.Collections;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Tickets;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class TicketVo
    {
        public const int IndexOrdenDiario = 0;
        public const int IndexPedido = 1;
        public const int IndexCodigo = 2;
        
        public const int IndexBocaDeCarga = 3;
        public const int IndexFecha = 4;
        public const int IndexHora = 5;
        public const int IndexCoche = 6;
        public const int IndexCliente = 7;
        public const int IndexPuntoDeEntrega = 8;
		public const int IndexBaseDestino = 9;
        public const int IndexEstado = 10;

        [GridMapping(Index = IndexOrdenDiario, ResourceName = "Labels", VariableName = "ORDEN_DIARIO", InitialSortExpression = true)]
        public int OrdenDiario { get; set; }

        [GridMapping(Index = IndexPedido, ResourceName = "Entities", VariableName = "PEDIDO", AllowGroup = true, IncludeInSearch = true)]
        public string CodigoPedido { get; set; }

        [GridMapping(Index = IndexBocaDeCarga, ResourceName = "Entities", VariableName = "PARTICK04", AllowGroup = true, IncludeInSearch = true)]
        public string BocaDeCarga { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:d}", AllowGroup = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexHora, ResourceName = "Labels", VariableName = "HORA", DataFormatString = "{0:t}")]
        public DateTime Hora { get; set; }

        [GridMapping(Index = IndexCoche, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, IncludeInSearch = true)]
        public string Coche { get; set; }

        [GridMapping(Index = IndexCliente, ResourceName = "Entities", VariableName = "CLIENT", AllowGroup = true, IncludeInSearch = true)]
        public string Cliente { get; set; }

        [GridMapping(Index = IndexPuntoDeEntrega, ResourceName = "Entities", VariableName = "PARENTI44", AllowGroup = true, IncludeInSearch = true)]
        public string PuntoDeEntrega { get; set; }

        [GridMapping(Index = IndexBaseDestino, ResourceName = "Labels", VariableName = "BASE_DESTINO", AllowGroup = true, IncludeInSearch = true)]
        public string BaseDestino { get; set; }

        [GridMapping(Index = IndexEstado, IsTemplate = true, AllowGroup = false, ResourceName = "Labels", VariableName = "STATE", Width = "100px")]
        public short Estado { get; set; }

        public bool PuntoEntregaNomenclado { get; set; }

        public bool ClienteNomenclado { get; set; }

        public bool HasCoche { get; set; }

        private ISet _detalles;

        public virtual ISet Detalles
        {
            get { if (_detalles == null) _detalles = new ListSet(); return _detalles; }
            set { _detalles = value; }
        }

        public virtual DateTime? FechaTicket { get; set; }

        [GridMapping(IsDataKey = true, HeaderText = "")]
        public int Id { get; set; }

        public TicketVo(Ticket ticket)
        {
            Id = ticket.Id;
            Codigo = ticket.Codigo;
            Estado = ticket.Estado;
            HasCoche = ticket.Vehiculo != null;
            Coche = ticket.Vehiculo != null ? ticket.Vehiculo.Interno : string.Empty;

            OrdenDiario = ticket.OrdenDiario.HasValue ? ticket.OrdenDiario.Value : 0;

            var first = ticket
				.Detalles
				.OfType<DetalleTicket>()
				.OrderBy(d => d.Programado.HasValue ? d.Programado.Value : DateTime.MinValue)
				.Select(d => d.Programado)
                .FirstOrDefault();

            Fecha = !first.HasValue || first.Equals(default(DateTime?))
                        ? (ticket.FechaTicket.HasValue ? ticket.FechaTicket.Value.ToDisplayDateTime() : DateTime.MinValue)
                        : first.Value.ToDisplayDateTime();
            Hora = Fecha;
            Detalles = ticket.Detalles;
            FechaTicket = ticket.FechaTicket;

            if (ticket.Pedido != null)
            {
                CodigoPedido = ticket.Pedido.Codigo;
                BocaDeCarga = ticket.Pedido.BocaDeCarga.Descripcion;
            }
            Cliente = ticket.Cliente.Descripcion;
            PuntoDeEntrega = ticket.PuntoEntrega.Descripcion;
            BaseDestino = ticket.BaseDestino != null ? ticket.BaseDestino.Descripcion : CultureManager.GetLabel("TODOS");

            PuntoEntregaNomenclado = ticket.PuntoEntrega.Nomenclado;
            ClienteNomenclado = ticket.Cliente.Nomenclado;
        }
    }
}
