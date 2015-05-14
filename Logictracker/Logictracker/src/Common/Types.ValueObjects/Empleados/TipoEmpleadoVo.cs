using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Empleados
{
    [Serializable]
    public class TipoEmpleadoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public TipoEmpleadoVo(TipoEmpleado tipoEmpleado)
        {
            Id = tipoEmpleado.Id;
            Codigo = tipoEmpleado.Codigo;
            Descripcion = tipoEmpleado.Descripcion;
        }
    }
}
