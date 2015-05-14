using System;
using Logictracker.Types.BusinessObjects.Documentos;

namespace Logictracker.Types.ValueObjects.Documentos
{
    [Serializable]
    public class TipoDocumentoVo
    {
        public const int IndexDescripcion = 0;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }
        
        public TipoDocumentoVo(TipoDocumento tipoDocumento)
        {
            Id = tipoDocumento.Id;
            Descripcion = tipoDocumento.Descripcion;
        }
    }
}
