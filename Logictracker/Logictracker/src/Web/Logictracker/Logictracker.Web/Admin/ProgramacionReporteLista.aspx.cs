using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Admin
{
    public partial class ProgramacionReporteLista : SecuredListPage<ProgramacionReporteVo>
    {
        protected override string RedirectUrl { get { return "ProgramacionReporteAlta.aspx"; } }
        protected override string VariableName { get { return "ADMIN_PROGRAMACION_REPORTE"; } }
        protected override string GetRefference() { return "ADMIN_PROGRAMACION_REPORTE"; }
        protected override bool AddButton { get { return false; } }
        protected override bool ExcelButton { get { return true; } }

        protected override List<ProgramacionReporteVo> GetListData()
        {
            var list = DAOFactory.ProgramacionReporteDAO.GetList(cbEmpresa.SelectedValues, cbLinea.SelectedValues);

            return list.Select(pr => new ProgramacionReporteVo(pr)).ToList();
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
