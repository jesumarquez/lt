using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class LineaVo
    {
        public const int IndexDescripcion = 0;
        public const int IndexEmpresa = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", IncludeInSearch = true)]
        public string Empresa { get; set; }

        public LineaVo(Linea linea)
        {
            Id = linea.Id;
            Descripcion = linea.Descripcion;
            Empresa = linea.Empresa.RazonSocial;
        }
    }
}
