using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class StockLista : SecuredListPage<StockVo>
    {
        protected override string VariableName { get { return "MAN_STOCK"; } }
        protected override string GetRefference() { return "MAN_STOCK"; }
        protected override string RedirectUrl { get { return string.Empty; } }
        protected override bool AddButton { get { return false; } }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            dtFecha.SetDate();
        }

        protected override List<StockVo> GetListData()
        {
            var stock = DAOFactory.StockDAO.GetList(new[] {cbEmpresa.Selected},
                                                    new[] {cbLinea.Selected},
                                                    new[] {cbDeposito.Selected},
                                                    new[] {cbInsumo.Selected});

            var results = new List<StockVo>();

            foreach (var stk in stock)
            {
                var consumos = DAOFactory.ConsumoDetalleDAO.GetStock(new[] { cbEmpresa.Selected },
                                                                     new[] { cbLinea.Selected },
                                                                     new[] { stk.Deposito.Id },
                                                                     new[] { stk.Insumo.Id },
                                                                     dtFecha.SelectedDate.HasValue 
                                                                        ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value) 
                                                                        : DateTime.UtcNow);

                var stockFinal = stk.Cantidad;
                foreach (var consumoDetalle in consumos)
                {
                    var stockVo = new StockVo(consumoDetalle, stk) {StockFinal = stockFinal.ToString("##0.00")};
                    var multiplicador = consumoDetalle.ConsumoCabecera.Deposito != null 
                                        && consumoDetalle.ConsumoCabecera.Deposito.Id == stk.Deposito.Id
                                            ? -1
                                            : consumoDetalle.ConsumoCabecera.DepositoDestino != null 
                                            && consumoDetalle.ConsumoCabecera.DepositoDestino.Id == stk.Deposito.Id
                                                  ? 1
                                                  : 0;
                    var cantidad = stockVo.Cant*multiplicador;
                    stockVo.Cantidad = cantidad.ToString("##0.00");
                    stockFinal = stockFinal - cantidad;
                    stockVo.StockInicial = stockFinal.ToString("##0.00");
                    
                    results.Add(stockVo);
                }
            }

            return results;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, StockVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticDeposito, cbDeposito);
            data.LoadStaticFilter(FilterData.StaticInsumo, cbInsumo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticDeposito, cbDeposito.Selected);
            data.AddStatic(FilterData.StaticInsumo, cbInsumo.Selected);
            return data;
        }

        #endregion
    }
}
