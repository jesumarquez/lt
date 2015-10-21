using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.Mantenimiento;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;

namespace Logictracker.Reportes.M2M
{
    public partial class ListaRendimiento : SecuredGridReportPage<RendimientoVo>
    {
        protected override string VariableName { get { return "M2M_RENDIMIENTO"; } }
        protected override string GetRefference() { return "M2M_RENDIMIENTO"; }
        protected override bool ExcelButton { get { return true; } }
        
        protected override List<RendimientoVo> GetResults()
		{
            var inicio = DateTime.UtcNow;

            try
            {
                var desde = dpDesde.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.Value) : DateTime.Today;
                var hasta = dpHasta.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.Value) : DateTime.Today.AddDays(1);

                var vehiculos = DAOFactory.CocheDAO.GetList(ddlLocacion.SelectedValues,
                                                            ddlPlanta.SelectedValues,
                                                            new[] { -1 }, // TIPO VEHICULO
                                                            ddlTransportista.SelectedValues,
                                                            new[] { -1 }) // DEPARTAMENTOS
                                                   .Where(v => QueryExtensions.IncludesAll((IEnumerable<int>) ddlVehiculo.SelectedValues) || ddlVehiculo.SelectedValues.Contains(v.Id));

                var despachos = DAOFactory.ConsumoDetalleDAO.GetList(ddlLocacion.SelectedValues,
                                                                     ddlPlanta.SelectedValues,
                                                                     ddlTransportista.SelectedValues,
                                                                     new[] { -1 }, // DEPARTAMENTOS
                                                                     new[] { -1 }, // CC
                                                                     new[] { -1 }, // TIPO VEHICULO
                                                                     ddlVehiculo.SelectedValues,
                                                                     new[] { -1 }, // TIPO EMPLEADO
                                                                     new[] { -1 }, // EMPLEADO
                                                                     new[] { -1 }, // TIPO PROVEEDOR
                                                                     new[] { -1 }, // PROOVEDOR
                                                                     new[] { -1 }, // DEPOSITO ORIGEN
                                                                     new[] { -1 }, // DEPOSITO DESTINO
                                                                     desde,
                                                                     hasta,
                                                                     new[] { -1 }) // CABECERA
                                                            .Where(c => c.Insumo.TipoInsumo.DeCombustible);

                var viajes = new List<ViajeDistribucion>();
                
                if (chkControlaCiclo.Checked)
                {
                    viajes = DAOFactory.ViajeDistribucionDAO.GetList(ddlLocacion.SelectedValues,
                                                                     ddlPlanta.SelectedValues,
                                                                     ddlTransportista.SelectedValues,
                                                                     new[] { -1 }, // DEPARTAMENTOS
                                                                     new[] { -1 }, // CC
                                                                     new[] { -1 }, // SUB CC
                                                                     ddlVehiculo.SelectedValues,
                                                                     desde,
                                                                     hasta);
                }

                var results = vehiculos.Select(vehiculo => new RendimientoVo(vehiculo, 
                                                                             desde,
                                                                             hasta,
                                                                             despachos.Where(d => d.ConsumoCabecera.Vehiculo == vehiculo).Sum(d => d.Cantidad),
                                                                             viajes.Count(d => d.Vehiculo == vehiculo && d.InicioReal.HasValue),
                                                                             chkControlaCiclo.Checked))
                                       .ToList();
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

                STrace.Trace("Listado de Rendimiento M2M", String.Format("Duración de la consulta total: {0} segundos", duracion));

                return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Listado de Rendimiento  M2M", e, String.Format("Duración de la consulta total: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();

            Bind();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, RendimientoVo dataItem)
        {
            if (!chkControlaCiclo.Checked)
            {
                GridUtils.GetColumn(RendimientoVo.IndexViajes).Visible = false;
                GridUtils.GetColumn(RendimientoVo.IndexConsumoXViaje).Visible = false;
                GridUtils.GetColumn(RendimientoVo.IndexDespachoXViaje).Visible = false;
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.ToString() },
                           { CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.ToString() }
                       };
        }
    }
}