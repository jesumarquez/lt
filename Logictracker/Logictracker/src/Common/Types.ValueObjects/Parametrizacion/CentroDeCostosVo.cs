using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class CentroDeCostosVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexDepartamento = 2;
        public const int IndexResponsable = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true, Width = "20%")]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexDepartamento, ResourceName = "Entities", VariableName = "PARENTI04", AllowGroup = true)]
        public string Departamento { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", AllowGroup = true, IncludeInSearch = true)]
        public string Responsable { get; set; }

        public CentroDeCostosVo(CentroDeCostos centroDeCostos)
        {
            Id = centroDeCostos.Id;
            Codigo = centroDeCostos.Codigo;
            Descripcion = centroDeCostos.Descripcion;
            Departamento = centroDeCostos.Departamento != null ? centroDeCostos.Departamento.Descripcion : string.Empty;
            Responsable = centroDeCostos.Empleado != null ? centroDeCostos.Empleado.Entidad.Descripcion : string.Empty;
        }
    }
}
