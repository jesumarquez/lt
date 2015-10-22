using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.ValueObjects.Entidades;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteVariacionMediciones : SecuredGraphReportPage<MedicionVo>
    {
        private const string Hora = "HORA";
        private const int PointsDistance = 3;
        private const string VariacionTemperaturaCaption = "VARIACION_MEDICIONES_CAPTION";
        private double _maxVal = 10;
        private double _minVal = -10;

        protected override string VariableName { get { return "M2M_REPORTE_VARIACION_MEDICIONES"; } }
        protected override string GetRefference() { return "M2M_REPORTE_VARIACION_MEDICIONES"; }
        protected override string XAxisLabel { get { return CultureManager.GetLabel(Hora); } }
        protected override string YAxisLabel 
        { 
            get
            {
                var ret = "";
                if (Subentidad > 0)
                {
                    var subentidad = DAOFactory.SubEntidadDAO.FindById(Subentidad);
                    if (subentidad != null && subentidad.Sensor != null
                     && subentidad.Sensor.TipoMedicion != null && subentidad.Sensor.TipoMedicion.UnidadMedida != null)
                        ret = subentidad.Sensor.TipoMedicion.UnidadMedida.Descripcion;
                }
                return ret;
            } 
        }
        protected override GraphTypes GraphType { get { return GraphTypes.Lines; } }
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
        private int TipoEntidad
        {
            get
            {
                if (ViewState["TipoEntidad"] == null)
                {
                    ViewState["TipoEntidad"] = Session["TipoEntidad"];
                    Session["TipoEntidad"] = null;
                }
                return (ViewState["TipoEntidad"] != null) ? Convert.ToInt32(ViewState["TipoEntidad"]) : -3;
            }
            set { ViewState["TipoEntidad"] = value; }
        }
        private int Entidad
        {
            get
            {
                if (ViewState["Entidad"] == null)
                {
                    ViewState["Entidad"] = Session["Entidad"];
                    Session["Entidad"] = null;
                }
                return (ViewState["Entidad"] != null) ? Convert.ToInt32(ViewState["Entidad"]) : 0;
            }
            set { ViewState["Entidad"] = value; }
        }
        private int Subentidad
        {
            get
            {
                if (ViewState["Subentidad"] == null)
                {
                    ViewState["Subentidad"] = Session["Subentidad"];
                    Session["Subentidad"] = null;
                }
                return (ViewState["Subentidad"] != null) ? Convert.ToInt32(ViewState["Subentidad"]) : 0;
            }
            set { ViewState["Subentidad"] = value; }
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
        
        #region Graph

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowGraph();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetInitialFilterValues();

            if (!IsPostBack) lnkHistorico.Visible = lnkCalidad.Visible = lbl.Visible = false;
        }

        private void OnTick(object sender, EventArgs e)
        {
            BtnSearchClick(sender, e);

            UpdatePanelGraph.Update();
        }

        protected override List<MedicionVo> GetResults()
        {
            SaveQueryValues();

            var iniDate = SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().Subtract(TimeSpan.FromMinutes(npMinutes.Value)));
            var finDate = SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(npMinutes.Value));

            var mediciones = DAOFactory.MedicionDAO.GetList(ddlDistrito.SelectedValues,
                                                            ddlBase.SelectedValues,
                                                            new[] {-1},
                                                            new[] {-1},
                                                            ddlTipoEntidad.SelectedValues,
                                                            GetEntidadesFiltradas(),
                                                            ddlSubentidad.SelectedValues,
                                                            new[] { DAOFactory.TipoMedicionDAO.FindByCode("TEMP").Id, DAOFactory.TipoMedicionDAO.FindByCode("CAUDAL").Id },
                                                            iniDate,
                                                            finDate);

            if (mediciones == null || mediciones.Count.Equals(0)) return new List<MedicionVo>();

            _maxVal = (from medicion in mediciones select medicion.ValorDouble).Max();
            _minVal = (from medicion in mediciones select medicion.ValorDouble).Min();

            BindLinks();

            return mediciones.Select(m => new MedicionVo(m)).ToList();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                InitializeGraph(helper);

                var previousDateIndex = 0; //for saving last date putted in graph

                for (var i = 0; i < ReportObjectsList.Count; i++)
                {
                    var item = new FusionChartsItem();

                    if (i == 0)
                    {
                        item.AddPropertyValue("name", ReportObjectsList[i].Fecha);
                    }
                    else 
                    {   
                        if ((DateTime.Parse(ReportObjectsList[previousDateIndex].Fecha).Minute
                              != DateTime.Parse(ReportObjectsList[i].Fecha).Minute) &&
                             ((i - previousDateIndex) > PointsDistance)) //filters minimum distance between dates (points in graph)
                        {
                            item.AddPropertyValue("name", DateTime.Parse(ReportObjectsList[i].Fecha).TimeOfDay.ToString());
                            previousDateIndex = i; //saves the index of the most recently date putted in graph
                        }
                    }
                    item.AddPropertyValue("hoverText", DateTime.Parse(ReportObjectsList[i].Fecha).TimeOfDay.ToString());
                    item.AddPropertyValue("value", ReportObjectsList[i].Valor.Replace(',','.'));
                    
                    helper.AddItem(item);
                }

                if (Subentidad > 0)
                {
                    var subentidad = DAOFactory.SubEntidadDAO.FindById(Subentidad);
                    if (subentidad != null && subentidad.Sensor != null
                     && subentidad.Sensor.TipoMedicion != null && subentidad.Sensor.TipoMedicion.ControlaLimites)
                    {
                        if (subentidad.ControlaMaximo)
                        {
                            var topInterval = new FusionChartsTrendline();
                            topInterval.AddPropertyValue("value", subentidad.Maximo.ToString(CultureInfo.InvariantCulture));
                            topInterval.AddPropertyValue("color", "ED1C24");
                            helper.AddTrendLine(topInterval);
                        }
                        if (subentidad.ControlaMinimo)
                        {
                            var lowInterval = new FusionChartsTrendline();
                            lowInterval.AddPropertyValue("value", subentidad.Minimo.ToString(CultureInfo.InvariantCulture));
                            lowInterval.AddPropertyValue("color", "579657");
                            helper.AddTrendLine(lowInterval);
                        }
                    }
                }

                if (ReportObjectsList.Count.Equals(0)) ThrowError("NO_MATCHES_FOUND");

                return helper.BuildXml();
            }
        }

        #endregion

        #region CSV and Print Methods

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => DateTime.Parse(t.Fecha).TimeOfDay.ToString(),
                                                  t => t.Valor);
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI79"), ddlEntidad.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI81"), ddlSubentidad.SelectedItem.Text},
                           {CultureManager.GetLabel("FECHA_HORA"), dtpFecha.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dtpFecha.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("MINUTOS"), npMinutes.Value.ToString("{0:0.00}")}
                       };
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(DateTime.Parse(t.Fecha).TimeOfDay.ToString());
                dataset.addValue(t.Valor);
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        #endregion

        #region List Link

        protected override void AddToolBarIcons()
        {
            base.AddToolBarIcons();

            ToolBar.AddCustomToolbarButton("__btListado", "List", "Ver Listado", "ViewList");
        }

        #endregion

        #region Initial Bindings

        protected void DdlDistritoPreBind(object sender, EventArgs e){if (District > -3) ddlDistrito.EditValue = District;}
        protected void DdlBasePreBind(object sender, EventArgs e) { if (Location > -3) ddlBase.EditValue = Location; }
        protected void DdlTipoEntidadPreBind(object sender, EventArgs e) { if (TipoEntidad > -3) ddlTipoEntidad.EditValue = TipoEntidad; }
        protected void DdlEntidadPreBind(object sender, EventArgs e) { if (Entidad > 0) ddlEntidad.EditValue = Entidad; }
        protected void DdlSubentidadPreBind(object sender, EventArgs e) { if (Subentidad > 0) ddlSubentidad.EditValue = Subentidad; }
    
        #endregion

        #region Private Methods

        private void SetInitialFilterValues()
        {
            if (IsPostBack) return;

            if (Date != DateTime.MinValue) 
                dtpFecha.SelectedDate = Date;
            else 
                dtpFecha.SetDate();

            if (Range != 0 ) 
                npMinutes.Value = Range;

            if (Date != DateTime.MinValue && Range != 0) 
                DoSearch();
        }

        private void ShowGraph()
        {
            Size.Tick = OnTick;

            if (!IsPostBack && Entidad > 0) Size.EnableTick = true;
        }

        private void SaveQueryValues()
        {
            District = ddlDistrito.Selected;
            Location = ddlBase.Selected;
            TipoEntidad = ddlTipoEntidad.Selected;
            Entidad = ddlEntidad.Selected;
            Subentidad = ddlSubentidad.Selected;
            Date = dtpFecha.SelectedDate.GetValueOrDefault();
            Range = Convert.ToInt32((double) npMinutes.Value);
        }

        private void InitializeGraph(FusionChartsHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(VariacionTemperaturaCaption), 
                                                           ddlSubentidad.SelectedItem.Text,
                                                           dtpFecha.SelectedDate, 
                                                           dtpFecha.SelectedDate.GetValueOrDefault().Subtract(TimeSpan.FromMinutes(npMinutes.Value)),
                                                           dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(npMinutes.Value)));

            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "1");
            helper.AddConfigEntry("showValues", "0");
            var numberSuffix = string.Empty;
            if (Subentidad > 0)
            {
                var subentidad = DAOFactory.SubEntidadDAO.FindById(Subentidad);
                if (subentidad != null && subentidad.Sensor != null
                 && subentidad.Sensor.TipoMedicion != null && subentidad.Sensor.TipoMedicion.UnidadMedida != null)
                    numberSuffix = subentidad.Sensor.TipoMedicion.UnidadMedida.Simbolo;
            }
            helper.AddConfigEntry("numberSuffix", numberSuffix);
            helper.AddConfigEntry("rotateNames", "1");
            helper.AddConfigEntry("limitsDecimalPrecision", "1");
            helper.AddConfigEntry("hoverCapSepChar", "-");
            var max = (((int)(_maxVal / 10)) + 1) * 10;
            var min = (((int)(_minVal / 10)) - 1) * 10;

            helper.AddConfigEntry("yAxisMaxValue", max > _maxVal ? max.ToString() : _maxVal.ToString());
            helper.AddConfigEntry("yAxisMinValue", min < _minVal ? min.ToString() : _minVal.ToString());
        }

        private IEnumerable<int> GetEntidadesFiltradas()
        {
            var entidadesId = ddlEntidad.SelectedValues;

            if (QueryExtensions.IncludesAll((IEnumerable<int>) entidadesId))
                entidadesId = DAOFactory.EntidadDAO.GetList(new[] { ddlDistrito.Selected },
                                                            new[] { ddlBase.Selected },
                                                            new[] { -1 },
                                                            new[] { ddlTipoEntidad.Selected })
                                                   .Select(e => e.Id).ToList();

            return entidadesId;
        }

        private void BindLinks()
        {
            var subentidad = DAOFactory.SubEntidadDAO.FindById(ddlSubentidad.Selected);

            if (subentidad != null && subentidad.Sensor != null)
            {
                var coche = DAOFactory.CocheDAO.FindMobileByDevice(subentidad.Sensor.Dispositivo.Id);
                if (coche != null && coche.Id > 0)
                {
                    var link = string.Format("../../Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={0}&TypeMobile={1}&Movil={2}&InitialDate={3}&FinalDate={4}&ShowMessages=0&ShowPOIS=0&Empresa={5}",
                                             coche.Linea != null ? coche.Linea.Id : -1,
                                             coche.TipoCoche.Id,
                                             coche.Id,
                                             SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().Subtract(TimeSpan.FromMinutes(npMinutes.Value))).ToString(CultureInfo.InvariantCulture),
                                             SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(npMinutes.Value)).ToString(CultureInfo.InvariantCulture),
                                             coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1);

                    lnkHistorico.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("OPE_MON_HISTORICO") + "')", link);

                    var linkCalidad = string.Format("../../Monitor/MonitorDeCalidad/monitorDeCalidad.aspx?Planta={0}&TypeMobile={1}&Movil={2}&InitialDate={3}&FinalDate={4}&Empresa={5}&Chofer={6}",
                                                    coche.Linea != null ? coche.Linea.Id : -1,
                                                    coche.TipoCoche.Id,
                                                    coche.Id,
                                                    SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().Subtract(TimeSpan.FromMinutes(npMinutes.Value))).ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture),
                                                    SecurityExtensions.ToDataBaseDateTime(dtpFecha.SelectedDate.GetValueOrDefault().AddMinutes(npMinutes.Value)).ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture),
                                                    coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1,
                                                    coche.Chofer != null ? coche.Chofer.Id : -1);

                    lnkCalidad.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("OPE_MON_CALIDAD") + "')", linkCalidad);

                    lnkHistorico.Visible = lnkCalidad.Visible = lbl.Visible = true;
                }
            }
        }

        #endregion
    }
}
