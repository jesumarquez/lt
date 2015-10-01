using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class ReportesEventos : SecuredGridReportPage<MobileEventVo>
    {
        protected override string VariableName { get { return "DOP_REP_EVENTOS"; } }
        protected override string GetRefference() { return "REP_EVENTOS"; }

        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }
        public Boolean recount = true;
        private const int Interval = 5;

        protected int totalVirtualRows;

        [Serializable]
        public class SearchData
        {
            public List<int> VehiclesId { get; set; }
            public IEnumerable<int> MessageId { get; set; }
            public List<int> DriverId { get; set; }
            public DateTime InitialDate { get; set; }
            public DateTime FinalDate{ get; set; }
        }

        private void SaveSearchData(SearchData data)
        {
            ViewState["SearchData"] = data;
        }

        private SearchData LoadSearchData()
        {
            var data = ViewState["SearchData"] as SearchData;
            if (data != null) return data;

            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            if (lbCamiones.SelectedValues.Contains(0)) lbCamiones.ToogleItems();
            if (lbMensajes.SelectedValues.Contains(0)) lbMensajes.ToogleItems();

            data = new SearchData
                       {
                           VehiclesId = lbCamiones.SelectedValues,
                           MessageId = lbMensajes.SelectedValues,
                           DriverId = lbChoferes.SelectedValues,
                           InitialDate = desde,
                           FinalDate = hasta
                       };


            if (Session["SearchData"] != null &&
               compare(data,(SearchData)Session["SearchData"]))
            {
                recount = false;
            }
            else
            {
                recount = true;
                Session["SearchData"] = data;
            }

            SaveSearchData(data);
            return data;
        }

        public bool compare(SearchData x, SearchData y)
        {
            bool equal = true;
            if (!y.DriverId.Count.Equals(x.DriverId.Count))
                return false;
            int index = 0;
            foreach (var itemy in y.DriverId)
            {
                if (!itemy.Equals(x.DriverId[index]))
                    return false;
                index++;
            }
            if (!x.FinalDate.Equals(y.FinalDate))
                return false;
            if (!x.InitialDate.Equals(y.InitialDate))
                return false;
            if (!y.MessageId.Count().Equals(x.MessageId.Count()))
                return false;
            index = 0;
            foreach (var itemy in y.MessageId.OrderBy(a => a).ToList())
            {
                if (!itemy.Equals(x.MessageId.OrderBy(a => a).ToList()[index]))
                    return false;
                index++;
            }
            if (!x.VehiclesId.Count().Equals(y.VehiclesId.Count()))
                return false;

            index = 0;
            foreach (var itemy in y.VehiclesId.OrderBy(a => a).ToList())
            {
                if (!itemy.Equals(x.VehiclesId.OrderBy(a => a).ToList()[index]))
                    return false;
                index++;
            }
            return equal;
        }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        private int Location
        {
            get
            {
                if (ViewState["EventsLocation"] == null)
                {
                    ViewState["EventsLocation"] = Session["EventsLocation"];
                    Session["EventsLocation"] = null;
                }

                return ViewState["EventsLocation"] != null ? Convert.ToInt32(ViewState["EventsLocation"]) : 0;
            }
        }
        private int Company
        {
            get
            {
                if (ViewState["EventsCompany"] == null)
                {
                    ViewState["EventsCompany"] = Session["EventsCompany"];
                    Session["EventsCompany"] = null;
                }

                return ViewState["EventsCompany"] != null ? Convert.ToInt32(ViewState["EventsCompany"]) : 0;
            }
        }
        private int MobileType
        {
            get
            {
                if (ViewState["EventsMobileType"] == null)
                {
                    ViewState["EventsMobileType"] = Session["EventsMobileType"];
                    Session["EventsMobileType"] = null;
                }

                return ViewState["EventsMobileType"] != null ? Convert.ToInt32(ViewState["EventsMobileType"]) : 0;
            }
        }
        private int Mobile
        {
            get
            {
                if (ViewState["EventsMobile"] == null)
                {
                    ViewState["EventsMobile"] = Session["EventsMobile"];
                    Session["EventsMobile"] = null;
                }

                return ViewState["EventsMobile"] != null ? Convert.ToInt32(ViewState["EventsMobile"]) : 0;
            }
        }
        private DateTime InitialDate
        {
            get
            {
                if (ViewState["EventsFrom"] == null)
                {
                    ViewState["EventsFrom"] = Session["EventsFrom"];
                    Session["EventsFrom"] = null;
                }

                return ViewState["EventsFrom"] != null ? Convert.ToDateTime(ViewState["EventsFrom"]) : DateTime.MinValue;
            }
        }
        private DateTime FinalDate
        {
            get
            {
                if (ViewState["EventsTo"] == null)
                {
                    ViewState["EventsTo"] = Session["EventsTo"];
                    Session["EventsTo"] = null;
                }

                return ViewState["EventsTo"] != null ? Convert.ToDateTime(ViewState["EventsTo"]) : DateTime.MinValue;
            }
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileEventVo dataItem)
        {
            var clickScript = ClientScript.GetPostBackEventReference(Grid, string.Format("Select:{0}", e.Row.RowIndex));

            e.Row.Attributes.Remove("onclick");

            var checkIndex = GridUtils.GetColumnIndex(MobileEventVo.IndexTieneFoto);
            for (var i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i != checkIndex)
                {
                    GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", clickScript);
                }
                else if (dataItem.TieneFoto)
                {
                    GridUtils.GetCell(e.Row, MobileEventVo.IndexTieneFoto).Text = @"<div class=""withPhoto""></div>";

                    const string link = "window.open('../../Common/Pictures?e={0}', 'Fotos_{0}', 'width=345,height=408,directories=no,location=no,menubar=no,resizable=no,scrollbars=no,status=no,toolbar=no');";
                    GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", string.Format(link, dataItem.Id));
                }
            }
            
            GridUtils.GetCell(e.Row, MobileEventVo.IndexIconUrl).Text = string.Format("<img src='{0}' />", IconDir + dataItem.IconUrl);
            grid.Columns[MobileEventVo.IndexInitialCross].Visible = chkVerEsquinas.Checked;
            grid.Columns[MobileEventVo.IndexFinalCross].Visible = chkVerEsquinas.Checked;

            grid.Columns[MobileEventVo.IndexAtendido].Visible = chkVerAtenciones.Checked;
            grid.Columns[MobileEventVo.IndexUsuario].Visible = chkVerAtenciones.Checked;
            grid.Columns[MobileEventVo.IndexFecha].Visible = chkVerAtenciones.Checked;
            grid.Columns[MobileEventVo.IndexMensaje].Visible = chkVerAtenciones.Checked;
            grid.Columns[MobileEventVo.IndexObservacion].Visible = chkVerAtenciones.Checked;
        }

        protected override void OnBinding()
        {
            SaveSearchData(null);
        }

        /// <summary>
        /// The list to be bounded to the report grid.
        /// </summary>
        /// <returns></returns>
        protected override List<MobileEventVo> GetResults()
        {
            var data = LoadSearchData();
            var inicio = DateTime.UtcNow;
            LblInfo.Visible = false;

            if (data.FinalDate.Subtract(data.InitialDate).TotalDays > 31)
            {
                ShowError("No es posible consultar per�odos mayores a 31 d�as.");
                return new List<MobileEventVo>();
            }

            try
            {
                var empresa = DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected);
                var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;
                var results = new List<MobileEventVo>();
                if (chkPaginar.Checked)
                {
                    totalVirtualRows = (int)Session["totalVirtualRows"];
                    results = ReportFactory.MobileEventDAO.GetMobilesEventsLinq(data.VehiclesId,
                                                                                data.MessageId,
                                                                                data.DriverId,
                                                                                data.InitialDate,
                                                                                data.FinalDate,
                                                                                maxMonths,
                                                                                Grid.PageIndex,
                                                                                this.PageSize,
                                                                                ref totalVirtualRows,
                                                                                recount)
                                                              .Select(o => new MobileEventVo(o) { HideCornerNearest = !chkVerEsquinas.Checked })
                                                              .ToList();

                    if (recount)
                    {
                        Session["totalVirtualRows"] = totalVirtualRows;
                        Grid.VirtualItemCount = totalVirtualRows;
                    }
                }
                else
                {
                    results = ReportFactory.MobileEventDAO.GetMobilesEvents(data.VehiclesId,
                                                                                data.MessageId,
                                                                                data.DriverId,
                                                                                data.InitialDate,
                                                                                data.FinalDate,
                                                                                maxMonths)
                                                              .Select(o => new MobileEventVo(o) { HideCornerNearest = !chkVerEsquinas.Checked })
                                                              .ToList();
                }

                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Reporte de Eventos", String.Format("Duraci�n de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
				STrace.Exception("Reporte de Eventos", e, String.Format("Reporte: Reporte de Eventos. Duraci�n de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        /// <summary>
        /// Initial data binding when called from another report.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (chkPaginar.Checked)
            {
                if (Session["totalVirtualRows"] == null)
                    Session["totalVirtualRows"] = totalVirtualRows;
                Grid.AllowPaging = true;
                Grid.AllowCustomPaging = true;
                Grid.PagerSettings.Mode = System.Web.UI.WebControls.PagerButtons.NumericFirstLast;
                Grid.PageIndexChanging += Grid_PageIndexChanging;
                GridUtils.CustomPagination = true;
            }
            else
            {

                if (Session["totalVirtualRows"] == null)
                    Session["totalVirtualRows"] = totalVirtualRows;
                Grid.AllowPaging = true;
                Grid.AllowCustomPaging = false;
                Grid.PagerSettings.Mode = System.Web.UI.WebControls.PagerButtons.NumericFirstLast;
                Grid.PageIndexChanging += Grid_PageIndexChanging;

                GridUtils.CustomPagination = true;
            }

            chkPaginar.CheckedChanged += chkPaginar_CheckedChanged;

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
            
            if (IsPostBack) return;

            if (Mobile.Equals(0)) return;

            SetInitialFilterValues();

            Bind();
        }

        void chkPaginar_CheckedChanged(object sender, EventArgs e)
        {
            Session["SelectedClient"] = null;
            if (chkPaginar.Checked)
            {               
                if (Session["totalVirtualRows"] == null)
                    Session["totalVirtualRows"] = totalVirtualRows;
                Grid.AllowPaging = true;
                Grid.AllowCustomPaging = true;
                Grid.PagerSettings.Mode = System.Web.UI.WebControls.PagerButtons.NumericFirstLast;
                Grid.PageIndexChanging += Grid_PageIndexChanging;
                GridUtils.CustomPagination = true;
            }
            else
            {;
                if (Session["totalVirtualRows"] == null)
                    Session["totalVirtualRows"] = totalVirtualRows;
                Grid.AllowPaging = true;
                Grid.AllowCustomPaging = false;
                Grid.PagerSettings.Mode = System.Web.UI.WebControls.PagerButtons.NumericFirstLast;
                Grid.PageIndexChanging += Grid_PageIndexChanging;

                GridUtils.CustomPagination = true;
            }
        }

        void Grid_PageIndexChanging(object sender, C1GridViewPageEventArgs e)
        {
            Grid.PageIndex = e.NewPageIndex;
        }

        /// <summary>
        /// Location initial binding when called from another report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlLocacionInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Location > 0) ddlLocacion.EditValue = Location; }

        /// <summary>
        /// Company initial binding when called from another report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlPlantaInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Company > 0) ddlPlanta.EditValue = Company; }

        /// <summary>
        /// Vehicle T initial binding when called from another report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlTipoVehiculoInitialBinding(object sender, EventArgs e) { if (!IsPostBack && MobileType > 0) ddlTipoVehiculo.EditValue = MobileType; }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.ToString()}, {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.ToString() }
                       };
        }

        //protected override Dictionary<string, string> GetFilterValuesProgramados()
        //{
        //    var dic = new Dictionary<string, string>();
        //    var sVehiculos = new StringBuilder();
        //    var sMensajes = new StringBuilder();
        //    var sChoferes = new StringBuilder();

        //    foreach (var vehiculo in lbCamiones.SelectedValues)
        //    {
        //        if (!sVehiculos.ToString().Equals(""))
        //            sVehiculos.Append(",");

        //        sVehiculos.Append(vehiculo.ToString("#0"));
        //    }
        //    foreach (var mensaje in lbMensajes.SelectedValues)
        //    {
        //        if (!sMensajes.ToString().Equals(""))
        //            sMensajes.Append(",");

        //        sMensajes.Append(mensaje.ToString("#0"));
        //    }
        //    foreach (var chofer in lbChoferes.SelectedValues)
        //    {
        //        if (!sChoferes.ToString().Equals(""))
        //            sChoferes.Append(",");

        //        sChoferes.Append(chofer.ToString("#0"));
        //    }

        //    dic.Add("VEHICULOS", sVehiculos.ToString());
        //    dic.Add("MENSAJES", sMensajes.ToString());
        //    dic.Add("CHOFERES", sChoferes.ToString());

        //    return dic;
        //}

        protected override List<int> GetVehicleList()
        {
            if (lbCamiones.SelectedValues.Contains(0)) lbCamiones.ToogleItems();
            return lbCamiones.SelectedValues;
        }

        protected override List<int> GetMessageTypeList()
        {
            if (lbMensajes.SelectedValues.Contains(0)) lbMensajes.ToogleItems();
            return lbMensajes.SelectedValues;
        }

        protected override List<int> GetDriverList()
        {
            return lbChoferes.SelectedValues;
        }
        
        protected override string GetDescription(string reporte)
        {
            var linea = GetLinea();
            if (lbCamiones.SelectedValues.Contains(0)) lbCamiones.ToogleItems();
            if (lbMensajes.SelectedValues.Contains(0)) lbMensajes.ToogleItems();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Reporte: {0} - ", reporte);
            sDescription.AppendFormat("Tipo de Vehiculo: {0} - ", ddlTipoVehiculo.SelectedItem.Text);
            sDescription.AppendFormat("Tipo de Mensajes: {0} ", ddlTipoMensaje.SelectedItem.Text);
            sDescription.AppendFormat("Cantidad Vehiculos: {0} ", lbCamiones.SelectedStringValues.Count);
            sDescription.AppendFormat("Cantidad Mensajes: {0} ", lbMensajes.SelectedStringValues.Count);
            sDescription.AppendFormat("Cantidad Conductores: {0}", lbChoferes.SelectedStringValues.Count);

            return sDescription.ToString();
        }
        
        protected override DateTime GetSinceDateTime()
        {
            return dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override DateTime GetToDateTime()
        {
            return dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();    
        }

        /// <summary>
        /// Initial filters set up when called from another report.
        /// </summary>
        private void SetInitialFilterValues()
        {
            ddlTipoMensaje.SetSelectedValue(-1);

            lbCamiones.SetSelectedIndexes(new List<int>{ Mobile });

            lbMensajes.ToogleItems();

            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
        }

        /// <summary>
        /// Add parameters to session in order to be used by the historic monitor.
        /// </summary>
        private void AddSessionParameters()
        {
            var message = DAOFactory.LogMensajeDAO.FindById(Convert.ToInt32(Grid.SelectedDataKey.Value));

            Session.Add("Distrito", message.Coche.Empresa != null ? message.Coche.Empresa.Id : message.Coche.Linea != null ? message.Coche.Linea.Empresa.Id : -1);
            Session.Add("Location", message.Coche.Linea != null ? message.Coche.Linea.Id : -1);
            Session.Add("TypeMobile", message.Coche.TipoCoche.Id);
            Session.Add("Mobile", message.Coche.Id);
            Session.Add("InitialDate", message.Fecha.AddMinutes(-Interval));
            Session.Add("FinalDate", (message.FechaFin.HasValue ? message.FechaFin.Value : message.Fecha).AddMinutes(Interval));
            Session.Add("MessageType", ddlTipoMensaje.Selected);

            if (!lbMensajes.SelectedValues.Contains(0)) Session.Add("MessagesIds", lbMensajes.SelectedStringValues);
            else Session.Add("ShowMessages", 1);

            Session.Add("MessageCenterIndex", message.Id);
            Session.Add("ShowPOIS", 0);
        }
    }
}