/* Copyright (c) 2006-2008 MetaCarta, Inc., published under the Clear BSD
 * license.  See http://svn.openlayers.org/trunk/openlayers/license.txt for the
 * full text of the license. */


/**
 * @requires OpenLayers/Handler/Drag.js
 */

/**
 * Class: OpenLayers.Handler.Circle
 * Handler to draw a regular polygon on the map.  Polygon is displayed on mouse
 *     down, moves or is modified on mouse move, and is finished on mouse up.
 *     The handler triggers callbacks for 'done' and 'cancel'.  Create a new
 *     instance with the <OpenLayers.Handler.Circle> constructor.
 * 
 * Inherits from:
 *  - <OpenLayers.Handler>
 */
OpenLayers.Handler.Circle = OpenLayers.Class(OpenLayers.Handler.Drag, {

    /**
     * APIProperty: radius
     * {Float} Optional radius in map units of the regular polygon.  If this is
     *     set to some non-zero value, a polygon with a fixed radius will be
     *     drawn and dragged with mose movements.  If this property is not
     *     set, dragging changes the radius of the polygon.  Set to null by
     *     default.
     */
    radius: null,
    

    /**
     * Property: feature
     * {<OpenLayers.Feature.Vector>} The currently drawn polygon feature
     */
    feature: null,

    /**
     * Property: layer
     * {<OpenLayers.Layer.Vector>} The temporary drawing layer
     */
    layer: null,

    /**
     * Property: origin
     * {<OpenLayers.Geometry.Point>} Location of the first mouse down
     */
    origin: null,

    /**
     * Constructor: OpenLayers.Handler.RegularPolygon
     * Create a new regular polygon handler.
     *
     * Parameters:
     * control - {<OpenLayers.Control>} The control that owns this handler
     * callbacks - {Object} An object with a properties whose values are
     *     functions.  Various callbacks described below.
     * options - {Object} An object with properties to be set on the handler.
     *     If the options.sides property is not specified, the number of sides
     *     will default to 4.
     *
     * Named callbacks:
     * create - Called when a sketch is first created.  Callback called with
     *     the creation point geometry and sketch feature.
     * done - Called when the sketch drawing is finished.  The callback will
     *     recieve a single argument, the sketch geometry.
     * cancel - Called when the handler is deactivated while drawing.  The
     *     cancel callback will receive a geometry.
     */
    initialize: function(control, callbacks, options) {
        this.style = OpenLayers.Util.extend(OpenLayers.Feature.Vector.style['default'], {});

        var oldStyle = this.style;
        this.style = {};
        for(w in oldStyle) this.style[w] = oldStyle[w];
        
        OpenLayers.Handler.prototype.initialize.apply(this,
                                                [control, callbacks, options]);
        this.options = (options) ? options : new Object();
    },
    
    /**
     * APIMethod: setOptions
     * 
     * Parameters:
     * newOptions - {Object} 
     */
    setOptions: function (newOptions) {
        OpenLayers.Util.extend(this.options, newOptions);
        OpenLayers.Util.extend(this, newOptions);
    },
    
    /**
     * APIMethod: activate
     * Turn on the handler.
     *
     * Return:
     * {Boolean} The handler was successfully activated
     */
    activate: function() {
        var activated = false;
        if(OpenLayers.Handler.prototype.activate.apply(this, arguments)) {
            // create temporary vector layer for rendering geometry sketch
            var options = {
                displayInLayerSwitcher: false,
                // indicate that the temp vector layer will never be out of range
                // without this, resolution properties must be specified at the
                // map-level for this temporary layer to init its resolutions
                // correctly
                calculateInRange: function() { return true; }
            };
            this.layer = new OpenLayers.Layer.Vector(this.CLASS_NAME, options);
            this.map.addLayer(this.layer);
            activated = true;
        }
        return activated;
    },

    /**
     * APIMethod: deactivate
     * Turn off the handler.
     *
     * Return:
     * {Boolean} The handler was successfully deactivated
     */
    deactivate: function() {
        var deactivated = false;
        if(OpenLayers.Handler.Drag.prototype.deactivate.apply(this, arguments)) {
            // call the cancel callback if mid-drawing
            if(this.dragging) {
                this.cancel();
            }
            // If a layer's map property is set to null, it means that that
            // layer isn't added to the map. Since we ourself added the layer
            // to the map in activate(), we can assume that if this.layer.map
            // is null it means that the layer has been destroyed (as a result
            // of map.destroy() for example.
            if (this.layer.map != null) {
                this.layer.destroy(false);
                if (this.feature) {
                    this.feature.destroy();
                }
            }
            this.layer = null;
            this.feature = null;
            deactivated = true;
        }
        return deactivated;
    },
    
    /**
     * Method: down
     * Start drawing a new feature
     *
     * Parameters:
     * evt - {Event} The drag start event
     */
    down: function(evt) {
        var maploc = this.map.getLonLatFromPixel(evt.xy);
        this.origin = new OpenLayers.Geometry.Point(maploc.lon, maploc.lat);
        // create the new polygon
        
        // smallest radius should not be less one pixel in map units
        // VML doesn't behave well with smaller
        this.radius = this.map.getResolution();
            
        if(this.persist) {
            this.clear();
        }
        this.feature = new OpenLayers.Feature.Vector();
        this.createGeometry();
        this.callback("create", [this.origin, this.feature]);
        this.layer.addFeatures([this.feature], {silent: true});
        this.layer.drawFeature(this.feature, this.style);
    },
    
    /**
     * Method: move
     * Respond to drag move events
     *
     * Parameters:
     * evt - {Evt} The move event
     */
    move: function(evt) {
        var maploc = this.map.getLonLatFromPixel(evt.xy);
        var point = new OpenLayers.Geometry.Point(maploc.lon, maploc.lat);
        this.radius = point.distanceTo(this.origin);
                                    /*Math.max(this.map.getResolution() / 2,
                                   point.distanceTo(this.origin));*/
            
        var gg = new OpenLayers.Projection("EPSG:4326");                       
        var p1 = point.clone().transform(this.layer.map.projection, gg);
        var p2 = this.origin.clone().transform(this.layer.map.projection, gg);
        
        this.radius2 = OpenLayers.Util.distVincenty(
                    {lon: p1.x, lat: p1.y}, {lon: p2.x, lat: p2.y})*1000;
        
        this.modifyGeometry();
        
        this.feature.radius = this.radius;
        
        this.layer.removeFeatures([this.feature], {silent: true});
        this.layer.addFeatures([this.feature], {silent: true});
        this.layer.drawFeature(this.feature, this.style);
    },

    /**
     * Method: up
     * Finish drawing the feature
     *
     * Parameters:
     * evt - {Event} The mouse up event
     */
    up: function(evt) {              
        this.finalize();
        // the mouseup method of superclass doesn't call the
        // "done" callback if there's been no move between
        // down and up
        if (this.start == this.last) {
            this.callback("done", [evt.xy]);
        }
    },

    /**
     * Method: out
     * Finish drawing the feature.
     *
     * Parameters:
     * evt - {Event} The mouse out event
     */
    out: function(evt) {
        this.finalize();
    },

    /**
     * Method: createGeometry
     * Create the new polygon geometry.  This is called at the start of the
     *     drag and at any point during the drag if the number of sides
     *     changes.
     */
    createGeometry: function() {           
        this.style.radius = this.radius;
        this.feature.geometry = this.origin;
    },
    
    /**
     * Method: modifyGeometry
     * Modify the polygon geometry in place.
     */
    modifyGeometry: function() {
        var centerPoint = new OpenLayers.LonLat(this.origin.x, this.origin.y);
        var borderPoint = centerPoint.clone();
        borderPoint.lon += this.radius;

        var centerPixel = this.map.getPixelFromLonLat(centerPoint);
        var borderPixel = this.map.getPixelFromLonLat(borderPoint);

        this.style.pointRadius = Math.abs(borderPixel.x - centerPixel.x);
        this.style.radius = this.radius;
        this.radio = this.radius;
        this.radio2 = this.radius2;
    },
    

    /**
     * APIMethod: cancel
     * Finish the geometry and call the "cancel" callback.
     */
    cancel: function() {
        // the polygon geometry gets cloned in the callback method
        this.callback("cancel", null);
        this.finalize();
    },

    /**
     * Method: finalize
     * Finish the geometry and call the "done" callback.
     */
    finalize: function() {
        this.layer.removeFeatures([this.feature], {silent: true});
        this.origin = null;
        this.radius = this.options.radius;
    },

    /**
     * APIMethod: clear
     * Clear any rendered features on the temporary layer.  This is called
     *     when the handler is deactivated, canceled, or done (unless persist
     *     is true).
     */
    clear: function() {
        if(this.layer != null)
        {
            this.layer.renderer.clear();
            this.layer.destroyFeatures();
        }
    },
    
    /**
     * Method: callback
     * Trigger the control's named callback with the given arguments
     *
     * Parameters:
     * name - {String} The key for the callback that is one of the properties
     *     of the handler's callbacks object.
     * args - {Array} An array of arguments with which to call the callback
     *     (defined by the control).
     */
    callback: function (name, args) {
        // override the callback method to always send the polygon geometry
        if (this.callbacks[name]) { 
            this.callbacks[name].apply(this.control,
                                       [this.feature.geometry.clone()]);
        }
        // since sketch features are added to the temporary layer
        // they must be cleared here if done or cancel
        if(!this.persist && (name == "done" || name == "cancel")) {
            this.clear();
        }
    },

    CLASS_NAME: "OpenLayers.Handler.Circle"
});
