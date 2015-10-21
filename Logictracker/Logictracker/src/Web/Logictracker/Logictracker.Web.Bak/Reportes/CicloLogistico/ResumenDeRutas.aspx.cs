using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Security;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class ResumenDeRutas : SecuredGridReportPage<ResumenDeRutasVo>
    {
        protected override string VariableName { get { return "REP_RESUMEN_RUTAS"; } }
        protected override string GetRefference() { return "REP_RESUMEN_RUTAS"; }
        public override int PageSize { get { return 500; } }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }

        public int TotalRutas { get; set; }
        public int TotalVehiculos { get; set; }
        public int RutasConVehiculo { get; set; }
        public int RutasSinVehiculo { get; set; }
        public int RutasIniciadas { get; set; }
        public int RutasSinIniciar { get; set; }
        public int RutasFinalizadas { get; set; }
        public int RutasEnCurso { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
        }

        protected override List<ResumenDeRutasVo> GetResults()
        {
            var results = new List<ResumenDeRutasVo>();
            var viajes = new List<ViajeDistribucion>();
            tbl_totales.Visible = false;
            
            if (lbVehiculo.Items.Count > 0)
            {
                if (QueryExtensions.IncludesAll(lbVehiculo.SelectedValues))
                    lbVehiculo.ToogleItems();

                viajes = DAOFactory.ViajeDistribucionDAO.GetList(new[] {ddlLocacion.Selected},
                                                                 new[] {ddlPlanta.Selected},
                                                                 new[] {-1},//lbTransportista.SelectedValues,}
                                                                 new[] { -1 },//lbDepartamento.SelectedValues,
                                                                 new[] { -1 },//lbCentroDeCostos.SelectedValues,
                                                                 new[] { -1 },//lbSubCentroDeCostos.SelectedValues,
                                                                 lbVehiculo.SelectedValues,
                                                                 dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(),
                                                                 dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime());

                if (txtRuta.Text.Trim() != string.Empty)
                    viajes = viajes.Where(v => v.Codigo.Contains(txtRuta.Text.Trim())).ToList();

                results = viajes.Select(v => new ResumenDeRutasVo(v,chkVerKm.Checked)).ToList();
            }
            
            ShowTotales(viajes);
            return results;
        }

        private void ShowTotales(IEnumerable<ViajeDistribucion> viajes)
        {
            tbl_totales.Visible = true;

            TotalRutas = viajes.Count();
            TotalVehiculos = viajes.Where(v => v.Vehiculo != null).Select(v => v.Vehiculo).Distinct().Count();
            RutasConVehiculo = viajes.Count(v => v.Vehiculo != null);
            RutasSinVehiculo = viajes.Count(v => v.Vehiculo == null);
            RutasIniciadas = viajes.Count(v => v.InicioReal.HasValue);
            RutasSinIniciar = viajes.Count(v => !v.InicioReal.HasValue);
            RutasFinalizadas = viajes.Count(v => v.Estado == ViajeDistribucion.Estados.Cerrado);
            RutasEnCurso = viajes.Count(v => v.Estado == ViajeDistribucion.Estados.EnCurso);
            
            lblTotalRutas.Text = TotalRutas.ToString("#0");
            lblTotalVehiculos.Text = TotalVehiculos.ToString("#0");
            lblRutasConVehiculo.Text = RutasConVehiculo.ToString("#0");
            lblRutasSinVehiculo.Text = RutasSinVehiculo.ToString("#0");
            lblRutasIniciadas.Text = RutasIniciadas.ToString("#0");
            lblRutasSinIniciar.Text = RutasSinIniciar.ToString("#0");
            lblRutasFinalizadas.Text = RutasFinalizadas.ToString("#0");
            lblRutasEnCurso.Text = RutasEnCurso.ToString("#0");
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {"Rutas_Totales", TotalRutas.ToString("#0")},
                           {"Vehiculos_Totales", TotalVehiculos.ToString("#0")},
                           {"Con_Vehiculo", RutasConVehiculo.ToString("#0")},
                           {"Sin_Vehiculo", RutasSinVehiculo.ToString("#0")},
                           {"Iniciadas", RutasIniciadas.ToString("#0")},
                           {"Sin_Iniciar", RutasSinIniciar.ToString("#0")},
                           {"Finalizadas", RutasFinalizadas.ToString("#0")},
                           {"En_Curso", RutasEnCurso.ToString("#0")}
                       };
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ResumenDeRutasVo dataItem)
        {
            base.OnRowDataBound(grid, e, dataItem);
            e.Row.Attributes.Add("id", dataItem.Id.ToString("#0"));

            grid.Columns[ResumenDeRutasVo.IndexRecepcion].Visible = chkVerRecepcion.Checked;

            if (chkVerRecepcion.Checked)
            {
                var inicio = dataItem.Inicio;
                var recepcion = dataItem.Recepcion;

                if (inicio.HasValue && recepcion.HasValue)
                {
                    var dif = recepcion.Value.Subtract(inicio.Value).TotalMinutes;

                    if (dif <= 5) e.Row.Cells[ResumenDeRutasVo.IndexRecepcion].BackColor = Color.YellowGreen;
                    else if (dif <= 30) e.Row.Cells[ResumenDeRutasVo.IndexRecepcion].BackColor = Color.Gold;
                    else e.Row.Cells[ResumenDeRutasVo.IndexRecepcion].BackColor = Color.Red;
                }
            }

            if (dataItem.Viaje.Estado == ViajeDistribucion.Estados.Pendiente 
                && (dataItem.Viaje.Vehiculo == null || dataItem.Viaje.Vehiculo.Estado != Coche.Estados.Activo || dataItem.Viaje.Vehiculo.Dispositivo == null))
            {
                e.Row.BackColor = Color.IndianRed;
            }

            grid.Columns[ResumenDeRutasVo.IndexKm].Visible = chkVerKm.Checked;
            grid.Columns[ResumenDeRutasVo.IndexRecorrido].Visible = chkVerKm.Checked;
        }

        protected override void SelectedIndexChanged()
        {
            var id = Grid.SelectedRow.Attributes["id"];
            var idViaje = Convert.ToInt32(id);
            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(idViaje);
            if (viaje.InicioReal.HasValue) OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlDistribucion(Convert.ToInt32(id))), "_blank");
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