using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1Gauge;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Web.Reportes.CicloLogistico
{
    public partial class KPIEstadoEntregas : SecuredBaseReportPage<KPIEstadoEntregas.KpiEstado>
    {
        protected override string GetRefference() { return "KPI_ESTADO_ENTREGAS"; }
        protected override string VariableName { get { return "KPI_ESTADO_ENTREGAS"; } }
        protected override InfoLabel LblInfo { get { return null; } }
        protected override bool CsvButton { get { return false; } }
        protected override bool ExcelButton { get { return false; } }
        protected override bool PrintButton { get { return false; } }
        protected override bool HideSearch { get { return true; } }

        protected override void ExportToCsv() { }
        protected override void ExportToExcel() { }
        protected override List<KpiEstado> GetResults() { return new List<KpiEstado>(); }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            CalcularEstadisticas();
        }

        private void CalcularEstadisticas()
        {
            var transportistas = DAOFactory.TransportistaDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected });

            var resultados = new List<KpiEstado>();

            var buscados = DAOFactory.ViajeDistribucionDAO.GetList(new[] { ddlEmpresa.Selected },
                                                                   new[] { ddlPlanta.Selected },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   DateTime.Today.ToDataBaseDateTime(),
                                                                   DateTime.Today.AddDays(1).ToDataBaseDateTime())
                                                          .Where(v => v.Vehiculo != null && v.Vehiculo.Transportista != null);

            foreach (var transportista in transportistas)
            {
                var viajes = buscados.Where(v => v.Vehiculo.Transportista.Id == transportista.Id);

                if (!viajes.Any()) continue;

                var completados = viajes.Sum(v => v.EntregasCompletadosCount);
                var visitados = viajes.Sum(v => v.EntregasVisitadosCount);
                var realizados = completados + visitados;
                var total = viajes.Sum(v => v.EntregasTotalCount);
                var porc = total > 0 && realizados > 0 ? (double) realizados/(double) total*100 : 0.00;

                var result = new KpiEstado
                {
                    Transportista = transportista.Descripcion,
                    Rutas = viajes.Count(),
                    Entregas = total,
                    Realizados = realizados,
                    Porc = porc,
                    Completados = completados,
                    Visitados = visitados,
                    EnSitio = viajes.Sum(v => v.EntregasEnSitioCount),
                    EnZona = viajes.Sum(v => v.EntregasEnZonaCount),
                    NoCompletados = viajes.Sum(v => v.EntregasNoCompletadosCount),
                    NoVisitados = viajes.Sum(v => v.EntregasNoVisitadosCount),
                    Pendientes = viajes.Sum(v => v.EntregasPendientesCount)
                };

                resultados.Add(result);
            }

            gridTransportistas.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI07");
            gridTransportistas.Columns[1].HeaderText = CultureManager.GetLabel("RUTAS");
            gridTransportistas.Columns[2].HeaderText = CultureManager.GetLabel("ENTREGAS");
            gridTransportistas.Columns[3].HeaderText = CultureManager.GetLabel("REALIZADOS");
            gridTransportistas.Columns[4].HeaderText = CultureManager.GetLabel("PORCENTAJE");
            gridTransportistas.Columns[5].HeaderText = CultureManager.GetLabel("COMPLETADOS");
            gridTransportistas.Columns[6].HeaderText = CultureManager.GetLabel("VISITADOS");
            gridTransportistas.Columns[7].HeaderText = CultureManager.GetLabel("EN_SITIO");
            gridTransportistas.Columns[8].HeaderText = CultureManager.GetLabel("EN_ZONA");
            gridTransportistas.Columns[9].HeaderText = CultureManager.GetLabel("NO_COMPLETADOS");
            gridTransportistas.Columns[10].HeaderText = CultureManager.GetLabel("NO_VISITADOS");
            gridTransportistas.Columns[11].HeaderText = CultureManager.GetLabel("PENDIENTES");

            for (var i = 0; i < gridTransportistas.Columns.Count; i++)
            {
                gridTransportistas.Columns[i].HeaderStyle.Font.Bold = true;
                gridTransportistas.Columns[i].HeaderStyle.Font.Size = FontUnit.Larger;
                gridTransportistas.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }

            gridTransportistas.DataSource = resultados;
            gridTransportistas.DataBind();
        }

        protected void GridTransportistasOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var result = e.Row.DataItem as KpiEstado;
                if (result != null)
                {
                    var lbl = e.Row.FindControl("lblTransportista") as Label;
                    if (lbl != null) lbl.Text = result.Transportista;

                    lbl = e.Row.FindControl("lblRutas") as Label;
                    if (lbl != null) lbl.Text = result.Rutas.ToString("#0");

                    lbl = e.Row.FindControl("lblEntregas") as Label;
                    if (lbl != null) lbl.Text = result.Entregas.ToString("#0");

                    lbl = e.Row.FindControl("lblRealizados") as Label;
                    if (lbl != null) lbl.Text = result.Porc.ToString("#0") + " % (" + result.Realizados.ToString("#0") + ")";

                    var gauge = e.Row.FindControl("gaugeCompletados") as C1Gauge;
                    if (gauge != null)
                    {
                        gauge.Gauges[0].Maximum = 100;
                        gauge.Gauges[0].Value = result.Porc;
                    }

                    lbl = e.Row.FindControl("lblCompletados") as Label;
                    if (lbl != null) lbl.Text = result.Completados.ToString("#0");

                    lbl = e.Row.FindControl("lblVisitados") as Label;
                    if (lbl != null) lbl.Text = result.Visitados.ToString("#0");

                    lbl = e.Row.FindControl("lblEnSitio") as Label;
                    if (lbl != null) lbl.Text = result.EnSitio.ToString("#0");

                    lbl = e.Row.FindControl("lblEnZona") as Label;
                    if (lbl != null) lbl.Text = result.EnZona.ToString("#0");

                    lbl = e.Row.FindControl("lblNoCompletados") as Label;
                    if (lbl != null) lbl.Text = result.NoCompletados.ToString("#0");

                    lbl = e.Row.FindControl("lblNoVisitados") as Label;
                    if (lbl != null) lbl.Text = result.NoVisitados.ToString("#0");

                    lbl = e.Row.FindControl("lblPendientes") as Label;
                    if (lbl != null) lbl.Text = result.Pendientes.ToString("#0");

                    e.Row.Font.Bold = true;
                    e.Row.Font.Size = FontUnit.Larger;
                    e.Row.HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }

        public class KpiEstado
        {
            public string Transportista { get; set; }
            public int Rutas { get; set; }
            public int Entregas { get; set; }
            public int Realizados { get; set; }
            public int Completados { get; set; }
            public int Visitados { get; set; }
            public int EnSitio { get; set; }
            public int EnZona { get; set; }
            public int NoCompletados { get; set; }
            public int NoVisitados { get; set; }
            public int Pendientes { get; set; }
            public double Porc { get; set; }
        }
    }
}
