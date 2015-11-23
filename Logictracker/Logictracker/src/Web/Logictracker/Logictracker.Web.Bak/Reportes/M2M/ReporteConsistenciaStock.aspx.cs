using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteConsistenciaStock : SecuredGraphReportPage<ConsumoCombustibleVo>
    {
        protected override string GetRefference() { return "COMB_CONSISTENCIA_STOCK"; }
        protected override string VariableName { get { return "COMB_CONSISTENCIA_STOCK"; } }
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

            var consumos = DAOFactory.ConsumoDetalleDAO.GetList(cbEmpresa.SelectedValues,
                                                                cbLinea.SelectedValues,
                                                                new[] {-1}, // TRANSPORTISTAS
                                                                new[] {-1}, // DEPARTAMENTOS
                                                                new[] {-1}, // CENTROS DE COSTO
                                                                new[] {-1}, // TIPOS DE VEHICULO
                                                                new[] {-1}, // VEHICULOS
                                                                new[] {-1}, // TIPOS DE EMPLEADO
                                                                new[] {-1}, // EMPLEADOS
                                                                new[] {-1}, // TIPOS DE PROVEEDOR
                                                                new[] {-1}, // PROVEEDORES
                                                                new[] {-1}, // DEPOSITOS ORIGEN
                                                                cbDeposito.SelectedValues,
                                                                desde,
                                                                hasta,
                                                                new[] { -1 })
                                                       .Where(c => c.ConsumoCabecera.Estado != ConsumoCabecera.Estados.Eliminado
                                                                && c.Insumo.Id == cbInsumo.Selected)
                                                       .OrderBy(c => c.ConsumoCabecera.Fecha);

            return consumos.Select(c => new ConsumoCombustibleVo(c)).ToList();
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

                for (var i = 0; i < despachos.Count; i++)
                {
                    var totalCargado = despachos[i].Cantidad;
                    var fechaHasta = i + 1 < despachos.Count ? despachos[i + 1].Fecha.ToDataBaseDateTime() : hasta;
                    var totalDespachos = GetDespachos(despachos[i].Fecha.ToDataBaseDateTime(), fechaHasta);

                    _despachos.Add(despachos[i].Fecha.ToString("dd/MM/yy HH:mm"), new[] { totalCargado, totalDespachos });
                }   
               
                foreach (var pair in _despachos)
                {
                    var item = new FusionChartsItem();
                    
                    item.AddPropertyValue("color", "AFD8F8");
                    item.AddPropertyValue("name", pair.Key);
                    item.AddPropertyValue("value", pair.Value[0].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);

                    item = new FusionChartsItem();
                    
                    item.AddPropertyValue("color", "FF5904");
                    item.AddPropertyValue("value", pair.Value[1].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);
                }

                lblLitrosCargados.Text = CultureManager.GetLabel("LITROS_CARGADOS");
                lblLitrosDespachados.Text = CultureManager.GetLabel("LITROS_DESPACHADOS");
                pnlInferior.Visible = true;
                
                return helper.BuildXml();
            }
        }

        private double GetDespachos(DateTime desde, DateTime hasta)
        {
            var despachos = DAOFactory.ConsumoDetalleDAO.GetList(cbEmpresa.SelectedValues,
                                                                 cbLinea.SelectedValues,
                                                                 new[] { -1 }, // TRANSPORTISTAS
                                                                 new[] { -1 }, // DEPARTAMENTOS
                                                                 new[] { -1 }, // CENTROS DE COSTO
                                                                 new[] { -1 }, // TIPOS DE VEHICULO
                                                                 new[] { -1 }, // VEHICULOS
                                                                 new[] { -1 }, // TIPOS DE EMPLEADO
                                                                 new[] { -1 }, // EMPLEADOS
                                                                 new[] { -1 }, // TIPOS DE PROVEEDOR
                                                                 new[] { -1 }, // PROVEEDORES
                                                                 cbDeposito.SelectedValues,
                                                                 new[] { -1 }, // DEPOSITOS DESTINO
                                                                 desde,
                                                                 hasta,
                                                                 new[] { -1 })
                                                        .Where(c => c.ConsumoCabecera.Estado != ConsumoCabecera.Estados.Eliminado
                                                                 && c.Insumo.Id == cbInsumo.Selected)
                                                        .OrderBy(c => c.ConsumoCabecera.Fecha);
            return despachos.Sum(c => c.Cantidad);
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