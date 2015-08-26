using C1.Web.UI.Controls.C1GridView;
using InfoSoftGlobal;
using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class ResumenDeEntregas : SecuredGridReportPage<ResumenDeEntregasVo>
    {
        protected override string VariableName { get { return "REP_RESUMEN_ENTREGAS"; } }
        protected override string GetRefference() { return "REP_RESUMEN_ENTREGAS"; } 
        protected override bool ExcelButton { get { return true; } }
        private static GraphTypes GraphType { get { return GraphTypes.StackedColumn; } }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
        }

        protected override List<ResumenDeEntregasVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var results = new List<ResumenDeEntregasVo>();
                
                
                var viajes = DAOFactory.ViajeDistribucionDAO.GetList(new[] {ddlLocacion.Selected},
                                                                     new[] {ddlPlanta.Selected},
                                                                     lbTransportista.SelectedValues,
                                                                     lbDepartamento.SelectedValues,
                                                                     lbCentroDeCostos.SelectedValues,
                                                                     lbSubCentroDeCostos.SelectedValues,
                                                                     lbVehiculo.SelectedValues,
                                                                     SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()),
                                                                     SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()))
                                                            .Where(v => v.Vehiculo != null)
                                                            .Where(v => chkPendientes.Checked || v.Estado != ViajeDistribucion.Estados.Pendiente)
                                                            .OrderBy(v => v.Vehiculo);

                var completados = 0;
                var noCompletados = 0;
                var visitados = 0;
                var noVisitados = 0;
                var enSitio = 0;
                var enZona = 0;
                var totalCompletados = 0;
                var totalNoCompletados = 0;
                var totalVisitados = 0;
                var totalNoVisitados = 0;
                var totalEnSitio = 0;
                var totalEnZona = 0;
                var rutas = 0;
                Coche vehiculo = null;
                ResumenDeEntregasVo resumenEntrega;

                foreach (var viajeDistribucion in viajes)
                {
                    if (vehiculo == null) vehiculo = viajeDistribucion.Vehiculo;
                    var cocheActual = viajeDistribucion.Vehiculo;

                    if (vehiculo != cocheActual)
                    {
                        resumenEntrega = new ResumenDeEntregasVo(vehiculo, completados, noCompletados, visitados, noVisitados, enSitio, enZona, rutas);
                        results.Add(resumenEntrega);
                        completados = noCompletados = visitados = noVisitados = 
                        enSitio = enZona = rutas = 0;
                        vehiculo = cocheActual;
                    }

                    var detalles = viajeDistribucion.Detalles.Where(d => d.Linea == null);
                    rutas++;
                    foreach (var entrega in detalles)
                    {
                        switch (entrega.Estado)
                        {
                            case EntregaDistribucion.Estados.Completado: completados++; totalCompletados++; break;
                            case EntregaDistribucion.Estados.NoCompletado: noCompletados++; totalNoCompletados++; break;
                            case EntregaDistribucion.Estados.Visitado: visitados++; totalVisitados++; break;
                            case EntregaDistribucion.Estados.EnSitio: enSitio++; totalEnSitio++; break;
                            case EntregaDistribucion.Estados.EnZona: enZona++; totalEnZona++; break;
                            default: noVisitados++; totalNoVisitados++; break;
                        }
                    }
                }

                if (vehiculo != null)
                {
                    resumenEntrega = new ResumenDeEntregasVo(vehiculo, completados, noCompletados, visitados, noVisitados, enSitio, enZona, rutas);
                    results.Add(resumenEntrega);
                }

                if (results.Count > 0)
                {
                    divChart.Visible = true;
                    divChart.InnerHtml = FusionCharts.RenderChartHTML(ChartXmlDefinition, "", GetGraphXml(results), "Report", "1050", "500", false);
                }

                ShowLabels(totalCompletados, totalNoCompletados, totalVisitados, totalNoVisitados, totalEnSitio, totalEnZona);

                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");
                STrace.Trace("Resumen de entregas", String.Format("Duración de la consulta: {0} segundos", duracion));
                return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Resumen de entregas", e, String.Format("Reporte: Resumen de entregas. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));
                throw;
            }
        }

        private void ShowLabels(int totalCompletados, int totalNoCompletados, int totalVisitados, int totalNoVisitados, int totalEnSitio, int totalEnZona)
        {
            var totalRealizados = totalCompletados + totalVisitados + totalEnSitio + totalEnZona;
            var total = totalRealizados + totalNoCompletados + totalNoVisitados;
            var porcCompletados = (total != 0 ?(double)totalCompletados / (double)total * 100 : 0);
            var porcNoCompletados = (total !=0 ? (double)totalNoCompletados / (double)total * 100 : 0);
            var porcVisitados = (total !=0 ? (double)totalVisitados / (double)total * 100 : 0);
            var porcNoVisitados = (total !=0 ? (double)totalNoVisitados / (double)total * 100 : 0);
            var porcRealizados = (total !=0 ? (double)totalRealizados / (double)total * 100 : 0);
            var porcEnSitio = (total != 0 ? (double)totalEnSitio / (double)total * 100 : 0);
            var porcEnZona = (total != 0 ? (double)totalEnZona / (double)total * 100 : 0);

            lblTotal.Text = total.ToString("#0");
            lblCantRealizadas.Text = totalRealizados.ToString("#0");
            lblPorcRealizadas.Text = porcRealizados.ToString("#0.00");
            lblCantCompletadas.Text = totalCompletados.ToString("#0");
            lblPorcCompletadas.Text = porcCompletados.ToString("#0.00");
            lblCantNoCompletadas.Text = totalNoCompletados.ToString("#0");
            lblPorcNoCompletadas.Text = porcNoCompletados.ToString("#0.00");
            lblCantVisitadas.Text = totalVisitados.ToString("#0");
            lblPorcVisitadas.Text = porcVisitados.ToString("#0.00");
            lblCantNoVisitadas.Text = totalNoVisitados.ToString("#0");
            lblPorcNoVisitadas.Text = porcNoVisitados.ToString("#0.00");
            lblCantEnSitio.Text = totalEnSitio.ToString("#0");
            lblPorcEnSitio.Text = porcEnSitio.ToString("#0.00");
            lblCantEnZona.Text = totalEnZona.ToString("#0");
            lblPorcEnZona.Text = porcEnZona.ToString("#0.00");

            tbl_totales.Visible = true;
        }

        private static string GetGraphXml(IEnumerable<ResumenDeEntregasVo> entregas)
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                helper.AddConfigEntry("xAxisName", CultureManager.GetLabel("INTERNO"));
                helper.AddConfigEntry("yAxisName", CultureManager.GetLabel("CANTIDAD"));
                helper.AddConfigEntry("decimalPrecision", "0");
                helper.AddConfigEntry("showValues", "0");
                helper.AddConfigEntry("showNames", "1");
                helper.AddConfigEntry("hoverCapSepChar", " - ");

                var completados = new FusionChartsDataset();
                completados.SetPropertyValue("color", "#008000");
                completados.SetPropertyValue("seriesName", CultureManager.GetLabel("COMPLETADOS"));
                var visitados = new FusionChartsDataset();
                visitados.SetPropertyValue("color", "#FFFF00");
                visitados.SetPropertyValue("seriesName", CultureManager.GetLabel("VISITADOS"));
                var enSitio = new FusionChartsDataset();
                enSitio.SetPropertyValue("color", "#00A2E8");
                enSitio.SetPropertyValue("seriesName", CultureManager.GetLabel("EN_SITIO"));
                var enZona = new FusionChartsDataset();
                enZona.SetPropertyValue("color", "#808080");
                enZona.SetPropertyValue("seriesName", CultureManager.GetLabel("EN_ZONA"));
                var noCompletados = new FusionChartsDataset();
                noCompletados.SetPropertyValue("color", "#FF0000");
                noCompletados.SetPropertyValue("seriesName", CultureManager.GetLabel("NO_COMPLETADOS"));
                var noVisitados = new FusionChartsDataset();
                noVisitados.SetPropertyValue("color", "#FF4500");
                noVisitados.SetPropertyValue("seriesName", CultureManager.GetLabel("NO_VISITADOS"));
                
                foreach (var entrega in entregas)
                {
                    helper.AddCategory(entrega.Vehiculo.Replace('&', 'y'));
                    completados.addValue(entrega.Completados.ToString("#0"));
                    visitados.addValue(entrega.Visitados.ToString("#0"));
                    enSitio.addValue(entrega.EnSitio.ToString("#0"));
                    enZona.addValue(entrega.EnZona.ToString("#0"));
                    noCompletados.addValue(entrega.NoCompletados.ToString("#0"));
                    noVisitados.addValue(entrega.NoVisitados.ToString("#0"));
                }

                helper.AddDataSet(completados);
                helper.AddDataSet(visitados);
                helper.AddDataSet(enSitio);
                helper.AddDataSet(enZona);
                helper.AddDataSet(noCompletados);
                helper.AddDataSet(noVisitados);

                return helper.BuildXml();
            }
        }

        private static string ChartXmlDefinition
        {
            get
            {
                var template = String.Empty;

                switch (GraphType)
                {
                    case GraphTypes.Barrs:
                        template = "FCF_Column3D.swf";
                        break;
                    case GraphTypes.Lines:
                        template = "FCF_Line.swf";
                        break;
                    case GraphTypes.MultiLine:
                        template = "FCF_MSLine.swf";
                        break;
                    case GraphTypes.MultiColumn:
                        template = "FCF_MSColumn2D.swf";
                        break;
                    case GraphTypes.StackedColumn:
                        template = "FCF_StackedColumn3D.swf";
                        break;
                }

                return String.Concat(FusionChartDir, template);
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ResumenDeEntregasVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");
            e.Row.Style.Add("cursor", "default");

            e.Row.Cells[ResumenDeEntregasVo.IndexCompletados].BackColor = System.Drawing.Color.Green;
            e.Row.Cells[ResumenDeEntregasVo.IndexPorcCompletados].BackColor = System.Drawing.Color.Green;
            e.Row.Cells[ResumenDeEntregasVo.IndexNoCompletados].BackColor = System.Drawing.Color.Red;
            e.Row.Cells[ResumenDeEntregasVo.IndexPorcNoCompletados].BackColor = System.Drawing.Color.Red;
            e.Row.Cells[ResumenDeEntregasVo.IndexVisitados].BackColor = System.Drawing.Color.Yellow;
            e.Row.Cells[ResumenDeEntregasVo.IndexPorcVisitados].BackColor = System.Drawing.Color.Yellow;
            e.Row.Cells[ResumenDeEntregasVo.IndexNoVisitados].BackColor = System.Drawing.Color.OrangeRed;
            e.Row.Cells[ResumenDeEntregasVo.IndexPorcNoVisitados].BackColor = System.Drawing.Color.OrangeRed;
            e.Row.Cells[ResumenDeEntregasVo.IndexEnSitio].BackColor = System.Drawing.Color.CornflowerBlue;
            e.Row.Cells[ResumenDeEntregasVo.IndexPorcEnSitio].BackColor = System.Drawing.Color.CornflowerBlue;
            e.Row.Cells[ResumenDeEntregasVo.IndexEnZona].BackColor = System.Drawing.Color.Gray;
            e.Row.Cells[ResumenDeEntregasVo.IndexPorcEnZona].BackColor = System.Drawing.Color.Gray;
        }
    }
}