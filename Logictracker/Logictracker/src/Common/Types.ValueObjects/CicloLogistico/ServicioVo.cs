using System;
using Logictracker.Types.BusinessObjects.Tickets;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class ServicioVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public ServicioVo(Servicio servicio)
        {
            Id = servicio.Id;
            Codigo = servicio.Codigo;
            Descripcion = servicio.Descripcion;
        }
    }
}
