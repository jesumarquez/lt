using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Security;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.Admin
{
    public partial class DatamartLog : SecuredGridReportPage<DatamartLogVo>
    {
        protected override string VariableName { get { return "DATAMART_LOG"; } }
        protected override string GetRefference() { return "DATAMART_LOG"; }
        protected override bool PrintButton { get { return false; } }
        protected override bool ExcelButton { get { return true; } }
        public override string SearchString { get { return txtBuscar.Text; } }

        protected override List<DatamartLogVo> GetResults()
        {
            return DAOFactory.DataMartsLogDAO.GetList(dtpDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                      dtpHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                          .Where(file => ddlDatamart.Selected==-1 || ddlDatamart.Selected == file.Modulo)
                                              .Select(file => new DatamartLogVo(file))
                                              .ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dtpDesde.SetDate();
            dtpHasta.SetDate();
        }
    }
}