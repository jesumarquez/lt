using System;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Tickets;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class PedidoVo
    {
        public const int IndexLinea = 0;
        public const int IndexBocaDeCarga = 1;
        public const int IndexCliente = 2;
        public const int IndexPuntoEntrega = 3;
        public const int IndexCodigo = 4;
        public const int IndexFechaEnObra = 5;
        public const int IndexHoraCarga = 6;
        public const int IndexNumeroBomba = 7;
        public const int IndexesMinimixer = 8;
        public const int IndexProducto = 9;
        public const int IndexM3 = 10;
        public const int IndexEstado = 11;
        public const int IndexContacto = 12;
        public const int IndexObservacion = 13;

        public int Id { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02", AllowGroup = true)]
        public string Linea { get; set; }

        [GridMapping(Index = IndexBocaDeCarga, ResourceName = "Entities", VariableName = "BOCADECARGA", AllowGroup = true)]
        public string BocaDeCarga { get; set; }

        [GridMapping(Index = IndexCliente, ResourceName = "Entities", VariableName = "CLIENT", AllowGroup = true, IncludeInSearch = true)]
        public string Cliente { get; set; }

        [GridMapping(Index = IndexPuntoEntrega, ResourceName = "Entities", VariableName = "PARENTI44", AllowGroup = true, IncludeInSearch = true)]
        public string PuntoEntrega { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexFechaEnObra, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false)]
        public DateTime FechaEnObra { get; set; }

        [GridMapping(Index = IndexHoraCarga, ResourceName = "Labels", VariableName = "HORA_CARGA", AllowGroup = false)]
        public DateTime HoraCarga { get; set; }

        [GridMapping(Index = IndexNumeroBomba, ResourceName = "Labels", VariableName = "NUMERO_BOMBA", AllowGroup = false, IncludeInSearch = true)]
        public string NumeroBomba { get; set; }

        [GridMapping(Index = IndexesMinimixer, ResourceName = "Labels", VariableName = "MULTIPLES_REMITOS", AllowGroup = true)]
        public string EsMinimixer { get; set; }

        [GridMapping(Index = IndexProducto, ResourceName = "Entities", VariableName = "PARENTI63", AllowGroup = true)]
        public string Producto { get; set; }

        [GridMapping(Index = IndexM3, ResourceName = "Labels", VariableName = "CANTIDAD_CARGA", AllowGroup = false)]
        public string Cantidad { get; set; }

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true)]
        public string Estado { get; set; }

        [GridMapping(Index = IndexContacto, ResourceName = "Labels", VariableName = "CONTACTO", AllowGroup = false, IncludeInSearch = true)]
        public string Contacto { get; set; }

        [GridMapping(Index = IndexObservacion, ResourceName = "Labels", VariableName = "OBSERVACION", AllowGroup = false, IncludeInSearch = true)]
        public string Observacion { get; set; }

        public int NumeroEstado { get; set; }

        public PedidoVo(Pedido pedido)
        {
            Id = pedido.Id;
            Linea = pedido.Linea.Descripcion;
            BocaDeCarga = pedido.BocaDeCarga.Descripcion;
            Cliente = pedido.Cliente.Descripcion;
            PuntoEntrega = pedido.PuntoEntrega.Descripcion;
            Producto = pedido.Producto != null ? pedido.Producto.Descripcion : "";
            Cantidad = pedido.Cantidad.ToString("#0.0");
            Codigo = pedido.Codigo;
            FechaEnObra = pedido.FechaEnObra.ToDisplayDateTime();
            NumeroEstado = pedido.Estado;
            if (pedido.HoraCarga != DateTime.MinValue)
                HoraCarga = pedido.HoraCarga.ToDisplayDateTime();
            NumeroBomba = pedido.NumeroBomba;
            Contacto = pedido.Contacto;
            Observacion = pedido.Observacion;

            string estado = null;
            switch(pedido.Estado)
            {
                case Pedido.Estados.Pendiente: estado = "ESTADO_PEDIDO_PENDIENTE"; break;
                case Pedido.Estados.EnCurso: estado = "ESTADO_PEDIDO_ENCURSO"; break;
                case Pedido.Estados.Entregado: estado = "ESTADO_PEDIDO_ENTREGADO"; break;
                case Pedido.Estados.Cancelado: estado = "ESTADO_PEDIDO_CANCELADO"; break;
            }
            if (estado != null) Estado = CultureManager.GetLabel(estado);

            EsMinimixer = pedido.EsMinimixer ? "SI" : "NO";
        }
    }
}
