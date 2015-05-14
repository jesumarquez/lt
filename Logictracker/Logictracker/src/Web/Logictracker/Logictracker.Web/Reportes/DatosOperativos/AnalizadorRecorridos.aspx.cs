using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class Reportes_DatosOperativos_AnalizadorRecorridos : SecuredGridReportPage<MobileTourVo>
    {
        protected override string VariableName { get { return "ANALIZADOR_RECORRIDOS"; } }
        protected override string GetRefference() { return "ANALIZADOR_RECORRIDOS"; }

        public override int PageSize { get { return 10; } }

        #region Protected Methods

        protected override List<MobileTourVo> GetResults()
        {
            ifResumenViaje.Visible = false;

            if (lbMobile.GetSelectedIndices().Length == 0) lbMobile.ToogleItems();
            var vehiculos = lbMobile.SelectedValues;
            var codigoInicio = ddlMensajeOrigen.Selected.ToString();
            var codigoFin = ddlMenasjeFin.Selected.ToString();
            var desde = SecurityExtensions.ToDataBaseDateTime(dpInitDate.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpEndDate.SelectedDate.GetValueOrDefault());
            var duracion = tpDuracion.SelectedTime;

            return DAOFactory.LogMensajeDAO.GetMobileTour(vehiculos, codigoInicio, codigoFin, desde, hasta, duracion)
                .Select(o=> new MobileTourVo(o)).ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack) return;
            dpInitDate.SetDate();
            dpEndDate.SetDate();
        }

        protected override void SelectedIndexChanged()
        {
            if (Grid.SelectedIndex < 0) return;
            ShowSelectedRouteDetails();
            updInf.Update();
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Shows the selected route fragment in the historic monitor or aletrs of any error situation.
        /// </summary>
        private void ShowSelectedRouteDetails()
        {
            var salida = GridUtils.GetCell(Grid.SelectedRow, MobileTourVo.IndexSalida).Text + ' ' + GridUtils.GetCell(Grid.SelectedRow, MobileTourVo.IndexSalidaHora).Text;
            var entrada = GridUtils.GetCell(Grid.SelectedRow, MobileTourVo.IndexEntrada).Text + ' ' + GridUtils.GetCell(Grid.SelectedRow, MobileTourVo.IndexEntradaHora).Text;

            try
            {
                Convert.ToDateTime(salida);
                Convert.ToDateTime(entrada);
            }
            catch
            {
                ShowErrorPopup(CultureManager.GetSystemMessage("NO_GEOCERCA_INFO"));
                return;
            }

            ShowRouteDetails(salida, entrada);
        }

        /// <summary>
        /// Displays the givenn fragment in the historic monitor or alerts if the time span is too big.
        /// </summary>
        /// <param name="salida"></param>
        /// <param name="entrada"></param>
        private void ShowRouteDetails(string salida, string entrada)
        {
            var from = Convert.ToDateTime(entrada);
            var to = Convert.ToDateTime(salida);

            if (to.Subtract(from).TotalHours > 24)
            {
                ShowErrorPopup(CultureManager.GetSystemMessage("ROUTE_TOO_LONG"));

                return;
            }

            AddSessionParameters(from, to);

            ifResumenViaje.Visible = true;
        }

        /// <summary>
        /// Adds route details query parameters into session.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        private void AddSessionParameters(DateTime from, DateTime to)
        {
            Session.Add("RouteDetailsMobile", Grid.SelectedDataKey.Value);
            Session.Add("RouteDetailsFrom", from);
            Session.Add("RouteDetailsTo", to);
        }

        /// <summary>
        /// Displays a popup with the givenn error message.
        /// </summary>
        /// <param name="error"></param>
        private void ShowErrorPopup(string error)
        {
            ifResumenViaje.Visible = false;
            var errorMessage = string.Format("alert('{0}');", error);
            ScriptManager.RegisterStartupScript(this, typeof(string), "ErrorPopup", errorMessage, true);
        }

        #endregion

    }
}
