using System;
using Logictracker.Types.BusinessObjects.Postal;

namespace Logictracker.Types.ValueObjects.Parametrizacion.Postal
{
    [Serializable]
    public class TipoServicioVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public TipoServicioVo(TipoServicio tipoServicio)
        {
            Id = tipoServicio.Id;
            Descripcion = tipoServicio.Descripcion;
            Codigo = tipoServicio.Codigo;
        }
    }
}
