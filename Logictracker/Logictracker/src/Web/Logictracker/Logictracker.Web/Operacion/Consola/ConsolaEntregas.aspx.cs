using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Operacion.Consola
{
    public partial class ConsolaEntregas : OnLineSecuredPage, ICallbackEventHandler
    {
        protected override string GetRefference() { return "CONSOLA"; }

        public const int MaxResults = 100;
        public const int TimerInterval = 20;
        private bool _auto;
        private List<int> _coches;
        private int _lastId;

        private bool _selectAllVehiculos;
        protected override InfoLabel LblInfo { get { return null; } }

        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
            if (_coches == null || _coches.Count == 0)
                return _auto ? "addMessages([], true, 0);" : "addMessages('', false, 0);";

            if (!_auto) _lastId = -1;

            var mensajesConsola = DAOFactory.LogMensajeDAO.GetMensajesConsolaEntregas(_coches, _lastId, MaxResults);
            var eventos = DAOFactory.EvenDistriDAO.GetByMensajes(mensajesConsola).OrderByDescending(e => e.LogMensaje.Fecha);

            var maxId = !mensajesConsola.Any() ? 0 : (from EvenDistri e in eventos select e.LogMensaje.Id).Max();

            var mensajes = (from EvenDistri evento in eventos select _auto ? GetRow(evento) : GetRowText(evento)).ToList();
            var textoMensajes = _auto
                                    ? string.Concat("[", string.Join(",", mensajes.ToArray()), "]")
                                    : string.Concat("\"", string.Join("", mensajes.ToArray()), "\"");
            var script = string.Format("addMessages({0}, {1}, {2});",
                                       textoMensajes,
                                       _auto ? "true" : "false",
                                       maxId);

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
        }

        #endregion

        #region Control Events

        protected void CbPlantaSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllVehiculos = true;
        }

        protected void CbTipoVehiculoSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllVehiculos = true;
        }
    
        protected void CbVehiculoSelectedIndexChanged(object sender, EventArgs e) { if (IsPostBack || IsCallback) Refresh(); }

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

        private static string GetRow(EvenDistri mensaje)
        {
            const string template = "\"{0}\"";
            return string.Format(template, GetRowText(mensaje));
        }

        private static string GetRowText(EvenDistri mensaje)
        {
            const string template = "<table o='{9}' style='background-color:#{7};color:#{8};'><tr><td style='width: 100px; align: left;'>{0} {1}</td><td style='width: 90px; align: left;'>{2}</td><td style='width: 120px; align: left;'>{3}</td><td style='width: 80px; align: left;'>{4}</td><td style='width: 80px; align: left;'>{5}</td><td style='width: 80px; align: left;'>{6}</td></tr></table>";
            var evento = mensaje.LogMensaje;

            var fecha = evento.Fecha.ToDisplayDateTime();
            var estado = GetEstado(evento);
            var viaje = mensaje.Viaje;
            var entrega = mensaje.Entrega;
            var proxima = GetProxima(mensaje);
            return string.Format(template,
                                 fecha.ToString("dd-MM"),
                                 fecha.ToString("HH:mm:ss"),
                                 evento.Coche.Interno + " - (" + evento.Coche.Patente + ")",
                                 viaje.Codigo,
                                 entrega != null ? entrega.Descripcion : string.Empty,
                                 estado,
                                 proxima,
                                 "DDDDDD",
                                 "000000",
                                 fecha.ToString("yyyyMMddHHmmss"));
        }

        private static string GetProxima(EvenDistri evento)
        {
            var proxima = string.Empty;

            if (evento.LogMensaje.Mensaje.Codigo == MessageCode.CicloLogisticoIniciado.GetMessageCode())
            {
                var primerEntrega = evento.Viaje.Detalles.FirstOrDefault(d => d.Linea == null);
                if (primerEntrega != null) proxima = primerEntrega.Descripcion;
            }
            else if (evento.LogMensaje.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplido.GetMessageCode()
                  || evento.LogMensaje.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoEntrada.GetMessageCode()
                  || evento.LogMensaje.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode()
                  || evento.LogMensaje.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoSalida.GetMessageCode())
            {
                if (evento.Entrega != null)
                {
                    var entregas = evento.Viaje.Detalles;
                    var orden = evento.Entrega.Orden;
                    var prox = orden < entregas.Count() 
                                ? entregas.FirstOrDefault(e => e.Orden == evento.Entrega.Orden + 1)
                                : null;
                    if (prox != null) proxima = prox.Descripcion;
                }
            }

            return proxima;
        }

        private static string GetEstado(LogMensajeBase evento)
        {
            var estado = string.Empty;

            if (evento.Mensaje.Codigo == MessageCode.CicloLogisticoIniciado.GetMessageCode()) estado = "En viaje";
            else if (evento.Mensaje.Codigo == MessageCode.CicloLogisticoCerrado.GetMessageCode()) estado = "En base";
            else if (evento.Texto.ToUpperInvariant().Contains("ENTRADA")) estado = "En cliente";
            else if (evento.Texto.ToUpperInvariant().Contains("SALIDA")) estado = "En viaje";
            else if (evento.Texto.ToUpperInvariant().Contains("MANUAL")) estado = "En tarea";

            return estado;
        }

        private string SerializeParameters()
        {
            var coches = Enumerable.ToArray<string>((from id in cbVehiculo.SelectedValues select id.ToString()));
            return string.Concat(string.Join(",", coches), ";");
        }
    
        private static IList[] DeserializeParameters(string value)
        {
            var splitted = value.Split(';');
            var strCoches = splitted[0].Split(',');
            var coches = (from id in strCoches select Convert.ToInt32(id)).ToList();
            return new IList[] { coches };
        }

        private void Refresh()
        {
            hidSerialized.Value = SerializeParameters();
            ScriptManager.RegisterStartupScript(this, typeof(string), "refresh", "setTimeout(refresh, 500, false);", true);
        }
    
        private void SelectAllVehiculos()
        {
            foreach (ListItem li in cbVehiculo.Items) li.Selected = true;
            updFiltros.Update();
        }
    
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RegisterSelectAll(cbVehiculo.ClientID, PanelVehiculo);
            RegisterCallbackFunctions();
            RegisterClockFunctions();

            RegisterExtJsStyleSheet();

            _selectAllVehiculos = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_selectAllVehiculos) SelectAllVehiculos();

            if (_selectAllVehiculos) Refresh();

            base.OnPreRender(e);
        }
    }
}
