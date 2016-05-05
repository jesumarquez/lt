using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Ordenes;
using System;
using Logictracker.Security;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ReporteAsignoVo
    {
        public const int IndexVehiculo = 0;
        public const int IndexViaje = 1;
        public const int IndexCliente = 2;
        public const int IndexPuntoEntrega = 3;
        public const int IndexEntrega = 4;
        public const int IndexOrden = 5;
        public const int IndexInsumo = 6;
        public const int IndexCantidad = 7;
        public const int IndexAjuste = 8;
        public const int IndexTotal = 9;
        public const int IndexCisterna = 10;
        public const int IndexFechaAlta = 11;
        public const int IndexFechaPedido = 12;
        public const int IndexFechaEntrega = 13;
        
        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IsInitialGroup = true, InitialGroupIndex = 0)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexViaje, ResourceName = "Entities", VariableName = "OPETICK03", IsInitialGroup = true, InitialGroupIndex = 1)]
        public string Viaje { get; set; }

        [GridMapping(Index = IndexCliente, ResourceName = "Entities", VariableName = "PARENTI18", IncludeInSearch = true)]
        public string Cliente { get; set; }

        [GridMapping(Index = IndexPuntoEntrega, ResourceName = "Entities", VariableName = "PARENTI44", IncludeInSearch = true)]
        public string PuntoEntrega { get; set; }

        [GridMapping(Index = IndexEntrega, ResourceName = "Entities", VariableName = "OPETICK04", IncludeInSearch = true)]
        public string Entrega { get; set; }

        [GridMapping(Index = IndexOrden, ResourceName = "Entities", VariableName = "PARENTI105", IncludeInSearch = true)]
        public string Orden { get; set; }

        [GridMapping(Index = IndexInsumo, ResourceName = "Entities", VariableName = "PARENTI58", IncludeInSearch = true)]
        public string Insumo { get; set; }

        [GridMapping(Index = IndexCantidad, ResourceName = "Labels", VariableName = "CANTIDAD", IncludeInSearch = true)]
        public int? Cantidad { get; set; }

        [GridMapping(Index = IndexAjuste, ResourceName = "Labels", VariableName = "AJUSTE", IncludeInSearch = true)]
        public int? Ajuste { get; set; }

        [GridMapping(Index = IndexTotal, ResourceName = "Labels", VariableName = "TOTAL", IncludeInSearch = true)]
        public int? Total { get; set; }

        [GridMapping(Index = IndexCisterna, ResourceName = "Labels", VariableName = "CISTERNA", IncludeInSearch = true)]
        public int? Cisterna { get; set; }

        [GridMapping(Index = IndexFechaAlta, ResourceName = "Labels", VariableName = "FECHA_ALTA")]
        public DateTime? FechaAlta { get; set; }

        [GridMapping(Index = IndexFechaPedido, ResourceName = "Labels", VariableName = "FECHA_PEDIDO")]
        public DateTime? FechaPedido { get; set; }

        [GridMapping(Index = IndexFechaEntrega, ResourceName = "Labels", VariableName = "FECHA_ENTREGA")]
        public DateTime? FechaEntrega { get; set; }

        public ReporteAsignoVo(EntregaDistribucion entrega, OrderDetail detalleOrden)
        {
            var vehiculo = entrega.Viaje.Vehiculo;            
            var ptoEntrega = entrega.PuntoEntrega;
            var cliente = entrega.Cliente ?? entrega.PuntoEntrega.Cliente;
            
            Vehiculo = vehiculo.Interno + " - " + vehiculo.Patente;
            Viaje = entrega.Viaje.Codigo;
            Cliente = cliente.Codigo + " - " + cliente.Descripcion;
            PuntoEntrega = ptoEntrega.Codigo + " - " + ptoEntrega.Descripcion;
            Entrega = entrega.Descripcion;

            if (detalleOrden != null)
            {
                var orden = detalleOrden.Order;

                Orden = orden.CodigoPedido;
                Insumo = detalleOrden.Insumo.Descripcion;
                Cantidad = detalleOrden.Cantidad;
                Ajuste = detalleOrden.Ajuste;
                Total = detalleOrden.Total;
                Cisterna = detalleOrden.Cuaderna;
                FechaAlta = orden.FechaAlta.ToDisplayDateTime();
                FechaPedido = orden.FechaPedido.ToDisplayDateTime();
                if (orden.FechaEntrega.HasValue) FechaEntrega = orden.FechaEntrega.Value.ToDisplayDateTime();
            }
            else
            {
                Orden = "-";
            }
        }
    }
}
