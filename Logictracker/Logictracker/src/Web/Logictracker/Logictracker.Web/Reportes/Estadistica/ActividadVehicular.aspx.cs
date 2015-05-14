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

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesActividadVehicular : SecuredGridReportPage<MobileActivityVo>
    {
        protected override string VariableName { get { return "STAT_ACTI_VEHICULAR"; } }
        protected override string GetRefference() { return "REP_ACT_VEHI"; }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }

        protected override Empresa GetEmpresa() { return null; }

        protected override Linea GetLinea() { return (ddlBase != null && ddlBase.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null; }

        /// <summary>
        /// Total report kilometers.
        /// </summary>
        private double Kilometers
        {
            get { return ViewState["Kilometers"] != null ? Convert.ToDouble(ViewState["Kilometers"]) : 0; }
            set { ViewState["Kilometers"] = value; }
        }

        /// <summary>
        /// Total report vehicles.
        /// </summary>
        private int Vehicles
        {
            get { return ViewState["Vehicles"] != null ? Convert.ToInt32(ViewState["Vehicles"]) : 0; }
            set { ViewState["Vehicles"] = value; }
        }

        #region Protected Methods

        /// <summary>
        /// Sets the date pickers initial values.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        /// <summary>
        /// Gets the results to be displayed at the grid.
        /// </summary>
        /// <returns></returns>
        protected override List<MobileActivityVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            ToogleItems(lbVehiculos);

            var idEmpresa = cbEmpresa.Selected;
            var idLinea = ddlBase.Selected;

            try
            {
                var activities = ReportFactory.MobileActivityDAO.GetMobileActivitys(desde, hasta, idEmpresa, idLinea, lbVehiculos.SelectedValues, Convert.ToInt32((double) npKm.Value));
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Reporte de Actividad Vehicular", String.Format("Duración de la consulta: {0} segundos", duracion));

				var results = (from activity in activities select new MobileActivityVo(activity, desde, hasta, chkDetalleInfracciones.Checked)).ToList();

				DisplayTotals(results);

				return results;
            }
            catch (Exception e)
            {
				STrace.Exception("Reporte de Actividad Vehicular", e, String.Format("Reporte: Reporte de Actividad Vehicular. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        /// <summary>
        /// Custom items data bound.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="e"></param>
        /// <param name="dataItem"></param>
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileActivityVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileActivityVo.IndexRecorrido).Text = String.Format("{0:0.00} km", dataItem.Recorrido);

            grid.Columns[MobileActivityVo.IndexLeves].Visible = chkDetalleInfracciones.Checked;
            grid.Columns[MobileActivityVo.IndexMedias].Visible = chkDetalleInfracciones.Checked;
            grid.Columns[MobileActivityVo.IndexGraves].Visible = chkDetalleInfracciones.Checked;
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath, "Reportes/Estadistica/ResumenVehicular.aspx"), "Vehiculo");
        }

        /// <summary>
        /// Remembers totalized values for the printed version.
        /// </summary>
        protected override void OnPrePrint()
        {
            lblTotalVehiclesPrint.Text = lblTotalVehicles.Text;
            lblTotalKilometersPrint.Text = lblTotalKilometers.Text;
        }

        /// <summary>
        /// Gets the current filter values for the csv exported version.
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected).RazonSocial : "Todos"},
                           {CultureManager.GetEntity("PARENTI02"), ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected).DescripcionCorta : "Todos"},
                           {CultureManager.GetLabel("KM_SUPERADOS"), npKm.Value.ToString()},
                           {String.Empty, String.Empty},
                           {CultureManager.GetLabel("CANTIDAD_KILOMETROS_TOTALES"), Kilometers.ToString()},
                           {CultureManager.GetLabel("CANTIDAD_VEHICULOS"), Vehicles.ToString()},
                           {CultureManager.GetLabel("DESDE"), String.Concat(dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString())},
                           {CultureManager.GetLabel("HASTA"), String.Concat(dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString())}
                       };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            var dic = new Dictionary<string, string>();
            var sVehiculos = new StringBuilder();
            
            foreach (var vehiculo in lbVehiculos.SelectedValues)
            {
                if (!sVehiculos.ToString().Equals(""))
                    sVehiculos.Append(",");

                sVehiculos.Append((string) vehiculo.ToString());
            }
            
            dic.Add("VEHICULOS", sVehiculos.ToString());
            dic.Add("BASE", ddlBase.SelectedValue);
            dic.Add("KM", npKm.Value.ToString());

            return dic;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add parameters to session.
        /// </summary>
        private void AddSessionParameters()
        {
            Session.Add("Base", ddlBase.Selected);
            Session.Add("VehicleType", ddlTipoVehiculo.Selected);
            Session.Add("Movil", Convert.ToInt32(Grid.SelectedDataKey.Value));
            Session.Add("Desde", dpDesde.SelectedDate);
            Session.Add("Hasta", dpHasta.SelectedDate);
        }

        /// <summary>
        /// Get a string that represents the ids of all selected mobiles.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedMobiles()
        {
            return Enumerable.Aggregate<int, string>(lbVehiculos.GetSelectedIndices(), string.Empty, (current, index) => string.Concat(current, string.Format("{0},", (object) lbVehiculos.Items[index].Text))).TrimEnd(',');
        }

        /// <summary>
        /// Displays totalized info for the current report.
        /// </summary>
        /// <param name="results"></param>
        private void DisplayTotals(ICollection<MobileActivityVo> results)
        {
            Kilometers = results.Sum(o => o.Recorrido);
            Vehicles = results.Count();

            lblTotalVehicles.Text = Vehicles.ToString();
            lblTotalKilometers.Text = String.Format("{0:0.00} km", Kilometers);

            lblTotalVehicles.Visible = lblTotalKilometers.Visible = TotalVehicles.Visible = TotalKilometers.Visible = results.Count > 0;
        }

        #endregion
    }
}
