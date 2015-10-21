using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Operacion
{
    public partial class Operacion_InfoEvent : OnLineSecuredPage
    {
        protected static string LayerVehiculos { get { return CultureManager.GetLabel("LAYER_VEHICULOS"); } }
        protected override InfoLabel LblInfo { get { return null; } }

        protected override string PageTitle { get { return string.Format("{0} - InfoEvent", ApplicationTitle); } }

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

        protected override string GetRefference() { return "MONITOR"; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Mode = Request.QueryString["mode"];
                var id = EventoId;
                if (id < 0) return;
                var mensaje = DAOFactory.LogMensajeDAO.FindById(id);
                if (mensaje == null) return;

                ShowMensaje(mensaje);
            }
        }
        protected void ShowMensaje(LogMensaje mensaje)
        {
            var iconDir = IconDir;

            var color = mensaje.Accion != null
                            ? Color.FromArgb(100, mensaje.Accion.Red, mensaje.Accion.Green, mensaje.Accion.Blue)
                            : Color.Gray;
            panelTitle.BackColor = color;
            panelTitle.ForeColor = color.GetBrightness() < 40 ? Color.White: Color.Black;

            if (mensaje.Accion != null && mensaje.Accion.PopIcon > 0)
                imgAccion.ImageUrl = string.Format("{0}/{1}", iconDir, mensaje.Accion.PopUpIcon.PathIcono);
            else imgAccion.Visible = false;

            lbMensaje.Text = mensaje.Texto;
            lblHora.Text = mensaje.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            lblRecepcion.Text = mensaje.FechaAlta.HasValue 
                                    ? "(" + mensaje.FechaAlta.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + ")"
                                    : string.Empty;

            var pos = mensaje.Fecha;
        
            var link = string.Format(
                "../Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={0}&TypeMobile={1}&Movil={2}&InitialDate={3}&FinalDate={4}&ShowMessages=0&ShowPOIS=0&Empresa={5}",
                mensaje.Coche.Linea != null ? mensaje.Coche.Linea.Id : -1, mensaje.Coche.TipoCoche.Id, mensaje.Coche.Id, pos.Subtract(TimeSpan.FromMinutes(15)).ToString(CultureInfo.InvariantCulture),
                pos.Add(TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture), mensaje.Coche.Empresa != null ? mensaje.Coche.Empresa.Id : mensaje.Coche.Linea != null ? mensaje.Coche.Linea.Empresa.Id : -1);

            lbMensaje.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("OPE_MON_HISTORICO") + "')", link);

            lblMovil.Text = mensaje.Coche.Interno;
            var imgMovl = string.Format("{0}/{1}", iconDir, mensaje.Coche.TipoCoche.IconoNormal.PathIcono);
            imgMovil.ImageUrl = imgMovl;

            var chofer = mensaje.Chofer ?? mensaje.Coche.Chofer;
            lblChofer.Text = chofer != null
                                 ? string.Format("{0} - {1}", chofer.Legajo, chofer.Entidad.Descripcion)
                                 : CultureManager.GetString("Labels", "SIN_CHOFER");

            lblDireccion.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(mensaje.Latitud, mensaje.Longitud);

            lblLatitud.Text = mensaje.Latitud.ToString(CultureInfo.InvariantCulture);
            lblLongitud.Text = mensaje.Longitud.ToString(CultureInfo.InvariantCulture);

            Monitor1.ImgPath = Config.Monitor.GetMonitorImagesFolder(this);
            Monitor1.GoogleMapsScript = GoogleMapsKey;
            Monitor1.EnableTimer = false;
            Monitor1.AddLayers(LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), 8),
                               //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, 8),
                               LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                               LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), 8),
                               LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), 8),
                               LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), 8),
                               LayerFactory.GetMarkers(LayerVehiculos, true));
            Monitor1.AddControls(ControlFactory.GetLayerSwitcher(),
                                 ControlFactory.GetNavigation());
            Monitor1.SetCenter(mensaje.Latitud, mensaje.Longitud, 7);
            Monitor1.AddMarkers(LayerVehiculos, MarkerFactory.CreateMarker(mensaje.Coche.Id.ToString("#0"), imgMovl, mensaje.Latitud, mensaje.Longitud));

            var empresa = mensaje.Coche.Empresa;
            var linea = mensaje.Coche.Linea;
            var user = WebSecurity.AuthenticatedUser != null ? DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id) : null;
            var mensajes = DAOFactory.MensajeDAO.FindByTipo(null, empresa, linea, user);
            var messages = mensajes.Where(m => m.TipoMensaje.DeAtencion).OrderBy(m => m.Descripcion);
            cbMensaje.ClearItems();
            cbMensaje.AddItem(CultureManager.GetControl("DDL_NO_MESSAGE"), cbMensaje.NoneValue);
            foreach (var msg in messages) cbMensaje.AddItem(msg.Descripcion, msg.Id);

            if (mensaje.Estado > 0)
            {
                btAceptar.Visible = false;
                btIgnorar.ImageUrl = "~/Operacion/btAtras.gif";

                var atencion = DAOFactory.AtencionEventoDAO.GetByEvento(mensaje.Id);
                if (atencion != null)
                {
                    panelUsuario.Visible = true;
                    lblUsuario.Text = atencion.Usuario.NombreUsuario;
                    panelFecha.Visible = true;
                    lblFecha.Text = atencion.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                    cbMensaje.SetSelectedValue(atencion.Mensaje.Id);
                    cbMensaje.Enabled = false;
                    txtObservacion.Text = atencion.Observacion;
                    txtObservacion.Enabled = false;
                }
            }

            if (!WebSecurity.IsSecuredAllowed(Securables.EventAttention))
            {
                btAceptar.Visible = false;
                btIgnorar.ImageUrl = "~/Operacion/btAtras.gif";
            }
        }

        protected void BtAceptarClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            var evento = DAOFactory.LogMensajeDAO.FindById(EventoId);
            var usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            if (evento.Estado == 0)
            {
                evento.Usuario = usuario;
                evento.Estado = 1;
            }

            DAOFactory.LogMensajeDAO.SaveOrUpdate(evento);

            var atencion = new AtencionEvento
                               {
                                   Fecha = DateTime.UtcNow,
                                   LogMensaje = evento,
                                   Mensaje = cbMensaje.Selected > 0 ? DAOFactory.MensajeDAO.FindById(cbMensaje.Selected) : null,
                                   Observacion = txtObservacion.Text,
                                   Usuario = usuario
                               };
            DAOFactory.AtencionEventoDAO.SaveOrUpdate(atencion);

            RegisterClose(false);
        }

        protected void BtIgnorarClick(object sender, EventArgs e)
        {
            RegisterClose(true);
        }

        protected void RegisterClose(bool reload)
        {
            var script = Mode == "Edit"
                             ? "window.parent.ShowEvents();"
                             : "window.close();";
            ClientScript.RegisterStartupScript(typeof(String), "close", script, true);

            if (reload)
                ClientScript.RegisterStartupScript(typeof(String), "reload", "window.parent.location.reload(true);", true);
        }
    }
}
