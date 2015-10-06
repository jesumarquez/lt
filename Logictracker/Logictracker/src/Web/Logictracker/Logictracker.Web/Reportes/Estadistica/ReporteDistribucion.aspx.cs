using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Tracker.Application.Reports;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;
using NHibernate.Transform;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReporteDistribucion : SecuredGridReportPage<ReporteDistribucionVo>
    {
        protected override string VariableName { get { return "REP_DISTRIBUCION"; } }
        protected override string GetRefference() { return "REP_DISTRIBUCION"; }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
                ddlEstados.SelectedIndex = -1;
                LoadQueryString();
            }
        }

        private void LoadQueryString()
        {
            if (Request.QueryString["Movil"] != null)
            {
                int movil;
                if (int.TryParse(Request.QueryString["Movil"], out movil) && movil > 0)
                    ddlVehiculo.SelectedValue = movil.ToString("#0");
            }
            if (Request.QueryString["Fecha"] != null && Request.QueryString["Fecha"].Length == 8)
            {
                int anio, mes, dia;
                var fecha = Request.QueryString["fecha"];
                if (int.TryParse(fecha.Substring(0, 4), out anio) &&
                    int.TryParse(fecha.Substring(4, 2), out mes) &&
                    int.TryParse(fecha.Substring(6, 2), out dia))
                {
                    var desde = new DateTime(anio, mes, dia);
                    var hasta = desde.AddHours(23).AddMinutes(59);
                    dpDesde.SelectedDate = desde;
                    dpHasta.SelectedDate = hasta;
                }
            }
        }

        protected override List<ReporteDistribucionVo> GetResults()
        {
            var reportService = new ReportService(DAOFactory, ReportFactory);
            
            if (QueryExtensions.IncludesAll(ddlVehiculo.SelectedValues))
                ddlVehiculo.ToogleItems();

            if (ddlEstados.SelectedStringValues.Count == 0)
                ddlEstados.ToogleItems();

            var ruta = ddlRuta.Selected;
            var confirmation = chkVerConfirmacion.Checked;
            var orden = chkVerOrdenManual.Checked;
            var empresa = ddlLocacion.Selected;
            var linea = ddlPlanta.Selected;
            var selectedVehicles = ddlVehiculo.SelectedValues;
            var puntoEntrega = ddlPuntoEntrega.Selected;
            var estadosEntrega = ddlEstados.SelectedStringValues.Count > 0
                ? ddlEstados.SelectedValues
                : new List<int> {-1};
            var transportista = ddlTransportista.SelectedValues;
                                                                                       
            var desde = dpDesde.SelectedDate.Value.ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.Value.ToDataBaseDateTime();

            var results = reportService.DeliverStatusRepor(empresa, linea, selectedVehicles, puntoEntrega,
                estadosEntrega, transportista, desde, hasta, ruta, confirmation, orden);

            return results;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReporteDistribucionVo dataItem)
        {
            base.OnRowDataBound(grid, e, dataItem);
            e.Row.Attributes.Add("id", dataItem.Id.ToString("#0"));
            
            var clickScript = ClientScript.GetPostBackEventReference(Grid, string.Format("Select:{0}", e.Row.RowIndex));
            e.Row.Attributes.Remove("onclick");

            var checkIndex = GridUtils.GetColumnIndex(ReporteDistribucionVo.IndexTieneFoto);
            for (var i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i != checkIndex)
                {
                    GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", clickScript);
                }
                else if (dataItem.TieneFoto)
                {
                    GridUtils.GetCell(e.Row, ReporteDistribucionVo.IndexTieneFoto).Text = @"<div class=""withPhoto""></div>";

                    const string link = "window.open('../../Common/Pictures/Default.aspx?d={0}&f={1}&t={2}', 'Fotos_{0}', 'width=345,height=408,directories=no,location=no,menubar=no,resizable=no,scrollbars=no,status=no,toolbar=no');";
                    GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", string.Format(link, dataItem.IdDispositivo, dataItem.Desde.ToString("dd/MM/yyyy HH:mm:ss"), dataItem.Hasta.ToString("dd/MM/yyyy HH:mm:ss")));
                }
            }

            grid.Columns[ReporteDistribucionVo.IndexConfirmacion].Visible = chkVerConfirmacion.Checked;
            grid.Columns[ReporteDistribucionVo.IndexHorario].Visible = chkVerConfirmacion.Checked;

            grid.Columns[ReporteDistribucionVo.IndexReadInactive].Visible = chkInteraccionGarmin.Checked;
            grid.Columns[ReporteDistribucionVo.IndexUnreadInactive].Visible = chkInteraccionGarmin.Checked;

            grid.Columns[ReporteDistribucionVo.IndexDescripcion].Visible = chkVerDescripcion.Checked;
        }

        protected override void SelectedIndexChanged()
        {
            OpenWin(String.Concat(ApplicationPath, "Ciclologistico/Distribucion/ViajeDistribucionAlta.aspx?id=" + Grid.SelectedRow.Attributes["id"]), "Distribución");
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm") }, 
                           { CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm") }
                       };
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
            if (ddlVehiculo.SelectedValues.Contains(0)) ddlVehiculo.ToogleItems();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Reporte: {0} - ", reporte);
            sDescription.AppendFormat("Tipo de Vehiculo: {0} - ", ddlVehiculo.SelectedItem.Text);

            return sDescription.ToString();
        }

        protected override DateTime GetSinceDateTime()
        {
            return dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override DateTime GetToDateTime()
        {
            return dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override List<int> GetVehicleList()
        {
            if (ddlVehiculo.SelectedValues.Contains(0)) ddlVehiculo.ToogleItems();
            return ddlVehiculo.SelectedValues;
        }
    }
}