using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ProductoLista : SecuredListPage<ProductoVo>
    {
        protected override string RedirectUrl { get { return "ProductoAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_PRODUCTO"; } }
        protected override string GetRefference() { return "PAR_PRODUCTO"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ProductoVo> GetListData()
        {
            var list = DAOFactory.ProductoDAO.GetList(new[]{cbEmpresa.Selected}, new[]{cbLinea.Selected}, new[]{cbBocaDeCarga.Selected});

            return list.Select(v => new ProductoVo(v)).ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, cbLinea);
            data.LoadStaticFilter(FilterData.StaticBocaDeCarga, cbBocaDeCarga);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, cbLinea.Selected);
            data.AddStatic(FilterData.StaticBocaDeCarga, cbBocaDeCarga.Selected);
            return data;
        }

        #endregion
    }
}
