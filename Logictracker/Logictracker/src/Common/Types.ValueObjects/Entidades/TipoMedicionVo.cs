using System;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class TipoMedicionVo
    {   
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexUnidadMedida = 2;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexUnidadMedida, ResourceName = "Entities", VariableName = "PARENTI85", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string UnidadMedida { get; set; }

        public TipoMedicionVo(TipoMedicion tipoMedicion)
        {
            Id = tipoMedicion.Id;
            Codigo = tipoMedicion.Codigo;
            Descripcion = tipoMedicion.Descripcion;
            UnidadMedida = tipoMedicion.UnidadMedida != null ? tipoMedicion.UnidadMedida.Descripcion : string.Empty;
        }
    }
}
