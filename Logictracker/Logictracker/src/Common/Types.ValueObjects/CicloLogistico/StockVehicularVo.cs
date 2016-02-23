using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class StockVehicularVo
    {
        public const int IndexZona = 0;
        public const int IndexTipoVehiculo = 1;
        public const int IndexVehiculo = 2;
        public const int IndexPatente = 3;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexZona, ResourceName = "Entities", VariableName = "PARENTI89", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string Zona { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Labels", VariableName = "INTERNO", AllowGroup = false, IncludeInSearch = true, InitialSortExpression = true, SortDirection = GridSortDirection.Ascending)]
        public string Interno { get; set; }

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", AllowGroup = false, IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Count)]
        public string Patente { get; set; }

        public StockVehicularVo(DetalleStockVehicular detalle)
        {
            Id = detalle.StockVehicular.Id;
            Zona = detalle.StockVehicular.Zona.Descripcion;
            TipoVehiculo = detalle.StockVehicular.TipoCoche.Descripcion;
            Interno = detalle.Vehiculo.Interno;
            Patente = detalle.Vehiculo.Patente;
        }
    }
}
