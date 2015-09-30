using System;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class EstadoLogisticoVo
    {
        public const int IndexDescripcion = 0;
        public const int IndexMensaje = 1;
        public const int IndexDemora = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexMensaje, ResourceName = "Entities", VariableName = "PAREVEN01", AllowGroup = false, IncludeInSearch = true)]
        public string Mensaje { get; set; }

        [GridMapping(Index = IndexDemora, ResourceName = "Labels", VariableName = "DEMORA", AllowGroup = false, IncludeInSearch = true)]
        public int Demora { get; set; }

        public EstadoLogisticoVo(EstadoLogistico estado)
        {
            Id = estado.Id;
            Descripcion = estado.Descripcion;
            Mensaje = estado.Mensaje != null ? estado.Mensaje.Descripcion : string.Empty;
            Demora = estado.Demora;
        }
    }
}
