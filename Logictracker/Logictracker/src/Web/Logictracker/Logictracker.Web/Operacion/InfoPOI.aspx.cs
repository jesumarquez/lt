using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Services.Helpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Operacion
{
    public partial class Operacion_InfoPOI : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }

        protected override string PageTitle { get { return string.Format("{0} - InfoPOI", ApplicationTitle); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                int p;
                if (Request.QueryString["p"] == null || !int.TryParse(Request.QueryString["p"], out p))
                {
                    lblDescripcion.Text = CultureManager.GetError("NO_POI_INFO");
                    return;
                }
                var idsLineas = Request.QueryString["l"].Split(',').Select(l => Convert.ToInt32(l)).ToList();
            
                if (idsLineas.Count <= 0)
                {
                    lblDescripcion.Text = CultureManager.GetError("NO_POI_INFO");
                    return;
                }

                var poi = DAOFactory.ReferenciaGeograficaDAO.FindById(p);
                lblTipo.Text = poi.TipoReferenciaGeografica.Descripcion;
                lblDescripcion.Text = poi.Codigo + @" - " + poi.Descripcion;
                lblDireccion.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(poi.Latitude, poi.Longitude);
                imgTipo.ImageUrl = IconDir + (poi.Icono != null ? poi.Icono.PathIcono : poi.TipoReferenciaGeografica.Icono.PathIcono);

                int idEmpresa;
                if (Request.QueryString["e"] == null || !int.TryParse(Request.QueryString["e"], out idEmpresa))
                    idEmpresa = poi.Empresa != null ? poi.Empresa.Id : -1;

                var pois = DAOFactory.ReferenciaGeograficaDAO.GetVehiculosCercanos(new[] { idEmpresa }, idsLineas, poi.Id);
                if (pois.Count > 10) pois.RemoveRange(10, pois.Count - 10);
                gridMov.DataSource = pois;
                gridMov.DataBind();
            }
        }

        protected override string GetRefference() { return "MONITOR"; }

        protected void gridMov_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var p = e.Row.DataItem as MobilePoi;
            e.Row.Style.Add(HtmlTextWriterStyle.Color, p.Velocidad > 0 ? "DarkGreen" : "DarkRed");
            e.Row.Cells[2].Text = FormatKm(p.Distancia);
        }
        private static string FormatKm(double metros)
        {
            if (metros < 1000) return metros.ToString("0.00m");
            return (metros / 1000).ToString("0.00km");
        }
    }
}
