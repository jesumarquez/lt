using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.Helpers.ControlHelper;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class EstadisticaGeocercasEvents : SecuredGridReportPage<MobileGeocercaVo>
    {
        #region Protected Properties

        public override int PageSize { get { return 25; } }

        protected override string VariableName { get { return "DOP_REP_GEOCERCAS"; } }
        protected override string GetRefference() { return "REP_GEOCERCAS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Get report objects.
        /// </summary>
        /// <returns></returns>
        protected override List<MobileGeocercaVo> GetResults()
        {
            ifResumenViaje.Visible = false;

            var desde = dpInitDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpEndDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            var inicio = DateTime.UtcNow;
            try
            {
                var selectedGeocercas = GetSelectedGeocercas();
                if (Logictracker.DAL.DAO.BaseClasses.QueryExtensions.IncludesAll(selectedGeocercas))
                {
                    if (Logictracker.DAL.DAO.BaseClasses.QueryExtensions.IncludesAll(lbTipoDomicilio.SelectedValues))
                        ShowError("Por favor, seleccione un Tipo de Referencia Geográfica.");
                    else
                        ShowError("No se encontraron Referencias Geográficas para los filtros seleccionados");
                    return new List<MobileGeocercaVo>();
                }

                var geocercas = ReportFactory.MobileGeocercaDAO.GetGeocercasEvent(GetSelectedMobiles(), selectedGeocercas, desde, hasta, tpEnGeocerca.SelectedTime.TotalSeconds);
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Reporte de Geocercas", String.Format("Duración de la consulta: {0} segundos", duracion));
				CalculateDurations(geocercas, chkCalcularKmRecorridos.Checked, DAOFactory);

				return FilterGeocercas(geocercas);
            }
            catch (Exception e)
            {
				STrace.Exception("Reporte de Geocercas", e, String.Format("Reporte: Reporte de Geocercas. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

				throw;
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileGeocercaVo dataItem)
        {
            grid.Columns[MobileGeocercaVo.IndexRecorrido].Visible = chkCalcularKmRecorridos.Checked;
        }

        protected override void SelectedIndexChanged()
        {
            ShowSelectedRouteDetails();
        }

        protected void DdlLocacionOnSelectedIndexChanged(object sender, EventArgs e)
        {
            lbGeocerca.Items.Clear();
            lbGeocerca.DataBind();
        }
        protected void DdlPlantaOnSelectedIndexChanged(object sender, EventArgs e)
        {
            lbGeocerca.Items.Clear();
            lbGeocerca.DataBind();
        }
        protected void LbTipoDomicilioOnSelecteIndexChanged(object sender, EventArgs e)
        {
            lbGeocerca.Items.Clear();
            var geocercas = DAOFactory.ReferenciaGeograficaDAO.GetListByEmpresaLineaTipos(ddlLocacion.Selected, ddlPlanta.Selected, lbTipoDomicilio.SelectedValues);
            foreach (var geocerca in geocercas) lbGeocerca.Items.Add(new ListItem(geocerca.Descripcion, geocerca.Id.ToString("#0")));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpInitDate.SetDate();
            dpEndDate.SetDate();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI17"), ddlTipoDeVehiculo.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpInitDate.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpInitDate.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpEndDate.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpEndDate.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            var dic = new Dictionary<string, string>();
            var sMoviles = new StringBuilder();
            var sGeocercas = new StringBuilder();

            foreach (var movil in lbMobile.SelectedValues)
            {
                if (!sMoviles.ToString().Equals(""))
                    sMoviles.Append(",");

                sMoviles.Append(movil.ToString(CultureInfo.InvariantCulture));
            }
            foreach (var cerca in ControlHelper.GetSelectedValues(lbGeocerca))
            {
                if (!sGeocercas.ToString().Equals(""))
                    sGeocercas.Append(",");

                sGeocercas.Append(cerca.ToString(CultureInfo.InvariantCulture));
            }
            
            dic.Add("MOVILES", sMoviles.ToString());
            dic.Add("GEOCERCAS", sGeocercas.ToString());
            dic.Add("ENCERCA", tpEnGeocerca.SelectedTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            dic.Add("ENMARCHA", tpEnMarcha.SelectedTime.ToString());

            return dic;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Shows the selected route fragment in the historic monitor or aletrs of any error situation.
        /// </summary>
        private void ShowSelectedRouteDetails()
        {

            var proxima = GridUtils.GetCell(Grid.SelectedRow, MobileGeocercaVo.IndexProximaGeocerca).Text;

            if (string.IsNullOrEmpty(proxima) || proxima.Equals("&nbsp;"))
            {
                ShowErrorPopup(CultureManager.GetSystemMessage("NO_GEOCERCA_INFO"));

                return;
            }

            var salida = GridUtils.GetCell(Grid.SelectedRow, MobileGeocercaVo.IndexSalida).Text;

            string entrada;

            if (Grid.SelectedIndex + 1 >= Grid.PageSize)
            {
                var g = Grid;
                var allowPaging = Grid.AllowPaging;

                g.AllowPaging = false;
                g.DataSource = ReportObjectsList;
                g.DataBind();

                g.AllowPaging = allowPaging;

                entrada = GridUtils.GetCell(Grid.Rows[Grid.SelectedIndex + 1], MobileGeocercaVo.IndexEntrada).Text;

                g.DataBind();
            }
            else entrada = GridUtils.GetCell(Grid.Rows[Grid.SelectedIndex + 1], MobileGeocercaVo.IndexEntrada).Text;
             
            ShowRouteDetails(salida, entrada);
        }

        /// <summary>
        /// Displays the givenn fragment in the historic monitor or alerts if the time span is too big.
        /// </summary>
        /// <param name="salida"></param>
        /// <param name="entrada"></param>
        private void ShowRouteDetails(string salida, string entrada)
        {
            var from = Convert.ToDateTime(salida);
            var to = Convert.ToDateTime(entrada);

            if (to.Subtract(from).TotalHours > 24)
            {
                ShowErrorPopup(CultureManager.GetSystemMessage("ROUTE_TOO_LONG"));

                return;
            }

            AddSessionParameters(to, from);

            ifResumenViaje.Visible = true;
        }

        /// <summary>
        /// Adds route details query parameters into session.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        private void AddSessionParameters(DateTime to, DateTime from)
        {
            Session.Add("RouteDetailsMobile", Convert.ToInt32(Grid.SelectedDataKey.Value));
            Session.Add("RouteDetailsFrom", from.ToDataBaseDateTime());
            Session.Add("RouteDetailsTo", to.ToDataBaseDateTime());
        }

        private void ShowErrorPopup(string error)
        {
            ifResumenViaje.Visible = false;

            var errorMessage = string.Format("alert('{0}');", error);

            ScriptManager.RegisterStartupScript(this, typeof(string), "ErrorPopup", errorMessage, true);
        }

        private IEnumerable<int> GetSelectedMobiles ()
        {
            if (lbMobile.GetSelectedIndices().Length == 0) lbMobile.ToogleItems();

            return lbMobile.SelectedValues;
        }

        private List<Int32> GetSelectedGeocercas()
        {
            if (lbGeocerca.GetSelectedIndices().Length == 0) ControlHelper.ToogleItems(lbGeocerca);

            return ControlHelper.GetSelectedValues(lbGeocerca);
        }

        /// <summary>
        /// Calculate time to next geocerca.
        /// </summary>
        /// <param name="geocercas"></param>
        /// <param name="calcularKms"></param>
        /// <param name="daoFactory"></param>
        private static void 
            CalculateDurations(IList<MobileGeocerca> geocercas, bool calcularKms, DAOFactory daoFactory)
        {
            for (var i = 0; i < geocercas.Count - 1; i++)
            {
                if (!geocercas[i].Interno.Equals(geocercas[i + 1].Interno)) continue;

                var salida = geocercas[i].Salida;
                var entrada = geocercas[i + 1].Entrada;

                if (salida.Equals(DateTime.MinValue) || entrada.Equals(DateTime.MinValue))
                {
                    geocercas[i].ProximaGeocerca = TimeSpan.MinValue;
                    geocercas[i].Recorrido = 0.0;
                }
                else
                {
                    geocercas[i].ProximaGeocerca = entrada.Subtract(salida);
                    geocercas[i].Recorrido = calcularKms ? daoFactory.CocheDAO.GetDistance(geocercas[i].IdMovil, salida.ToDataBaseDateTime(), entrada.ToDataBaseDateTime()) : 0.0;
                }

                geocercas[i].ProximaGeocerca = salida.Equals(DateTime.MinValue) || entrada.Equals(DateTime.MinValue) ? TimeSpan.MinValue : entrada.Subtract(salida);
            }
        }

        /// <summary>
        /// Filter geocercas events based in its duration.
        /// </summary>
        /// <param name="geocercas"></param>
        /// <returns></returns>
        private List<MobileGeocercaVo> FilterGeocercas(IList<MobileGeocerca> geocercas)
        {
            var results = new List<MobileGeocerca>(geocercas.Count);

            for (var i = 0; i < geocercas.Count; i++)
            {
                if (!geocercas[i].ProximaGeocerca.Equals(TimeSpan.MinValue) && geocercas[i].ProximaGeocerca < tpEnMarcha.SelectedTime) continue;

                results.Add(geocercas[i]);

                if (i >= geocercas.Count - 1) continue;

                if (!geocercas[i].Interno.Equals(geocercas[i + 1].Interno)) continue;

                results.Add(geocercas[i + 1]);

                i++;
            }

            return results.Select(geo => new MobileGeocercaVo(geo)).ToList();
        }

        #endregion

        #region Print and CSV Methods

        protected override void OnPrePrint()
        {
            if(Grid.SelectedIndex < 0) return;
            
            Session["KeepInSes"] = true;

            ShowSelectedRouteDetails();

            Iframe1.Visible = true;
        }

        #endregion

        protected override string GetSelectedVehicles()
        {
            var sVehiculos = new StringBuilder();

            if (lbMobile.SelectedValues.Contains(0)) lbMobile.ToogleItems();

            foreach (var vehiculo in lbMobile.SelectedValues)
            {
                if (!sVehiculos.ToString().Equals(""))
                    sVehiculos.Append(",");

                sVehiculos.Append(vehiculo.ToString());
            }

            return sVehiculos.ToString();
        }

        protected override string GetSelectedGeofences()
        {
            var sGeocercas = new StringBuilder();

            foreach (var geof in ControlHelper.GetSelectedValues(lbGeocerca))
            {
                if (!sGeocercas.ToString().Equals(""))
                    sGeocercas.Append(",");

                sGeocercas.Append(geof.ToString(CultureInfo.InvariantCulture));
            }
            return sGeocercas.ToString();
        }

        protected override bool GetCalculateKilometers()
        {
            return chkCalcularKmRecorridos.Checked;    
        }

        protected override double GetInGeofenceTime()
        {
            return tpEnGeocerca.SelectedTime.TotalSeconds;
        }

        protected override string GetDescription(string reporte)
        {
            var linea = GetLinea();
            if (lbMobile.SelectedValues.Contains(0)) lbMobile.ToogleItems();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Reporte: {0} - ", reporte);
            sDescription.AppendFormat("Tipo de Vehiculo: {0} - ", ddlTipoDeVehiculo.SelectedItem.Text);
            //sDescription.AppendFormat("Geocerca: {0} ", lbGeocerca.SelectedItem.Text);
            sDescription.AppendFormat("Cantidad Vehiculos: {0} ", lbMobile.SelectedStringValues.Count);

            return sDescription.ToString();
        }

        protected override int GetCompanyId()
        {
            return GetEmpresa().Id;
        }

        protected override List<int> GetSelectedListByField(string field)
        {
            if ("geofences".Equals(field)) return GetSelectedGeocercas();
            
            if (lbMobile.SelectedValues.Contains(0)) lbMobile.ToogleItems();
            return lbMobile.SelectedValues;
        }

        protected override DateTime GetSinceDateTime()
        {
            return dpInitDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override DateTime GetToDateTime()
        {
            return dpEndDate.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }
    }
}