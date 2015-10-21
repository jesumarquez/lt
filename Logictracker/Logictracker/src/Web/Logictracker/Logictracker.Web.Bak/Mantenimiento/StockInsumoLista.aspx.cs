using C1.Web.UI.Controls.C1GridView;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class StockInsumoLista : SecuredListPage<StockInsumoVo>
    {
        protected override string RedirectUrl { get { return "StockInsumoAlta.aspx"; } }
        protected override string VariableName { get { return "MAN_STOCK_INSUMO"; } }
        protected override string GetRefference() { return "MAN_STOCK_INSUMO"; }
        protected override bool AddButton { get { return false; } }
        protected override bool ExcelButton { get { return true; } }

        protected override List<StockInsumoVo> GetListData()
        {
            var list = DAOFactory.StockDAO.GetList(new[] { cbEmpresa.Selected }, 
                                                   new[] { cbLinea.Selected },
                                                   cbDeposito.SelectedValues,
                                                   cbInsumo.SelectedValues);

            return list.Select(st => new StockInsumoVo(st)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, StockInsumoVo stock)
        {
            if (stock.StockActual < stock.StockCritico)
            {
                e.Row.BackColor = Color.Red;
                e.Row.ForeColor = Color.White;
                e.Row.Font.Bold = true;
            }
            else if (stock.StockActual < stock.PuntoReposicion)
            {
                e.Row.BackColor = Color.Yellow;
                e.Row.Font.Bold = true;
            }
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            return data;
        }

        #endregion
    }
}
