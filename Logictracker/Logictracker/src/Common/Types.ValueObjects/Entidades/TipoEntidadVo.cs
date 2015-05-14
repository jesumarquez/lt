using System;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class TipoEntidadVo
    {   
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public TipoEntidadVo(TipoEntidad tipoEntidad)
        {
            Id = tipoEntidad.Id;
            Codigo = tipoEntidad.Codigo;
            Descripcion = tipoEntidad.Descripcion;
        }
    }
}
