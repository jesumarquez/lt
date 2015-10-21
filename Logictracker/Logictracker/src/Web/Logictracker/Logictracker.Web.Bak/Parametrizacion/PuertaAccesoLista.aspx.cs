using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PuertaAccesoLista : SecuredListPage<PuertaAccesoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_PUERTAS"; } }
        protected override string RedirectUrl { get { return "PuertaAccesoAlta.aspx"; } }
        protected override string GetRefference() { return "PUERTA"; }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override List<PuertaAccesoVo> GetListData()
        {
            var res = DAOFactory.PuertaAccesoDAO.GetList(new[]{cbEmpresa.Selected}, new[]{cbLinea.Selected}).Select(p => new PuertaAccesoVo(p)).ToList();

            return res;
        }
        #endregion

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
    }
}