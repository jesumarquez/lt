using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class PreasignacionViajeVehiculoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexVehiculo = 1;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE_DISTRIBUCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        public PreasignacionViajeVehiculoVo(PreasignacionViajeVehiculo viajeVehiculo)
        {
            Id = viajeVehiculo.Id;
            Codigo = viajeVehiculo.Codigo;
            Vehiculo = viajeVehiculo.Vehiculo.Interno;
            if (viajeVehiculo.Vehiculo.Patente != viajeVehiculo.Vehiculo.Interno)
                Vehiculo += " - " + viajeVehiculo.Vehiculo.Patente;
        }
    }
}
