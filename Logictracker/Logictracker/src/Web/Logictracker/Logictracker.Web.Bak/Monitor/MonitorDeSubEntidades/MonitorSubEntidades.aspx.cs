using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Messaging;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.Labels.Popup;
using Image = System.Web.UI.WebControls.Image;

namespace Logictracker.Monitor.MonitorDeSubEntidades
{
    public partial class MonitorSubEntidades : OnLineSecuredPage
    {
        protected override string GetRefference() { return "OPE_MON_SUBENTIDAD"; }
        protected override InfoLabel LblInfo { get { return null; } }
        
        private const int ImageWidth = 850;
        private Size _imageSize;
              
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                ParsearQueryString();
                RegisterStatusCheck();
                RegisterEvents();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                SelectedSubEntidades = null;
                SelectedLineas = new List<int> { -1 };
                RegisterExtJsStyleSheet();
            }
        }

        #region Properties

        private List<int> IdLinea
        {
            get
            {
                if (cbPlanta.SelectedIndex <= 0) return Enumerable.ToList<int>((from ListItem item in cbPlanta.Items where item.Value != @"-1" select Convert.ToInt32((string) item.Value)));
                return new List<int> { Convert.ToInt32((string) cbPlanta.SelectedValue) };
            }
        }

        private int IdEmpresa
        {
            get { if (cbLocacion.SelectedIndex < 0) return -1; return Convert.ToInt32((string) cbLocacion.SelectedValue); }
        }

        protected List<int> SelectedLineas
        {
            get
            {
                var v = (List<int>)Session["MONITOR_SelectedLinea"];
                return v ?? IdLinea;
            }
            set
            {
                if (value.Contains(-1))
                    Session["MONITOR_SelectedLinea"] = null;
                else
                    Session["MONITOR_SelectedLinea"] = value;
            }
        }

        protected int SelectedEmpresa
        {
            get
            {
                var v = Session["MONITOR_SelectedEmpresa"];
                return v != null ? (int)v : IdEmpresa;
            }
            set
            {
                if (value == -1)
                    Session["MONITOR_SelectedEmpresa"] = null;
                else
                    Session["MONITOR_SelectedEmpresa"] = value;
            }
        }

        protected int SelectedEntidad
        {
            get
            {
                var v = Session["MONITOR_SelectedEntidad"];
                return v != null ? (int)v : IdEmpresa;
            }
            set
            {
                if (value == -1)
                    Session["MONITOR_SelectedEntidad"] = null;
                else
                    Session["MONITOR_SelectedEntidad"] = value;
            }
        }

        protected List<SubEntidad> SelectedSubEntidades
        {
            get { return Session["MONITOR_SelectedSubEntidades"] != null ? Session["MONITOR_SelectedSubEntidades"] as List<SubEntidad> : new List<SubEntidad>(); }
            set { Session["MONITOR_SelectedSubEntidades"] = value; }
        }

        #endregion

        #region Events

        protected void CbLocacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            SelectedEmpresa = IdEmpresa;
        }

        protected void CbPlanta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            UpdatePanel1.Update();
            
            SelectedLineas = IdLinea;
        }

        protected void CbTipoEntidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            SelectedSubEntidades = null;
        }

        protected void CbEntidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");
            
            SelectedSubEntidades = null;

            SetSelectedSubEntidades();
        }

        protected void CbSubEntidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            SetSelectedSubEntidades();
        }
        
        protected void ConnectionStatusClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            SelectedLineas = IdLinea;

            SetSelectedSubEntidades();
        }

        protected void BtBuscar_Click(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            var subentidades = DAOFactory.SubEntidadDAO.GetByDescripcion(new[] { cbLocacion.Selected },
                                                                         new[] { cbPlanta.Selected },
                                                                         new[] { cbTipoEntidad.Selected },
                                                                         new[] { cbEntidad.Selected },
                                                                         txtSubEntidad.Text.Trim());

            if (!subentidades.Any())
            {
                JsAlert(string.Format(CultureManager.GetError("SUBENTIDAD_NOT_FOUND"), txtSubEntidad.Text.Trim()));
                
                SetSelectedSubEntidades();

                return;
            }

            foreach (var selectedSubEntidad in SelectedSubEntidades)
            {
                var li = cbSubEntidad.Items.FindByValue(selectedSubEntidad.Id.ToString());
                if (li != null) li.Selected = false;
            }

            SelectedSubEntidades = null;

            foreach (var subentidad in subentidades)
            {
                var item = cbSubEntidad.Items.FindByValue(subentidad.Id.ToString());
                if (item != null) item.Selected = true;
            }

            SetSelectedSubEntidades();
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            LoadPlano();

            if (SelectedSubEntidades.Count > 0)
            {
                foreach (var subentidad in SelectedSubEntidades)
                {
                    var style = GetMarkerStyle(subentidad);
                    AddMarker(subentidad, style);
                }
            }
        }

        #endregion

        #region Helper Functions

        private void AddMarker(SubEntidad subentidad, string style)
        {
            if (subentidad == null) return;
            var ultimaMedicion = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subentidad.Sensor.Dispositivo.Id, subentidad.Sensor.Id, DateTime.UtcNow);
            
            var ctrl = divPlano.FindControl("sub" + subentidad.Id);
            if (ctrl != null) return;

            var panel = new Panel();
            divPlano.Controls.Add(panel);

            var styleSplit = style.Split('_');
            var color = "grey";

            if (styleSplit.Length == 4)
                color = styleSplit[3];

            var bal = new BaloonTip { Url = ImagesDir + "button-" + color + ".png",
                                      Text = GetValor(subentidad, ultimaMedicion),
                                      ID = "sub" + subentidad.Id,
                                      Css = "monitor" };

            var ratio = _imageSize.Width * 1.0 / ImageWidth;
            var x = (short)(subentidad.X / ratio);
            var y = (short)(subentidad.Y / ratio);
            bal.Style.Add(HtmlTextWriterStyle.Left, x + "px");
            bal.Style.Add(HtmlTextWriterStyle.Top, y + "px");
            bal.Style.Add(HtmlTextWriterStyle.Position, "absolute");
            bal.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");

            panel.Controls.Add(bal);            

            var lbl = new Label { Text = subentidad.Descripcion, CssClass = style };
            lbl.Style.Add(HtmlTextWriterStyle.Left, (x - 5) + "px");
            lbl.Style.Add(HtmlTextWriterStyle.Top, (y + 33) + "px");
            lbl.Style.Add(HtmlTextWriterStyle.Position, "absolute");
                    
            divPlano.Controls.Add(lbl);
        }

        private string GetValor(SubEntidad subentidad, Medicion ultimaMedicion)
        {
            var valor = "<b>" + CultureManager.GetLabel("DESCRIPCION") + ":</b> " + subentidad.Descripcion + "<br>";

            switch (subentidad.Sensor.TipoMedicion.Codigo)
            {
                case "TEMP":
                    if (ultimaMedicion != null)
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> " + ultimaMedicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                        valor += "<br><b>";
                        valor += CultureManager.GetLabel("VALOR") + ":</b> ";
                        valor += ultimaMedicion.Valor;
                        valor += "<br>";

                        //EVALUO CONEXION
                        valor += "<b>" + CultureManager.GetLabel("ENERGIA") + ":</b> ";
                        var ultimoEventoConexion =
                            DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subentidad.Sensor.Id },
                                                                              new[] { Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString(),
                                                                                  Convert.ToInt32(MessageIdentifier.TemperaturePowerReconected).ToString() });

                        if (ultimoEventoConexion != null &&
                            ultimoEventoConexion.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString())
                            valor += "Off";
                        else
                            valor += "On";

                        if (ultimoEventoConexion != null)
                            valor += " (" + ultimoEventoConexion.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + ")";

                        valor += "<br>";

                        //EVALUO DESCONGELAMIENTO
                        valor += "<b>" + CultureManager.GetLabel("DESCONGELAMIENTO") + ":</b> ";

                        var ultimoEventoDescongelamiento =
                            DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subentidad.Sensor.Id },
                                                                              new[] { Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString(),
                                                                                  Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonUnpressed).ToString() });

                        if (ultimoEventoDescongelamiento != null &&
                            ultimoEventoDescongelamiento.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString())
                            valor += "On";
                        else
                            valor += "Off";

                        if (ultimoEventoDescongelamiento != null)
                            valor += " (" + ultimoEventoDescongelamiento.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + ")";
                    }
                    else
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("VALOR") + ":</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("ENERGIA") + ":</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("DESCONGELAMIENTO") + ":</b> -";
                    }
                    break;
                case "EST":
                    if (ultimaMedicion != null)
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> " + ultimaMedicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                        valor += "<br>";

                        //EVALUO CHECK
                        var ultimoCheckPoint =
                            DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subentidad.Sensor.Id },
                                                                              new[] { Convert.ToInt32(MessageIdentifier.CheckpointReached).ToString() });

                        if (ultimoCheckPoint == null)
                        {
                            //EVALUO 911
                            valor += "<b>911:</b> ";
                            var ultimoEvento911 =
                                DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subentidad.Sensor.Id },
                                                                                  new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton1).ToString() });

                            if (ultimoEvento911 != null &&
                                ultimoEvento911.Fecha > DateTime.UtcNow.AddMinutes(-10))
                                valor += "On (" + ultimoEvento911.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + ")";
                            else
                                valor += "Off";
                            valor += "<br>";

                            //EVALUO AMBULANCIA
                            valor += "<b>" + CultureManager.GetLabel("AMBULANCIA") + ":</b> ";
                            var ultimoEventoAmbulancia =
                                DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subentidad.Sensor.Id },
                                                                                  new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton2).ToString() });

                            if (ultimoEventoAmbulancia != null &&
                                ultimoEventoAmbulancia.Fecha > DateTime.UtcNow.AddMinutes(-10))
                                valor += "On (" + ultimoEventoAmbulancia.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + ")";
                            else
                                valor += "Off";
                            valor += "<br>";

                            //EVALUO BOMBEROS
                            valor += "<b>" + CultureManager.GetLabel("BOMBEROS") + ":</b> ";
                            var ultimoEventoBombero =
                                DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subentidad.Sensor.Id },
                                                                                  new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton3).ToString() });

                            if (ultimoEventoBombero != null &&
                                ultimoEventoBombero.Fecha > DateTime.UtcNow.AddMinutes(-10))
                                valor += "On (" + ultimoEventoBombero.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") + ")";
                            else
                                valor += "Off";
                        }
                    }
                    else
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> -<br>";
                        valor += "<b>911:</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("AMBULANCIA") + ":</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("BOMBEROS") + ":</b> -";
                    }
                    break;
                case "NU":
                    if (ultimaMedicion != null)
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> " + ultimaMedicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                        valor += "<br>";
                        valor += "<b>" + CultureManager.GetLabel("VALOR") + ":</b> ";
                        valor += ultimaMedicion.Valor;
                    }
                    else
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("VALOR") + ":</b> -";
                    }
                    break;
                case "TIME":
                    if (ultimaMedicion != null)
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> " + ultimaMedicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                        valor += "<br>";
                        valor += "<b>" + CultureManager.GetLabel("VALOR") + ":</b> ";
                        var time = new TimeSpan(0, 0, 0, int.Parse(ultimaMedicion.Valor));
                        valor += time.ToString();
                    }
                    else
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("VALOR") + ":</b> -";
                    }
                    break;
                default:
                    if (ultimaMedicion != null)
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> " + ultimaMedicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
                        valor += "<br>";
                        valor += "<b>" + CultureManager.GetLabel("VALOR") + ":</b> ";
                        valor += ultimaMedicion.Valor;
                    }
                    else
                    {
                        valor += "<b>" + CultureManager.GetLabel("ULTIMO_REPORTE") + ":</b> -<br>";
                        valor += "<b>" + CultureManager.GetLabel("VALOR") + ":</b> -";
                    }
                    break;
            }
            
            return valor;
        }

        private void JsAlert(string msg)
        {
            ScriptManager.RegisterStartupScript(this, typeof(string), "alert", "alert('" + msg + "')", false);
        }

        private void RegisterStatusCheck()
        {
            var script = @"var lastUpdate = new Date(); setInterval(check_status, 10000);
        function check_status(){
            var secs = Math.floor(new Date().getTime() - lastUpdate.getTime())/1000;
            if (secs > 20){
                $get('" + connection_status.ClientID + @"').src = '../../Operacion/connected.png';
                $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_DISCONNECTED") + @"';
                if(secs < 60)"
                         + ClientScript.GetPostBackEventReference(connection_status, "") +
                         @";return false;}
            else {
                $get('" + connection_status.ClientID + @"').src = '../../Operacion/connected.png';
                $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_CONNECTED") + @"'; 
                return true;}}";
            ClientScript.RegisterStartupScript(typeof(string), "check_status", script, true);
            connection_status.OnClientClick = "return !check_status();";
        }

        private void RegisterEvents()
        {
            var script =
                string.Format(
                    @" var PopupPanelOpen = false;
                function ClearEvents(){{ 
                    var cont = $get('{1}');                           
                    cont.innerHTML = ''; 
                }}
                function ShowEvents(){{ HideDetail(); $get('{0}').style.display = '';PopupPanelOpen = true; }} 
                function HideEvents(){{ $get('{0}').style.display = 'none';PopupPanelOpen = false; }} 
                function ToggleEvents(){{ if($get('{0}').style.display == 'none') ShowEvents(); else HideEvents(); }} 
                function AddEvent(txt){{ 
                    var cont = $get('{1}');
                    if(cont.childNodes.length > 20)
                        cont.removeChild(cont.lastChild);
                            
                    cont.innerHTML = txt + cont.innerHTML; 
                }}
                function ShowDetail(){{  $get('{1}').style.display = 'none'; $get('{2}').style.display = '';PopupPanelOpen = true; }}
                function HideDetail(){{ $get('{1}').style.display = ''; $get('{2}').style.display = 'none'; PopupPanelOpen = false;}}
                function LoadDetail(id){{ $get('ifrPopupDetail').src = '../MonitorDeEntidades/InfoEventM2M.aspx?evt='+id; ShowDetail(); }}
                ",
                    panelPopup.ClientID,
                    panelPopupEvents.ClientID,
                    panelPopupDetail.ClientID
                    );
            ClientScript.RegisterStartupScript(typeof(string), "popup_events", script, true);
        }

        private void LoadPlano()
        {
            if (cbEntidad.Selected <= 0) return;

            var entidad = DAOFactory.EntidadDAO.FindById(cbEntidad.Selected);
            var img = new Image
                          {
                              ID = "imgPlano",
                              ImageUrl = Config.Directory.AttachDir + entidad.Url,
                              Width = Unit.Pixel(ImageWidth)
                          };
            img.Style.Add(HtmlTextWriterStyle.Position, "absolute");

            divPlano.Controls.Clear();
            divPlano.Controls.Add(img);

            if (entidad.Url != null)
            {
                var im = Server.MapPath(Config.Directory.AttachDir + entidad.Url);
                using (var bitmap = System.Drawing.Image.FromFile(im))
                {
                    _imageSize = bitmap.Size;
                }
            }
        }

        private void ParsearQueryString()
        {
            if (Request.QueryString.AllKeys.Contains("ID_ENTIDAD"))
            {
                var entidad = DAOFactory.EntidadDAO.FindById(Convert.ToInt32(Request.QueryString["ID_ENTIDAD"]));
                cbLocacion.SetSelectedValue(entidad.Empresa != null
                                            ? entidad.Empresa.Id
                                            : entidad.Linea != null
                                              ? entidad.Linea.Empresa.Id
                                              : 0);
                cbPlanta.SetSelectedValue(entidad.Linea != null
                                          ? entidad.Linea.Id
                                          : 0);
                cbTipoEntidad.SetSelectedValue(entidad.TipoEntidad.Id);
                cbEntidad.SelectedValue = entidad.Id.ToString();
                cbSubEntidad.SelectAll();

                SetSelectedSubEntidades();
            }

            if (Request.QueryString.AllKeys.Contains("ID_SUBENTIDAD"))
            {
                var subentidad = DAOFactory.SubEntidadDAO.FindById(Convert.ToInt32(Request.QueryString["ID_SUBENTIDAD"]));
                cbLocacion.SetSelectedValue(subentidad.Empresa != null
                                            ? subentidad.Empresa.Id
                                            : subentidad.Linea != null
                                              ? subentidad.Linea.Empresa.Id
                                              : 0);
                cbPlanta.SetSelectedValue(subentidad.Linea != null
                                          ? subentidad.Linea.Id
                                          : 0);
                cbTipoEntidad.SetSelectedValue(subentidad.Entidad.TipoEntidad.Id);
                cbEntidad.SetSelectedValue(subentidad.Entidad.Id);
                cbSubEntidad.SelectedValue = subentidad.Id.ToString();

                SetSelectedSubEntidades();
            }
        }

        #endregion

        #region Selection

        private void SetSelectedSubEntidades()
        {
            SelectedSubEntidades = null;

            foreach (var id in from ListItem li in cbSubEntidad.Items where li.Selected select Convert.ToInt32((string) li.Value))
            {
                SelectSubEntidad(id);
            }

            if (SelectedSubEntidades == null) return;

            LoadPlano();

            foreach (var subentidad in SelectedSubEntidades)
            {
                var style = GetMarkerStyle(subentidad);
                AddMarker(subentidad, style);
            }
        }

        private void SelectSubEntidad(int id)
        {
            var subentidad = DAOFactory.SubEntidadDAO.FindById(id);
            var sv = SelectedSubEntidades;
            if (!sv.Contains(subentidad)) sv.Add(subentidad);
            SelectedSubEntidades = sv;
            var li = cbSubEntidad.Items.FindByValue(id.ToString());
            if (li != null) li.Selected = true;
        }

        private string GetMarkerStyle(SubEntidad subEntidad)
        {
            var style = "";

            var ultimaMedicion = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, DateTime.UtcNow);
            var ultimoEventoDescongelamiento = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                                 new[] { Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString(),
                                                                                                         Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonUnpressed).ToString() });
            var ultimoEventoConexion = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                         new[] { Convert.ToInt32(MessageIdentifier.TemperaturePowerReconected).ToString(),
                                                                                                 Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString() });
            var ultimoEventoBotonera = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                         new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton1).ToString(),
                                                                                                 Convert.ToInt32(MessageIdentifier.KeyboardButton2).ToString(),
                                                                                                 Convert.ToInt32(MessageIdentifier.KeyboardButton3).ToString() });
            var enDescogelamiento = ultimoEventoDescongelamiento != null
                                 && ultimoEventoDescongelamiento.Mensaje != null
                                 && ultimoEventoDescongelamiento.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString();

            var energiaDesconectada = ultimoEventoConexion != null
                                   && ultimoEventoConexion.Mensaje != null
                                   && ultimoEventoConexion.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString();

            var enEmergencia = ultimoEventoBotonera != null && ultimoEventoBotonera.Fecha > DateTime.UtcNow.AddMinutes(-10);

            if (ultimaMedicion != null)
            {
                switch (ultimaMedicion.TipoMedicion.Codigo)
                {
                    case "TEMP":
                        if (enDescogelamiento)
                            style = "ol_marker_labeled_blue";
                        else
                        {
                            if (energiaDesconectada 
                            || (ultimaMedicion.ValorDouble > subEntidad.Maximo && subEntidad.ControlaMaximo)
                            || (ultimaMedicion.ValorDouble < subEntidad.Minimo && subEntidad.ControlaMinimo))
                                return "ol_marker_labeled_red";

                            if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-3))
                            {
                                if (style != "ol_marker_labeled_blue")
                                    style = "ol_marker_labeled";
                            }
                            else
                            {
                                if (style == "")
                                    style = "ol_marker_labeled_green";
                            }
                        }
                        break;
                    case "EST":
                        if (enEmergencia)
                            return "ol_marker_labeled_red";

                        if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-3))
                        {
                            if (style != "ol_marker_labeled_blue")
                                style = "ol_marker_labeled";
                        }
                        else
                        {
                            if (style == "")
                                style = "ol_marker_labeled_green";
                        }
                        break;
                    case "NU":
                        if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-2))
                        {
                            if (style != "ol_marker_labeled_blue")
                                style = "ol_marker_labeled";
                        }
                        else
                        {
                            if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-1))
                            {
                                if (style == "ol_marker_labeled_green" || style == "")
                                    style = "ol_marker_labeled_yellow";
                            }
                            else
                            {
                                if (style == "")
                                    style = "ol_marker_labeled_green";
                            }
                        }
                        break;
                    default:
                        if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-2))
                        {
                            if (style != "ol_marker_labeled_blue")
                                style = "ol_marker_labeled";
                        }
                        else
                        {
                            if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-1))
                            {
                                if (style == "ol_marker_labeled_green" || style == "")
                                    style = "ol_marker_labeled_yellow";
                            }
                            else
                            {
                                if (style == "")
                                    style = "ol_marker_labeled_green";
                            }
                        }
                        break;
                }
            }

            return style != "" ? style : "ol_marker_labeled";
        }

        #endregion
    }
}