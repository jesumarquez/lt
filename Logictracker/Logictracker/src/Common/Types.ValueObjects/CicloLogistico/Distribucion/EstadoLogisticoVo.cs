using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class EstadoLogisticoVo
    {
        public const int IndexDescripcion = 0;
        public const int IndexMensajeInicio = 1;
        public const int IndexMensajeFin = 2;
        public const int IndexDemora = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexMensajeInicio, ResourceName = "Labels", VariableName = "INICIO", AllowGroup = false, IncludeInSearch = true)]
        public string MensajeInicio { get; set; }
        
        [GridMapping(Index = IndexMensajeFin, ResourceName = "Labels", VariableName = "FIN", AllowGroup = false, IncludeInSearch = true)]
        public string MensajeFin { get; set; }

        [GridMapping(Index = IndexDemora, ResourceName = "Labels", VariableName = "DEMORA", AllowGroup = false, IncludeInSearch = true)]
        public int Demora { get; set; }

        public EstadoLogisticoVo(EstadoLogistico estado)
        {
            Id = estado.Id;
            Descripcion = estado.Descripcion;
            MensajeInicio = estado.MensajeInicio != null ? estado.MensajeInicio.Descripcion : string.Empty;
            MensajeFin = estado.MensajeFin != null ? estado.MensajeFin.Descripcion : string.Empty;
            Demora = estado.Demora;
        }
    }
}
