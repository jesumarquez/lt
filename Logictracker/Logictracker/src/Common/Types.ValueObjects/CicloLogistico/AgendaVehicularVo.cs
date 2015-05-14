using System;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class AgendaVehicularVo
    {
        public const int IndexVehiculo = 0;
        public const int IndexEmpleado = 1;
        public const int IndexDesde = 2;
        public const int IndexHasta = 3;
        public const int IndexEstado = 4;

        public int Id { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexEmpleado, ResourceName = "Entities", VariableName = "PARENTI09", AllowGroup = true, IncludeInSearch = true)]
        public string Empleado { get; set; }

        [GridMapping(Index = IndexDesde, ResourceName = "Labels", VariableName = "DESDE")]
        public DateTime Desde { get; set; }

        [GridMapping(Index = IndexHasta, ResourceName = "Labels", VariableName = "HASTA")]
        public DateTime Hasta { get; set; }

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = false)]
        public string Estado { get; set; }

        public AgendaVehicularVo(AgendaVehicular agenda)
        {
            Id = agenda.Id;
            Vehiculo = agenda.Vehiculo.Interno;
            Empleado = agenda.Empleado.Entidad.Descripcion;
            Desde = agenda.FechaDesde.ToDisplayDateTime();
            Hasta = agenda.FechaHasta.ToDisplayDateTime();
            Estado = CultureManager.GetLabel(AgendaVehicular.Estados.GetLabelVariableName((short)agenda.Estado));
        }
    }
}
