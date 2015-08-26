using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class ListaBocaDeCarga : SecuredListPage<BocaDeCargaVo>
    {
        protected override string RedirectUrl { get { return "BocaDeCargaAlta.aspx"; } }
        protected override string VariableName { get { return "CLOG_BOCADECARGA"; } }
        protected override string GetRefference() { return "BOCADECARGA"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<BocaDeCargaVo> GetListData()
        {
            return DAOFactory.BocaDeCargaDAO.GetList(new[]{cbEmpresa.Selected}, new[]{ cbLinea.Selected})
                .Select(boca => new BocaDeCargaVo(boca))
                .ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) cbEmpresa.SetSelectedValue((int)empresa);
            if (linea != null) cbLinea.SetSelectedValue((int)linea);
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
