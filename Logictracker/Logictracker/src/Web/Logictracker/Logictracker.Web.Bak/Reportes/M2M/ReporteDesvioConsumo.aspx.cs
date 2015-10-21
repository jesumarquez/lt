using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteDesvioConsumo : SecuredGraphReportPage<DatamartDAO.SummarizedDatamart>
    {
        protected override string GetRefference() { return "REP_DESVIO_CONSUMO"; }
        protected override string VariableName { get { return "REP_DESVIO_CONSUMO"; } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("DIAS"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("CONSUMO"); } }

        private Dictionary<string, double[]> _dias = new Dictionary<string, double[]>();
        
        protected string[] IndicesDias
        {
            get { return Session["IndicesDias"] as string[] ?? new string[0]; }
            set { Session.Add("IndicesDias", value); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                dtFechaDesde.SetDate();
        }

        protected override void OnUnload(EventArgs e)
        {
        	 base.OnUnload(e);
             if (_dias.Count > 0)
                 IndicesDias = _dias.Select(m => m.Key).ToArray();
        }

        protected override List<DatamartDAO.SummarizedDatamart> GetResults()
        {
            if (dtFechaDesde.SelectedDate == null)
                return null;

            var desde = SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.Value);
            var hasta = desde.AddMonths(1);
            
            return new List<DatamartDAO.SummarizedDatamart>
                       {
                           DAOFactory.DatamartDAO.GetSummarizedDatamart(desde, hasta, cbVehiculo.Selected)
                       };
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                var desde = dtFechaDesde.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.Value) : new DateTime();
                var hasta = desde.AddMonths(1).AddSeconds(-1);
                
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("DESDE_HASTA"), desde.ToDisplayDateTime(), hasta.ToDisplayDateTime()));

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", " - ");
                
                _dias.Clear();
                var diasMes = GetDiasMes(desde.ToDisplayDateTime().Month, desde.ToDisplayDateTime().Year);

                for (var i = 0; i < diasMes; i++)
                {
                    var fechaDesde = desde.AddDays(i).AddSeconds(-1);
                    var fechaHasta = fechaDesde.AddDays(1);

                    var dm = DAOFactory.DatamartDAO.GetSummarizedDatamart(fechaDesde, fechaHasta, cbVehiculo.Selected);

                    var litrosConsumidos = dm.Consumo;
                    var kmRecorridos = dm.Kilometros;
                    var hsEnMarcha = dm.HsMarcha;

                    var litrosXKm = litrosConsumidos > 0.0 && kmRecorridos > 0.0 ? litrosConsumidos / kmRecorridos : 0;
                    var litrosXHora = litrosConsumidos > 0.0 && hsEnMarcha > 0.0 ? litrosConsumidos / hsEnMarcha : 0;

                    _dias.Add((i + 1) + "/" + desde.ToDisplayDateTime().ToString("MM/yyyy"), new[] { litrosXHora, litrosXKm });
                }

                foreach (var pair in _dias)
                {
                    var item = new FusionChartsItem();
                    
                    item.AddPropertyValue("color", "AFD8F8");
                    item.AddPropertyValue("name", pair.Key.Split('/')[0]);
                    item.AddPropertyValue("value", pair.Value[1].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);

                    item = new FusionChartsItem();
                    
                    item.AddPropertyValue("color", "FF5904");
                    item.AddPropertyValue("value", pair.Value[0].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);
                }

                lblLitrosXKm.Text = CultureManager.GetLabel("LITROS_X_KM");
                lblLitrosXHora.Text = CultureManager.GetLabel("LITROS_X_HORA");
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

        private static int GetDiasMes(int mes, int año)
        {
            var dias = 0;

            switch (mes)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    dias = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    dias = 30;
                    break;
                case 2:
                    var a = (decimal)año / 4;
                    var b = Math.Round(a, 0);
                    dias = a == b ? 29 : 28;
                    break;
            }

            return dias;
        }
    }
}