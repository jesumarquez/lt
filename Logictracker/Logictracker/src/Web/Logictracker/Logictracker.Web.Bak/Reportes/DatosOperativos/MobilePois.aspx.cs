using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class MobilePois : SecuredGridReportPage<MobilePoiVo>
    {
        protected override string VariableName { get { return "DOP_CERCANIA_POI"; } }
        protected override string GetRefference() { return "MOBILE_POIS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text },
                           {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text },
                           {CultureManager.GetEntity("PARENTI10"), ddlTipoDomicilio.SelectedItem.Text },
                       };
        }
        
        protected override List<MobilePoiVo> GetResults()
        {
            var coches = DAOFactory.CocheDAO.GetList(ddlDistrito.SelectedValues, ddlBase.SelectedValues);

            return ReportFactory.MobilePoiDAO.GetMobileNearPois(coches, lbPOIs.SelectedValues, npDistancia.Number).Select(mp => new MobilePoiVo(mp)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobilePoiVo dataItem)
        {
            e.Row.BackColor = dataItem.Velocidad > 0 ? Color.LightGreen : Color.LightGray;
        }

        /// <summary>
        /// Automatically updates mobiles near selected points of interest.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void tmrPois_Tick(object sender, EventArgs e) { Bind(); }
    }
}
