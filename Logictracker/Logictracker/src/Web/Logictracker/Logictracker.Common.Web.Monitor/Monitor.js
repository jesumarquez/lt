if ("undefined" == typeof Function.prototype.bind) {
    Function.prototype.bind = function(oThis) {
        if (typeof this !== "function") {
            // closest thing possible to the ECMAScript 5 internal IsCallable function
            throw new TypeError("Function.prototype.bind - what is trying to be bound is not callable");
        }

        var aArgs = Array.prototype.slice.call(arguments, 1),
        fToBind = this,
        fNOP = function() { },
        fBound = function() {
            return fToBind.apply(this instanceof fNOP && oThis
                                 ? this
                                 : oThis,
                               aArgs.concat(Array.prototype.slice.call(arguments)));
        };

        fNOP.prototype = this.prototype;
        fBound.prototype = new fNOP();

        return fBound;
    };
}

var OL = {V: OpenLayers.Feature.Vector,
          M: OpenLayers.Marker,
          LM: OpenLayers.Marker.Labeled,
          P: OpenLayers.Geometry.Point,
          L: OpenLayers.Geometry.LineString,
          LR: OpenLayers.Geometry.LinearRing,
          PY: OpenLayers.Geometry.Polygon,
          I: OpenLayers.Icon,
          LL: OpenLayers.LonLat,
          S: OpenLayers.Size,
          PX: OpenLayers.Pixel,
          XM: OpenLayers.Control.ContextMenu,
          XI: OpenLayers.Control.ContextMenu.ContextMenuItem,
          XT: OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes
      };

      document.createStyleSheet2 = document.createStyleSheet;
      document.createStyleSheet = function() {
          try { return document.createStyleSheet2(); }
          catch (e) { return document.styleSheets[document.styleSheets.length - 1]; }
      };
          
var Logictracker = 
{
    maps: {},
    createMap: function(div, options)
    {
        var map = new Logictracker.Monitor(div, options);
        this.maps[div] = map;
        return map;
    },
    getMap: function(id)
    {
        return this.maps[id];
    }
};
Logictracker.Monitor = function(div, options) {
    this.id = div;
    this.div = document.getElementById(div);
    this.resizeMap();
    this.projection = options.projection;
    this.displayProjection = options.displayProjection;
    this.clickMethod = options.clickMethod;
    this.drawPolygonMethod = options.drawPolygonMethod;
    this.drawCircleMethod = options.drawCircleMethod;
    this.drawPolylineMethod = options.drawPolylineMethod;
    this.modFeatureMethod = options.modFeatureMethod;
    this.drawPolygonPostBack = options.drawPolygonPostBack;
    this.drawCirclePostBack = options.drawCirclePostBack;
    this.drawPolylinePostBack = options.drawPolylinePostBack;
    this.modFeaturePostBack = options.modFeaturePostBack;
    this.postbackOnMove = options.postbackOnMove;
    this.postbackOnMoveZoom = options.postbackOnMoveZoom;
    this.map = new OpenLayers.Map(this.div, options);
    this.map.events.register('zoomend', this, this.resizeGeocercas);
    this.map.events.register('moveend', this, this.onMove);

    setTimeout(function() { setInterval(this.resizeMap.bind(this), 1000); } .bind(this), 5000);
};
Logictracker.Monitor.prototype =
{
    div: null,
    layers: {},
    elements: {},
    geocercas: {},
    size: {},
    displayProjection: null,
    projection: null,
    map: null,
    timerCallback: null,
    clickMethod: 'C',
    drawPolygonMethod: 'C',
    drawCircleMethod: 'C',
    drawPolylineMethod: 'C',
    modFeatureMethod: 'C',
    selectedPolygonId: '',
    moving: 0,
    lastSize: null,
    resizeMap: function() {
        try {
            var par = this.div.parentNode.parentNode;
            var wd = par.style.width;
            var hg = par.style.height;
            var s = wd + ',' + hg;
            if (s == this.lastSize) return;
            this.lastSize = s;
            var newSize = { 'w': parseInt(wd.substr(0, wd.length - 2)), 'h': parseInt(hg.substr(0, hg.length - 2)) };
            if (this.size.w != newSize.w || this.size.h != newSize.h) {
                this.div.style.width = newSize.w + 'px';
                this.div.style.height = newSize.h + 'px';
                if (this.map) this.map.updateSize();
                this.size.w = newSize.w;
                this.size.h = newSize.h;
            }
        } catch (e) { }
    },
    getValueHidden: function() { return $get(this.id + "_value"); },
    onMove: function() {
        if (!this.postbackOnMove) return;
        this.moving++;
        setTimeout(this.triggerMove.bind(this), 500);
    },
    triggerMove: function() {
        this.moving--;
        if (this.moving > 0) return;
        /*
        TODO: FIX: se usa en el editor de qtree
        var z = this.map.getZoom();
        if (z < this.postbackOnMoveZoom) return;
        var b = this.reverseTransform(this.map.getExtent());
        CallServerPB('Move:z=' + z + ',t=' + b.top + ',b=' + b.bottom + ',l=' + b.left + ',r=' + b.right);
        */
    },
    setValue: function() {
        this.getValueHidden().value = this.selectedPolygonId;
    },
    setMultiplePopUps: function(value) {
        this.map.multiplePopUps = value;
    },
    setCenterOn: function(layer, zoom) {
        if (!this.layers[layer]) return null;
        var newBound = this.layers[layer].getDataExtent();
        if (newBound)
            this.map.setCenter(newBound.getCenterLonLat(), zoom);
    },
    setCenter: function(lonlat, zoom) {
        if (!lonlat) return null;
        this.map.setCenter(this.transform(lonlat), zoom);
    },
    setZoomOn: function(layer) {
        if (!this.layers[layer]) return null;
        var newBound = this.layers[layer].getDataExtent();
        if (newBound)
            this.map.zoomToExtent(newBound);
    },
    zoomTo: function(zoom) {
        this.map.zoomTo(zoom);
    },
    switchMode: function(mode) {
        if (mode != "pan") this.navigateButton.deactivate();
        if (this.ctl_draw_poly && mode != "drawpolygon") { this.drawPolygonButton.deactivate(); this.ctl_draw_poly.deactivate(); }
        if (this.ctl_draw_circle && mode != "drawcircle") { this.drawCircleButton.deactivate(); this.ctl_draw_circle.deactivate(); }
        if (this.ctl_draw_square && mode != "drawsquare") { this.drawSquareButton.deactivate(); this.ctl_draw_square.deactivate(); }
        if (this.ctl_draw_line && mode != "drawline") { this.drawLineButton.deactivate(); this.ctl_draw_line.deactivate(); }
        if (this.ctl_mod_feature && mode != "modfeature") { this.modFeatureButton.deactivate(); this.ctl_mod_feature.deactivate(); }
        if (this.ctl_select_poly && mode != "selectpolygon") { this.selectPolygonButton.deactivate(); this.ctl_select_poly.deactivate(); }
        if (this.ctl_measure_path && mode != "measurepath") { this.measurePathButton.deactivate(); this.ctl_measure_path.deactivate(); }
        if (this.ctl_measure_poly && mode != "measurepolygon") { this.measurePolygonButton.deactivate(); this.ctl_measure_poly.deactivate(); }
        if (this.ctl_measure_output) this.ctl_measure_output.panel_div.innerHTML = "";

        switch (mode) {
            case "pan": this.navigateButton.activate(); break;
            case "measurepath": this.measurePathButton.activate(); this.ctl_measure_path.activate(); break;
            case "measurepolygon": this.measurePolygonButton.activate(); this.ctl_measure_poly.activate(); break;
            case "drawpolygon": this.drawPolygonButton.activate(); this.ctl_draw_poly.activate(); break;
            case "drawcircle": this.drawCircleButton.activate(); this.ctl_draw_circle.activate(); break;
            case "drawsquare": this.drawSquareButton.activate(); this.ctl_draw_square.activate(); break;
            case "drawline": this.drawLineButton.activate(); this.ctl_draw_line.activate(); break;
            case "modfeature": this.modFeatureButton.activate(); this.ctl_mod_feature.activate(); break;
            case "selectpolygon": if (this.ctl_select_poly) { this.selectPolygonButton.activate(); this.ctl_select_poly.activate(); } else this.switchMode("pan"); break;
        }
    },
    createToolbar: function(include_draw_poly, include_draw_circle, include_draw_square, include_draw_line, include_select_poly, include_mod_feature, include_measure_path, include_measure_poly, select_layer_name) {
        this.navigateButton = this.createToolbarButton("Navegar mapa", this.switchMode.bind(this, "pan"), "olToolbarButtonPan");
        var buttons = [this.navigateButton];
        var layer = new OpenLayers.Layer.Vector("draw_polygon_layer", { visibility: true, displayInLayerSwitcher: false });
        this.map.addLayer(layer);
        this.map.setLayerIndex(layer, 0);
        this.drawLayer = layer;

        if (include_select_poly) {
            this.ctl_select_poly = new OpenLayers.Control.SelectFeature(this.layers[select_layer_name], {
                'multiple': false,
                'clickout': true,
                'toggle': true,
                'hover': false,
                'highlightOnly': false,
                'box': false,
                'onSelect': this.onPolygonSelect.bind(this),
                'onUnselect': this.onPolygonUnselect.bind(this),
                //'selectStyle': null,
                'displayClass': 'olControlSelectFeaturePolygon'
            });
            this.addControl(this.ctl_select_poly);
            //this.ctl_select_poly.events.on({"featureadded": this.onpolygon.bindAsEventListener(this)});
            this.selectPolygonButton = this.createToolbarButton("Seleccionar poligono", this.switchMode.bind(this, "selectpolygon"), "olToolbarButtonSelectPolygon");
            buttons.push(this.selectPolygonButton);
        }

        if (include_draw_poly) {
            this.ctl_draw_poly = new OpenLayers.Control.DrawFeature(layer, OpenLayers.Handler.Polygon, { 'displayClass': 'olControlDrawFeaturePolygon' });
            this.addControl(this.ctl_draw_poly);
            this.ctl_draw_poly.events.on({ "featureadded": OpenLayers.Function.bindAsEventListener(this.onpolygon, this) });
            this.drawPolygonButton = this.createToolbarButton("Dibujar poligono", this.switchMode.bind(this, "drawpolygon"), "olToolbarButtonDrawPolygon");
            buttons.push(this.drawPolygonButton);
        }
        if (include_draw_circle) {
            this.ctl_draw_circle = new OpenLayers.Control.DrawFeature(layer, OpenLayers.Handler.Circle, { 'displayClass': 'olControlDrawFeaturePolygon' });
            this.addControl(this.ctl_draw_circle);
            this.ctl_draw_circle.events.on({ "featureadded": OpenLayers.Function.bindAsEventListener(this.oncircle, this) });
            this.drawCircleButton = this.createToolbarButton("Dibujar Circulo", this.switchMode.bind(this, "drawcircle"), "olToolbarButtonDrawCircle");
            buttons.push(this.drawCircleButton);
        }
        if (include_draw_square) {
            this.ctl_draw_square = new OpenLayers.Control.DrawFeature(layer, OpenLayers.Handler.Square, { 'displayClass': 'olControlDrawFeaturePolygon' });
            this.addControl(this.ctl_draw_square);
            this.ctl_draw_square.events.on({ "featureadded": OpenLayers.Function.bindAsEventListener(this.onsquare, this) });
            this.drawSquareButton = this.createToolbarButton("Dibujar Cuadrado", this.switchMode.bind(this, "drawsquare"), "olToolbarButtonDrawSquare");
            buttons.push(this.drawSquareButton);
        }
        if (include_draw_line) {
            this.ctl_draw_line = new OpenLayers.Control.DrawFeature(layer, OpenLayers.Handler.Path, { 'displayClass': 'olControlDrawFeaturePolygon' });
            this.addControl(this.ctl_draw_line);
            this.ctl_draw_line.events.on({ "featureadded": OpenLayers.Function.bindAsEventListener(this.onpolyline, this) });
            this.drawLineButton = this.createToolbarButton("Dibujar trazo", this.switchMode.bind(this, "drawline"), "olToolbarButtonDrawLine");
            buttons.push(this.drawLineButton);
        }
        if (include_mod_feature) {
            var sel = new OpenLayers.Control.SelectFeature(this.layers[select_layer_name], { 'multiple': false, 'clickout': true, 'toggle': true, 'hover': false, 'highlightOnly': false, 'box': false });
            this.addControl(sel);
            this.ctl_mod_feature = new OpenLayers.Control.ModifyFeature(this.layers[select_layer_name]);
            this.addControl(this.ctl_mod_feature);
            this.layers[select_layer_name].events.on({ "afterfeaturemodified": OpenLayers.Function.bindAsEventListener(this.onfeaturemodified, this) });
            this.modFeatureButton = this.createToolbarButton("Modificar elemento", this.switchMode.bind(this, "modfeature"), "olToolbarButtonModFeature");
            buttons.push(this.modFeatureButton);
        }
        if (include_measure_path) {
            this.ctl_measure_path = new OpenLayers.Control.Measure(OpenLayers.Handler.Path, { persist: true, id: 'MeasurePath', geodesic: true });
            this.addControl(this.ctl_measure_path);
            this.measurePathButton = this.createToolbarButton("Medir Trazado", this.switchMode.bind(this, "measurepath"), "olToolbarButtonMeasurePath");
            buttons.push(this.measurePathButton);

            this.ctl_measure_path.events.on({
                "measure": this.handleMeasurements.bind(this),
                "measurepartial": this.handleMeasurements.bind(this)
            });
        }
        if (include_measure_poly) {
            this.ctl_measure_poly = new OpenLayers.Control.Measure(OpenLayers.Handler.Polygon, { persist: true, id: 'MeasurePolygon', geodesic: true });
            this.addControl(this.ctl_measure_poly);
            this.measurePolygonButton = this.createToolbarButton("Medir Area", this.switchMode.bind(this, "measurepolygon"), "olToolbarButtonMeasurePolygon");
            buttons.push(this.measurePolygonButton);

            this.ctl_measure_poly.events.on({
                "measure": this.handleMeasurements.bind(this),
                "measurepartial": this.handleMeasurements.bind(this)
            });
        }

        var toolbar = new OpenLayers.Control.Panel({ type: OpenLayers.Control.TYPE_TOGGLE, displayClass: "olToolbar" });
        this.addControl(toolbar);
        toolbar.addControls(buttons);


        for (w in buttons) this.map.removeControl(buttons[w]);

        if (include_measure_path || include_measure_poly) {
            var panel_measure_output = new OpenLayers.Control.Panel({ type: OpenLayers.Control.TYPE_TOGGLE, displayClass: "measure_output" });
            this.ctl_measure_output = new OpenLayers.Control.Button({ 'trigger': function() { } });
            panel_measure_output.addControls([this.ctl_measure_output]);
            this.addControl(panel_measure_output);
            this.map.removeControl(this.ctl_measure_output);
        }

        buttons[0].activate();
    },
    createToolbarButton: function(title, trigger, cssclass) {
        return new OpenLayers.Control.Button({ 'displayClass': cssclass, 'title': title, 'trigger': trigger });
    },
    onfeaturemodified: function(event) {
        var pol = this.reverseTransform(event.feature.geometry.clone());

        if (this.modFeatureMethod == 'C') {
            CallServer('FeatureModified:' + pol, '');
        }
        else {
            eval(this.modFeaturePostBack.replace('FeatureModified:', 'FeatureModified:' + pol));
        }
        this.switchMode("pan");
        this.drawLayer.destroyFeatures();
    },
    onpolygon: function(event) {
        var pol = this.reverseTransform(event.feature.geometry.clone());

        if (this.drawPolygonMethod == 'C') {
            CallServer('DrawPolygon:' + pol, '');
        }
        else {
            eval(this.drawPolygonPostBack.replace('DrawPolygon:', 'DrawPolygon:' + pol));
        }
        this.switchMode("selectpolygon");
        this.drawLayer.destroyFeatures();
    },
    onpolyline: function(event) {
        var pol = this.reverseTransform(event.feature.geometry.clone());

        if (this.drawPolylineMethod == 'C') {
            CallServer('DrawLine:' + pol, '');
        }
        else {
            eval(this.drawPolylinePostBack.replace('DrawLine:', 'DrawLine:' + pol));
        }
        this.switchMode("pan");
        this.drawLayer.destroyFeatures();
    },
    onsquare: function(event) {
        var pol = this.reverseTransform(event.feature.geometry.clone());

        if (this.drawPolygonMethod == 'C') {
            CallServer('DrawSquare:' + pol, '');
        }
        else {
            eval(this.drawPolygonPostBack.replace('DrawPolygon:', 'DrawSquare:' + pol));
        }
        this.switchMode("selectpolygon");
        this.drawLayer.destroyFeatures();
    },
    oncircle: function(event) {
        var radio = parseInt(event.object.handler.radio2);
        var feat = event.feature.geometry.clone();
        var pol = this.reverseTransform(feat) + '(' + radio + ')';

        if (this.drawPolygonMethod == 'C') {
            CallServer('DrawCircle:' + pol, '');
        }
        else {
            eval(this.drawCirclePostBack.replace('DrawCircle:', 'DrawCircle:' + pol));
        }

        this.switchMode("selectpolygon");
        this.drawLayer.destroyFeatures();
    },
    onPolygonSelect: function(feature) {
        this.selectedPolygonId = feature.id;
        this.setValue();
    },
    onPolygonUnselect: function(feature) {
        this.selectedPolygonId = '';
        this.setValue();
    },
    addControl: function(control) {
        if (!control) return;
        this.map.addControl(control);
    },
    handleMeasurements: function(event) {
        var geometry = event.geometry;
        var units = event.units;
        var order = event.order;
        var measure = event.measure;
        var element = this.ctl_measure_output.panel_div;
        var out = "";
        if (order == 1) out += measure.toFixed(2) + " " + units;
        else out += measure.toFixed(2) + " " + units + "<sup>2</" + "sup>";

        element.innerHTML = out;
    },
    addLayer: function(layerName, layer) {
        layer.lastVisibility = layer.getVisibility();
        this.layers[layerName] = layer;
        this.elements[layerName] = {};
        this.map.addLayer(layer);
        layer.events.register('visibilitychanged', layer, this.visibilityChanged);
    },
    getLayer: function(layer) {
        if (!this.layers[layer]) return null;
        return this.layers[layer];
    },
    getElement: function(id, layer) {
        if (!this.elements[layer] || !this.elements[layer][id]) return null;
        return this.elements[layer][id];
    },
    aM: function(id, marker, layer) { return this.addMarker(id, marker, layer); },
    addMarker: function(id, marker, layer) {
        marker.lonlat = this.transform(marker.lonlat);
        this.removeMarker(id, layer);
        this.elements[layer][id] = marker;
        this.getLayer(layer).addMarker(marker);
    },
    aMP: function(id, marker, layer, contentHTML) { return this.addMarkerWithPopup(id, marker, layer, contentHTML); },
    addMarkerWithPopup: function(id, marker, layer, contentHTML) {
        var element = this.getElement(id, layer);
        var openPopup = element && element.popup && element.popup.div && element.popup.div.style.display != 'none';

        this.addMarker(id, marker, layer);
        marker.popupContentHTML = contentHTML;
        marker.events.register("mousedown", marker, this.markerClick);

        if (openPopup) this.markerClick.bind(marker)();
    },
    markerClick: function() {
        if (this.popup && this.popup.div && this.popup.div.style.display != 'none') {
            this.popup.destroy();
            this.popup = null;
        }
        else {
            var jsPrefix = "javascript:";
            var popupContent = this.popupContentHTML;
            if (this.popupContentHTML.substr(0, jsPrefix.length) == jsPrefix)
                popupContent = eval(this.popupContentHTML.substr(jsPrefix.length));

            if (popupContent) {
                if (!onPause && typeof timerCallback != 'undefined') {
                    timerCallback.pause();
                    onPause = true;
                }
                var popup = new OpenLayers.Popup.FramedCloud("popup",
                       this.lonlat,
                       null,
                       popupContent,
                       this.icon,
                       true, function() { onPause = false; lastUpdate = new Date(); this.popup.destroy(); this.popup = null; } .bind(this));
                this.popup = popup;
                /*if (!this.map.multiplePopUps) {
                var old = this.map.popups[0];
                if (old) {
                this.map.removePopup(old);
                old.destroy();
                }
                }*/
                this.map.addPopup(popup, !this.map.multiplePopUps);
            }
        }
    },
    removeMarker: function(id, layer) {
        var element = this.getElement(id, layer);
        var layr = this.getLayer(layer);
        if (element == null) return;
        if (element.popup) element.popup.destroy();
        if (layr != null) layr.removeMarker(element);
        element.destroy();
        delete element;
        this.elements[layer][id] = undefined;
    },
    clearLayer: function(layer) {
        var lyr = this.getLayer(layer);
        if (lyr == null) return;
        if (lyr.destroyFeatures) {
            if (this.elements[layer]) this.elements[layer] = {};
            if (this.geocercas[layer]) this.geocercas[layer] = {};
            lyr.destroyFeatures();
        }
        else {
            var ms = this.elements[layer];
            for (w in ms) this.removeMarker(w, layer);
        }
    },
    clearElements: function() {
        for (layer in this.elements)
            this.elements[layer] = {};

        this.geocercas = {};
    },
    transform: function(obj) {
        return obj.transform(this.displayProjection, this.projection);
    },
    reverseTransform: function(obj) {
        return obj.transform(this.projection, this.displayProjection);
    },
    visible: true,
    show: function() {
        if (this.visible) return;
        this.div.style['visibility'] = 'visible';
        for (w in this.layers) this.layers[w].setVisibility(this.layers[w].lastVisibility);
        this.visible = true;
    },
    hide: function() {
        if (!this.visible) return;
        this.div.style['visibility'] = 'hidden';
        for (w in this.layers) {
            var lastVisibility = this.layers[w].getVisibility();
            this.layers[w].setVisibility(false);
            this.layers[w].lastVisibility = lastVisibility;
        }
        this.visible = false;
    },
    aG: function(id, feature, layer) { return this.addGeometry(id, feature, layer); },
    addGeometry: function(id, feature, layer) {
        this.removeGeometry(id, layer);
        if (!feature.omitTransform)
            if (feature.tranform) this.transform(feature);
        else this.transform(feature.geometry);
        feature.id = id;
        this.elements[layer][id] = feature;
        this.getLayer(layer).addFeatures([feature]);
    },
    rG: function(id, layer) { return this.removeGeometry(id, layer); },
    removeGeometry: function(id, layer) {
        var element = this.getElement(id, layer);
        if (element == null) return;
        var layr = this.getLayer(layer);
        if (layr != null) layr.destroyFeatures([element]);
        this.elements[layer][id] = undefined;
        this.removeGeoCerca(id, layer);
    },
    aGC: function(id, geometry, layer) { return this.addGeoCerca(id, geometry, layer); },
    addGeoCerca: function(id, geometry, layer) {
        if (!this.geocercas[layer]) this.geocercas[layer] = {};
        this.addGeometry(id, geometry, layer);
        this.geocercas[layer][id] = geometry;
    },
    getGeoCerca: function(id, layer) {
        if (!this.geocercas[layer] || !this.geocercas[layer][id]) return null;
        return this.geocercas[layer][id];
    },
    removeGeoCerca: function(id, layer) {
        var element = this.getGeoCerca(id, layer);
        if (element == null) return;
        this.geocercas[layer][id] = undefined;
    },
    GCS: function(x, y, radius, style) { return this.getGeoCercaStyle(x, y, radius, style); },
    getGeoCercaStyle: function(x, y, radius, style, omitTransform) {
        if (style) {
            var centerLonLat = new OpenLayers.LonLat(x, y);
            if (!omitTransform) centerLonLat = this.transform(centerLonLat);
            var borderLonLat = centerLonLat.clone();
            borderLonLat.lon += radius;

            var centerPoint = new OpenLayers.Geometry.Point(centerLonLat.lon, centerLonLat.lat);
            var borderPoint = new OpenLayers.Geometry.Point(borderLonLat.lon, borderLonLat.lat);

            var geodesicDistance = this.calculateDistance(centerPoint, borderPoint);
            var linearDistance = centerPoint.distanceTo(borderPoint);

            var realRadius = parseInt(radius * (linearDistance / geodesicDistance));

            borderLonLat.lon += (realRadius - radius);

            var centerPixel = this.map.getPixelFromLonLat(centerLonLat);
            var borderPixel = this.map.getPixelFromLonLat(borderLonLat);

            style.pointRadius = Math.abs(borderPixel.x - centerPixel.x);
            style.radius = radius;
        }
        return style;
    },
    calculateDistance: function(point1, point2) {
        var gg = new OpenLayers.Projection("EPSG:4326");
        var p1 = point1.clone().transform(this.map.projection, gg);
        var p2 = point2.clone().transform(this.map.projection, gg);

        return OpenLayers.Util.distVincenty(
                    { lon: p1.x, lat: p1.y }, { lon: p2.x, lat: p2.y }) * 1000;
    },
    resizeGeocercas: function() {
        for (layer in this.geocercas)
            for (id in this.geocercas[layer]) {
            var geom = this.geocercas[layer][id].clone();
            geom.omitTransform = true;
            geom.style = this.getGeoCercaStyle(geom.geometry.x, geom.geometry.y, geom.style.radius, geom.style, true);
            this.addGeoCerca(id, geom, layer);
        }
    },
    visibilityChanged: function() {
        this.lastVisibility = this.getVisibility();
    },
    triggerEvent: function(id, layer, event) {
        var element = this.getElement(id, layer);
        if (element == null) return;
        element.events.triggerEvent(event, null);
    },
    setDefaultCenter: function(lon, lat) {
        this.map.defaultCenter = this.transform(new OpenLayers.LonLat(lon, lat));
    }
};

var $M = function(id) { return Logictracker.getMap(id); };