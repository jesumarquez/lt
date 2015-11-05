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

            var itemsReporte = viajes.Select(v => new ReporteEstado(v));

            var tipoCiclo = DAOFactory.TipoCicloLogisticoDAO.FindById(cbTipoCiclo.Selected);

            gridViajes.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI02");
            gridViajes.Columns[1].HeaderText = CultureManager.GetLabel("FECHA");
            gridViajes.Columns[2].HeaderText = CultureManager.GetEntity("OPETICK03");
            gridViajes.Columns[3].HeaderText = CultureManager.GetEntity("PARENTI03");
            gridViajes.Columns[4].HeaderText = CultureManager.GetEntity("PARENTI07");

            var index = 5;
            foreach(var estado in tipoCiclo.Estados)
            {
                gridViajes.Columns[index].HeaderText = estado.Descripcion;
                index++;
            }

            gridViajes.DataSource = itemsReporte;
            gridViajes.DataBind();
        }

        protected void GridViajesOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var result = e.Row.DataItem as ReporteEstado;
                if (result != null)
                {
                    var lbl = e.Row.FindControl("lblBase") as Label;
                    if (lbl != null) lbl.Text = result.Base;

                    lbl = e.Row.FindControl("lblFecha") as Label;
                    if (lbl != null) lbl.Text = result.Fecha;

                    lbl = e.Row.FindControl("lblViaje") as Label;
                    if (lbl != null) lbl.Text = result.Viaje;

                    lbl = e.Row.FindControl("lblVehiculo") as Label;
                    if (lbl != null) lbl.Text = result.Vehiculo;

                    lbl = e.Row.FindControl("lblTransportista") as Label;
                    if (lbl != null) lbl.Text = result.Transportista; 
                }
            }
        }
        
        private class ReporteEstado
        {
            public ReporteEstado(ViajeDistribucion viaje)
            {
                Base = viaje.Linea.Descripcion;
                Fecha = viaje.Inicio.ToString("dd/MM/yyyy");
                Viaje = viaje.Codigo;
                Vehiculo = viaje.Vehiculo != null ? viaje.Vehiculo.Interno : string.Empty;
                Transportista = viaje.Vehiculo != null && viaje.Vehiculo.Transportista != null ? viaje.Vehiculo.Transportista.Descripcion : string.Empty;
            }

            public string Base { get; set; }
            public string Fecha { get; set; }
            public string Viaje { get; set; }
            public string Vehiculo { get; set; }
            public string Transportista { get; set; }
        }
    }
}
