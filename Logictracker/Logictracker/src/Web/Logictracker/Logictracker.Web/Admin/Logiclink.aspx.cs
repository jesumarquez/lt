using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Security;
using Logictracker.Culture;
using System;

namespace Logictracker.Admin
{
    public partial class Logiclink : SecuredListPage<LogicLinkFileVo>
    {
        protected override string RedirectUrl { get { return "LogiclinkAlta.aspx"; } }
        protected override string VariableName { get { return "LOGICLINK_ADMIN"; } }
        protected override string GetRefference() { return "LOGICLINK_ADMIN"; }
        protected override bool AddButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                dtpDesde.SetDate();
                dtpHasta.SetDate();
            }

            base.OnLoad(e);
        }

        protected override List<LogicLinkFileVo> GetListData()
        {
            return DAOFactory.LogicLinkFileDAO.GetList(ddlEmpresa.SelectedValues,
                                                       ddlBase.SelectedValues,
                                                       dtpDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                       dtpHasta.SelectedDate.Value.ToDataBaseDateTime())
                                              .Where(file => ddlEstadoArchivo.Selected == -1 || ddlEstadoArchivo.Selected == file.Status)
                                              .Select(file => new LogicLinkFileVo(file))
                                              .ToList();
        }

        protected override void OnLoadFilters(FilterData data)
        {
            data.LoadStaticFilter(FilterData.StaticDistrito, ddlEmpresa);
            data.LoadStaticFilter(FilterData.StaticBase, ddlBase);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticDistrito, ddlEmpresa.Selected);
            data.AddStatic(FilterData.StaticBase, ddlBase.Selected);
            return data;
        }
    }
}
