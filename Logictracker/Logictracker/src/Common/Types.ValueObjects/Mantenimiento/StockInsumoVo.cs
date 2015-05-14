using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class StockInsumoVo
    {
        public const int IndexDeposito = 0;
        public const int IndexInsumo = 1;
        public const int IndexCapacidadMaxima = 2;
        public const int IndexPuntoReposicion = 3;
        public const int IndexStockCritico = 4;
        public const int IndexStockActual = 5;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexDeposito, ResourceName = "Entities", VariableName = "PARENTI87", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string Deposito { get; set; }

        [GridMapping(Index = IndexInsumo, ResourceName = "Entities", VariableName = "PARENTI58", AllowGroup = true, InitialSortExpression = true)]
        public string Insumo { get; set; }

        [GridMapping(Index = IndexCapacidadMaxima, ResourceName = "Labels", VariableName = "CAPACIDAD_MAXIMA", DataFormatString = "{0:#0.00}", AllowGroup = false)]
        public double CapacidadMaxima { get; set; }

        [GridMapping(Index = IndexPuntoReposicion, ResourceName = "Labels", VariableName = "PUNTO_REPOSICION", DataFormatString = "{0:#0.00}")]
        public double PuntoReposicion { get; set; }

        [GridMapping(Index = IndexStockCritico, ResourceName = "Labels", VariableName = "NIVEL_CRITICO", DataFormatString = "{0:#0.00}")]
        public double StockCritico { get; set; }

        [GridMapping(Index = IndexStockActual, ResourceName = "Labels", VariableName = "STOCK_ACTUAL", DataFormatString = "{0:#0.00}")]
        public double StockActual { get; set; }

        public StockInsumoVo(Stock stock)
        {
            Id = stock.Id;
            Deposito = stock.Deposito.ToString();
            Insumo = stock.Insumo.ToString();
            CapacidadMaxima = stock.CapacidadMaxima;
            PuntoReposicion = stock.PuntoReposicion;
            StockCritico = stock.StockCritico;
            StockActual = stock.Cantidad;
        }
    }
}
