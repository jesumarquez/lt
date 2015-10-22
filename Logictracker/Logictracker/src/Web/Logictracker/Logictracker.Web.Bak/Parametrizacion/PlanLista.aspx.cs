using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.Parametrizacion;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PlanLista : SecuredListPage<PlanVo>
    {
        protected override string RedirectUrl { get { return "PlanAlta.aspx"; } }
        protected override string VariableName { get { return "PAR_PLANES"; } }
        protected override string GetRefference() { return "PAR_PLANES"; }
        protected override bool ExcelButton { get { return true; } }

        protected override List<PlanVo> GetListData()
        {
            var list = DAOFactory.PlanDAO.GetList(new[] { cbEmpresa.Selected }, new[] { cbTipoLinea.Selected });

            return list.Select(v => new PlanVo(v)).ToList();
        }

        #region Filter Memory

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticEmpresaTelefonica, cbEmpresa);
            data.LoadStaticFilter(FilterData.StaticTipoLineaTelefonica, cbTipoLinea);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticEmpresaTelefonica, cbEmpresa.Selected);
            data.AddStatic(FilterData.StaticTipoLineaTelefonica, cbTipoLinea.Selected);
            return data;
        }

        #endregion
    }
}
