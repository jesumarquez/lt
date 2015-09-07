using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TransportistaLista : SecuredListPage<TransportistaVo>
    {
        protected override string RedirectUrl { get { return "TransportistaAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_TRANSPORTISTAS"; } }
        protected override string GetRefference() { return "TRANSP"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<TransportistaVo> GetListData()
        {
            return DAOFactory.TransportistaDAO.GetList(new []{ddlDistrito.Selected},new []{ddlBase.Selected})
                .Select(t => new TransportistaVo(t))
                .ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            var empresa = data[FilterData.StaticDistrito];
            var linea = data[FilterData.StaticBase];
            if (empresa != null) ddlDistrito.SetSelectedValue((int)empresa);
            if (linea != null) ddlBase.SetSelectedValue((int)linea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlDistrito.Selected);
            data.AddStatic(FilterData.StaticBase, ddlBase.Selected);
            return data;
        }
    }
}
