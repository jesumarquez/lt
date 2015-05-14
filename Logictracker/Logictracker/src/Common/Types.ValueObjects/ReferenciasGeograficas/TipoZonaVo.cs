using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

namespace Logictracker.Types.ValueObjects.ReferenciasGeograficas
{
    [Serializable]
    public class TipoZonaVo
    {
        public static class Index
        {
            public const int Codigo = 0;
            public const int Descripcion = 1;
        }

        public int Id { get; set; }

        [GridMapping(Index = Index.Codigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, InitialSortExpression = true)]
        public string Codigo { get; set;}

        [GridMapping(Index = Index.Descripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        public TipoZonaVo(TipoZona tipoZonaAcceso)
        {
            Id = tipoZonaAcceso.Id;
            Codigo = tipoZonaAcceso.Codigo;
            Descripcion = tipoZonaAcceso.Descripcion;
        }
    }
}
