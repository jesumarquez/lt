using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class DepartamentoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexEmpresa = 2;
        public const int IndexLinea = 3;
        public const int IndexResponsable = 4;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", IncludeInSearch = false)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02", IncludeInSearch = false)]
        public string Linea { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE_DEPTO", IncludeInSearch = true)]
        public string Responsable { get; set; }

        public DepartamentoVo(Departamento departamento)
        {
            Id = departamento.Id;
            Codigo = departamento.Codigo;
            Descripcion = departamento.Descripcion;
            Empresa = departamento.Empresa != null ? departamento.Empresa.RazonSocial : string.Empty;
            Linea = departamento.Linea != null ? departamento.Linea.Descripcion : string.Empty;
            Responsable = departamento.Empleado != null ? departamento.Empleado.Entidad.Descripcion : string.Empty;
        }
    }
}
