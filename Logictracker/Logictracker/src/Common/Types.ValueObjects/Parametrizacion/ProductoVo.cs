using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class ProductoVo
    {   
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexObservaciones = 2;
        public const int IndexBocaDeCarga = 3;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexObservaciones, ResourceName = "Labels", VariableName = "OBSERVACIONES", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Observaciones { get; set; }

        [GridMapping(Index = IndexBocaDeCarga, ResourceName = "Entities", VariableName = "PARTICK04", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string BocaDeCarga { get; set; }

        public ProductoVo(Producto producto)
        {
            Id = producto.Id;
            Codigo = producto.Codigo;
            Descripcion = producto.Descripcion;
            Observaciones = producto.Observaciones;
            BocaDeCarga = producto.BocaDeCarga != null ? producto.BocaDeCarga.Descripcion : string.Empty;
        }
    }
}
