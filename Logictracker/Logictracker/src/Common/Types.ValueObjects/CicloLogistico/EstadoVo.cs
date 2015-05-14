using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class EstadoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true, Width = "20%")]
        public int Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public EstadoVo(Estado estado)
        {
            Id = estado.Id;
            Codigo = estado.Codigo;     
            Descripcion = estado.Descripcion;
        }
    }
}
