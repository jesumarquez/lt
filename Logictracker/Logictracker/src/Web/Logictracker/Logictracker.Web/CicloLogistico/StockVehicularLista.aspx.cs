using C1.Web.UI.Controls.C1GridView;
using System;
using System.Linq;
using System.Collections.Generic;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.ValueObjects.Mantenimiento;

namespace Logictracker.CicloLogistico
{
    public partial class StockVehicularLista : SecuredListPage<StockVehicularVo>
    {
        protected override string VariableName { get { return "PAR_STOCK_VEHICULAR"; } }
        protected override string GetRefference() { return "PAR_STOCK_VEHICULAR"; }
        protected override string RedirectUrl { get { return "StockVehicularAlta.aspx"; } }
        protected override bool AddButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }


        protected override List<StockVehicularVo> GetListData()
        {
            var stock = DAOFactory.StockVehicularDAO.GetList(new[] {cbEmpresa.Selected},
                                                             new[] {cbZona.Selected},
                                                             new[] {cbTipoVehiculo.Selected});

            var results = new List<StockVehicularVo>();

            foreach (var stk in stock)
            {
                results.AddRange(stk.Detalles.Select(d => new StockVehicularVo(d)));
            }

            return results;
        }
        
        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticZona, cbZona);
            data.LoadStaticFilter(FilterData.StaticTipoVehiculo, cbTipoVehiculo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticZona, cbZona.Selected);
            data.AddStatic(FilterData.StaticTipoVehiculo, cbTipoVehiculo.Selected);
            return data;
        }

        #endregion
    }
}
