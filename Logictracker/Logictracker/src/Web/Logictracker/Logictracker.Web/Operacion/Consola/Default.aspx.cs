using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Operacion.Consola
{
    public partial class Operacion_Consola_Default : OnLineSecuredPage, ICallbackEventHandler
    {
        public const int MaxResults = 100;
        public const int TimerInterval = 15;
        private bool _auto;
        private List<int> _coches;
        private int _lastId;
        private List<string> _mensajes;

        private bool _selectAllTipoMensaje;
        private bool _selectAllMensajes;
        private bool _selectAllVehiculos;
        protected override InfoLabel LblInfo { get { return null; } }

        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
            if (_coches == null || _mensajes == null || _coches.Count == 0 || _mensajes.Count == 0)
                return _auto ? "addMessages([], true, 0);" : "addMessages('', false, 0);";

            if (!_auto)
                _lastId = -1;

            var mensajesConsola = DAOFactory.LogMensajeDAO.GetMensajesConsola(_coches, _mensajes, _lastId, MaxResults);

            var maxId = !mensajesConsola.Any() ? 0 : (from LogMensaje m in mensajesConsola select m.Id).Max();

            var mensajes = (from LogMensaje mensaje in mensajesConsola select _auto ? GetRow(mensaje) : GetRowText(mensaje)).ToList();
            var sonido = mensajesConsola.FirstOrDefault(x => x.Accion != null && x.Accion.EsAlarmaSonora);
            var textoMensajes = _auto
                                    ? string.Concat("[", string.Join(",", mensajes.ToArray()), "]")
                                    : string.Concat("\"", string.Join("", mensajes.ToArray()), "\"");
            var script = string.Format("addMessages({0}, {1}, {2});",
                                       textoMensajes,
                                       (_auto ? "true" : "false"),
                                       maxId);

            if(sonido != null)
            {
                var scriptSonido = GetSoundScript(string.Concat(Config.Directory.SoundsDir, sonido.Accion.Sonido.URL));
                script += scriptSonido;
            }
            return script;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            var args = eventArgument;
            _auto = args.StartsWith("AUTO:");
            if (_auto) args = args.Substring(5);

            var idx = args.IndexOf(':');
            _lastId = Convert.ToInt32(args.Substring(0, idx));
            args = args.Substring(idx + 1);

            var deserialized = DeserializeParameters(args);
            _coches = deserialized[0] as List<int>;
            _mensajes = deserialized[1] as List<string>;
        }

        #endregion

        #region Control Events

        protected void CbPlantaSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllTipoMensaje= _selectAllVehiculos = _selectAllMensajes = true;
            Mensajeria1.IdEmpresa = cbLocacion.Selected;
            Mensajeria1.IdLinea = cbPlanta.Selected;
        }

        protected void CbTipoVehiculoSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllVehiculos = true;
            Mensajeria1.UpdateVehicles();
        }
    
        protected void CbVehiculoSelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack || IsCallback) Refresh(); }

        protected void CbTipoMensajeSelectedIndexChanged(object sender, EventArgs e) { _selectAllMensajes = true;  }

        protected void CbMensajesSelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack || IsCallback) Refresh(); }

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

        private static string GetRow(LogMensaje mensaje)
        {
            const string template = "\"{0}\"";
            return string.Format(template, GetRowText(mensaje));
        }

        private static string GetRowText(LogMensaje mensaje)
        {
            const string template = "<table o='{8}' style='background-color:#{5};color:#{6};' onclick='s({7});'><tr><td class='r1'><h6>{0}</h6> {1}</td><td class='r2'>{2}</td><td class='r3'>{3}</td><td>{4}</td><td class='r4'>{9}</td></tr></table>";
            var col = mensaje.Accion != null
                          ? Color.FromArgb(100, mensaje.Accion.Red, mensaje.Accion.Green, mensaje.Accion.Blue)
                          : Color.White;
            var fecha = mensaje.Fecha.ToDisplayDateTime();
            return string.Format(template,
                                 fecha.ToString("dd-MM"),
                                 fecha.ToString("HH:mm:ss"),
                                 mensaje.Coche.Interno,
                                 mensaje.Chofer != null ? mensaje.Chofer.Entidad.Descripcion : string.Empty,
                                 mensaje.Texto.Replace("\"", "\\\""),
                                 mensaje.Accion != null ? mensaje.Accion.RGB : "DDDDDD",
                                 col.GetBrightness() > 0.45 ? "000000" : "FFFFFF",
                                 mensaje.Id,
                                 fecha.ToString("yyyyMMddHHmmss"),
                                 mensaje.TieneFoto ? "<div class='withPhoto' onclick='showFoto("+mensaje.Id+");'></div>" : string.Empty
                );
        }

        private string SerializeParameters()
        {
            var coches = (from id in cbVehiculo.SelectedValues select id.ToString()).ToArray();
            var mensajes = (from id in cbMensajes.SelectedValues select id.ToString()).ToArray();
            return string.Concat(string.Join(",", coches), ";", string.Join(",", mensajes));
        }
    
        private static IList[] DeserializeParameters(string value)
        {
            var splitted = value.Split(';');
            var strCoches = splitted[0].Split(',');
            var strMensajes = splitted[1].Split(',');
            var coches = (from id in strCoches select Convert.ToInt32(id)).ToList();
            var mensajes = (from id in strMensajes select id.PadLeft(2,'0')).ToList();
            return new IList[] { coches, mensajes };
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
    
        private void SelectAllVehiculos()
        {
            foreach (ListItem li in cbVehiculo.Items) li.Selected = true;
            updFiltros.Update();
        }
    
        #endregion

        protected override string GetRefference() { return "CONSOLA"; }

        protected override void OnInit(EventArgs e)
        {
            if (WebSecurity.ShowDriver) cbVehiculo.ShowDriverName = true;

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            EmbedSoundPlayer();

            RegisterSelectAll(cbVehiculo.ClientID, PanelVehiculo);
            RegisterSelectAll(cbMensajes.ClientID, PanelMensajes);
            RegisterCallbackFunctions();
            RegisterClockFunctions();

            RegisterExtJsStyleSheet();

            _selectAllMensajes = true;
            _selectAllVehiculos = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_selectAllTipoMensaje) SelectAllTipoMensaje();
            if (_selectAllMensajes) SelectAllMensajes();
            if (_selectAllVehiculos) SelectAllVehiculos();

            if (_selectAllMensajes || _selectAllVehiculos) Refresh();

            base.OnPreRender(e);
        }
    }
}
