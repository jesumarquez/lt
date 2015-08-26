#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class Reportes_CombustibleEnPozos_consumoDiario : SecuredGraphReportPage<ConsumoDiario>
    {
        #region Protected Properties

        protected override string VariableName { get { return "CONSUMO_DIARIO"; } }

        protected override GraphTypes GraphType { get { return GraphTypes.MultiColumn; } }

        protected override string XAxisLabel { get { return CultureManager.GetLabel("FECHA"); } }

        protected override string YAxisLabel { get { return CultureManager.GetLabel("VOLUMEN"); } }

        protected override string GetRefference() { return "CONSUMOS_DIARIOS"; }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<ConsumoDiario> GetResults()
        {   
            var Desde = dpDesde.SelectedDate.GetValueOrDefault();
            var Hasta = dpHasta.SelectedDate.GetValueOrDefault();
            var tanque = ddlTanque.Selected;     
            var list = new List<ConsumoDiario>();

            if(tanque <= 0) return list;

            var results = ReportFactory.ConsumoDiarioDAO.FindConsumosByDate(tanque, Desde, Hasta);

            return results.ToList();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                SetGraphProperties(helper);

                var consumos = new FusionChartsDataset();
                var ingresos = new FusionChartsDataset();
                var conciliacionEgreso = new FusionChartsDataset();
                var conciliacionIngreso = new FusionChartsDataset();

                var categories = new List<string>();

                consumos.SetPropertyValue("seriesName", CultureManager.GetLabel("CONSUMOS"));

                ingresos.SetPropertyValue("seriesName", CultureManager.GetLabel("INGRESOS"));
                ingresos.SetPropertyValue("color", "008ED6");

                conciliacionEgreso.SetPropertyValue("seriesName", CultureManager.GetLabel("EGRESOS_CONCILIACION"));
                conciliacionEgreso.SetPropertyValue("color", "FF9933");

                conciliacionIngreso.SetPropertyValue("seriesName", CultureManager.GetLabel("INGRESOS_CONCILIACION"));
                conciliacionIngreso.SetPropertyValue("color", "339900");

                foreach (var c in ReportObjectsList)
                {
                    var category = String.Format("{0}", c.Fecha.ToShortDateString());

                    if (!categories.Contains(category))
                    {
                        categories.Add(category);
                        helper.AddCategory(category);
                    }

                    consumos.addValue(c.VolumenConsumido.ToString(CultureInfo.InvariantCulture));
                    ingresos.addValue(c.Ingresos.ToString(CultureInfo.InvariantCulture));
                    conciliacionEgreso.addValue(c.EgresosPorConciliacion.ToString(CultureInfo.InvariantCulture));
                    conciliacionIngreso.addValue(c.IngresosPorConciliacion.ToString(CultureInfo.InvariantCulture));
                }
                helper.AddDataSet(consumos);
                helper.AddDataSet(ingresos);
                helper.AddDataSet(conciliacionEgreso);
                helper.AddDataSet(conciliacionIngreso);

                GraphCategories = helper.GetCategoriesList();
                GraphDataSet = new List<FusionChartsDataset> {consumos,ingresos,conciliacionEgreso,conciliacionIngreso};

                var s = helper.BuildXml();
                return s;
            }
        }

        protected override void GetGraphCategoriesAndDatasets() { GetGraphXml(); }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string> 
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocation.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI19"), ddlEquipo.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI36"), ddlTanque.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the graph configuration
        /// </summary>
        /// <param name="helper"></param>
        private void SetGraphProperties(FusionChartsMultiSeriesHelper helper)
        {

            helper.AddConfigEntry("caption", string.Format("Tanque: {0} - Desde: {1} {2}  Hasta: {3} - {4}",
                                                           ddlTanque.Selected != 0 ? DAOFactory.TanqueDAO.FindById(ddlTanque.Selected).Descripcion : "",
                                                           dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(),dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString(),
                                                           dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()));
            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "0");
            helper.AddConfigEntry("thousandSeparator", ".");
            helper.AddConfigEntry("formatNumberScale", "0");
            helper.AddConfigEntry("numberSuffix", "lt");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("showNames", "1");
            helper.AddConfigEntry("rotateNames", "1");
        }

        #endregion
    }
}

