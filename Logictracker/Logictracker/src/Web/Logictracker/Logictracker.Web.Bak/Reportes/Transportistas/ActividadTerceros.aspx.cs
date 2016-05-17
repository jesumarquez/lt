using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.Transportistas;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Transportistas
{
    public partial class EstadisticaActividadTerceros : SecuredGridReportPage<TransportActivityVo>
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

        #region Protected Properties

        protected override string GetRefference() { return "REP_TERCEROS"; }
        protected override string VariableName { get { return "TRAN_ACTI_TRANSPORTISTAS"; } }
        protected override bool ExcelButton { get { return true; } }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<TransportActivityVo> GetResults()
        {
            ToogleItems(lbTransport);

            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            var results = (from o in ReportFactory.TransportActivityDAO.GetTransportActivities(desde, hasta, GetMobilesIds(), Convert.ToInt32(npKm.Value))
                        select new TransportActivityVo(o)).ToList();

            DisplayTotals(results);
            return results;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, TransportActivityVo dataItem)
        {
            GridUtils.GetCell(e.Row, TransportActivityVo.IndexRecorrido).Text = String.Format("{0:0.00} km", dataItem.Recorrido);
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text },
                                {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                                {CultureManager.GetLabel("KM_SUPERADOS"), npKm.Value.ToString() },
                                {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                                {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                            };
        }

        #endregion

        #region Private Methods

        private string GetSelectedTransporters()
        {
            if (lbTransport.SelectedValues.Contains(0)) return "Todos";

            var str = lbTransport.SelectedValues.Aggregate(String.Empty, (current, id) => String.Concat(current, DAOFactory.EmpresaDAO.FindById(Convert.ToInt32(id)).RazonSocial, ", "));

            return str.TrimEnd(',');
        }

        /// <summary>
        /// Displays tthe totals detail in the DetalleInferior ContentPlaceHolder.
        /// </summary>
        /// <param name="results"></param>
        private void DisplayTotals(List<TransportActivityVo> results)
        {
            Kilometers = results.Sum(o => o.Recorrido);
            Vehicles = results.Count();
            
            lblTotalVehicles.Text = Vehicles.ToString();
            lblTotalKilometers.Text = String.Format("{0:0.00} km",Kilometers);

            lblTotalVehicles.Visible = lblTotalKilometers.Visible = TotalVehicles.Visible = TotalKilometers.Visible = results.Count > 0;
        }

        /// <summary>
        /// Gets all the ids of the selected transport company.
        /// </summary>
        /// <returns></returns>
        private List<Int32> GetMobilesIds()
        {
            var ids = new List<int>();

            if (lbTransport.SelectedValues.Contains(0)) return ids;

            var mobiles = DAOFactory.CocheDAO.GetList(ddlDistrito.SelectedValues, ddlBase.SelectedValues, new[]{-1}, lbTransport.SelectedValues);

            if (mobiles == null) return ids;

            ids.AddRange(from Coche mobile in mobiles select mobile.Id);

            return ids;
        }

        #endregion

        #region Print Methods

        protected override void OnPrePrint()
        {
            lblTotalKilometersPrint.Text = lblTotalKilometers.Text;
            lblTotalVehiclesPrint.Text = lblTotalVehicles.Text;
        }

        #endregion
    }
}
