using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class EquipoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true, Width = "20%")]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        public EquipoVo(Equipo equipo)
        {
            Id = equipo.Id;
            Codigo = equipo.Codigo;
            Descripcion = equipo.Descripcion;
        }
    }
}
