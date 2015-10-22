using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class ReportesRecorrido : SecuredGridReportPage<RouteEventVo>
    {
        protected override string GetRefference() { return "REP_RECORRIDO"; }
        protected override string VariableName { get { return "DOP_REP_RECORRIDO"; } }
        protected override bool ExcelButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            if (ddlDistrito.Selected > 0)
                return DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);

            return null;
        }
        protected override Linea GetLinea()
        {
            if (ddlPlanta.Selected > 0)
                return DAOFactory.LineaDAO.FindById(ddlPlanta.Selected);

            return null;
        }

        private int StoppedEvent
        {
            get { return Convert.ToInt32(ViewState["StoppedEvent"]); }
            set { ViewState["StoppedEvent"] = value; }
        }
        private int Distance
        {
            get { return Convert.ToInt32(ViewState["Distance"]); }
            set { ViewState["Distance"] = value; }
        }

        private TimeSpan TiempoMovimiento { get; set; }
        private int EventosMovimiento { get; set; }
        private TimeSpan TiempoDetenido { get; set; }
        private int EventosDetenido{ get; set; }
        private double Distancia { get; set; }
        private double VelocidadPromedio { get; set; }

        private DateTime FinalDate
        {
            get { return (DateTime)ViewState["FinalTime"]; }
            set { ViewState["FinalTime"] = value; }
        }
        private DateTime InitialDate
        {
            get { return (DateTime)ViewState["InitialTime"]; }
            set { ViewState["InitialTime"] = value; }
        }

        private string Distrito
        {
            get { return ViewState["Distrito"] != null ? ViewState["Distrito"].ToString() : null; }
            set { ViewState["Distrito"] = value; }
        }
        private string Location
        {
            get { return ViewState["Location"].ToString(); }
            set { ViewState["Location"] = value; }
        }
        private string TypeMobile
        {
            get { return ViewState["TypeMobile"].ToString(); }
            set { ViewState["TypeMobile"] = value; }
        }
        private string Mobile
        {
            get { return ViewState["Mobile"] != null ? ViewState["Mobile"].ToString() : null; }
            set { ViewState["Mobile"] = value; }
        }

        private List<RouteEvent> FilterEvents(IEnumerable<RouteEvent> originalEvents)
        {
            var distance = npDistance.Number / 1000.0;
            var time = TimeSpan.FromMinutes(npStopped.Number);

            return originalEvents.Where(aEvent => aEvent.Distance >= distance || (aEvent.MaximumSpeed == 0 && aEvent.ElapsedTime >= time)).ToList();
        }

        private void ShowTotals(IEnumerable<RouteEvent> events)
        {
            int movement = 0, stopped = 0;
            double distance = 0;
            TimeSpan movementSpan = new TimeSpan(), stoppedSpan = new TimeSpan();

            foreach (var aEvent in events)
            {
                if (aEvent.MaximumSpeed == 0)
                {
                    stopped++;
                    stoppedSpan += aEvent.ElapsedTime;
                }
                else
                {
                    movement++;
                    movementSpan += aEvent.ElapsedTime;
                }

                distance += aEvent.Distance;
            }

            TiempoMovimiento = movementSpan;
            EventosMovimiento = movement;
            TiempoDetenido = stoppedSpan;
            EventosDetenido = stopped;
            VelocidadPromedio = distance/(movementSpan.TotalMinutes/60.0);
            Distancia = distance;

            lblMovement.Text = string.Format(CultureManager.GetLabel("TIME_AND_EVENTS"), movementSpan, movement);
            lblStopped.Text = string.Format(CultureManager.GetLabel("TIME_AND_EVENTS"), stoppedSpan, stopped);
            lblDistance.Text = string.Format("{0:0.00}km", distance);
            lblAverageSpeed.Text = string.Format("{0:0.00}km/h",distance.Equals(0) || movementSpan.TotalMinutes == 0 
                                                        ? distance / (movementSpan.TotalMinutes / 60.0)
                                                        : 0 );
            lblMovimiento.Visible = true;
            lblMovement.Visible = true;
            lblDetenido.Visible = true;
            lblStopped.Visible = true;
            lblDistancia.Visible = true;
            lblDistance.Visible = true;
            lblVelocidadPromedio.Visible = true;
            lblAverageSpeed.Visible = true;
        }

        private void HideTotals()
        {
            lblMovimiento.Visible = false;
            lblMovement.Visible = false;
            lblDetenido.Visible = false;
            lblStopped.Visible = false;
            lblDistancia.Visible = false;
            lblDistance.Visible = false;
            lblVelocidadPromedio.Visible = false;
            lblAverageSpeed.Visible = false;
        }

        private void AddSessionParameters(bool addCenterIndex)
        {
            Session.Add("Distrito", Distrito);
            Session.Add("Location", Location);
            Session.Add("TypeMobile", TypeMobile); 
            Session.Add("Mobile", Mobile);
            Session.Add("InitialDate", InitialDate);
            Session.Add("FinalDate", FinalDate);
            Session.Add("Distance", Distance);
            Session.Add("StoppedEvent", StoppedEvent);
            Session.Add("ShowMessages", 1);
            Session.Add("ShowPOIS", 0);

            if (addCenterIndex) Session.Add("CenterIndex", (Grid.PageSize * Grid.PageIndex) + Grid.SelectedIndex);
        }

        private void View()
        {
            if (Distrito == null)
            {
                ShowError(new Exception(CultureManager.GetError("MUST_SEARCH")));
                return;
            }
                
            AddSessionParameters(false);

            OpenWin(String.Concat(ApplicationPath,"Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        private static string GetIconUrl(RouteEventVo aEvent)
        {
            return aEvent.MaxSpeed == 0 ? string.Concat(ImagesDir, "stopped.png") : string.Format("{0}Common/EditImage.ashx?file={1}&angle={2}",
                ApplicationPath, string.Concat(ImagesDir, "arrow.png"), aEvent.Direction);
        }

        private void SaveQueryParameters()
        {
            Distrito = ddlDistrito.SelectedValue;
            Location = ddlPlanta.SelectedValue;
            TypeMobile = ddlTipoVehiculo.SelectedValue;
            Mobile = ddlMovil.SelectedValue;
            InitialDate = dpDesde.SelectedDate.GetValueOrDefault();
            FinalDate = dpHasta.SelectedDate.GetValueOrDefault();
            Distance = npDistance.Number;
            StoppedEvent = npStopped.Number;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            var coche = Mobile ?? ddlMovil.SelectedValue;

            var mobile =  DAOFactory.CocheDAO.FindById(Convert.ToInt32(coche));

            return new Dictionary<string, string>
                                 {
                                     {"PARENTI01", mobile.Empresa != null ? mobile.Empresa.RazonSocial : ""},
                                     {"PARENTI02", mobile.Linea != null ? mobile.Linea.Descripcion : ""},
                                     {"PARENTI17", mobile.TipoCoche.Descripcion},
                                     {"INTERNO", mobile.Interno},
                                     {"PATENTE", mobile.Patente},
                                     {"MARCA", (mobile.Marca != null) ? mobile.Marca.Descripcion : string.Empty},
                                     {"PARENTI08", (mobile.Dispositivo != null) ? mobile.Dispositivo.Codigo : string.Empty},
                                     {"DISTANCE",npDistance.Number.ToString("#0")},
                                     {"DETENCION",npStopped.Number.ToString("#0")},
                                     {"DESDE", dpDesde.SelectedDate.ToString()},
                                     {"HASTA", dpHasta.SelectedDate.ToString()},
                                     {"", ""},
                                     {"TOTALES", ""},
                                     {"TIEMPO_MOV", TiempoMovimiento.ToString()},
                                     {"EVENTOS_MOV", EventosMovimiento.ToString()},
                                     {"TIEMPO_DET", TiempoDetenido.ToString()},
                                     {"EVENTOS_DET", EventosDetenido.ToString()},
                                     {"DISTANCIA_RECORRIDA", Distancia.ToString("0.00")},
                                     {"VELOCIDAD_AVERAGE", VelocidadPromedio.ToString("0.00")}
                                 };
        }

        protected override void AddToolBarIcons()
        {
            base.AddToolBarIcons();

            ToolBar.AddMapToolbarButton();
        }

        protected override void ToolbarItemCommand(object sender, CommandEventArgs e)
        {
            base.ToolbarItemCommand(sender, e);

            if (e.CommandName.Equals("View")) View();
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParameters(true);
            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, RouteEventVo dataItem)
        {
            GridUtils.GetCell(e.Row, RouteEventVo.IndexIcono).Text = string.Format("<img heigth='28' width='28' src='{0}' />", GetIconUrl(dataItem));
        }

        protected override List<RouteEventVo> GetResults()
        {
            SaveQueryParameters();

            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            var results = ReportFactory.RouteEventDAO.GetRouteEvents((int) ddlMovil.Selected, desde, hasta, numNoReport.Number);

            if (results.Count > 0) ShowTotals(results);
            else HideTotals();

            return FilterEvents(results).Select(re => new RouteEventVo(re)).ToList();
        }

        protected override void OnPrePrint()
        {
            lblAverageSpeedPrint.Text = lblAverageSpeed.Text;
            lblDistancePrint.Text = lblDistance.Text;
            lblMovementPrint.Text = lblMovement.Text;
            lblStoppedPrint.Text = lblStopped.Text;
        }
    }
}
