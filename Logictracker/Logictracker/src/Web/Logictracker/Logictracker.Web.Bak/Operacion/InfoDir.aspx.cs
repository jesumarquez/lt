#region Usings

using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Operacion
{
    public partial class OperacionInfoDir : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }

        protected override string PageTitle { get { return string.Format("{0} - InfoDir", ApplicationTitle); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            double la, lo;

            if (Request.QueryString["d"] == null)
            {
                lblDescripcion.Text = CultureManager.GetError("NO_ADDRESS_INFO");
                return;
            }
            if (Request.QueryString["la"] == null || !double.TryParse(Request.QueryString["la"], NumberStyles.Any, CultureInfo.InvariantCulture, out la))
            {
                lblDescripcion.Text = CultureManager.GetError("NO_ADDRESS_INFO");
                return;
            }
            if (Request.QueryString["lo"] == null || !double.TryParse(Request.QueryString["lo"], NumberStyles.Any, CultureInfo.InvariantCulture, out lo))
            {
                lblDescripcion.Text = CultureManager.GetError("NO_ADDRESS_INFO");
                return;
            }

            var idsLineas = Request.QueryString["l"].Split(' ').Select(l => Convert.ToInt32(l)).ToList();

            if (idsLineas.Count <= 0)
            {
                lblDescripcion.Text = CultureManager.GetError("NO_ADDRESS_INFO");
                return;
            }

            lblTipo.Text = CultureManager.GetLabel("ADDRESS");
            lblDescripcion.Text = Request.QueryString["d"];
            imgTipo.ImageUrl = "salida.gif";
            lblLatitud.Text = la.ToString();
            lblLongitud.Text = lo.ToString();

            var vehicles = DAOFactory.CocheDAO.GetList(new[]{-1}, idsLineas);

            var pois = ReportFactory.MobilePoiDAO.GetMobilesNearPoint(vehicles, la, lo, 1000);

            if (pois.Count > 10) pois.RemoveRange(10, pois.Count - 10);

            gridMov.DataSource = pois;
            gridMov.DataBind();
        }

        protected override string GetRefference() { return "MONITOR"; }

        protected void gridMov_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var p = e.Row.DataItem as MobilePoi;

            if (p != null) e.Row.Style.Add(HtmlTextWriterStyle.Color, p.Velocidad > 0 ? "DarkGreen" : "DarkRed");
        }
    }
}
