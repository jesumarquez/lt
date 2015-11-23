using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteDeDespachos : SecuredGraphReportPage<ConsumoCombustibleVo>
    {
        protected override string GetRefference() { return "REP_DESPACHOS"; }
        protected override string VariableName { get { return "REP_DESPACHOS"; } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("DESPACHOS"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("CONSUMO"); } }

        private Dictionary<string, double[]> _despachos = new Dictionary<string, double[]>();
        
        protected string[] IndicesDespachos
        {
            get { return Session["IndicesDespachos"] as string[] ?? new string[0]; }
            set { Session.Add("IndicesDespachos", value); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dtFechaDesde.SetDate();
                dtFechaHasta.SetDate();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
        	 base.OnUnload(e);
             if (_despachos.Count > 0)
                 IndicesDespachos = _despachos.Select(m => m.Key).ToArray();
        }

        protected override List<ConsumoCombustibleVo> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dtFechaHasta.SelectedDate.GetValueOrDefault());

            var consumos = DAOFactory.ConsumoCabeceraDAO.GetList(cbEmpresa.SelectedValues,
                                                                 cbLinea.SelectedValues,
                                                                 new[] { -1 }, // TRANSPORTISTAS
                                                                 new[] { -1 }, // DEPARTAMENTOS
                                                                 new[] { -1 }, // CENTROS DE COSTO
                                                                 cbTipoVehiculo.SelectedValues,
                                                                 cbVehiculo.SelectedValues,
                                                                 new[] { -1 }, // TIPOS DE EMPLEADO
                                                                 new[] { -1 }, // EMPLEADOS
                                                                 new[] { -1 }, // TIPOS DE PROVEEDOR
                                                                 new[] { -1 }, // PROVEEDORES
                                                                 new[] { -1 }, // DEPOSITOS ORIGEN
                                                                 new[] { -1 }, // DEPOSITOS DESTINO
                                                                 desde,
                                                                 hasta)
                                                        .Where(c => c.Estado != ConsumoCabecera.Estados.Eliminado);

            var detalles = (from consumo in consumos
                            from detalle in consumo.Detalles.Cast<ConsumoDetalle>()
                            where detalle.Insumo.TipoInsumo.DeCombustible
                            select detalle).ToList();

            return detalles.Select(c => new ConsumoCombustibleVo(c)).ToList();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                var desde = dtFechaDesde.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.Value) : new DateTime();
                var hasta = dtFechaHasta.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFechaHasta.SelectedDate.Value) : new DateTime();

                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("DESDE_HASTA"), desde.ToDisplayDateTime(), hasta.ToDisplayDateTime()));

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", " - ");

                _despachos.Clear();
                var despachos = GetResults();
                var vehiculo = DAOFactory.CocheDAO.FindById(cbVehiculo.Selected);
                var entidad = DAOFactory.EntidadDAO.FindByDispositivo(new[] { vehiculo.Empresa != null ? vehiculo.Empresa.Id : -1 },
                                                                      new[] { vehiculo.Linea != null ? vehiculo.Linea.Id : -1 },
                                                                      new[] { vehiculo.Dispositivo != null ? vehiculo.Dispositivo.Id : -1 });
                var usaTelemetria = entidad != null && entidad.Id != 0;

                for (var i = 0; i < despachos.Count; i++)
                {
                    var fecha = despachos[i].Fecha.ToString("dd/MM/yy HH:mm");
                    var totalDespacho = despachos[i].Cantidad;
                    var fechaHasta = i + 1 < despachos.Count ? despachos[i + 1].Fecha : hasta;
                    double totalConsumo;
                    double km;
                    double rendimiento;

                    if (usaTelemetria)
                        totalConsumo = DAOFactory.DatamartDAO.GetSummarizedDatamart(despachos[i].Fecha, fechaHasta, cbVehiculo.Selected).Consumo;
                    else
                    {
                        km = DAOFactory.CocheDAO.GetDistance(vehiculo.Id, despachos[i].Fecha, fechaHasta);
                        rendimiento = vehiculo.CocheOperacion != null && vehiculo.CocheOperacion.Rendimiento > 0.0
                                          ? vehiculo.CocheOperacion.Rendimiento
                                          : vehiculo.Modelo != null
                                                ? vehiculo.Modelo.Rendimiento
                                                : 0.0;
                        totalConsumo = km > 0 && rendimiento > 0 ? (km / 100.0) * rendimiento : 0.0;
                    }
                    
                    while (totalConsumo == 0 && i+1 < despachos.Count && despachos[i].Fecha.AddMinutes(5) > despachos[i+1].Fecha)
                    {
                        fecha = despachos[i + 1].Fecha.ToString("dd/MM/yy HH:mm");
                        totalDespacho += despachos[i + 1].Cantidad;
                        fechaHasta = i + 2 < despachos.Count ? despachos[i + 2].Fecha : hasta;

                        if (usaTelemetria)
                            totalConsumo += DAOFactory.DatamartDAO.GetSummarizedDatamart(despachos[i+1].Fecha, fechaHasta, cbVehiculo.Selected).Consumo;
                        else
                        {
                            km = DAOFactory.CocheDAO.GetDistance(vehiculo.Id, despachos[i+1].Fecha, fechaHasta);
                            rendimiento = vehiculo.CocheOperacion != null && vehiculo.CocheOperacion.Rendimiento > 0.0
                                              ? vehiculo.CocheOperacion.Rendimiento
                                              : vehiculo.Modelo != null
                                                    ? vehiculo.Modelo.Rendimiento
                                                    : 0.0;
                            totalConsumo += km > 0 && rendimiento > 0 ? (km / 100.0) * rendimiento : 0.0;
                        }
                        i++;
                    }
                    
                    _despachos.Add(fecha, new[] { totalDespacho, totalConsumo });
                }   
               
                foreach (var pair in _despachos)
                {
                    var item = new FusionChartsItem();
                    
                    item.AddPropertyValue("color", "AFD8F8");
                    item.AddPropertyValue("name", pair.Key);
                    item.AddPropertyValue("value", pair.Value[0].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);

                    item = new FusionChartsItem();

                    var color = usaTelemetria ? "FF5904" : "FAB802";

                    item.AddPropertyValue("color", color);
                    item.AddPropertyValue("value", pair.Value[1].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);
                }

                lblLitrosCargados.Text = CultureManager.GetLabel("LITROS_CARGADOS");
                lblLitros.Text = usaTelemetria ? CultureManager.GetLabel("LITROS_CONS") : CultureManager.GetLabel("LITROS_CALCULADOS");

                tdLitros.BgColor = usaTelemetria ? "FF5904" : "FAB802";

                pnlInferior.Visible = true;
                
                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), cbEmpresa.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text}
                            };
        }
    }
}