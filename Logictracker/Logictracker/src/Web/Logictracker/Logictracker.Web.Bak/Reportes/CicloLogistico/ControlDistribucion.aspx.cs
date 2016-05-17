using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class ControlDistribucion : SecuredGridReportPage<EntregaVo>
    {
        protected override string VariableName { get { return "REP_CONTROL_DISTRIBUCION"; } }
        protected override string GetRefference() { return "REP_CONTROL_DISTRIBUCION"; }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }
        public override bool HasTotalRow { get { return true; } }
        public override int PageSize { get { return 500; } }
        protected override bool ExcelButton { get { return true; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }
        }

        protected override List<EntregaVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var results = new List<EntregaVo>();

                var viajes = DAOFactory.ViajeDistribucionDAO.GetList(new[] { ddlLocacion.Selected },
                                                                     new[] { ddlPlanta.Selected },
                                                                     lbTransportista.SelectedValues,
                                                                     new[] { -1 }, // DEPARTAMENTOS
                                                                     lbCentroDeCostos.SelectedValues,
                                                                     lbSubCentroDeCostos.SelectedValues,
                                                                     lbVehiculo.SelectedValues,
                                                                     new[] { -1 }, // EMPLEADOS
                                                                     new[] { -1 }, // ESTADOS
                                                                     new[] { cbTipoDistribucion.Selected },
                                                                     SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()),
                                                                     SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()));
                
                foreach (var viajeDistribucion in viajes)
                {
                    EntregaDistribucion anterior = null;
                    var detalles = viajeDistribucion.Detalles;

                    if (chkVerOrdenManual.Checked)
                        detalles = viajeDistribucion.GetEntregasPorOrdenManual();
                    else if (viajeDistribucion.Tipo == ViajeDistribucion.Tipos.Desordenado)
                        detalles = viajeDistribucion.GetEntregasPorOrdenReal();

                    var orden = 0;
                    foreach (var entrega in detalles)
                    {
                        var tiempoRecorrido = new TimeSpan(0);
                        var kms = 0.0;
                        DateTime? desde = null;
                        if (anterior != null && 
                            (entrega.Estado.Equals(EntregaDistribucion.Estados.Completado)
                            || entrega.Estado.Equals(EntregaDistribucion.Estados.Visitado)))
                        {
                            desde = anterior.SalidaOManualExclusiva.HasValue ? anterior.SalidaOManualExclusiva.Value : (DateTime?)null;
                            if (entrega.EntradaOManualExclusiva.HasValue && anterior.SalidaOManualExclusiva.HasValue)
                            {
                                tiempoRecorrido = entrega.EntradaOManualExclusiva.Value.Subtract(anterior.SalidaOManualExclusiva.Value);
                                if (tiempoRecorrido.TotalMinutes < 0) tiempoRecorrido = new TimeSpan(0);
                            }
                            
                            if (entrega.Viaje.Vehiculo != null && anterior.FechaMin < entrega.FechaMin && entrega.FechaMin < DateTime.MaxValue)
                                kms = DAOFactory.CocheDAO.GetDistance(entrega.Viaje.Vehiculo.Id, anterior.FechaMin, entrega.FechaMin);
                        }
                        
                        results.Add(new EntregaVo(orden, entrega, tiempoRecorrido, kms, desde));
                        orden++;

                        if (entrega.Estado.Equals(EntregaDistribucion.Estados.Completado)
                         || entrega.Estado.Equals(EntregaDistribucion.Estados.Visitado))
                            anterior = entrega;
                    }
                }
                
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Retraso de Ruta", String.Format("Duración de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Retraso de Ruta", e);
                throw;
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, EntregaVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");
            e.Row.Style.Add("cursor","default");

            if (!chkVerDesvio.Checked)
            {
                GridUtils.GetColumn(EntregaVo.IndexProgramado).Visible = false;
                GridUtils.GetColumn(EntregaVo.IndexDesvio).Visible = false;
            }
            
            var checkIndex = GridUtils.GetColumnIndex(EntregaVo.IndexTieneFoto);
            for (var i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == checkIndex && dataItem.TieneFoto)
                {
                    GridUtils.GetCell(e.Row, EntregaVo.IndexTieneFoto).Text = @"<div class=""withPhoto""></div>";

                    const string link = "window.open('../../Common/Pictures/Default.aspx?d={0}&f={1}&t={2}', 'Fotos_{0}', 'width=345,height=408,directories=no,location=no,menubar=no,resizable=no,scrollbars=no,status=no,toolbar=no');";
                    GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", string.Format(link, dataItem.IdDispositivo, dataItem.Desde.ToString("dd/MM/yyyy HH:mm:ss"), dataItem.Hasta.ToString("dd/MM/yyyy HH:mm:ss")));
                }
            }
        }
    }
}
