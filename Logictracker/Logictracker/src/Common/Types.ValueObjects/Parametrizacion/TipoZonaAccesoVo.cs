using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class TipoZonaAccesoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, InitialSortExpression = true)]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}
        
        
        public TipoZonaAccesoVo(TipoZonaAcceso tipoZonaAcceso)
        {
            Id = tipoZonaAcceso.Id;
            Codigo = tipoZonaAcceso.Codigo;
            Descripcion = tipoZonaAcceso.Descripcion;
        }
    }
}
