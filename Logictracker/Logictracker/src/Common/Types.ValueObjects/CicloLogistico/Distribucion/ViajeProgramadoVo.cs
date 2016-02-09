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
        public const int IndexClientes = 2;
        public const int IndexHoras = 3;
        public const int IndexKm = 4;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE_DISTRIBUCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexClientes, ResourceName = "Menu", VariableName = "PTOS_ENTREGA", AllowGroup = false)]
        public int Clientes { get; set; }

        [GridMapping(Index = IndexHoras, ResourceName = "Labels", VariableName = "HORAS", AllowGroup = false)]
        public string Horas { get; set; }

        [GridMapping(Index = IndexKm, ResourceName = "Labels", VariableName = "KM", AllowGroup = false)]
        public double Km { get; set; }

        public ViajeProgramadoVo(ViajeProgramado viaje)
        {
            Id = viaje.Id;
            Codigo = viaje.Codigo;
            Transportista = viaje.Transportista != null ? viaje.Transportista.Descripcion : string.Empty;
            Clientes = viaje.Detalles.Count;
            Km = viaje.Km;
            var ts = new TimeSpan(0, 0, (int)(viaje.Horas * 3600));
            Horas = ((int)Math.Truncate(ts.TotalHours)).ToString("00") + ":" + ((int)(ts.Minutes)).ToString("00") + ":" + ((int)(ts.Seconds)).ToString("00");
        }
    }
}
