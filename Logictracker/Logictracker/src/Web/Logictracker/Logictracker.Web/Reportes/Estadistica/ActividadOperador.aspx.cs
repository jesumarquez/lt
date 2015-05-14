using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesActividadOperador : SecuredGridReportPage<OperatorActivityVo>
    {
        protected override string VariableName { get { return "STAT_ACTI_OPERADOR"; } }
        protected override string GetRefference() { return "REP_ACT_OPER"; }
        protected override bool  ExcelButton { get {  return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<OperatorActivityVo> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            var results = ReportFactory.OperatorActivityDAO.GetOperatorActivitys(desde, hasta, ddlDistrito.Selected, ddlBase.Selected, lbEmpleados.SelectedValues, Convert.ToInt32((double) npKm.Value));

            return (from result in results select new OperatorActivityVo(result)).ToList();
        }

        private string GetSelectedEmpleados()
        {
            var str = Enumerable.Aggregate<int, string>(lbEmpleados.GetSelectedIndices(), String.Empty, (current, index) => String.Concat(current, String.Format("{0},", (object) lbEmpleados.Items[index].Text)));

            return str.TrimEnd(',');
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, OperatorActivityVo dataItem)
        {
            GridUtils.GetCell(e.Row, OperatorActivityVo.IndexRecorrido).Text = String.Format("{0:0.00} km", dataItem.Recorrido);
        }

        protected override Dictionary<String, String> GetFilterValues()
        {
            return new Dictionary<String, String>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI09"), GetSelectedEmpleados()},
                           {CultureManager.GetLabel("DESDE"), String.Concat(dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString())},
                           {CultureManager.GetLabel("HASTA"), String.Concat(dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString())}
                       };
        }
    }
}