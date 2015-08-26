using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DAL.DAO.BusinessObjects.Entidades;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

namespace Logictracker.Reportes.Bolland
{
    public partial class Sintetico : SecuredGraphReportPage<ValoresDiarios>
    {
        protected override string VariableName { get { return "REP_SINTETICO"; } }
        protected override string GetRefference() { return "REP_SINTETICO"; }
        protected override bool ExcelButton { get { return true; } }
        protected override string XAxisLabel { get { return string.Empty; } }
        protected override string YAxisLabel { get { return string.Empty; } }
        protected override int? DefaultHeight { get { return 500; } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }

        private const string SensorKm = "Tacometro_1_Odometro";
        private const string SensorVelo = "Tacometro_1_Velocidad";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SelectedDate = DateTime.Today;
                dpHasta.SelectedDate = DateTime.Today.AddDays(1).AddMinutes(-1);
                CbPeriodoSelectedIndexChanged(cbPeriodo, EventArgs.Empty);
            }
        }

        protected void CbPeriodoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPeriodo.Selected <= 0) return;
            var periodo = DAOFactory.PeriodoDAO.FindById(cbPeriodo.Selected);
            dpDesde.SelectedDate = periodo.FechaDesde;
            dpHasta.SelectedDate = periodo.FechaHasta;
        }

        protected override List<ValoresDiarios> GetResults()
        {
            panelDetalleInferior.Visible = false;
            panelDetalleSuperior.Visible = false;
            var vehiculo = cbVehiculo.Selected;
            var desde = dpDesde.SelectedDate.Value;
            var hasta = dpHasta.SelectedDate.Value;
            var coche = DAOFactory.CocheDAO.FindById(vehiculo);
            var dispositivo = coche.Dispositivo;

            List<ValoresDiarios> maximasDiarias = null;

            if (dispositivo != null)
            {
                maximasDiarias = DAOFactory.MedicionDAO.GetAggregatesDiarios(dispositivo.Id, SensorVelo, desde, hasta).ToList();
            }

            if (maximasDiarias != null && maximasDiarias.Count > 0)
            {
                SetDetalleSuperior(coche, desde, hasta);
                SetDetalleInferior(coche, desde, hasta);
            }
            return maximasDiarias ?? new List<ValoresDiarios>(0);
        }

        private void SetDetalleSuperior(Coche coche, DateTime desde, DateTime hasta)
        {
            var dispositivo = coche.Dispositivo;
            double recorridoTotal = 0;
            double recorridoPromedio = 0;
            var diasConActividad = 0;
            var diasTotal = (int) Math.Ceiling(hasta.Subtract(desde).TotalDays);
            var velocidadMaxima = 0;
            var velocidadPromedio = 0;
            double tiempoMovimiento = 0;
            double tiempoDetenido = 0;
            var countDesconexiones = DAOFactory.InfraccionDAO.Count(coche.Id, Infraccion.Codigos.BateriaDesconectada, desde, hasta);
            var countAceleraciones = DAOFactory.InfraccionDAO.Count(coche.Id, Infraccion.Codigos.AceleracionBrusca, desde, hasta);
            var countDesaceleraciones = DAOFactory.InfraccionDAO.Count(coche.Id, Infraccion.Codigos.FrenadaBrusca, desde, hasta);
            var countInfracciones = DAOFactory.InfraccionDAO.Count(coche.Id, Infraccion.Codigos.ExcesoVelocidad, desde, hasta);

            if (dispositivo != null)
            {
                var aggregatesVelo = DAOFactory.MedicionDAO.GetAggregates(dispositivo.Id, SensorVelo, desde, hasta);
                if (aggregatesVelo != null)
                {
                    velocidadMaxima = Convert.ToInt32(aggregatesVelo.Max);
                    velocidadPromedio = Convert.ToInt32(aggregatesVelo.Avg);
                }

                var aggregatesKm = DAOFactory.MedicionDAO.GetAggregates(dispositivo.Id, SensorKm, desde, hasta);
                if (aggregatesKm != null)
                {
                    recorridoTotal = aggregatesKm.Max - aggregatesKm.Min;
                }

                var aggregatesDiariosKm = DAOFactory.MedicionDAO.GetAggregatesDiarios(dispositivo.Id, SensorKm, desde,
                                                                                      hasta);
                if (aggregatesDiariosKm != null)
                {
                    diasConActividad = aggregatesDiariosKm.Count();
                    recorridoPromedio = diasConActividad > 0 ? aggregatesDiariosKm.Average(m => m.Max - m.Min) : 0;
                }

                var datamart = DAOFactory.DatamartDAO.GetSummarizedDatamart(desde, hasta, coche.Id);
                if (datamart != null)
                {
                    tiempoMovimiento = datamart.HsMovimiento;
                    tiempoDetenido = datamart.HsDetenido;
                }
            }

            lblRecorridoTotalPrint.Text = lblRecorridoTotal.Text = recorridoTotal.ToString("0.00");
            lblRecorridoPromedioPrint.Text = lblRecorridoPromedio.Text = recorridoPromedio.ToString("0.00");
            lblTotalDiasPrint.Text = lblTotalDias.Text = diasTotal.ToString("0");
            lblDiasConActividadPrint.Text = lblDiasConActividad.Text = diasConActividad.ToString("0");
            lblDiasSinActividadPrint.Text = lblDiasSinActividad.Text = (diasTotal - diasConActividad).ToString("0");
            lblInternoPrint.Text = lblInterno.Text = coche.Interno;
            lblPatentePrint.Text = lblPatente.Text = coche.Patente;
            lblMarcaPrint.Text = lblMarca.Text = coche.Marca != null ? coche.Marca.Descripcion : string.Empty;
            lblModeloPrint.Text = lblModelo.Text = coche.Modelo != null ? coche.Modelo.Descripcion : string.Empty;
            lblAnioPrint.Text = lblAnio.Text = coche.AnioPatente.ToString("0");
            lblTipoVehiculoPrint.Text = lblTipoVehiculo.Text = coche.TipoCoche.Descripcion;
            lblReferenciaPrint.Text = lblReferencia.Text = coche.Referencia;
            lblVelocidadMaximaPrint.Text = lblVelocidadMaxima.Text = velocidadMaxima.ToString("0");
            lblVelocidadPromedioPrint.Text = lblVelocidadPromedio.Text = velocidadPromedio.ToString("0");
            lblTiempoMovimientoPrint.Text = lblTiempoMovimiento.Text = ToString(TimeSpan.FromHours(tiempoMovimiento), "d", "h", "m", null);
            lblTiempoDetenidoPrint.Text = lblTiempoDetenido.Text = ToString(TimeSpan.FromHours(tiempoDetenido), "d", "h", "m", null);
            lblDesconexionesPrint.Text = lblDesconexiones.Text = countDesconexiones.ToString("0");
            lblAceleracionesPrint.Text = lblAceleraciones.Text = countAceleraciones.ToString("0");
            lblDesaceleracionesPrint.Text = lblDesaceleraciones.Text = countDesaceleraciones.ToString("0");
            lblInfraccionesPrint.Text = lblInfracciones.Text = countInfracciones.ToString("0");

            panelDetalleSuperior.Visible = true;
        }
        private void SetDetalleInferior(Coche coche, DateTime desde, DateTime hasta)
        {
            var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;
            
            var logins = DAOFactory.LogMensajeDAO.GetDrivers(coche.Id, desde, hasta, maxMonths).ToList();
            grid.DataSource = logins;
            grid.DataBind();

            gridPrint.DataSource = logins;
            gridPrint.DataBind();

            panelDetalleInferior.Visible = true;
        }
        protected void GridRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var item = e.Row.DataItem as LoginLogout;
            if (item == null) return;
            e.Row.Cells[0].Text = item.Empleado != null ? item.Empleado.Entidad.Descripcion : "Desconocido";
            e.Row.Cells[1].Text = item.LoginDate.ToString("dd/MM/yyyy HH:mm");
			e.Row.Cells[2].Text = item.LogoutDate.HasValue ? ToString(item.LogoutDate.Value.Subtract(item.LoginDate), null, "h", "m", null) : string.Empty;
            e.Row.Cells[3].Text = item.Empleado != null && item.Empleado.Tarjeta != null ? item.Empleado.Tarjeta.Numero : string.Empty;            
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", CultureManager.GetLabel("MAXIMAS_DIARIAS"));
                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "0");
                helper.AddConfigEntry("showValues", "0");
                helper.AddConfigEntry("numberSuffix", string.Empty);
                helper.AddConfigEntry("rotateNames", "1");
                helper.AddConfigEntry("limitsDecimalPrecision", "0");
                helper.AddConfigEntry("hoverCapSepChar", "-");

                foreach (var t in ReportObjectsList)
                {
	                var item = new FusionChartsItem();
	                item.AddPropertyValue("name", t.Date.ToString("dd/MM"));
	                item.AddPropertyValue("hoverText", t.Max.ToString("0"));
	                item.AddPropertyValue("value", t.Max.ToString("0"));
                    
	                helper.AddItem(item);
                }

                if (ReportObjectsList.Count.Equals(0)) ThrowError("NO_MATCHES_FOUND");

                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset {Name = YAxisLabel};
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(t.Date.ToString("dd/MM"));
                dataset.addValue(t.Date.ToString("dd/MM"));
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> {dataset};
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.Date.ToString("dd/MM"),
                                                  t => t.Max.ToString("0"));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            var dict = new Dictionary<string, string>();
            var vehiculo = DAOFactory.CocheDAO.FindById(cbVehiculo.Selected);

            dict.Add(CultureManager.GetLabel("FECHA"), DateTime.UtcNow.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm"));
            dict.Add(CultureManager.GetEntity("PARENTI01"), cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected).RazonSocial : CultureManager.GetLabel("TODOS"));
            dict.Add(CultureManager.GetEntity("PARENTI02"), cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected).DescripcionCorta : CultureManager.GetLabel("TODOS"));
            dict.Add(CultureManager.GetEntity("PARENTI03"), vehiculo.Interno + " - " + vehiculo.Patente);
            dict.Add(CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            dict.Add(CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            
            return dict;
        }
		private static string ToString(TimeSpan timeSpan, string daysText, string hoursText, string minutesText, string secondsText)
		{
			if (timeSpan == TimeSpan.Zero)
			{
				if (secondsText != null) return string.Concat("0", secondsText);
				if (minutesText != null) return string.Concat("0", minutesText);
				if (hoursText != null) return string.Concat("0", hoursText);
				if (daysText != null) return string.Concat("0", daysText);
			}
			var days = timeSpan.TotalDays;
			var hours = (daysText != null ? timeSpan.Hours : timeSpan.TotalHours);
			var minutes = (hoursText != null ? timeSpan.Minutes : timeSpan.TotalMinutes);
			var seconds = (minutesText != null ? timeSpan.Seconds : timeSpan.TotalSeconds);

			var daysPart = daysText != null && days > 0 ? string.Concat(days.ToString("0"), daysText) : string.Empty;
			var hoursPart = hoursText != null && hours > 0 ? string.Concat(hours.ToString("0"), hoursText) : string.Empty;
			var minutesPart = minutesText != null && minutes > 0 ? string.Concat(minutes.ToString("0"), minutesText) : string.Empty;
			var secondsPart = secondsText != null && seconds > 0 ? string.Concat(seconds.ToString("0"), secondsText) : string.Empty;

			return string.Format("{0} {1} {2} {3}", daysPart, hoursPart, minutesPart, secondsPart);
		}
	}
}