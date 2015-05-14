using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Tickets;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Hormigon
{
    [Serializable]
    public class ConsolaTicketsVo
    {
        public static class Index
        {
            public const int Linea = 0;
            public const int BocaDeCarga = 1;
            public const int Codigo = 2;
            public const int Cliente = 3;
            public const int PuntoEntrega = 4;
            public const int Producto = 5;
            public const int FechaEnObra = 6;
            public const int MultiplesRemitos = 7;
            public const int Tickets = 8;
            public const int M3 = 9;
            public const int Ajuste = 10;
        }

        public int Id { get; set; }

        [GridMapping(Index = Index.Linea, ResourceName = "Entities", VariableName = "PARENTI02", AllowGroup = true)]
        public string Linea { get; set; }

        [GridMapping(Index = Index.BocaDeCarga, ResourceName = "Entities", VariableName = "BOCADECARGA", AllowGroup = true, IncludeInSearch = true)]
        public string BocaDeCarga { get; set; }

        [GridMapping(Index = Index.Codigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false)]
        public string Codigo { get; set; }

        [GridMapping(Index = Index.Cliente, ResourceName = "Entities", VariableName = "CLIENT", AllowGroup = true, IncludeInSearch = true)]
        public string Cliente { get; set; }

        [GridMapping(Index = Index.PuntoEntrega, ResourceName = "Entities", VariableName = "PARENTI44", AllowGroup = true, IncludeInSearch = true)]
        public string PuntoEntrega { get; set; }

        [GridMapping(Index = Index.Producto, ResourceName = "Entities", VariableName = "PARENTI63", AllowGroup = true, IncludeInSearch = true)]
        public string Producto { get; set; }

        [GridMapping(Index = Index.FechaEnObra, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false)]
        public DateTime FechaEnObra { get; set; }

        [GridMapping(Index = Index.MultiplesRemitos, ResourceName = "Labels", VariableName = "MULTIPLES_REMITOS")]
        public string MultiplesRemitos { get; set; }

        [GridMapping(Index = Index.Tickets, ResourceName = "Labels", VariableName = "TICKETS", AllowGroup = false)]
        public string Tickets { get; set; }

        [GridMapping(Index = Index.M3, ResourceName = "Labels", VariableName = "CANTIDAD_CARGA", AllowGroup = false)]
        public string Cantidad { get; set; }

        [GridMapping(Index = Index.Ajuste, ResourceName = "Labels", VariableName = "Ajuste", AllowGroup = false)]
        public string Ajuste { get; set; }


        public ConsolaTicketsVo(Pedido pedido)
        {
            Id = pedido.Id;
            Linea = pedido.Linea.Descripcion;
            BocaDeCarga = pedido.BocaDeCarga.Descripcion;
            Codigo = pedido.Codigo;
            Cliente = pedido.Cliente.Descripcion;
            PuntoEntrega = pedido.PuntoEntrega.Descripcion;
            Producto = pedido.Producto != null ? pedido.Producto.Descripcion : "";
            FechaEnObra = pedido.FechaEnObra.ToDisplayDateTime();
            MultiplesRemitos = pedido.EsMinimixer ? "SI" : "NO";
            Tickets = string.Empty;
            Cantidad = pedido.Cantidad.ToString("#0.0");
            Ajuste = string.Empty;
        }
    }
}
