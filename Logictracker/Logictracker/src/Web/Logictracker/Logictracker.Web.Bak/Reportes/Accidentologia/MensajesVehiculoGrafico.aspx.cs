using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.Accidentologia
{
    public partial class AccidentologiaMensajesVehiculoGrafico : SecuredGraphReportPage<MobileMessageVo>
    {
        private const string Hora = "HORA";
        private const int PointsDistance = 3;
        private const string VariacionVelocidadCaption = "VARIACION_VELOCIDAD_CAPTION";

        protected override string VariableName { get { return "ACC_REP_VARIACION_VELOCIDAD"; } }
        protected override string GetRefference() { return "REP_MSG_MOVIL_GRAFICO"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Lines; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel(Hora); } }
        protected override string YAxisLabel { get { return "Km/h"; } }
        protected override bool ExcelButton { get { return true; } }

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
                if(ViewState["Range"]==null)
                {
                    ViewState["Range"] = Session["Range"];
                    Session["Range"] = null;
                }
                return (ViewState["Range"] != null) ? Convert.ToInt32(ViewState["Range"]) : 0;
            }
            set { ViewState["Range"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowGraph();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetInitialFilterValues();
        }

        private void OnTick(object sender, EventArgs e)
        {
            BtnSearchClick(sender, e);

            UpdatePanelGraph.Update();
        }

        protected override List<MobileMessageVo> GetResults()
        {
            SaveQueryValues();

            var iniDate = SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().Subtract(TimeSpan.FromMinutes(npMinutes.Value)));
            var finDate = SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(npMinutes.Value));

            var eventos = ReportFactory.MobileMessageDAO.GetMobileVelocities(ddlMovil.Selected, iniDate, finDate);

            if (eventos == null || eventos.Count.Equals(0)) return new List<MobileMessageVo>();

            var maxSpeed = (from evento in eventos select evento.Velocidad).Max();

            return maxSpeed <= 5 ? new List<MobileMessageVo>() : eventos.Select(m => new MobileMessageVo(m, false)).ToList();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                InitializeGraph(helper);

                var previousDateIndex = 0; //for saving last date putted in graph

                for (var i = 0; i < ReportObjectsList.Count; i++)
                {
                    if (!ReportObjectsList[i].Velocidad.HasValue || ReportObjectsList[i].Velocidad < 0) continue;

                    var item = new FusionChartsItem();

                    if (i == 0)
                    {
                        item.AddPropertyValue("name", ReportObjectsList[i].FechaHora.TimeOfDay.ToString());
                        ReportObjectsList[i].Hora = ReportObjectsList[i].FechaHora.ToString("hh:mm:ss");
                    }
                    else if ((ReportObjectsList[previousDateIndex].FechaHora.Minute
                              != ReportObjectsList[i].FechaHora.Minute) &&
                             ((i - previousDateIndex) > PointsDistance))//filters minimum distance between dates (points in graph)
                    {
                        item.AddPropertyValue("name", ReportObjectsList[i].FechaHora.TimeOfDay.ToString());
                        ReportObjectsList[i].Hora = ReportObjectsList[i].FechaHora.ToString("hh:mm:ss");
                        previousDateIndex = i; //saves the index of the most recently date putted in graph
                    }
                    item.AddPropertyValue("hoverText", ReportObjectsList[i].FechaHora.TimeOfDay.ToString());
                    item.AddPropertyValue("value", ReportObjectsList[i].Velocidad.ToString());
                    AddLink(item, i);

                    helper.AddItem(item);
                }

                if (ReportObjectsList.Count.Equals(0)) ThrowError("NO_MATCHES_FOUND");

                return helper.BuildXml();
            }
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.FechaHora.ToString("hh:mm:ss"),
                                                  t => t.Velocidad.ToString());
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI03"), ddlMovil.SelectedItem.Text},
                           {CultureManager.GetLabel("FECHA_HORA"), dtpFecha.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dtpFecha.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(t.FechaHora.TimeOfDay.ToString());
                dataset.addValue(t.Velocidad.ToString());
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        #region List Link

        protected override void AddToolBarIcons()
        {
            base.AddToolBarIcons();

            ToolBar.AddCustomToolbarButton("__btListado", "List", "Ver Listado", "ViewList");
        }

        protected override void ToolbarItemCommand(object sender, CommandEventArgs e)
        {
            base.ToolbarItemCommand(sender, e);
            if (e.CommandName.Equals("ViewList")) View();
        }

        #endregion

        #region Initial Bindings

        protected void DdlDistritoPreBind(object sender, EventArgs e){if (District > -3) ddlDistrito.EditValue = District;}
        protected void DdlBasePreBind(object sender, EventArgs e) { if (Location > -3) ddlBase.EditValue = Location; }
        protected void DdlTipoVehiculoPreBind(object sender, EventArgs e) { if (TypeMobile > -3) ddlTipoVehiculo.EditValue = TypeMobile;  }
        protected void DdlMovilPreBind(object sender, EventArgs e) { if (Mobile > 0) ddlMovil.EditValue = Mobile; }
    
        #endregion
               
        #region Private Methods

        private void AddLink(FusionChartsItem item, int index)
        {
            item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                "n-{0}Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={1}&TypeMobile={2}&Movil={3}&InitialDate={4}&FinalDate={5}&PosCenterIndex={6}&ShowMessages=1&ShowPOIS=1&Empresa={7}",
                ApplicationPath, Location, TypeMobile, Mobile, Date.AddMinutes(-Range).ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture),
                Date.AddMinutes(Range).ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture), ReportObjectsList[index].Indice, District)));
        }

        private void SetInitialFilterValues()
        {
            if (IsPostBack) return;

            if (Date != DateTime.MinValue) dtpFecha.SelectedDate = Date;
            else dtpFecha.SetDate();

            if(Range != 0 ) npMinutes.Value = Range;

            if (Date != DateTime.MinValue && Range != 0) DoSearch();
        }

        private void ShowGraph()
        {
            Size.Tick = OnTick;

            if (!IsPostBack && Mobile > 0) Size.EnableTick = true;
        }

        private void SaveQueryValues()
        {
            District = Convert.ToInt32((string) ddlDistrito.SelectedValue);
            Location = Convert.ToInt32((string) ddlBase.SelectedValue);
            TypeMobile = Convert.ToInt32((string) ddlTipoVehiculo.SelectedValue);
            Mobile = Convert.ToInt32((string) ddlMovil.SelectedValue);
            Date = dtpFecha.SelectedDate.GetValueOrDefault();
            Range = Convert.ToInt32((double) npMinutes.Value);
        }

        private void AddSessionParameters()
        {
            Session.Add("District", District);
            Session.Add("Location", Location);
            Session.Add("TypeMobile",TypeMobile);
            Session.Add("Mobile", Mobile);
            Session.Add("Date", Date);
            Session.Add("Range", Range);
        }

        private void View()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath,"Reportes/Accidentologia/MensajesVehiculo.aspx"), "Mensajes del Vehiculo");
        }

        private void InitializeGraph(FusionChartsHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(VariacionVelocidadCaption), ddlMovil.SelectedItem.Text,
                                                           dtpFecha.SelectedDate, dtpFecha.SelectedDate.GetValueOrDefault().Subtract(TimeSpan.FromMinutes(npMinutes.Value)),
                                                           dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(npMinutes.Value)));

            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "0");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("numberSuffix", "Km/h");
            helper.AddConfigEntry("rotateNames", "1");
            helper.AddConfigEntry("limitsDecimalPrecision", "0");
            helper.AddConfigEntry("hoverCapSepChar", "-");
        }

        #endregion
    }
}
