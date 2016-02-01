using System;
using System.Linq;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class ViajeProgramadoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexTransportista = 1;
        public const int IndexTipoCoche = 2;        
        public const int IndexHoras = 3;
        public const int IndexKm = 4;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE_DISTRIBUCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexTipoCoche, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true)]
        public string TipoCoche { get; set; }

        [GridMapping(Index = IndexHoras, ResourceName = "Labels", VariableName = "HORAS", AllowGroup = false)]
        public double Horas { get; set; }

        [GridMapping(Index = IndexKm, ResourceName = "Labels", VariableName = "KM", AllowGroup = false)]
        public double Km { get; set; }

        public ViajeProgramadoVo(ViajeProgramado viaje)
        {
            Id = viaje.Id;
            Codigo = viaje.Codigo;
            Transportista = viaje.Transportista != null ? viaje.Transportista.Descripcion : string.Empty;
            TipoCoche = viaje.TipoCoche != null ? viaje.TipoCoche.Descripcion : string.Empty;
            Horas = viaje.Horas;
            Km = viaje.Km;
        }
    }
}
