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
    public partial class ReportesEstadisticaActividadCliente : SecuredGridReportPage<MobileActivityVo>
    {
        #region Private Properties

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

        #endregion

        protected override string VariableName { get { return "STAT_ACTI_CLIENTE"; } }
        protected override string GetRefference() { return "REP_ACT_CLI"; }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<MobileActivityVo> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            ToogleItems(lbClientes);
            ToogleItems(lbVehiculos);

            var linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;

            var idEmpresa = linea != null ? linea.Empresa.Id : -1;
            var idLinea = linea != null ? linea.Id : -1;

            var activities = ReportFactory.MobileActivityDAO.GetMobileActivitys(desde, hasta, idEmpresa, idLinea, lbVehiculos.SelectedValues, Convert.ToInt32((double) npKm.Value));

            var results = (from activity in activities select new MobileActivityVo(activity)).ToList();

            DisplayTotals(results);

            return results;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileActivityVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileActivityVo.IndexRecorrido).Text = String.Format("{0:0.00} km", dataItem.Recorrido);
        }

        #region Private Methods

        /// <summary>
        /// Displays report sumarized values.
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

        #region CSV and Print Methods

        /// <summary>
        /// Gets the filter values for the csv exported report.
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI02"), ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected).DescripcionCorta : null},
                           {CultureManager.GetEntity("PARENTI18"), GetSelectedClientsDescriptions()},
                           {CultureManager.GetEntity("PARENTI03"), GetSelectedMobilesDescriptions()},
                           {CultureManager.GetLabel("KM_SUPERADOS"), npKm.Value.ToString()},
                           {String.Empty, String.Empty},
                           {CultureManager.GetLabel("CANTIDAD_KILOMETROS"), Kilometers.ToString()},
                           {CultureManager.GetLabel("CANTIDAD_VEHICULOS"), Vehicles.ToString()},
                           {CultureManager.GetLabel("DESDE"), String.Concat(dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString())},
                           {CultureManager.GetLabel("HASTA"), String.Concat(dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), String.Empty, dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString())}
                       };
        }

        /// <summary>
        /// Get a string that represents the ids of all selected mobiles.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedMobilesDescriptions()
        {
            return Enumerable.Aggregate<int, string>(lbVehiculos.GetSelectedIndices(), string.Empty, (current, index) => string.Concat(current, string.Format("{0},", (object) lbVehiculos.Items[index].Text))).TrimEnd(',');
        }

        /// <summary>
        /// Get a string that represents the ids of all selected mobiles.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedClientsDescriptions()
        {
            if (Enumerable.Count<int>(lbClientes.GetSelectedIndices()) == 0) lbClientes.ToogleItems();
            return Enumerable.Aggregate<int, string>(lbClientes.GetSelectedIndices(), string.Empty, (current, index) => string.Concat(current, string.Format("{0},", (object) lbClientes.Items[index].Text))).TrimEnd(',');
        }

        /// <summary>
        /// Reminds sumarized values for the printed version.
        /// </summary>
        protected override void OnPrePrint()
        {
            lblTotalVehiclesPrint.Text = lblTotalVehicles.Text;
            lblTotalKilometersPrint.Text = lblTotalKilometers.Text;
        }

        #endregion
    }
}
