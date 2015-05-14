using System;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.CicloLogistico
{
    public partial class InfoEntrega : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string GetRefference() { return "DISTRIBUCION_GLOBAL_ENTREGAS"; }
        protected override string PageTitle { get { return string.Format("{0} - InfoEntrega", ApplicationTitle); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int id;
                if (Request.QueryString["id"] == null || !int.TryParse(Request.QueryString["id"], out id))
                {
                    LblInfo.Text = CultureManager.GetError("NO_ENTREGA_INFO");
                    return;
                }
                
                var entrega = DAOFactory.EntregaDistribucionDAO.FindById(id);
                if (entrega != null)
                {
                    lblEntrega.Text = entrega.Descripcion;
                    lblViaje.Text = entrega.Viaje.Codigo;
                    lblVehiculo.Text = entrega.Viaje.Vehiculo != null ? entrega.Viaje.Vehiculo.Interno : CultureManager.GetError("NO_VEHICLE_ASSIGNED");
                    lblEstado.Text = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado));
                    if (entrega.ReferenciaGeografica != null)
                        lblDireccion.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(entrega.ReferenciaGeografica.Latitude, entrega.ReferenciaGeografica.Longitude);
                    lblEntrada.Text = (entrega.Entrada.HasValue ? entrega.Entrada.Value.ToDisplayDateTime().ToString("yyyy/MM/dd HH:mm") : string.Empty);
                    lblManual.Text = (entrega.Manual.HasValue ? entrega.Manual.Value.ToDisplayDateTime().ToString("yyyy/MM/dd HH:mm") : string.Empty);
                    lblSalida.Text = (entrega.Salida.HasValue ? entrega.Salida.Value.ToDisplayDateTime().ToString("yyyy/MM/dd HH:mm") : string.Empty);
                    lnkMonitorCiclo.Attributes.Add("id", entrega.Viaje.Id.ToString("#0"));
                }
            }
        }

        protected void LnkMonitorCicloOnClick(object sender, EventArgs e)
        {
            var lnk = sender as ResourceLinkButton;
            if (lnk == null) return;
            int id;
            if (int.TryParse(lnk.Attributes["id"], out id))
                OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlDistribucion(id)), "_blank");
        }
    }
}
