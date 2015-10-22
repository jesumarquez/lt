using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Security;

namespace Logictracker.Reportes.ControlDeAccesos
{
    public partial class AccessControl : SecuredGridReportPage<AccessEventVo>
    {
        protected override string VariableName { get { return "REP_EVENTOS_ACCESO"; } }
        protected override string GetRefference() { return "REP_EVENTOS_ACCESO"; }
        protected override bool ExcelButton { get { return true; } }

        /// <summary>
        /// The selected mobile.
        /// </summary>
        private int Empleado
        {
            get
            {
                if (ViewState["ACEmpleado"] == null)
                {
                    ViewState["ACEmpleado"] = Session["ACEmpleado"];
                    Session["ACEmpleado"] = null;
                }
                return (ViewState["ACEmpleado"] != null) ? Convert.ToInt32(ViewState["ACEmpleado"]) : 0;
            }
            set { ViewState["ACEmpleado"] = value; }
        }

        /// <summary>
        /// The initial date.
        /// </summary>
        private DateTime InitialDate
        {
            get
            {
                if (ViewState["ACInitialDate"] == null)
                {
                    ViewState["ACInitialDate"] = Session["ACInitialDate"];
                    Session["ACInitialDate"] = null;
                }
                return (ViewState["ACInitialDate"] != null) ? Convert.ToDateTime(ViewState["ACInitialDate"]) : DateTime.Today;
            }
            set { ViewState["ACInitialDate"] = value; }
        }

        /// <summary>
        /// The final date.
        /// </summary>
        private DateTime FinalDate
        {
            get
            {
                if (ViewState["ACFinalDate"] == null)
                {
                    ViewState["ACFinalDate"] = Session["ACFinalDate"];
                    Session["ACFinalDate"] = null;
                }
                return (ViewState["ACFinalDate"] != null) ? Convert.ToDateTime(ViewState["ACFinalDate"]) : DateTime.Today.Add(new TimeSpan(23, 59, 59));
            }
            set { ViewState["ACFinalDate"] = value; }
        }

        protected override List<AccessEventVo> GetResults()
        {
            return DAOFactory.EventoAccesoDAO.FindByEmpresaLineaEmpleadosAndFecha(ddlLocacion.Selected, ddlPlanta.Selected, lbEmpleado.SelectedValues,
                                                                                  SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.Value), SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.Value)).Select(e => new AccessEventVo(e)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, AccessEventVo dataItem)
        {
            e.Row.BackColor = dataItem.Entrada ? Color.LightGreen : Color.IndianRed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();

            SetInitialFilterValues();

            Bind();
        }

        /// <summary>
        /// Sets up initial filter values according to how the page was called.
        /// </summary>
        private void SetInitialFilterValues()
        {
            GetQueryStringParameters();

            if (Empleado <= 0) return;

            var emp = DAOFactory.EmpleadoDAO.FindById(Empleado);

            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
            ddlLocacion.SetSelectedValue(emp.Empresa != null ? emp.Empresa.Id : -1);
            ddlPlanta.SetSelectedValue(emp.Linea != null ? emp.Linea.Id : -1);
            lbEmpleado.SetSelectedIndexes(new List<int>{Empleado});
        }

        /// <summary>
        /// Get filter initial values from query string.
        /// </summary>
        private void GetQueryStringParameters()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Emp"])) Empleado = Convert.ToInt32(Request.QueryString["Emp"]);

            if (!string.IsNullOrEmpty(Request.QueryString["InitialDate"]))
                InitialDate = Convert.ToDateTime(Request.QueryString["InitialDate"], CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(Request.QueryString["FinalDate"]))
                FinalDate = Convert.ToDateTime(Request.QueryString["FinalDate"], CultureInfo.InvariantCulture);
        }
    }
}
