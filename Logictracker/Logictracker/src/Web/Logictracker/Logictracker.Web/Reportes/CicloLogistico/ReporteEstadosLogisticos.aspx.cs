using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1Gauge;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.BaseClasses.Util;
using System.Drawing;

namespace Logictracker.Web.Reportes.CicloLogistico
{
    public partial class ReporteEstadosLogisticos : SecuredBaseReportPage<EstadoLogistico>
    {
        protected override string GetRefference() { return "REP_ESTADOS_LOGISTICOS"; }
        protected override string VariableName { get { return "REP_ESTADOS_LOGISTICOS"; } }
        protected override InfoLabel LblInfo { get { return null; } }
        protected override bool CsvButton { get { return true; } }
        protected override bool ExcelButton { get { return false; } }
        protected override bool PrintButton { get { return false; } }
        protected override bool HideSearch { get { return true; } }
        public GridUtils<EstadoLogistico> GridUtils { get; set; }
        protected override void ExportToCsv() 
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            BtnSearchClick(null, null);

            builder.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());
            builder.GenerateColumns(gridViajes);
            builder.GenerateFields(gridViajes);

            SetCsvSessionVars(builder.Build());

            OpenWin(String.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }
        protected void SetCsvSessionVars(string csv)
        {
            Session["CSV_EXPORT"] = csv;
            Session["CSV_FILE_NAME"] = CultureManager.GetMenu(Module.Name);
        }
        protected override void ExportToExcel() { }
        protected override List<EstadoLogistico> GetResults() { return new List<EstadoLogistico>(); }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RegisterExtJsStyleSheet();
            dtDesde.SetDate();
            dtHasta.SetDate();
            gridViajes.Visible = false;
        }

        protected void FiltersSelectedIndexChanged(object sender, EventArgs e)
        {
            gridViajes.Visible = false;
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            gridViajes.Visible = true;
            var desde = dtDesde.SelectedDate.Value.ToDataBaseDateTime();
            var hasta = dtHasta.SelectedDate.Value.ToDataBaseDateTime();

            var viajes = DAOFactory.ViajeDistribucionDAO.GetList(cbEmpresa.SelectedValues,
                                                                 cbPlanta.SelectedValues,
                                                                 new[] { -1 },
                                                                 new[] { -1 },
                                                                 new[] { -1 },
                                                                 new[] { -1 },
                                                                 new[] { -1 },
                                                                 desde,
                                                                 hasta)
                                                        .Where(v => v.TipoCicloLogistico != null && v.TipoCicloLogistico.Id == cbTipoCiclo.Selected);
            
            var tipoCiclo = DAOFactory.TipoCicloLogisticoDAO.FindById(cbTipoCiclo.Selected);

            gridViajes.Columns.Clear();

            var templateField = new C1TemplateField();
            templateField.HeaderText = CultureManager.GetEntity("PARENTI02");
            gridViajes.Columns.Add(templateField);
            templateField = new C1TemplateField();
            templateField.HeaderText = CultureManager.GetLabel("FECHA");
            gridViajes.Columns.Add(templateField);
            templateField = new C1TemplateField();
            templateField.HeaderText = CultureManager.GetEntity("OPETICK03");
            gridViajes.Columns.Add(templateField);
            templateField = new C1TemplateField();
            templateField.HeaderText = CultureManager.GetEntity("PARENTI03");
            gridViajes.Columns.Add(templateField);
            templateField = new C1TemplateField();
            templateField.HeaderText = CultureManager.GetEntity("PARENTI07");
            gridViajes.Columns.Add(templateField);            

            foreach(var estado in tipoCiclo.Estados)
            {
                templateField = new C1TemplateField();
                templateField.HeaderText = (estado.Iterativo ? CultureManager.GetLabel("CANTIDAD") : CultureManager.GetLabel("INICIO")) + " " + estado.Descripcion;
                gridViajes.Columns.Add(templateField);

                templateField = new C1TemplateField();
                templateField.HeaderText = (estado.Iterativo ? CultureManager.GetLabel("PROMEDIO") : CultureManager.GetLabel("FIN")) + " " + estado.Descripcion;
                gridViajes.Columns.Add(templateField);

                templateField = new C1TemplateField();
                templateField.HeaderText = CultureManager.GetLabel("DURACION") + " " + estado.Descripcion;
                gridViajes.Columns.Add(templateField);             
            }

            var template = new C1TemplateField();
            template.HeaderText = CultureManager.GetLabel("TOTAL");
            gridViajes.Columns.Add(template);

            template = new C1TemplateField();
            template.HeaderText = CultureManager.GetLabel("ESTADO");
            gridViajes.Columns.Add(template);

            gridViajes.DataSource = viajes;
            gridViajes.DataBind();
        }

        protected void GridViajesOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var index = 0;
                var viaje = e.Row.DataItem as ViajeDistribucion;
                if (viaje != null)
                {
                    e.Row.Cells[index].Text = viaje.Linea.Descripcion;
                    index++;
                    e.Row.Cells[index].Text = viaje.Inicio.ToString("dd/MM/yyyy");
                    e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                    index++;
                    e.Row.Cells[index].Text = viaje.Codigo;
                    index++;
                    e.Row.Cells[index].Text = viaje.Vehiculo != null ? viaje.Vehiculo.Interno : string.Empty;
                    index++;
                    e.Row.Cells[index].Text = viaje.Vehiculo != null && viaje.Vehiculo.Transportista != null ? viaje.Vehiculo.Transportista.Descripcion : string.Empty;
                    index++;

                    foreach (var estado in viaje.TipoCicloLogistico.Estados)
                    {
                        if (estado.Iterativo)
                        {
                            var estadosDistribucion = viaje.EstadosCumplidos.Where(ec => ec.EstadoLogistico.Id == estado.Id);
                            var cantidad = estadosDistribucion.Count();
                            var tTotal = (int)estadosDistribucion.Where(ed => ed.Inicio.HasValue && ed.Fin.HasValue).Sum(ed => ed.Fin.Value.Subtract(ed.Inicio.Value).TotalSeconds);
                            var tiempoTotal = new TimeSpan(0, 0, tTotal);
                            var prom = tTotal > 0 && cantidad > 0 ? tTotal / cantidad : 0;
                            var promedio = new TimeSpan(0, 0, prom);

                            e.Row.Cells[index].Text = cantidad.ToString("#0");
                            e.Row.Cells[index].BackColor = Color.Orange;
                            e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                            index++; 
                            e.Row.Cells[index].Text = promedio.TotalSeconds > 0
                                                        ? promedio.Hours.ToString("00") + ":" + promedio.Minutes.ToString("00") + ":" + promedio.Seconds.ToString("00")
                                                        : "00:00:00";
                            e.Row.Cells[index].BackColor = Color.Orange;
                            e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                            index++;                            
                            e.Row.Cells[index].Text = tiempoTotal.TotalSeconds > 0
                                                        ? tiempoTotal.Hours.ToString("00") + ":" + tiempoTotal.Minutes.ToString("00") + ":" + tiempoTotal.Seconds.ToString("00")
                                                        : "00:00:00";
                            e.Row.Cells[index].BackColor = Color.Green;
                            e.Row.Cells[index].ForeColor = Color.White;
                            e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                            index++;
                        }
                        else
                        {
                            var estadoDistribucion = viaje.EstadosCumplidos.Where(ec => ec.EstadoLogistico.Id == estado.Id).FirstOrDefault();
                            e.Row.Cells[index].Text = estadoDistribucion != null && estadoDistribucion.Inicio.HasValue
                                                        ? estadoDistribucion.Inicio.Value.ToDisplayDateTime().ToString("HH:mm")
                                                        : "-";
                            e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                            index++;
                            e.Row.Cells[index].Text = estadoDistribucion != null && estadoDistribucion.Fin.HasValue
                                                        ? estadoDistribucion.Fin.Value.ToDisplayDateTime().ToString("HH:mm")
                                                        : "-";
                            e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                            index++;
                            var duracion = estadoDistribucion != null && estadoDistribucion.Inicio.HasValue && estadoDistribucion.Fin.HasValue
                                                        ? estadoDistribucion.Fin.Value.Subtract(estadoDistribucion.Inicio.Value)
                                                        : new TimeSpan();
                            e.Row.Cells[index].Text = duracion.TotalSeconds > 0
                                                        ? duracion.Hours.ToString("00") + ":" + duracion.Minutes.ToString("00") + ":" + duracion.Seconds.ToString("00")
                                                        : "-";
                            e.Row.Cells[index].BackColor = Color.Green;
                            e.Row.Cells[index].ForeColor = Color.White;
                            e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                            index++;
                        }
                    }

                    var min = viaje.EstadosCumplidos.Where(ec => ec.Inicio.HasValue).Min(ec => ec.Inicio);
                    var max = viaje.EstadosCumplidos.Where(ec => ec.Fin.HasValue).Max(ec => ec.Fin);
                    var total = min.HasValue && max.HasValue
                                ? max.Value.Subtract(min.Value)
                                : new TimeSpan();
                    e.Row.Cells[index].Text = total.TotalSeconds > 0
                                                ? total.Hours.ToString("00") + ":" + total.Minutes.ToString("00") + ":" + total.Seconds.ToString("00")
                                                : "-";
                    e.Row.Cells[index].HorizontalAlign = HorizontalAlign.Center;
                    index++;
                    e.Row.Cells[index].Text = CultureManager.GetLabel(ViajeDistribucion.Estados.GetLabelVariableName(viaje.Estado));
                }
            }
        }
    }
}
