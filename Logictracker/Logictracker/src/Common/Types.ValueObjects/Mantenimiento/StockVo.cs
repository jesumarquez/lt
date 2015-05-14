using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class StockVo
    {
        public const int IndexDeposito = 0;
        public const int IndexInsumo = 1;
        public const int IndexFecha = 2;
        public const int IndexStockInicial = 3;
        public const int IndexConcepto = 4;
        public const int IndexCantidad = 5;
        public const int IndexStockFinal = 6;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexDeposito, ResourceName = "Entities", VariableName = "PARENTI87", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string Deposito { get; set; }

        [GridMapping(Index = IndexInsumo, ResourceName = "Entities", VariableName = "PARENTI58", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string Insumo { get; set; }

        [GridMapping(Index = IndexInsumo, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Ascending)]
        public string Fecha { get; set; }

        [GridMapping(Index = IndexStockInicial, ResourceName = "Labels", VariableName = "STOCK_INICIAL")]
        public string StockInicial { get; set; }

        [GridMapping(Index = IndexConcepto, ResourceName = "Labels", VariableName = "CONCEPTO")]
        public string Concepto { get; set; }

        [GridMapping(Index = IndexCantidad, ResourceName = "Labels", VariableName = "CANTIDAD")]
        public string Cantidad { get; set; }

        [GridMapping(Index = IndexStockFinal, ResourceName = "Labels", VariableName = "STOCK_FINAL")]
        public string StockFinal { get; set; }

        public double Cant;

        public StockVo(ConsumoDetalle consumoDetalle, Stock stock)
        {
            Id = stock.Id;
            Deposito = stock.Deposito.ToString();
            Insumo = stock.Insumo.ToString();
            Fecha = consumoDetalle.ConsumoCabecera.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            StockInicial = "0.00";
            Cant = consumoDetalle.Cantidad;
            Cantidad = consumoDetalle.Cantidad.ToString();
            StockFinal = "0.00";

            if (consumoDetalle.ConsumoCabecera.Deposito != null 
             && consumoDetalle.ConsumoCabecera.Deposito.Id == stock.Deposito.Id)
            {
                if (consumoDetalle.ConsumoCabecera.Vehiculo != null)
                    Concepto = consumoDetalle.ConsumoCabecera.Vehiculo.Interno;
                if (consumoDetalle.ConsumoCabecera.DepositoDestino != null)
                    Concepto = consumoDetalle.ConsumoCabecera.DepositoDestino.Descripcion;
            }
            else
            {
                if (consumoDetalle.ConsumoCabecera.DepositoDestino != null
                && consumoDetalle.ConsumoCabecera.DepositoDestino.Id == stock.Deposito.Id)
                {
                    if (consumoDetalle.ConsumoCabecera.Proveedor != null)
                        Concepto = consumoDetalle.ConsumoCabecera.Proveedor.Descripcion;
                    if (consumoDetalle.ConsumoCabecera.Deposito != null)
                        Concepto = consumoDetalle.ConsumoCabecera.Deposito.Descripcion;
                }   
            }

        }
    }
}
