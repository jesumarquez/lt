using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Operacion.Consola
{
    public partial class ConsolaM2M : OnLineSecuredPage, ICallbackEventHandler
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string GetRefference() { return "CONSOLA_M2M"; }

        public const int MaxResults = 100;
        public const int TimerInterval = 20;
        private bool _auto;
        private List<int> _subEntidades;
        private int _lastId;
        private List<string> _mensajes;
        private bool _selectAllTipoMensaje;
        private bool _selectAllMensajes;
        private bool _selectAllSubEntidades;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RegisterSelectAll(cbSubEntidad.ClientID, PanelEntidad);
            RegisterSelectAll(cbMensajes.ClientID, PanelMensajes);
            RegisterCallbackFunctions();
            RegisterClockFunctions();

            RegisterExtJsStyleSheet();

            _selectAllMensajes = true;
            _selectAllSubEntidades = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_selectAllTipoMensaje) SelectAllTipoMensaje();
            if (_selectAllMensajes) SelectAllMensajes();
            if (_selectAllSubEntidades) SelectAllSubEntidades();

            if (_selectAllMensajes || _selectAllSubEntidades) Refresh();

            base.OnPreRender(e);
        }

        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
            if (_subEntidades == null || _mensajes == null || _subEntidades.Count == 0 || _mensajes.Count == 0)
                return _auto ? "addMessages([], true, 0);" : "addMessages('', false, 0);";

            var eventosConsola = !_auto
                                     ? DAOFactory.LogEventoDAO.GetAllMensajesConsola(_subEntidades, _mensajes, MaxResults)
                                     : DAOFactory.LogEventoDAO.GetLastMensajesConsola(_subEntidades, _mensajes, _lastId);

            var maxId = eventosConsola.Count == 0 ? 0 : (from LogEvento e in eventosConsola select e.Id).Max();

            var eventos = (from LogEvento evento in eventosConsola select _auto ? GetRow(evento) : GetRowText(evento)).ToList();
            
            var textoEventos = _auto ? string.Concat("[", string.Join(",", eventos.ToArray()), "]")
                                     : string.Concat("\"", string.Join("", eventos.ToArray()), "\"");
            
            return string.Format("addMessages({0}, {1}, {2});",
                                 textoEventos,
                                 (_auto ? "true" : "false"),
                                 maxId);
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            var args = eventArgument;
            _auto = args.StartsWith("AUTO:");
            if (_auto) 
                args = args.Substring(5);

            var idx = args.IndexOf(':');
            _lastId = Convert.ToInt32(args.Substring(0, idx));
            args = args.Substring(idx + 1);

            var deserialized = DeserializeParameters(args);
            _subEntidades = deserialized[0] as List<int>;
            _mensajes = deserialized[1] as List<string>;
        }

        #endregion

        #region Control Events

        protected void CbPlanta_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllTipoMensaje = _selectAllSubEntidades = _selectAllMensajes = true;
        }

        protected void CbTipoEntidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllSubEntidades = true;
        }

        protected void CbEntidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllSubEntidades = true;
        }

        protected void CbSubEntidad_SelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack || IsCallback) Refresh(); }

        protected void CbTipoMensaje_SelectedIndexChanged(object sender, EventArgs e) { _selectAllMensajes = true;  }

        protected void CbMensajes_SelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack || IsCallback) Refresh(); }

        protected void BtSalidaDigitalOn_Click(object sender, EventArgs e)
        {
            SendMessage(M2mMessageSender.Comandos.SetParameter, "On"); 
        }

        protected void BtSalidaDigitalOff_Click(object sender, EventArgs e)
        {
            SendMessage(M2mMessageSender.Comandos.SetParameter, "Off");
        }

        #endregion

        #region Register JavaScript
        
        private void RegisterSelectAll(string controlId, WebControl panel)
        {
            const string selectFunction = "function {0}(){{ var cb = document.getElementById('{1}'); var st = cb.selectedIndex < 0; for(var i = 0; i < cb.options.length; i++) {{cb.options[i].selected = st;}} {2};}}";
            var functionName = string.Concat("select_" + controlId);
            ClientScript.RegisterStartupScript(typeof(string), functionName, string.Format(selectFunction, functionName, controlId, ClientScript.GetPostBackEventReference(FindControl(controlId), "")), true);
            panel.Style.Add("cursor", "pointer");
            panel.Attributes.Add("onclick", functionName + "();");
        }

        protected void RegisterCallbackFunctions()
        {
            var cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerData", "context");
            var callbackScript = @"function CallServer(arg, context){" + cbReference + "; }";

            const string receiveServerData = @"function ReceiveServerData(result){eval(result);delete result;}";

            ClientScript.RegisterStartupScript(typeof(string), "ReceiveServerData", receiveServerData, true);
            ClientScript.RegisterStartupScript(typeof(string), "CallServer", callbackScript, true);
        } 

        protected void RegisterClockFunctions()
        {
            var clock = string.Format("$get('clock').innerHTML = '{0}<blink>:</blink>{1}';", DateTime.UtcNow.ToDisplayDateTime().ToString("HH"), DateTime.UtcNow.ToDisplayDateTime().ToString("mm"));
            var startClock = string.Format("startClock(new Date({0},{1},{2},{3},{4},{5}));", DateTime.UtcNow.ToDisplayDateTime().Year, DateTime.UtcNow.ToDisplayDateTime().Month, DateTime.UtcNow.ToDisplayDateTime().Day, DateTime.UtcNow.ToDisplayDateTime().Hour, DateTime.UtcNow.ToDisplayDateTime().Minute, DateTime.UtcNow.ToDisplayDateTime().Second);
            ClientScript.RegisterStartupScript(typeof(string), "clock", clock, true);
            ClientScript.RegisterStartupScript(typeof(string), "startClock", startClock, true);
        }
        
        #endregion

        #region Helper Methods

        private static string GetRow(LogEvento evento)
        {
            const string template = "\"{0}\"";
            return string.Format(template, GetRowText(evento));
        }

        private static string GetRowText(LogEvento evento)
        {
            const string template = "<table o='{8}' style='background-color:#{5};color:#{6};' onclick='s({7})'><tr><td class='r1'><h6>{0}</h6> {1}</td><td class='r2'>{2}</td><td class='r3'>{3}</td><td>{4}</td></tr></table>";
            var col = evento.Accion != null
                          ? Color.FromArgb(100, evento.Accion.Red, evento.Accion.Green, evento.Accion.Blue)
                          : Color.White;
            var fecha = evento.Fecha.ToDisplayDateTime();
            return string.Format(template,
                                 fecha.ToString("dd-MM"),
                                 fecha.ToString("HH:mm:ss"),
                                 evento.SubEntidad.Entidad.Descripcion,
                                 evento.SubEntidad.Descripcion,
                                 evento.Texto,
                                 evento.Accion != null ? evento.Accion.RGB : "DDDDDD",
                                 col.GetBrightness() > 0.45 ? "000000" : "FFFFFF",
                                 evento.Id,
                                 fecha.ToString("yyyyMMddHHmmss"));
        }

        private string SerializeParameters()
        {
            var subentidades = Enumerable.ToArray<string>((from id in cbSubEntidad.SelectedValues select id.ToString()));
            var mensajes = Enumerable.ToArray<string>((from id in cbMensajes.SelectedValues select id.ToString()));
            return string.Concat(string.Join(",", subentidades), ";", string.Join(",", mensajes));
        }
        
        private static IList[] DeserializeParameters(string value)
        {
            var splitted = value.Split(';');
            var strSubentidades = splitted[0].Split(',');
            var strMensajes = splitted[1].Split(',');
            var subentidades = (from id in strSubentidades select Convert.ToInt32(id)).ToList();
            var mensajes = (from id in strMensajes select id.PadLeft(2,'0')).ToList();
            return new IList[] { subentidades, mensajes };
        }

        private void Refresh()
        {
            hidSerialized.Value = SerializeParameters();
            ScriptManager.RegisterStartupScript(this, typeof(string), "refresh", "setTimeout(refresh, 500, false);", true);
        }
        
        private void SelectAllTipoMensaje()
        {
            cbTipoMensaje.SetSelectedValue(-1);
        }
        
        private void SelectAllMensajes()
        {
            foreach (ListItem li in cbMensajes.Items) li.Selected = true;
            updFiltros.Update();
        }
        
        private void SelectAllSubEntidades()
        {
            foreach (ListItem li in cbSubEntidad.Items) li.Selected = true;
            updFiltros.Update();
        }

        private void SendMessage(string cmd, string par)
        {
            if (cbSubEntidadMensaje.SelectedIndex < 0)
            {
                JsAlert(string.Format(CultureManager.GetError("NO_SELECTED"), CultureManager.GetEntity("PARENTI81")));
                return;
            }

            var ok = 0;
            var error = 0;

            var sent = string.Empty;
            var failed = string.Empty;

            foreach (ListItem li in cbSubEntidadMensaje.Items)
            {
                if (!li.Selected) continue;

                var c = DAOFactory.SubEntidadDAO.FindById(Convert.ToInt32(li.Value));

                if (c.Sensor == null || c.Sensor.Dispositivo == null)
                {   
                    JsAlert(string.Format(CultureManager.GetSystemMessage("UNASIGNED_VEHICLE"), c.Descripcion));
                    continue;
                }

                var dispositivo = c.Sensor.Dispositivo;
                var message = M2mMessageSender.Create(dispositivo, new M2mMessageSaver(DAOFactory)).AddCommand(cmd);

                var cfgValue = par.Equals("On") ? "true" : "false";
                message.AddConfigParameter("DigitalOutput1", cfgValue, 0);
                
                var sendState = message.Send();

                if (sendState)
                {
                    ok++;
                    sent += "\\n" + li.Text;
                }
                else
                {
                    error++;
                    failed += "\\n" + li.Text;
                }
            }

            if (ok > 0) JsAlert(string.Format(CultureManager.GetSystemMessage("MESSAGE_SENT"), sent));
            if (error > 0) JsAlert(string.Format(CultureManager.GetError("MESSAGE_NOT_SENT"), failed));
        }

        private void JsAlert(string msg) { ScriptManager.RegisterStartupScript(Page, typeof(string), "al", "alert(\'" + msg + "\');", true); }

        #endregion
    }
}
