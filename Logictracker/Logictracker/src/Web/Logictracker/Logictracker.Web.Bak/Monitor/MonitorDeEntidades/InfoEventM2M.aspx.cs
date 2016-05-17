using System;
using System.Drawing;
using Logictracker.Configuration;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using System.Web.UI.WebControls;

namespace Logictracker.Monitor.MonitorDeEntidades
{
    public partial class InfoEventM2M : OnLineSecuredPage
    {
        protected static string LayerEntidades { get { return CultureManager.GetLabel("LAYER_ENTIDADES"); } }
        protected override InfoLabel LblInfo { get { return null; } }

        protected int EventoId
        {
            get
            {
                int id;
                if (Request.QueryString["evt"] != null && int.TryParse(Request.QueryString["evt"], out id))
                    return id;
                return -1;
            }
        }
        protected string Mode
        {
            get { return (string)(ViewState["Mode"]??"Edit"); }
            set { ViewState["Mode"] = value; }
        }

        protected override string GetRefference() { return "OPE_MON_ENTIDAD"; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Mode = Request.QueryString["mode"];
                var id = EventoId;
                if (id < 0) return;
                var evento = DAOFactory.LogEventoDAO.FindById(id);
                if (evento == null) return;

                ShowEvento(evento);
            }
        }
        protected void ShowEvento(LogEvento evento)
        {
            var iconDir = IconDir;

            var color = evento.Accion != null
                            ? Color.FromArgb(100, evento.Accion.Red, evento.Accion.Green, evento.Accion.Blue)
                            : Color.Gray;
            panelTitle.BackColor = color;
            panelTitle.ForeColor = color.GetBrightness() < 40 ? Color.White : Color.Black;

            if (evento.Accion != null && evento.Accion.PopIcon > 0)
                imgAccion.ImageUrl = string.Format("{0}/{1}", iconDir, evento.Accion.PopUpIcon.PathIcono);
            else
                imgAccion.Visible = false;

            lbMensaje.Text = evento.Texto;
            lblHora.Text = evento.Fecha.ToDisplayDateTime().ToString("dd \"de\" MMMM \"de\" yyyy HH:mm");
            lblEntidad.Text = evento.SubEntidad != null ? evento.SubEntidad.Entidad.Descripcion : string.Empty;
            lblSubEntidad.Text = evento.SubEntidad != null ? evento.SubEntidad.Descripcion : string.Empty;
            if (evento.SubEntidad != null)
            {
                foreach (DetalleValor detalle in evento.SubEntidad.Entidad.Detalles)
                {
                    var valor = string.Empty;
                    switch (detalle.Detalle.Tipo)
                    {
                        case 1:
                            valor = detalle.ValorStr;
                            break;
                        case 2:
                            valor = detalle.ValorNum.ToString("#0.00");
                            break;
                        case 3:
                            valor = detalle.ValorDt.HasValue ? detalle.ValorDt.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
                            break;
                    }

                    var lbl = new Label { Text = @"<b>" + detalle.Detalle.Nombre + @":</b> " + valor + @"<br/><br/>" };
                    panelDetalles.Controls.Add(lbl);
                }

                lblDireccion.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(evento.SubEntidad.Entidad.ReferenciaGeografica.Latitude, evento.SubEntidad.Entidad.ReferenciaGeografica.Longitude);
            }

            Monitor1.ImgPath = Config.Monitor.GetMonitorImagesFolder(this);
            Monitor1.GoogleMapsScript = GoogleMapsKey;
            Monitor1.EnableTimer = false;
            Monitor1.AddLayers(LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), 8), 
                               //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, 8),
                               LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                               LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), 8),
                               LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), 8),
                               LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), 8),
                               LayerFactory.GetMarkers(LayerEntidades, true));
            Monitor1.AddControls(ControlFactory.GetLayerSwitcher(), ControlFactory.GetNavigation());

            if (evento.SubEntidad != null)
            {
                Monitor1.SetCenter(evento.SubEntidad.Entidad.ReferenciaGeografica.Latitude, evento.SubEntidad.Entidad.ReferenciaGeografica.Longitude, 7);
                var imgEntidad = string.Format("{0}{1}", iconDir, evento.SubEntidad.Entidad.ReferenciaGeografica.Icono.PathIcono);
            
                Monitor1.AddMarkers(LayerEntidades, MarkerFactory.CreateMarker(evento.SubEntidad.Entidad.Id.ToString("#0"),
                                                                               imgEntidad,
                                                                               evento.SubEntidad.Entidad.ReferenciaGeografica.Latitude,
                                                                               evento.SubEntidad.Entidad.ReferenciaGeografica.Longitude));
            }
        
            if (evento.Estado > 0)
            {
                btAceptar.Visible = false;
                btIgnorar.ImageUrl = "~/Operacion/btAtras.gif";
            }
        }

        protected void BtAceptarClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            var evento = DAOFactory.LogEventoDAO.FindById(EventoId);

            if (evento.Estado == 0)
            {
                //evento.Usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
                evento.Estado = 1;
            }

            DAOFactory.LogEventoDAO.SaveOrUpdate(evento);

            RegisterClose();
        }

        protected void BtIgnorarClick(object sender, EventArgs e)
        {
            RegisterClose();
        }

        protected void RegisterClose()
        {
            var script = Mode == "Edit" ? "window.parent.ShowEvents();" : "window.close();";
            ClientScript.RegisterStartupScript(typeof(String), "close", script, true);
        }
    }
}
