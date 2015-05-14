using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class UnidadMedidaVo
    {   
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexSimbolo = 2;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexSimbolo, ResourceName = "Labels", VariableName = "SIMBOLO", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Simbolo { get; set; }

        public UnidadMedidaVo(UnidadMedida unidadMedida)
        {
            Id = unidadMedida.Id;
            Codigo = unidadMedida.Codigo;
            Descripcion = unidadMedida.Descripcion;
            Simbolo = unidadMedida.Simbolo;
        }
    }
}
