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

namespace Logictracker.Web.Reportes.CicloLogistico
{
    public partial class ReporteEstadosLogisticos : SecuredBaseReportPage<EstadoLogistico>
    {
        protected override string GetRefference() { return "REP_ESTADOS_LOGISTICOS"; }
        protected override string VariableName { get { return "REP_ESTADOS_LOGISTICOS"; } }
        protected override InfoLabel LblInfo { get { return null; } }
        protected override bool CsvButton { get { return false; } }
        protected override bool ExcelButton { get { return false; } }
        protected override bool PrintButton { get { return false; } }
        protected override bool HideSearch { get { return true; } }
        protected override void ExportToCsv() { }
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

            gridViajes.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI02");
            gridViajes.Columns[1].HeaderText = CultureManager.GetLabel("FECHA");
            gridViajes.Columns[2].HeaderText = CultureManager.GetEntity("OPETICK03");
            gridViajes.Columns[3].HeaderText = CultureManager.GetEntity("PARENTI03");
            gridViajes.Columns[4].HeaderText = CultureManager.GetEntity("PARENTI07");

            while (gridViajes.Columns.Count > 5)
                gridViajes.Columns.RemoveAt(5);

            foreach(var estado in tipoCiclo.Estados)
            {
                var templateField = new C1TemplateField();
                templateField.HeaderText = CultureManager.GetLabel("INICIO") + " " + estado.Descripcion;
                gridViajes.Columns.Add(templateField);

                templateField = new C1TemplateField();
                templateField.HeaderText = CultureManager.GetLabel("FIN") + " " + estado.Descripcion;
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
                var viaje = e.Row.DataItem as ViajeDistribucion;
                if (viaje != null)
                {
                    var lbl = e.Row.FindControl("lblBase") as Label;
                    if (lbl != null) lbl.Text = viaje.Linea.Descripcion;

                    lbl = e.Row.FindControl("lblFecha") as Label;
                    if (lbl != null) lbl.Text = viaje.Inicio.ToString("dd/MM/yyyy");

                    lbl = e.Row.FindControl("lblViaje") as Label;
                    if (lbl != null) lbl.Text = viaje.Codigo;

                    lbl = e.Row.FindControl("lblVehiculo") as Label;
                    if (lbl != null) lbl.Text = viaje.Vehiculo != null ? viaje.Vehiculo.Interno : string.Empty;

                    lbl = e.Row.FindControl("lblTransportista") as Label;
                    if (lbl != null) lbl.Text = viaje.Vehiculo != null && viaje.Vehiculo.Transportista != null ? viaje.Vehiculo.Transportista.Descripcion : string.Empty;

                    var index = 5;
                    foreach (var estado in viaje.TipoCicloLogistico.Estados)
                    {
                        var estadoDistribucion = viaje.EstadosCumplidos.Where(ec => ec.EstadoLogistico.Id == estado.Id).FirstOrDefault();
                        e.Row.Cells[index].Text = estadoDistribucion != null && estadoDistribucion.Inicio.HasValue 
                                                    ? estadoDistribucion.Inicio.Value.ToDisplayDateTime().ToString("HH:mm")
                                                    : "-";
                        index++;
                        e.Row.Cells[index].Text = estadoDistribucion != null && estadoDistribucion.Fin.HasValue 
                                                    ? estadoDistribucion.Fin.Value.ToDisplayDateTime().ToString("HH:mm")
                                                    : "-";
                        index++;
                        var duracion = estadoDistribucion != null && estadoDistribucion.Inicio.HasValue && estadoDistribucion.Fin.HasValue
                                                    ? estadoDistribucion.Fin.Value.Subtract(estadoDistribucion.Inicio.Value)
                                                    : new TimeSpan();
                        e.Row.Cells[index].Text = duracion.TotalSeconds > 0
                                                    ? duracion.Hours.ToString("00") + ":" + duracion.Minutes.ToString("00") + ":" + duracion.Seconds.ToString("00")
                                                    : "-";
                        index++;
                    }

                    var min = viaje.EstadosCumplidos.Where(ec => ec.Inicio.HasValue).Min(ec => ec.Inicio);
                    var max = viaje.EstadosCumplidos.Where(ec => ec.Fin.HasValue).Max(ec => ec.Fin);
                    var total = min.HasValue && max.HasValue
                                ? max.Value.Subtract(min.Value)
                                : new TimeSpan();
                    e.Row.Cells[index].Text = total.TotalSeconds > 0
                                                ? total.Hours.ToString("00") + ":" + total.Minutes.ToString("00") + ":" + total.Seconds.ToString("00")
                                                : "-";
                    index++;
                    e.Row.Cells[index].Text = CultureManager.GetLabel(ViajeDistribucion.Estados.GetLabelVariableName(viaje.Estado));
                }
            }
        }
    }
}
