using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class ConsumoVo
    {
        public const int IndexFecha = 0;
        public const int IndexCentroDeCosto = 1;
        public const int IndexTipoDestino = 2;
        public const int IndexDestino = 3;
        public const int IndexNumeroFactura = 4;
        public const int IndexOrigen = 5;
        public const int IndexInsumo = 6;
        public const int IndexTipoInsumo = 7;
        public const int IndexCantidad = 8;
        public const int IndexImporteTotal = 9;

        public int Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexCentroDeCosto, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 0, IncludeInSearch = true)]
        public string CentroDeCosto { get; set; }

        [GridMapping(Index = IndexTipoDestino, ResourceName = "Labels", VariableName = "TIPO", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 1, IncludeInSearch = true)]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexDestino, ResourceName = "Labels", VariableName = "DESTINO", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 2, IncludeInSearch = true)]
        public string Destino { get; set; }

        [GridMapping(Index = IndexNumeroFactura, ResourceName = "Labels", VariableName = "NRO_FACTURA", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 3, IncludeInSearch = true)]
        public string NumeroFactura { get; set; }

        [GridMapping(Index = IndexOrigen, ResourceName = "Labels", VariableName = "ORIGEN", AllowGroup = true, IncludeInSearch = true)]
        public string Origen { get; set; }

        [GridMapping(Index = IndexInsumo, ResourceName = "Entities", VariableName = "PARENTI58", AllowGroup = true)]
        public string Insumo { get; set; }

        [GridMapping(Index = IndexTipoInsumo, ResourceName = "Entities", VariableName = "PARENTI60", AllowGroup = true)]
        public string TipoInsumo { get; set; }

        [GridMapping(Index = IndexCantidad, ResourceName = "Labels", VariableName = "CANTIDAD", DataFormatString = "{0:0.00}", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}")]
        public double Cantidad { get; set; }

        [GridMapping(Index = IndexImporteTotal, ResourceName = "Labels", VariableName = "IMPORTE_TOTAL", DataFormatString = "{0:0.00}", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}")]
        public double ImporteTotal { get; set; }

        public ConsumoVo(ConsumoDetalle consumo)
        {
            Id = consumo.ConsumoCabecera.Id;
            Fecha = consumo.ConsumoCabecera.Fecha.ToDisplayDateTime();
            CentroDeCosto = consumo.ConsumoCabecera.Vehiculo != null &&
                            consumo.ConsumoCabecera.Vehiculo.CentroDeCostos != null
                                ? consumo.ConsumoCabecera.Vehiculo.CentroDeCostos.Descripcion
                                : string.Empty;
            Tipo = consumo.ConsumoCabecera.Vehiculo != null
                       ? consumo.ConsumoCabecera.Vehiculo.TipoCoche.Descripcion
                       : consumo.ConsumoCabecera.DepositoDestino != null
                             ? "Depósito"
                             : string.Empty;
            Destino = consumo.ConsumoCabecera.Vehiculo != null
                          ? consumo.ConsumoCabecera.Vehiculo.Interno
                          : consumo.ConsumoCabecera.DepositoDestino != null
                                ? consumo.ConsumoCabecera.DepositoDestino.Descripcion
                                : string.Empty;
            NumeroFactura = consumo.ConsumoCabecera.NumeroFactura;
            Origen = consumo.ConsumoCabecera.Proveedor != null
                         ? consumo.ConsumoCabecera.Proveedor.Descripcion
                         : consumo.ConsumoCabecera.Deposito != null
                               ? consumo.ConsumoCabecera.Deposito.Descripcion
                               : string.Empty;
            TipoInsumo = consumo.Insumo.TipoInsumo != null
                             ? consumo.Insumo.TipoInsumo.Descripcion
                             : string.Empty;
            Insumo = consumo.Insumo.Descripcion;
            Cantidad = consumo.Cantidad;
            ImporteTotal = consumo.ImporteTotal;
        }
    }
}
