using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class PuntoEntregaVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexClienteDelPunto = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexClienteDelPunto, ResourceName = "Labels", VariableName = "CLIENTE", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public string ClienteDelPunto { get; set; }

        public bool Nomenclado { get; set;}

        public PuntoEntregaVo(PuntoEntrega puntoEntrega)
        {
            Id = puntoEntrega.Id;
            Codigo = puntoEntrega.Codigo;
            Descripcion = puntoEntrega.Descripcion;
            Nomenclado = puntoEntrega.Nomenclado;
            ClienteDelPunto = puntoEntrega.Cliente.Descripcion;
        }
    }
}
