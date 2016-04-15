using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Security;
using Logictracker.Utils;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class ReporteTraslado : SecuredGridReportPage<ReporteTrasladoVo>
    {
        protected override string VariableName { get { return "REPORTE_TRASLADO"; } }
        protected override string GetRefference() { return "REPORTE_TRASLADO"; }
        public override int PageSize { get { return 10000; } }
        protected override bool ExcelButton { get { return true; } }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

        public int TotalRutas { get; set; }
        public double TotalKms { get; set; }
        public double PromedioKm { get; set; }
        public double TotalKmProductivos { get; set; }
        public double TotalKmImproductivos { get; set; }
        public double TotalKmProgramados { get; set; }
        public TimeSpan TotalMinutos { get; set; }
        public TimeSpan PromedioMinutos { get; set; }
        public TimeSpan TotalMinutosProgramados { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
        }

        protected override List<ReporteTrasladoVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var desde = dpDesde.SelectedDate.Value.ToDataBaseDateTime();
                var hasta = dpHasta.SelectedDate.Value.ToDataBaseDateTime();

                if (hasta > DateTime.Today.ToDataBaseDateTime())
                    hasta = DateTime.Today.ToDataBaseDateTime();

                if (lbVehiculo.SelectedValues.Contains(0)) 
                    lbVehiculo.ToogleItems();

                var viajes = DAOFactory.ViajeDistribucionDAO.GetList(ddlLocacion.SelectedValues,
                                                                     ddlPlanta.SelectedValues,
                                                                     new[] {-1}, // TRANSPORTISTAS
                                                                     new[] {-1}, // DEPTOS
                                                                     new[] {-1}, // CENTROS
                                                                     new[] {-1}, // SUBCC
                                                                     lbVehiculo.SelectedValues,
                                                                     desde,
                                                                     hasta)
                                                            .Where(v => v.Vehiculo != null && v.InicioReal.HasValue);

                var idsViajes = viajes.Select(v => v.Id);
                var dms = new List<DatamartViaje>();
                foreach (var ids in idsViajes.InSetsOf(1500))
                {
                    var datamarts = DAOFactory.DatamartViajeDAO.GetList(ids.ToArray());
                    if (datamarts != null && datamarts.Any())
                        dms.AddRange(datamarts);
                }

                var results = dms.Select(datamartViaje => new ReporteTrasladoVo(datamartViaje.Viaje, datamartViaje.KmTotales, datamartViaje.KmImproductivos, datamartViaje.KmProductivos, datamartViaje.KmProgramados)).ToList();

                ActualizarTotales(results);
                
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Reporte de traslados", String.Format("Duración de la consulta: {0} segundos", duracion));

                ShowTotales();

				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Reporte de traslados", e);
                throw;
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReporteTrasladoVo dataItem)
        {
            base.OnRowDataBound(grid, e, dataItem);
            e.Row.Attributes.Add("id", dataItem.Id.ToString("#0"));

            grid.Columns[ReporteTrasladoVo.IndexPorcDiferenciaKm].Visible = chkVerDesvios.Checked;
            grid.Columns[ReporteTrasladoVo.IndexPorcDiferenciaTiempo].Visible = chkVerDesvios.Checked;
            grid.Columns[ReporteTrasladoVo.IndexDiferenciaKm].Visible = chkVerDesvios.Checked;
            grid.Columns[ReporteTrasladoVo.IndexDiferenciaTiempo].Visible = chkVerDesvios.Checked;
            grid.Columns[ReporteTrasladoVo.IndexKmProductivo].Visible = chkVerDesvios.Checked;
            grid.Columns[ReporteTrasladoVo.IndexKmImproductivo].Visible = chkVerDesvios.Checked;
        }

        private void ActualizarTotales(List<ReporteTrasladoVo> reportes)
        {
            TotalRutas = reportes.Count;
            TotalKms = reportes.Sum(r => r.KmReales);
            TotalKmProductivos = reportes.Sum(r => r.KmProductivos);
            TotalKmImproductivos = reportes.Sum(r => r.KmImproductivos);
            TotalKmProgramados = reportes.Sum(r => r.KmProgramado);
            var ticks = reportes.Sum(r => r.TiempoReal.Ticks);
            TotalMinutos = new TimeSpan(ticks);
            var ticksProgramados = reportes.Sum(r => r.TiempoProgramado.Ticks);
            TotalMinutosProgramados = new TimeSpan(ticksProgramados);
        }

        private void ShowTotales()
        {
            PromedioKm = TotalRutas > 0 ? TotalKms / TotalRutas : 0.00;
            PromedioMinutos = TotalRutas > 0 ? new TimeSpan(0,0,(int)TotalMinutos.TotalSeconds/TotalRutas) : new TimeSpan();

            lblTotalRutas.Text = TotalRutas.ToString("#0");
            lblKmTotales.Text = TotalKms.ToString("#0.00");
            var totalHs = (TotalMinutos.Days * 24) + TotalMinutos.Hours;
            var totalMin = TotalMinutos.Minutes;
            lblTiempoTotal.Text = totalHs.ToString("00") + ":" + totalMin.ToString("00");
            lblPromedioKm.Text = PromedioKm.ToString("#0.00");
            var promHs = (PromedioMinutos.Days*24) + PromedioMinutos.Hours;
            var promMin = PromedioMinutos.Minutes;
            lblPromedioMin.Text = promHs.ToString("00") + ":" + promMin.ToString("00");
            lblKmProgramados.Text = TotalKmProgramados.ToString("#0.00");
            var progHs = (TotalMinutosProgramados.Days * 24) + TotalMinutosProgramados.Hours;
            var progMin = TotalMinutosProgramados.Minutes;
            lblMinProgramados.Text = progHs.ToString("00") + ":" + progMin.ToString("00");
            lblKmProductivos.Text = TotalKmProductivos.ToString("#0.00");
            lblKmImproductivos.Text = TotalKmImproductivos.ToString("#0.00");

            tbl_totales.Visible = true;
        }

        protected override void SelectedIndexChanged()
        {
            var id = Grid.SelectedRow.Attributes["id"];
            var idViaje = Convert.ToInt32(id);
            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(idViaje);
            if (viaje.InicioReal.HasValue) OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlDistribucion(Convert.ToInt32(id))), "_blank");
        }

        private bool GetDetoursCheck()
        {
            return true;
        }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        protected override string GetDescription(string reporte)
        {
            var linea = GetLinea();
            if (lbVehiculo.SelectedValues.Contains(0)) lbVehiculo.ToogleItems();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Reporte: {0} - ", reporte);
            sDescription.AppendFormat("Tipo de Vehiculo: {0} - ", lbVehiculo.SelectedItem.Text);

            return sDescription.ToString();
        }

        protected override List<int> GetVehicleList()
        {
            if (lbVehiculo.SelectedValues.Contains(0)) lbVehiculo.ToogleItems();
            return lbVehiculo.SelectedValues;
        }

        protected override DateTime GetSinceDateTime()
        {
            return dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override DateTime GetToDateTime()
        {
            return dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

    }
}
