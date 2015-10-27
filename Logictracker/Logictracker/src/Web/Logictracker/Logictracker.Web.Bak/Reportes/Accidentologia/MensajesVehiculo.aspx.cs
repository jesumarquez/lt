using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reportes.Accidentologia
{
    public partial class Accidentologia_MensajesVehiculo : SecuredGridReportPage<MobileMessageVo>
    {
        protected override string VariableName { get { return "ACC_REP_DETALLE_RECORRIDO"; } }
        protected override string GetRefference() { return "REP_MSG_MOVIL"; }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }
        protected override Empresa GetEmpresa()
        {
            return ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlBase != null && ddlBase.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
        }

        private int District
        {
            get
            {
                if (ViewState["District"] == null)
                {
                    ViewState["District"] = Session["District"];
                    Session["District"] = null;
                }
                return (ViewState["District"] != null) ? Convert.ToInt32(ViewState["District"]) : -3;
            }
            set { ViewState["District"] = value; }
        }
        private int Location
        {
            get
            {
                if (ViewState["Location"] == null)
                {
                    ViewState["Location"] = Session["Location"];
                    Session["Location"] = null;
                }
                return (ViewState["Location"] != null) ? Convert.ToInt32(ViewState["Location"]) : -3;
            }
            set { ViewState["Location"] = value; }
        }
        private int TypeMobile
        {
            get
            {
                if (ViewState["TypeMobile"] == null)
                {
                    ViewState["TypeMobile"] = Session["TypeMobile"];
                    Session["TypeMobile"] = null;
                }
                return (ViewState["TypeMobile"] != null) ? Convert.ToInt32(ViewState["TypeMobile"]) : -3;
            }
            set { ViewState["TypeMobile"] = value; }
        }
        private int Mobile
        {
            get
            {
                if (ViewState["Mobile"] == null)
                {
                    ViewState["Mobile"] = Session["Mobile"];
                    Session["Mobile"] = null;
                }
                return (ViewState["Mobile"] != null) ? Convert.ToInt32(ViewState["Mobile"]) : 0;
            }
            set { ViewState["Mobile"] = value; }
        }
        private DateTime Date
        {
            get
            {
                if (ViewState["Date"] == null)
                {
                    ViewState["Date"] = Session["Date"];
                    Session["Date"] = null;
                }
                return (ViewState["Date"] != null) ? Convert.ToDateTime(ViewState["Date"]) : DateTime.MinValue;
            }
            set { ViewState["Date"] = value; }
        }
        private int Range
        {
            get
            {
                if (ViewState["Range"] == null)
                {
                    ViewState["Range"] = Session["Range"];
                    Session["Range"] = null;
                }
                return (ViewState["Range"] != null) ? Convert.ToInt32(ViewState["Range"]) : 0;
            }
            set { ViewState["Range"] = value; }
        }

        private void ViewGraph()
        {
            AddSessionParametersForGraph();
            OpenWin("MensajesVehiculoGrafico.aspx", "Grafico de Mensajes del Vehiculo");
        }

        private void AddSessionParametersForGraph()
        {
            Session.Add("District", District);
            Session.Add("Location", Location);
            Session.Add("TypeMobile", TypeMobile);
            Session.Add("Mobile", Mobile);
            Session.Add("Date", Date);
            Session.Add("Range", Range);
        }

        private void AddSessionParametersForMonitor()
        {
            var velocidad = GridUtils.GetCell(Grid.SelectedRow, MobileMessageVo.IndexVelocidad).Text;

            Session.Add("Distrito", District);
            Session.Add("Location", Location);
            Session.Add("TypeMobile", TypeMobile);
            Session.Add("Mobile", Mobile);
            Session.Add("InitialDate", Date.AddMinutes(-Range));
            Session.Add("FinalDate", Date.AddMinutes(Range));
            Session.Add(velocidad != "-" ? "PosCenterIndex" : "MessageCenterIndex", Grid.DataKeys[Grid.SelectedIndex].Value);
            Session.Add("ShowMessages", 1);
        }

        private void SaveLastQueryValues()
        {
            District = Convert.ToInt32(ddlDistrito.SelectedValue);
            Location = Convert.ToInt32(ddlBase.SelectedValue);
            TypeMobile = Convert.ToInt32(ddlTipo.SelectedValue);
            Mobile = Convert.ToInt32(ddlMovil.SelectedValue);
            Date = dtpFecha.SelectedDate.GetValueOrDefault();
            Range = Convert.ToInt32(npRango.Value);
        }

        private void ShowList()
        {
            try { if (Mobile > 0) Bind(); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void SetInitialFilterValues()
        {
            if (Mobile != 0) ddlMovil.SelectedValue = Mobile.ToString("#0");

            if (Date != DateTime.MinValue) dtpFecha.SelectedDate = Date;
            else dtpFecha.SetDate();

            if (Range != 0) npRango.Value = Range;
        }
   
        protected override List<MobileMessageVo> GetResults()
        {
            var desde = dtpFecha.SelectedDate.GetValueOrDefault().Subtract(TimeSpan.FromMinutes(npRango.Value)).ToDataBaseDateTime();
            var hasta = dtpFecha.SelectedDate.GetValueOrDefault().Add(TimeSpan.FromMinutes(npRango.Value)).ToDataBaseDateTime();

            SaveLastQueryValues();

            return ReportFactory.MobileMessageDAO.GetMobileMessages(ddlMovil.Selected, desde, hasta).Select(m => new MobileMessageVo(m, chkDirecciones.Checked)).ToList();
        }
       
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileMessageVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileMessageVo.IndexVelocidad).Text = (dataItem.Velocidad.HasValue) ? dataItem.Velocidad.ToString() : "-";

            grid.Columns[MobileMessageVo.IndexGeocerca].Visible = chkDirecciones.Checked;
        }

        protected override void AddToolBarIcons()
        {
            base.AddToolBarIcons();

            ToolBar.AddCustomToolbarButton("__btListado", "Map", "Ver Gráfico", "ViewGraph");
        }

        protected override void ToolbarItemCommand(object sender, CommandEventArgs e)
        {
            base.ToolbarItemCommand(sender, e);

            if (e.CommandName.Equals("ViewGraph")) ViewGraph();
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParametersForMonitor();

            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            SetInitialFilterValues();

            ShowList();
        }

        protected void DdlDistritoPreBind(object sender, EventArgs e) { if (District > -3) ddlDistrito.EditValue = District; }
        protected void DdlBasePreBind(object sender, EventArgs e) { if (Location > -3) ddlBase.EditValue = Location; }
        protected void DdlTipoPreBind(object sender, EventArgs e) { if (TypeMobile > 0) ddlTipo.EditValue = TypeMobile; }
        protected void DdlMovilPreBind(object sender, EventArgs e) { if (Mobile > 0) ddlMovil.EditValue = Mobile; }
       
        #region CSV Methods

        protected override Dictionary<string, string> GetFilterValues()
        {
            var desde = dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(-npRango.Value);
            var hasta = dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(npRango.Value);

            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI03"), ddlMovil.Selected > 0 ? DAOFactory.CocheDAO.FindById(ddlMovil.Selected).Interno : string.Empty},
                           {CultureManager.GetLabel("DESDE"), desde.ToShortDateString() + " " + desde.ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), hasta.ToShortDateString() + " " + hasta.ToShortTimeString()}
                       };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            return new Dictionary<string, string> { { "PARENTI03", (ddlMovil.Selected > 0) ? ddlMovil.Selected.ToString("#0") : "0" } };
        }

        #endregion
    }
}