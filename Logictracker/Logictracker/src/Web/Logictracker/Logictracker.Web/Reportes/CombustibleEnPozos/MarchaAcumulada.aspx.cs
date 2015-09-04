#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Utils;
using Logictracker.Web.Helpers.ColorHelpers;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class Reportes_CombustibleEnPozos_MarchaAcumulada : SecuredGraphReportPage<MarchaMotor>
    {
        #region Protected Properties

        protected override string VariableName { get { return "MARCHA_ACUMULADA"; } }

        protected override GraphTypes GraphType { get { return GraphTypes.MultiColumn; } }

        private readonly Dictionary<int, List<MarchaMotor>> _horas = new Dictionary<int, List<MarchaMotor>>();

        protected override string XAxisLabel { get { return CultureManager.GetLabel("FECHA"); } }

        protected override string YAxisLabel { get { return CultureManager.GetLabel("HS_EN_MARCHA"); } }

        protected override string GetRefference() { return "MARCHA_ACUMULADA"; }

        protected override bool PrintButton { get { return false; } }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        /// <summary>
        /// Gets the Results for the selected parameters.
        /// </summary>
        /// <returns></returns>
        protected override List<MarchaMotor> GetResults()
        {
            ToogleItems(lbMotores);
            foreach (var m in lbMotores.SelectedValues)
            {
                var list = ReportFactory.MarchaPorMotorDAO.FindAllHsDeMarchaForMotor(m, dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault());
            
                _horas.Add(m, (from MarchaMotor marcha in list select marcha).ToList());
            }

            return new List<MarchaMotor> {new MarchaMotor()};
        }

        /// <summary>
        /// Gets the graph XML file.
        /// </summary>
        /// <returns></returns>
        protected override string GetGraphXml()
        {
            var datasets = new List<FusionChartsDataset>();
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                SetGraphProperties(helper);
                var flag = true;
            
                var colores = new ColorGenerator();

                foreach (var m in _horas.Keys)
                {
                    var horas = new FusionChartsDataset();
                    var descripcion = DAOFactory.CaudalimetroDAO.FindById(m).Descripcion;
                    horas.SetPropertyValue("seriesName", descripcion);
                    horas.SetPropertyValue("color", HexColorUtil.ColorToHex(colores.GetNextColor()));
                    var list = _horas[m];

                    if (list.Count.Equals(0)) continue;

                    foreach (var h in list)
                    {
                        if (flag)
                        {
                            helper.AddCategory(h.Fecha.ToShortDateString());
                        }
                        horas.addValue(h.HsEnMarcha.ToString(CultureInfo.InvariantCulture));
                    }
                    flag = false;
                    helper.AddDataSet(horas);
                    datasets.Add(horas);
                }
                var categories = new List<string>();
                categories.AddRange(helper.GetCategoriesList());
                GraphCategories = categories;
                GraphDataSet = datasets;
                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            GetResults();
            GetGraphXml();
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI19"), ddlEquipo.SelectedItem.Text},
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

            helper.AddConfigEntry("caption", string.Format("Desde: {0} {1}  Hasta: {2} - {3}",
                                                           dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString(),
                                                           dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()));
            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "0");
            helper.AddConfigEntry("formatNumberScale", "0");
            helper.AddConfigEntry("numberSuffix", "hs");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("showNames", "1");
            helper.AddConfigEntry("rotateNames", "1");
        }

        #endregion
    }
}
