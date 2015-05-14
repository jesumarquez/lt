#region Usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Services.Helpers;
using Logictracker.Web.Monitor.Base;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using LatLon = Logictracker.Web.Monitor.Base.LatLon;

#endregion

namespace Logictracker.Web.Monitor
{
    public enum EventMethods { Callback, PostBack }
    public class MapMode
    {
        public const string DrawPolygon = "drawpolygon";
        public const string MeasurePath = "measurepath";
        public const string MeasurePolygon = "measurepolygon";
        public const string Pan = "pan";
    }

    [ToolboxData("<{0}:Monitor ID=\"Monitor1\" runat=\"server\"></{0}:Monitor>")]
    public class Monitor : WebControl, ICallbackEventHandler, IPostBackDataHandler
    {
        #region Public Events

        /// <summary>
        /// Evento del Callback
        /// </summary>
        public event EventHandler<MonitorEventArgs> Tick;

        public event EventHandler<MonitorClickEventArgs> Click;

        public event EventHandler<MonitorDrawPolygonEventArgs> DrawPolygon;

        public event EventHandler<MonitorDrawSquareEventArgs> DrawSquare;

        public event EventHandler<MonitorDrawCircleEventArgs> DrawCircle;

        public event EventHandler<MonitorDrawLineEventArgs> DrawLine;

        public event EventHandler<MonitorModifyFeatureEventArgs> FeatureModified;

        public event EventHandler<CallbackEventArgs> Callback;

        public event EventHandler<PostbackEventArgs> ContextMenuPostback;

        public event EventHandler<MapMoveEventArgs> MapMove;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Register the control as a post back required control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.RegisterRequiresPostBack(this);

            var scriptManager = ScriptManager.GetCurrent(Page);

            if (scriptManager != null) scriptManager.RegisterAsyncPostBackControl(this);
        }
       
        /// <summary>
        /// Llama al evento Tick pasando eventArgument como dato del MonitorEventArgs
        /// </summary>
        /// <param name="eventArgument">Argumento del evento</param>
        protected void OnTick(string eventArgument)
        {
            if (Tick != null)
            {                
                Tick(this, new MonitorEventArgs(eventArgument));
            }
        }

        protected void OnClick(double lat, double lon)
        {
            if (Click != null) Click(this, new MonitorClickEventArgs(lat, lon));
        }

        protected void OnDrawPolygon(List<PointF> points)
        {
            if(DrawPolygon != null) DrawPolygon(this, new MonitorDrawPolygonEventArgs(points));
        }

        protected void OnDrawSquare(Bounds bounds)
        {
            if (DrawSquare != null) DrawSquare(this, new MonitorDrawSquareEventArgs(bounds));
        }

        protected void OnDrawCircle(double lat, double lon, int radio)
        {
            if (DrawCircle != null) DrawCircle(this, new MonitorDrawCircleEventArgs(lat, lon, radio));
        }
        protected void OnDrawLine(List<PointF> points)
        {
            if (DrawLine != null) DrawLine(this, new MonitorDrawLineEventArgs(points));
        }
        protected void OnFeatureModified(string tipo, List<PointF> points)
        {
            if (FeatureModified != null) FeatureModified(this, new MonitorModifyFeatureEventArgs(tipo, points));
        }
        protected void OnCallback(string commandName, string commandArgs, double lat, double lon)
        {
            if(Callback != null) Callback(this, new CallbackEventArgs(commandName, commandArgs, lat, lon));
        }
        protected void OnContextMenuPostback(string commandArgs, double lat, double lon)
        {
            if (ContextMenuPostback != null) ContextMenuPostback(this, new PostbackEventArgs(commandArgs, lat, lon));
        }

        protected void OnMapMove(int zoom, Bounds bounds)
        {
            if(MapMove != null) MapMove(this, new MapMoveEventArgs(zoom, bounds));
        }

        protected void RegisterStartupScript(string key, string script)
        {
            RegisterStartupScript(key, script, true);
        }

        protected void RegisterStartupScript(string key, string script, bool addtags)
        {
            if (Page.IsPostBack) ScriptManager.RegisterStartupScript(Page, typeof(string), key, script, addtags);
            else Page.ClientScript.RegisterStartupScript(typeof(string), key, script, addtags);
        }

        #endregion

        #region Private Properties

        private string _callbackScript = string.Empty;
//        private bool _inCallback;

        public string Map { get { return string.Format("$M('{0}')", MapDivId); } }

        public string MapDivId { get { return string.Concat(ClientID, "_map"); } }

        private bool RequireGoogleMapsScript
        {
            get
            {
                var o = ViewState["RequireGoogleMapsScript"];
                return o != null && (bool)o;
            }
            set { ViewState["RequireGoogleMapsScript"] = value; }
        }

        private double Latitud
        {
            get
            {
                var o = ViewState["Latitud"];
                return o != null ? (double)o : 0;
            }
            set { ViewState["Latitud"] = value; }
        }

        private double Longitud
        {
            get
            {
                var o = ViewState["Longitud"];
                return o != null ? (double)o : 0;
            }
            set { ViewState["Longitud"] = value; }
        }

        private List<Layer> Layers
        {
            get
            {
                var o = ViewState["Layers"] as List<Layer>;
                return o ?? new List<Layer>();
            }
            set { ViewState["Layers"] = value; }
        }

        private List<Control> OlControls
        {
            get
            {
                var o = ViewState["OLControls"] as List<Control>;
                return o ?? new List<Control>();
            }
            set { ViewState["OLControls"] = value; }
        }


        #endregion

        #region Overriden Methods
        protected override void Render(HtmlTextWriter writer)
        {
            if (!Page.IsPostBack)
            {
                RenderContents(writer);
            }
            else
            {
                RegisterStartupScript(Guid.NewGuid().ToString(), _callbackScript);
            }            
        }
        protected override void RenderContents(HtmlTextWriter output)
        {
            RenderMapDiv(output);
            if (RequireGoogleMapsScript && IsGoogleOnLine()) RegisterGoogleMaps();
            RegisterOpenLayers();
            RegisterGlobalVars();
            RegisterCallbackFunctions();            
            RegisterTimerScript();            
            CreateMap();
        }

        private void RegisterGlobalVars()
        {
            const string script = @"var lastUpdate = new Date(); var onCall = false; var onPause = false; var timerCallback;";
            RegisterStartupScript("MonitorGlobalVars", script);
        }

        #endregion

        #region Control Creation
        protected void RegisterTimerScript()
        {
            const string timerScript = @"
                function Timer(callback, delay) {
                    var timerId, start, remaining = delay;

                    this.pause = function() {
                        window.clearTimeout(timerId);
                        remaining -= new Date() - start;
                    };

                    this.resume = function() {
                        start = new Date();
                        window.clearTimeout(timerId);
                        timerId = window.setTimeout(callback, remaining);
                    };

                    this.clear = function() {
                        window.clearTimeout(timerId);
                        delete(this);
                    };

                    this.resume();
                }";
            RegisterStartupScript("TimerScript", timerScript);
        }

        protected void RegisterCallbackFunctions()
        {
            var cbReference = Page.ClientScript.GetCallbackEventReference(this, "args", "ReceiveServerData", "context", "ReceiveServerError", true);
            var callbackScript = @"function CallServer(args, context){ console.log('Begin CallServer(%s)', args); if (onCall) { console.log('Return on CallServer(%s)', args); return true; }" + cbReference + "; onCall = true; lastUpdate = new Date(); console.log('End CallServer(%s)', args); return true; }";
            RegisterStartupScript("CallServer", callbackScript);

            var pbReference = Page.ClientScript.GetPostBackEventReference(this, "");
            var postbackScript = @"function CallServerPB(args){ console.log('Begin CallServerPB(%s)', args);" + pbReference.TrimEnd('\'', ')') + "args); console.log('End CallServerPB(%s)', args);}";
            RegisterStartupScript("CallServerPB", postbackScript);

            const string receiveServerData = @"function ReceiveServerData(result){ console.log('ReceiveServerData(%s)', result); onCall = false; lastUpdate = new Date(); eval(result); delete result;}";
            RegisterStartupScript("ReceiveServerData", receiveServerData);

            const string receiveServerError = @"function ReceiveServerError(result){console.log('ReceiveServerError(%s)', result); delete result; onCall = false; lastUpdate = new Date(); }";
            RegisterStartupScript("ReceiveServerError", receiveServerError);
        }

        protected void RenderMapDiv(HtmlTextWriter output)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Id, MapDivId);
            output.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
            output.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
            output.RenderBeginTag(HtmlTextWriterTag.Div);

            output.AddAttribute(HtmlTextWriterAttribute.Id, MapDivId + "_value");
            output.AddAttribute(HtmlTextWriterAttribute.Name, MapDivId + "_value");
            output.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            output.RenderBeginTag(HtmlTextWriterTag.Input);
            output.RenderEndTag();

            output.RenderEndTag();
        }

        protected void RegisterGoogleMaps()
        {
            RegisterStartupScript("GoogleMaps",
                                  string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>",
                                                GoogleMapsScript), false);
        }
        protected void RegisterOpenLayers()
        {
            string[] scriptNames =
            { 
                "Logictracker.Web.Monitor.OpenLayers.js",
                "Logictracker.Web.Monitor.OpenLayers.Marker.Labeled.js",
                "Logictracker.Web.Monitor.ContextMenu.OpenLayers.Control.ContextMenu.js",
                "Logictracker.Web.Monitor.ContextMenu.OpenLayers.Control.ContextMenu.ContextMenuItem.js",
                "Logictracker.Web.Monitor.OpenLayers.Handler.Circle.js",
                "Logictracker.Web.Monitor.OpenLayers.Handler.Square.js",
                "Logictracker.Web.Monitor.Monitor.js"};

            foreach (var scriptName in scriptNames)
            {
                RegisterPageScriptInclude(GetType(), scriptName);
            }
        }

        private void RegisterPageScriptInclude(Type type, string scriptName)
        {
            var url = Page.ClientScript.GetWebResourceUrl(type, scriptName);
            RegisterStartupScript(scriptName,
                                  string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>",
                                                url), false);
        }

        protected void CreateMap()
        {
            var script = string.Format("OpenLayers.ImgPath = '{0}';", ResolveUrl(ImgPath));
            var postbackScript = Page.ClientScript.GetPostBackEventReference(this, "Click:");
            postbackScript = postbackScript.Replace("'Click:'", "'Click:'+lonlat.lon+','+lonlat.lat");

            var multiplePopups = string.Format("{0}.setMultiplePopUps({1});", Map,
                                               MultiplePopUps.ToString().ToLower());
            var click = Click != null?
                                         @" map.map.events.register('click', map, 
                        function(evt){
                            OpenLayers.Event.stop(evt);
                            var px = evt.xy.clone();
                            if(this.waitDblClick){
                                this.waitDblClick = false;
                                return;
                            }
                            else
                            {
                                this.waitDblClick = true;
                                setTimeout(
                                    function(px){
                                        if(!this.waitDblClick) return;
                                        this.waitDblClick = false;
                                        var lonlat = this.reverseTransform(this.map.getLonLatFromPixel(px));
                                        if(this.clickMethod == 'C')
                                            CallServer('Click:'+lonlat.lon+','+lonlat.lat,'');
                                        else
                                            " + postbackScript + @";
                                    }.bind(this, px),500);
                            }
                        });" : string.Empty;


            script += string.Format(@"
             Sys.Application.add_init(function ()
            {{
                var map = Logictracker.createMap('{0}', {{ {1} }});
                {2}
                {3}
                {4}
                {5}
            }});
            ", MapDivId, BuildMapOptions(), GetCallbackScript(), GetCallServerTickScript(), click, multiplePopups);
              
            RegisterStartupScript("Globals", script);
        }

        protected string BuildMapOptions()
        {
            var options = new StringBuilder();
            options.AppendFormat("'projection': {0},", Projection);
            options.AppendFormat("'displayProjection': {0},", DisplayProjection);
            options.AppendFormat("'clickMethod': '{0}',", ClickMethod == EventMethods.Callback ? 'C' : 'P');
            options.AppendFormat("'drawPolygonMethod': '{0}',", DrawPolygonMethod == EventMethods.Callback ? 'C' : 'P');
            options.AppendFormat("'drawCircleMethod': '{0}',", DrawCircleMethod == EventMethods.Callback ? 'C' : 'P');
            options.AppendFormat("'drawPolylineMethod': '{0}',", DrawLineMethod == EventMethods.Callback ? 'C' : 'P');
            options.AppendFormat("'drawmodFeatureMethod': '{0}',", ModFeatureMethod == EventMethods.Callback ? 'C' : 'P');
            options.AppendFormat("'postbackOnMove': '{0}',", MapMove !=null ? "true" : "false");
            options.AppendFormat("'postbackOnMoveZoom': '{0}',", PostbackOnMoveZoom);
            options.AppendFormat("'panDuration': '{0}',", 10);
            options.AppendFormat("'zoomDuration': '{0}',", 10);
            options.AppendFormat("'zoomMethod': {0},", "null");
            if (DrawPolygonMethod == EventMethods.PostBack)
            {
                var postbackScript = Page.ClientScript.GetPostBackEventReference(this, "DrawPolygon:");
                options.AppendFormat("'drawPolygonPostBack': \"{0}\",", postbackScript);
            }
            if (DrawCircleMethod == EventMethods.PostBack)
            {
                var postbackScript = Page.ClientScript.GetPostBackEventReference(this, "DrawCircle:");
                options.AppendFormat("'drawCirclePostBack': \"{0}\",", postbackScript);
            }
            if (DrawLineMethod == EventMethods.PostBack)
            {
                var postbackScript = Page.ClientScript.GetPostBackEventReference(this, "DrawLine:");
                options.AppendFormat("'drawPolylinePostBack': \"{0}\",", postbackScript);
            }
            if (ModFeatureMethod == EventMethods.PostBack)
            {
                var postbackScript = Page.ClientScript.GetPostBackEventReference(this, "FeatureModified:");
                options.AppendFormat("'modFeaturePostBack': \"{0}\",", postbackScript);
            }
            
            options.Append("'units': 'm',");
            if (!UseDefaultControls)
                options.Append("'controls':[],");
            if (MaxResolution != -1)
                options.AppendFormat("'maxResolution': {0},", MaxResolution.ToString(CultureInfo.InvariantCulture));
            options.Append("'maxExtent': new OpenLayers.Bounds(-20037508, -20037508, 20037508, 20037508.34)");
            return options.ToString();
        }
        private string GetCallbackScript()
        {
            var script = _callbackScript;
            if (EnableTimer)
                script += GetTimerScript();
            return script;
        }
        private string GetCallServerTickScript()
        {
            return String.Format("timerCallBack = new Timer(function() {{ CallServer('Tick:{0}',''); }}, 2000);", CallbackArgument);
        }

        private string getCleanCallServerTickTimer()
        {
            return "if (typeof timerCallback != 'undefined') { timerCallback = timerCallback.clear(); }; onCall = false; lastUpdate = new Date();";
        }

        private string GetTimerScript()
        {
            return string.Format("{2}timerCallback = new Timer(function(){{{1}}}, {0});", TimerInterval * 1000, GetCallServerTickScript(), getCleanCallServerTickTimer());
        }
        #endregion

        #region Public Methods

        public void ExecuteCallServerTick(bool cleanTimer)
        {
            var script = (cleanTimer ? getCleanCallServerTickTimer():String.Empty) + GetCallServerTickScript();
            AddCallbackScript(script);
        }

        public void ResetTimer()
        {
            AddCallbackScript(GetTimerScript());
        }

        /// <summary>
        /// Agrega al monitor los layers especificados
        /// </summary>
        /// <param name="layers"><see cref="Layer"/> a agregar</param>
        public void AddLayers(params Layer[] layers)
        {
            var list = Layers;

            foreach (var l in layers)
            {
                list.Add(l);
                if (l.RequireGoogleMapsScript && !IsGoogleOnLine()) continue;
                RequireGoogleMapsScript = RequireGoogleMapsScript || l.RequireGoogleMapsScript;
                AddCallbackScript(string.Format("{0}.addLayer('{1}', {2});", Map, l.Name, l.Code));
                if (l.executePostCode()) AddCallbackScript(string.Format(l.PostCode, Map, l.Name));
            }

            Layers = list;
        }

        /// <summary>
        /// Agrega al monitor los controles especificados
        /// </summary>
        /// <param name="controls"><see cref="Control"/> a agregar</param>
        public void AddControls(params Control[] controls)
        {
            var list = OlControls;

            foreach (var c in controls)
            {
                list.Add(c);
                AddCallbackScript(string.Format("{0}.addControl({1});", Map, c.Code.Replace("{map}", Map)));
            }

            OlControls = list;
        }

        /// <summary>
        /// Agrega markers al monitor en el layer especificado
        /// </summary>
        /// <param name="layerName">Nombre del layer</param>
        /// <param name="markers"><see cref="Marker"/> a agregar</param>
        public void AddMarkers(string layerName, params Marker[] markers)
        {
            var added = string.Empty;

            foreach (var m in markers)
                if(string.IsNullOrEmpty(m.PopupContentHTML))
                    added += string.Format("{0}.aM('{1}', {2}, '{3}');", Map, m.Id, m.Code(), layerName);
                else
                    //if (m.PopupContentHTML.StartsWith("javascript:"))
                    //    added += string.Format("{0}.aMP('{1}', {2}, '{3}', {4});", map, m.Id, m.Code(), LayerName, m.PopupContentHTML.Replace("\"", "\\\"").Substring("javascript:".Length));
                    //else
                    added += string.Format("{0}.aMP('{1}', {2}, '{3}', '{4}');", Map, m.Id, m.Code(), layerName, m.PopupContentHTML.Replace("'", "\""));

            AddCallbackScript(added);
        }

        /// <summary>
        /// Adds the givenn geometries to the specified layer of the current map.
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="geoms"></param>
        public void AddGeometries(string layerName, params Geometry[] geoms)
        {
            var added = string.Empty;

            foreach (var geom in geoms)
                added += string.Format(geom.EsGeoCerca ? "{0}.aGC('{1}', {2}, '{3}');"
                                           : "{0}.aG('{1}', {2}, '{3}');", Map, geom.Id, geom.Code(Map), layerName);

            AddCallbackScript(added);
        }

        public void RemoveGeometries(string layerName, params string[] idgeoms)
        {
            var added = string.Empty;

            foreach (var geom in idgeoms)
                added += string.Format("{0}.rG('{1}', '{2}');", Map, geom, layerName);

            AddCallbackScript(added);
        }

        /// <summary>
        /// Setea el centro del mapa
        /// </summary>
        /// <param name="latitud">Latitud del punto</param>
        /// <param name="longitud">Longitud del punto</param>
        public void SetCenter(double latitud, double longitud)
        {
            Latitud = latitud;
            Longitud = longitud;

            AddCallbackScript(string.Format("{0}.setCenter(new OL.LL({1},{2}));", Map,
                                        Longitud.ToString(CultureInfo.InvariantCulture), Latitud.ToString(CultureInfo.InvariantCulture)));
        }

        public void SetCenter(double latitud, double longitud, int zoom)
        {
            Latitud = latitud;
            Longitud = longitud;

            AddCallbackScript(string.Format("{0}.setCenter(new OL.LL({1},{2}),{3});", Map,
                                        Longitud.ToString(CultureInfo.InvariantCulture), Latitud.ToString(CultureInfo.InvariantCulture), zoom));
        }

        public void SetCenterOn(string layer, int zoom)
        {
            AddCallbackScript(string.Format("{0}.setCenterOn('{1}', {2});", Map,
                                        layer, zoom));
        }

        public void SetCenterOn(string layer)
        {
            AddCallbackScript(string.Format("{0}.setCenterOn('{1}');", Map,
                                        layer));
        }

        public void SetZoomOn(string layer)
        {
            AddCallbackScript(string.Format("{0}.setZoomOn('{1}');", Map,
                                        layer));
        }

        public void SetCenter(string layer, double latitud, double longitud, int zoom)
        {
            Latitud = latitud;
            Longitud = longitud;

            AddCallbackScript(string.Format("{0}.setCenter('{1}', new OL.LL({2},{3}),{4});", Map,
                                        layer, Longitud.ToString(CultureInfo.InvariantCulture), Latitud.ToString(CultureInfo.InvariantCulture), zoom));
        }

        public void ZoomTo(int zoom)
        {
            AddCallbackScript(string.Format("{0}.zoomTo({1});", Map, zoom));
        }

        /// <summary>
        /// Agrega una sentencia Javascript personalizada al codigo devuelto por el callback
        /// </summary>
        /// <param name="script">Sentencia Javascript</param>
        public void AddCallbackScript(string script) { _callbackScript += script; }

        /// <summary>
        /// Removes and destroys all the elements of the specified layer.
        /// </summary>
        /// <param name="layer">The layer to be cleared.</param>
        public void ClearLayer(string layer)
        {
            AddCallbackScript(string.Format("{0}.clearLayer('{1}');", Map, layer));
        }

        /// <summary>
        /// Removes and destroys all the elements of all layers.
        /// </summary>
        public void ClearLayers() { foreach (var layer in Layers) ClearLayer(layer.Name); }

        /// <summary>
        /// Clears all layers and remove all elements from the monitor.
        /// </summary>
        public void Clear()
        {
            ClearLayers();

            ClearElements();
        }

        /// <summary>
        /// Removes elements and geocercas from the monitor.
        /// </summary>
        public void ClearElements() { AddCallbackScript(string.Format("{0}.clearElements();", Map)); }

        /// <summary>
        /// Changes the map visibility status to visible.
        /// </summary>
        public void Show() { AddCallbackScript(string.Format("map = $M('{0}');", MapDivId)); AddCallbackScript(string.Format("{0}.show();", Map)); }

        /// <summary>
        /// Changes the map visibility to hidden.
        /// </summary>
        public void Hide() { AddCallbackScript(string.Format("{0}.hide();", Map)); }

        /// <summary>
        /// Triggers the execution of the givenn event on the selected feature.
        /// </summary>
        /// <param name="id">The element id.</param>
        /// <param name="layer">The element layer.</param>
        /// <param name="eventName">The event to be triggered.</param>
        public void TriggerEvent(string id, string layer, string eventName)
        {
            AddCallbackScript(string.Format("{0}.triggerEvent('{1}', '{2}', '{3}');", Map, id, layer, eventName));
        }

        /// <summary>
        /// Sets the map default center to the givenn lon lat value.
        /// </summary>
        /// <param name="longitude">The default center longitude.</param>
        /// <param name="latitude">The default center latitude.</param>
        public void SetDefaultCenter(double latitude, double longitude)
        {
            AddCallbackScript(string.Format("{0}.setDefaultCenter({1}, {2});", Map,
                                        longitude.ToString(CultureInfo.InvariantCulture), latitude.ToString(CultureInfo.InvariantCulture)));
        }
        public void ActivateMode(string mapMode)
        {
            AddCallbackScript(string.Format("{0}.switchMode('{1}');", Map, mapMode));
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Si el evento click es postback o callback(default)
        /// </summary>
        public EventMethods ClickMethod
        {
            get { return (EventMethods) (ViewState["ClickMethod"] ?? EventMethods.Callback); }
            set { ViewState["ClickMethod"] = value; }
        }
        /// <summary>
        /// Si el evento DrawPolygon es postback o callback(default)
        /// </summary>
        public EventMethods DrawPolygonMethod
        {
            get { return (EventMethods)(ViewState["DrawPolygonMethod"] ?? EventMethods.Callback); }
            set { ViewState["DrawPolygonMethod"] = value; }
        }
        /// <summary>
        /// Si el evento DrawCircle es postback o callback(default)
        /// </summary>
        public EventMethods DrawCircleMethod
        {
            get { return (EventMethods)(ViewState["DrawCircleMethod"] ?? EventMethods.Callback); }
            set { ViewState["DrawCircleMethod"] = value; }
        }
        
        /// <summary>
        /// Si el evento DrawLine es postback o callback(default)
        /// </summary>
        public EventMethods DrawLineMethod
        {
            get { return (EventMethods)(ViewState["DrawLineMethod"] ?? EventMethods.Callback); }
            set { ViewState["DrawLineMethod"] = value; }
        }
        public EventMethods ModFeatureMethod
        {
            get { return (EventMethods)(ViewState["ModFeatureMethod"] ?? EventMethods.Callback); }
            set { ViewState["ModFeatureMethod"] = value; }
        }
        
        /// <summary>
        /// Argumento que sera devuelto al evento <see cref="Tick"/> en el siguiente callback
        /// </summary>
        public string CallbackArgument
        {
            get
            {
                var dp = (string)ViewState["CallbackArgument"];
                return dp ?? "";
            }
            set { ViewState["CallbackArgument"] = value; }
        }
        /// <summary>
        /// Habilita el timer que genera los callbacks
        /// </summary>
        public bool EnableTimer
        {
            get
            {
                var o = ViewState["EnableTimer"];
                return o == null || (bool)o;
            }
            set { ViewState["EnableTimer"] = value; }
        }
        /// <summary>
        /// Intervalo en segundos del timer que genera los callbacks
        /// </summary>
        public int TimerInterval
        {
            get
            {
                var o = ViewState["TimerInterval"];
                return ((o == null) ? 60 : (int)o);
            }

            set
            {
                ViewState["TimerInterval"] = value;
            }
        }

        
        /// <summary>
        /// Intervalo en segundos del timer que genera los callbacks
        /// </summary>
        public int PostbackOnMoveZoom
        {
            get
            {
                var o = ViewState["PostbackOnMoveZoom"];
                return ((o == null) ? 12 : (int)o);
            }

            set
            {
                ViewState["PostbackOnMoveZoom"] = value;
            }
        }

        /// <summary>
        /// Usa los controles por default del OpenLayers
        /// </summary>
        public bool UseDefaultControls
        {
            get
            {
                var o = ViewState["UseDefaultControls"];
                return o != null && (bool)o;
            }
            set { ViewState["UseDefaultControls"] = value; }
        }

        /// <summary>
        /// DisplayProjection de Openlayers
        /// </summary>
        public string DisplayProjection
        {
            get
            {
                var dp = (string)ViewState["DisplayProjection"];
                return dp ?? ProjectionFactory.GetGoogle();
            }
            set { ViewState["DisplayProjection"] = value; }
        }
        /// <summary>
        /// Projection de Openlayers
        /// </summary>
        public string Projection
        {
            get
            {
                var dp = (string)ViewState["Projection"];
                return dp ?? ProjectionFactory.GetEPSG900913();
            }
            set { ViewState["Projection"] = value; }
        }
        /// <summary>
        /// Url del api de Google Maps
        /// </summary>
        public string GoogleMapsScript
        {
            get
            {
                var s = (String)ViewState["GoogleMapsScript"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["GoogleMapsScript"] = value;
            }
        }
        /// <summary>
        /// MaxResolution
        /// </summary>
        public double MaxResolution
        {
            get
            {
                var o = ViewState["MaxResolution"];
                return ((o == null) ? 156543.0339 : (double)o);
            }

            set
            {
                ViewState["MaxResolution"] = value;
            }
        }

        /// <summary>
        /// Images directory path.
        /// </summary>
        public string ImgPath
        {
            get { return ViewState["ImgPath"] != null ? ViewState["ImgPath"].ToString() : "img/"; }
            set
            {
                var path = value.Replace('\\', '/');
                ViewState["ImgPath"] = path.EndsWith("/") ? path : string.Format("{0}/", path);
            }
        }

        /// <summary>
        /// Determines if it is allowed to show multiple pop-ups on the monitor.
        /// </summary>
        public bool MultiplePopUps
        {
            get { return ViewState["MultiplePopUps"] != null && Convert.ToBoolean(ViewState["MultiplePopUps"]); }
            set { ViewState["MultiplePopUps"] = value; }
        }

        /// <summary>
        /// The marker layer for Geocoder queries
        /// </summary>
        public string GeocoderMarkersLayer
        {
            get { return (string)ViewState["GeocoderMarkersLayer"]; }
            set { ViewState["GeocoderMarkersLayer"] = value; }
        }

        /// <summary>
        /// The default marker icon for Geocoder queries
        /// </summary>
        public string DefaultMarkerIcon
        {
            get { return (string)ViewState["DefaultMarkerIcon"]; }
            set { ViewState["DefaultMarkerIcon"] = value; }
        }
        #endregion

        private string _postedValue = string.Empty;

        protected string Value
        {
            get { return Page.Request.Form[MapDivId + "_value"]; }
        }

        public string SelectedPolygonId
        {
            get 
            { 
                var polid = Value.Split('|')[0];
                return string.IsNullOrEmpty(polid) ? null : polid;
            }
        }

        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
            return GetCallbackScript();
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
//            _inCallback = true;
            var idx = eventArgument.IndexOf(':');
            var eventName = eventArgument.Substring(0, idx);
            var eventArgs = eventArgument.Length > idx?eventArgument.Substring(idx+1): "";
            switch(eventName)
            {
                case "Tick":
                    OnTick(eventArgs);
                    break;
                case "Click":
                    var clickCoord = ParseCoord(eventArgs);
                    if (clickCoord != null) OnClick(clickCoord.Latitud, clickCoord.Longitud);   
                    break;
                case "MarkAddress":
                    var markCoord = ParseCoord(eventArgs);
                    if (markCoord != null)
                    {
                        var dir = GeocoderHelper.GetDescripcionEsquinaMasCercana(markCoord.Latitud, markCoord.Longitud);
                        AddMarkers(GeocoderMarkersLayer, MarkerFactory.CreateMarker("0", DefaultMarkerIcon, markCoord.Latitud, markCoord.Longitud, dir));   
                    }
                    break;
                //case "ContextMenuCallback":
                //    break;
                case "DrawPolygon":
                    var points = ParsePolygon(eventArgs);
                    OnDrawPolygon(points);
                    break;
                case "DrawSquare":
                    var bounds = ParseSquare(eventArgs);
                    OnDrawSquare(bounds);
                    break;
                case "DrawCircle":
                    var circle = ParseCircle(eventArgs);
                    if (circle != null) OnDrawCircle(circle.Latitud, circle.Longitud, circle.Radio);
                    break;
                case "DrawLine":
                    var pointsPath = ParseLine(eventArgs);
                    OnDrawLine(pointsPath);
                    break;
                default:
                    var args = eventArgs.Split(',');
                    var defaultCoord = ParseCoord(eventArgs);
                    if (defaultCoord != null) OnCallback(eventName, args[2], defaultCoord.Latitud, defaultCoord.Longitud);  
                    break;
            }
//            _inCallback = false;
        }

        #endregion

        #region IPostBackDataHandler Members

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if(postCollection["__EVENTTARGET"] != UniqueID) return false;

            _postedValue = postCollection["__EVENTARGUMENT"];
            return true;
        }
        
        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            var idx = _postedValue.IndexOf(':');
            var eventName = _postedValue.Substring(0, idx);
            var eventArgs = _postedValue.Length > idx ? _postedValue.Substring(idx + 1) : "";
            switch (eventName)
            {
                case "Click":
                    var clickCoord = ParseCoord(eventArgs);
                    if (clickCoord != null && ClickMethod == EventMethods.PostBack) OnClick(clickCoord.Latitud, clickCoord.Longitud);
                    break;
                case "DrawPolygon":
                    var points = ParsePolygon(eventArgs);
                    OnDrawPolygon(points);
                    break;
                case "DrawSquare":
                    var bounds = ParseSquare(eventArgs);
                    OnDrawSquare(bounds);
                    break;
                case "DrawCircle":
                    var circle = ParseCircle(eventArgs);
                    if (circle != null) OnDrawCircle(circle.Latitud, circle.Longitud, circle.Radio);
                    break;
                case "DrawLine":
                    var pointsPath = ParseLine(eventArgs);
                    OnDrawLine(pointsPath);
                    break;
                case "ContextMenuPostback":
                    var args = eventArgs.Split(',');
                    var ctxCoord = ParseCoord(eventArgs);
                    if (ctxCoord != null) OnContextMenuPostback(args[2], ctxCoord.Latitud, ctxCoord.Longitud);  
                    break;
                case "Move":
                    int zoom;
                    var newBounds = GetBounds(eventArgs, out zoom);
                    OnMapMove(zoom, newBounds);
                    break;
                case "FeatureModified":
                    List<PointF> pointsMod = null;
                    string tipo = null;
                    if (eventArgs.StartsWith("LINESTRING("))
                    {
                        tipo = MonitorModifyFeatureEventArgs.Tipos.Line;
                        pointsMod = ParseLine(eventArgs);
                    }
                    else if(eventArgs.StartsWith("POLYGON("))
                    {
                        tipo = MonitorModifyFeatureEventArgs.Tipos.Polygon;
                        pointsMod = ParsePolygon(eventArgs);
                    }
                    if (pointsMod != null) OnFeatureModified(tipo, pointsMod);
                    break;
            }
        }

        private LatLon ParseCoord(string geom)
        {
            var args = geom.Split(',');
            double lon, lat;
            if (args.Length >= 2 && double.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out lon)
                && double.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out lat))
                return new LatLon(lat, lon);

            return null;
        }

        private List<PointF> ParseLine(string geom)
        {
            if (geom.StartsWith("LINESTRING(")) geom = geom.Substring(11);
            geom = geom.TrimEnd(')');
            var args = geom.Split(',');
            var pointsPath = new List<PointF>();
            foreach (var s in args)
            {
                var coords = s.Split(' ');
                var point = new PointF((float)Convert.ToDouble(coords[0], CultureInfo.InvariantCulture), (float)Convert.ToDouble(coords[1], CultureInfo.InvariantCulture));
                pointsPath.Add(point);
            }
            return pointsPath;
        }

        private Circle ParseCircle(string geom)
        {
            if (geom.StartsWith("POINT(")) geom = geom.Substring(6);
            var pointArg = geom.Split(')')[0];
            var radioArg = geom.Split('(')[1].TrimEnd(')');
            var args = pointArg.Split(' ');
            int radio;
            int.TryParse(radioArg, out radio);
            double lat, lon;
            if (args.Length >= 2 && double.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out lon)
                && double.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out lat))
                return new Circle {Latitud = lat, Longitud = lon, Radio = radio};

            return null;
        }
        private Bounds ParseSquare(string geom)
        {
            if (geom.StartsWith("POLYGON((")) geom = geom.Substring(9);
            geom = geom.TrimEnd(')');
            var args = geom.Split(',');
            var squarePoints = args.Select(s => s.Split(' ')).Select(coords => new PointF((float)Convert.ToDouble(coords[0], CultureInfo.InvariantCulture), (float)Convert.ToDouble(coords[1], CultureInfo.InvariantCulture))).ToList();
            if (squarePoints.Count != 5) throw new ArgumentException("El poligono no es un area rectangular");
            var bounds = new Bounds
            {
                Top = squarePoints.Max(p => p.Y),
                Bottom = squarePoints.Min(p => p.Y),
                Right = squarePoints.Max(p => p.X),
                Left = squarePoints.Min(p => p.X)
            };
            return bounds;
        }
        private List<PointF> ParsePolygon(string geom)
        {
            if (geom.StartsWith("POLYGON((")) geom = geom.Substring(9);
            geom = geom.TrimEnd(')');
            var args = geom.Split(',');
            var points = new List<PointF>();
            foreach (var s in args)
            {
                var coords = s.Split(' ');
                var point = new PointF((float)Convert.ToDouble(coords[0], CultureInfo.InvariantCulture), (float)Convert.ToDouble(coords[1], CultureInfo.InvariantCulture));
                points.Add(point);
            }
            return points;
        }

        private Bounds GetBounds(string eventArgs, out int zoom)
        {
            var args = eventArgs.Split(',');
            zoom = 0;
            var bounds = new Bounds();
            foreach (var arg in args)
            {
                double d;
                var vals = arg.Split('=');
                if(!double.TryParse(vals[1], NumberStyles.Any, CultureInfo.InvariantCulture, out d)) continue;
                
                switch(vals[0])
                {
                    case "z": zoom = Convert.ToInt32(d); break;
                    case "t": bounds.Top = d; break;
                    case "b": bounds.Bottom = d; break;
                    case "l": bounds.Left = d; break;
                    case "r": bounds.Right = d; break;
                }
            }
            return bounds;
        }

        #endregion

        public void ExecuteScript(string script)
        {
            AddCallbackScript(script);
        }

        #region Check If Google is OnLine
        //private bool _isGoogleOnLine;
        //private bool _googleChecked;
        private bool IsGoogleOnLine()
        {
            return true;
            //if (_googleChecked) return _isGoogleOnLine;
            //try
            //{
            //    var wr = WebRequest.Create(GoogleMapsScript);
            //    var resp = wr.GetResponse();
            //    _isGoogleOnLine = true;
            //    resp.Close();
            //}
            //catch
            //{
            //    _isGoogleOnLine = false;
            //}
            //finally
            //{
            //    _googleChecked = true;
            //}
            //return _isGoogleOnLine;
        } 
        #endregion

        #region Initialize (Autoconfigure)
        /// <summary>
        /// Autoconfigurar con opciones básicas
        /// </summary>
        public void Initialize(bool googleMapsEnabled)
        {
            if (!Page.IsPostBack)
            {
                const int minZoomLevel = 4;
                ImgPath = Config.Monitor.GetMonitorImagesFolder(Page);
                GoogleMapsScript = Config.Map.GoogleMapsKey;

                if (googleMapsEnabled)
                {
                    AddLayers(LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), minZoomLevel),
                              //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, minZoomLevel),
                              LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                              LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), minZoomLevel),
                              LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), minZoomLevel),
                              LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), minZoomLevel));
                }
                else
                {
                    AddLayers(LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")));
                }

                AddControls(ControlFactory.GetLayerSwitcher(),
                            ControlFactory.GetNavigation(),
                            ControlFactory.GetPanZoomBar());

                SetDefaultCenter(-34.6134981326759, -58.4255323559046);
                SetCenter(-34.6134981326759, -58.4255323559046, 10);
            }
        }
        #endregion
    }
}