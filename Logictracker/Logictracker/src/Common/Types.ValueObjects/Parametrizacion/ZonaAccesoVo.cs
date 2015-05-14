using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class ZonaAccesoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexTipoZonaAcceso = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, InitialSortExpression = true)]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexTipoZonaAcceso, ResourceName = "Entities", VariableName = "PARENTI91", AllowGroup = true)]
        public string TipoZonaAcceso { get; set; }
        
        public ZonaAccesoVo(ZonaAcceso zonaAcceso)
        {
            Id = zonaAcceso.Id;
            Codigo = zonaAcceso.Codigo;
            Descripcion = zonaAcceso.Descripcion;
            TipoZonaAcceso = zonaAcceso.TipoZonaAcceso.Descripcion;
        }
    }
}
