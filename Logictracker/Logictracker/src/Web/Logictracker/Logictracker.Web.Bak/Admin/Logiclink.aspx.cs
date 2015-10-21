using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.Admin
{
    public partial class Logiclink : SecuredGridReportPage<LogicLinkFileVo>
    {
        protected override string VariableName { get { return "LOGICLINK_ADMIN"; } }
        protected override string GetRefference() { return "LOGICLINK_ADMIN"; }
        protected override bool PrintButton { get { return false; } }
        protected override bool ExcelButton { get { return true; } }
        public override string SearchString { get { return txtBuscar.Text; } }

        protected override List<LogicLinkFileVo> GetResults()
        {
            return DAOFactory.LogicLinkFileDAO.GetList(ddlEmpresa.SelectedValues, 
                                                       ddlBase.SelectedValues, 
                                                       dtpDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                       dtpHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                        .Where(file => ddlEstadoArchivo.Selected==-1 || ddlEstadoArchivo.Selected == file.Status)
                                              .Select(file => new LogicLinkFileVo(file))
                                              .ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(IsPostBack) return;

            dtpDesde.SetDate();
            dtpHasta.SetDate();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlEmpresa.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dtpDesde.SelectedDate.ToString()}, {CultureManager.GetLabel("HASTA"), dtpHasta.SelectedDate.ToString() }
                       };
        }
    }
}