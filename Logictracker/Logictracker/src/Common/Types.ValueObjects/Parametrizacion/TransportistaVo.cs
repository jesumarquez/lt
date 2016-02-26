using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class TransportistaVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexTarifaTramoCorto = 2;
        public const int IndexTarifaTramoLargo = 3;
        public const int IndexCostoBulto = 4;
        public const int IndexCostoHora = 5;
        public const int IndexCostoKm = 6;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Entities", VariableName = "PARENTI07", InitialSortExpression = true, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexTarifaTramoCorto, ResourceName = "Labels", VariableName = "TARIFA_TRAMO_CORTO", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double TarifaTramoCorto { get; set;}

        [GridMapping(Index = IndexTarifaTramoLargo, ResourceName = "Labels", VariableName = "TARIFA_TRAMO_LARGO", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double TarifaTramoLargo { get; set; }

        [GridMapping(Index = IndexCostoBulto, ResourceName = "Labels", VariableName = "COSTO_BULTO", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double CostoBulto { get; set; }

        [GridMapping(Index = IndexCostoHora, ResourceName = "Labels", VariableName = "COSTO_HORA", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double CostoHora { get; set; }

        [GridMapping(Index = IndexCostoKm, ResourceName = "Labels", VariableName = "COSTO_KM", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double CostoKm { get; set; }
        
        public TransportistaVo(Transportista transportista)
        {
            Id = transportista.Id;
            Codigo = transportista.Codigo;
            Descripcion = transportista.Descripcion;
            TarifaTramoCorto = transportista.TarifaTramoCorto;
            TarifaTramoLargo = transportista.TarifaTramoLargo;
            CostoBulto = transportista.CostoPorBulto;
            CostoHora = transportista.CostoPorHora;
            CostoKm = transportista.CostoPorKm;
        }
    }
}
