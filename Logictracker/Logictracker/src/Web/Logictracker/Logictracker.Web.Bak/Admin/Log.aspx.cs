using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.DatabaseTracer.Enums;
using Logictracker.Security;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Admin
{
    public partial class AdminLog : SecuredGridReportPage<LogEntryVo>
    {
        protected override string VariableName { get { return "LOGS_ADMIN"; } }
        protected override string GetRefference() { return "LOGS_ADMIN"; }
        protected override bool PrintButton { get { return false; } }
        protected override bool ExcelButton { get { return true; } }
        public override string SearchString { get { return txtBuscar.Text; } }

        protected override List<LogEntryVo> GetResults()
        {
            var reader = new Reader();

            var tipo = ddlLogType.Selected < 0 ? null : (LogTypes?)Enum.Parse(typeof(LogTypes), ddlLogType.SelectedItem.Text);
            var modulo = ddlLogModule.Selected < 0 ? null : (LogModules?)Enum.Parse(typeof(LogModules), ddlLogModule.SelectedItem.Text);
            var componente = ddlLogComponent.Selected < 0 ? null : (LogComponents?)Enum.Parse(typeof(LogComponents), ddlLogComponent.SelectedItem.Text);

            return reader.GetEntries(dtpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(),
                                     dtpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(), 
                                     tipo,
                                     modulo,
                                     componente)
                         .Select(log => new LogEntryVo(log)).ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dtpDesde.SetDate();
            dtpHasta.SetDate();
        }

        protected override void SelectedIndexChanged()
        {
            Session.Add("IdLogEntry", Grid.SelectedDataKey.Value);

            OpenWin(String.Concat(ApplicationPath, "Admin/Log/LogContext.aspx"), "Log");
        }
    }
}