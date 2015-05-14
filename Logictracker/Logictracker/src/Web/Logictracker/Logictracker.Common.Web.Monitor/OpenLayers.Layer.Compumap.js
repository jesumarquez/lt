/**
 * Namespace: Util.Compumap
 */
OpenLayers.Util.Compumap = {};

/**
 * Constant: MISSING_TILE_URL
 * {String} URL of image to display for missing tiles
 */
// OpenLayers.Util.Compumap.MISSING_TILE_URL = "http://190.51.191.251/mapas/tiles/404.png";

OpenLayers.Util.Compumap.MISSING_TILE_URL = "img/404.png";
/**
 * Property: originalOnImageLoadError
 * {Function} Original onImageLoadError function.
 */
OpenLayers.Util.Compumap.originalOnImageLoadError = OpenLayers.Util.onImageLoadError;

/**
 * Function: onImageLoadError
 */
OpenLayers.Util.onImageLoadError = function() {
    if (this.src.match(/^http:\/\/[abc]\.[a-z]+\.190.51.191.11\//)) {
        this.src = OpenLayers.Util.Compumap.MISSING_TILE_URL;
    } else if (this.src.match(/^http:\/\/[def]\.tah\.190.51.191.11\//)) {
        // do nothing - this layer is transparent
    } else {

        this.src = OpenLayers.Util.Compumap.MISSING_TILE_URL;
        //OpenLayers.Util.Compumap.originalOnImageLoadError;
    }
};

/**
 * @requires OpenLayers/Layer/TMS.js
 *
 * Class: OpenLayers.Layer.Compumap
 *
 * Inherits from:
 *  - <OpenLayers.Layer.TMS>
 */
OpenLayers.Layer.Compumap = OpenLayers.Class(
OpenLayers.Layer.TMS,
OpenLayers.Layer.FixedZoomLevels,
{
    buffer: 0,
    /** 
    * Constant: MIN_ZOOM_LEVEL
    * {Integer} 0 
    */
    MIN_ZOOM_LEVEL: 0,

    /** 
    * Constant: MAX_ZOOM_LEVEL
    * {Integer} 19
    */
    MAX_ZOOM_LEVEL: 18,
    RESOLUTIONS: [
            156543,
            78271.5,
            39135.75,
            19567.875,
            9783.9375,
            4891.96875,
            2445.984375,
            1222.9921875,
            611.49609375,
            305.748046875,
            152.8740234375,
            76.43701171875,
            38.218505859375,
            19.1092529296875,
            9.55462646484375,
            4.777313232421875,
            2.3886566162109375,
            1.1943283081054688,
            0.5971641540527344
    /*1.40625, 
    0.703125, 
    0.3515625, 
    0.17578125, 
    0.087890625, 
    0.0439453125,
    0.02197265625, 
    0.010986328125, 
    0.0054931640625, 
    0.00274658203125,
    0.001373291015625, 
    0.0006866455078125, 
    0.00034332275390625,
    0.000171661376953125, 
    0.0000858306884765625, 
    0.00004291534423828125,
    0.00002145767211914062, 
    0.00001072883605957031,
    0.00000536441802978515, 
    0.00000268220901489257*/
    ],
    totalExtent: new OpenLayers.Bounds(-6487128, -1767606, -6537616, -1787006),
    /**
    * Constructor: OpenLayers.Layer.Compumap
    *
    * Parameters:
    * name - {String}
    * url - {String}
    * options - {Object} Hashtable of extra options to tag onto the layer
    */
    initialize: function(name, url, options) {
        //  maxExtent: new OpenLayers.Bounds(-20037508,-20037508,20037508,20037508),
        options = OpenLayers.Util.extend({
        attribution: "<a href='http://www.mapasysistemas.com.ar/'>Logictracker</a>",
            //maxExtent: new OpenLayers.Bounds(-95.36404,-55.06131,-31.80558,-21.78128).transform(new OpenLayers.Projection("EPSG:4326"), new OpenLayers.Projection("EPSG:900913")),
            //maxExtent: new OpenLayers.Bounds(-20037508,-20037508,20037508,20037508),
            //maxExtent: new OpenLayers.Bounds(-6487128,-1767606,-6537616,-1787006),
            maxResolution: 156543,
            units: "m",
            projection: "EPSG:900913",
            transitionEffect: "resize",
            minZoomLevel: 0
        }, options);
        this.tileOrigin = new OpenLayers.LonLat(-20037508, 20037508);
        var newArguments = [name, url, options];
        OpenLayers.Layer.TMS.prototype.initialize.apply(this, newArguments);
        OpenLayers.Layer.FixedZoomLevels.prototype.initialize.apply(this);

        OpenLayers.Util.Compumap.MISSING_TILE_URL = this.url + '/404.png';
    },
    /**
    * Method: getUrl
    *
    * Parameters:
    * bounds - {<OpenLayers.Bounds>}
    *
    * Returns:
    * {String} A string with the layer's url and parameters and also the
    *          passed-in bounds and appropriate tile size specified as
    *          parameters
    */
    getURL: function(bounds) {
        //bounds = this.adjustBounds(bounds);
        var res = this.map.getResolution();
        var x = Math.round((bounds.left - this.tileOrigin.lon) / (res * this.tileSize.w));
        var y = Math.round((this.tileOrigin.lat - bounds.top) / (res * this.tileSize.h));
        //var z = this.map.getZoomForResolution(res)+this.minZoomLevel;
        var z = this.map.getZoom() + this.minZoomLevel;
        var limit = Math.pow(2, z);

        if (y < 0 || y >= limit) return OpenLayers.Util.Compumap.MISSING_TILE_URL;

        x = ((x % limit) + limit) % limit;

        var path = "/" + z + "/" + x + "/" + y + ".png";
        var url = this.url;

        if (url instanceof Array) {
            url = this.selectUrl(path, url);
        }

        return url + path;
    },
    /*getURL: function (bounds) {                  
    //this.maxExtent = new OpenLayers.Bounds(-20037508,-20037508,20037508,20037508);
    var res = this.map.getResolution();       
    var x = Math.round((bounds.left - this.maxExtent.left) / (res * this.tileSize.w));
    var y = Math.round((this.maxExtent.top - bounds.top) / (res * this.tileSize.h));
    //var z = this.map.getZoom();
    var z = this.map.getZoomForResolution(res)+this.minZoomLevel;
    var limit = Math.pow(2, z);
        
    delete bounds;
    delete ext;        
        
    //this.maxExtent = this.map.maxExtent;
        
    if (y < 0 || y >= limit)
    {
    return OpenLayers.Util.Compumap.MISSING_TILE_URL;
    }
    else
    {
    x = ((x % limit) + limit) % limit;

            var url = this.url;
    //     var path = "?z="+ z + "&x=" + x + "&y=" + y ;
    var path = "/"+ z + "/" + x + "/" + y  + ".png";
            
    if (url instanceof Array)
    {
    url = this.selectUrl(path, url);
    }

            return url + path;
    }
    },*/
    CLASS_NAME: "OpenLayers.Layer.Compumap"
});

/**
 * Class: OpenLayers.Layer.Compumap.Mapnik
 *
 * Inherits from:
 *  - <OpenLayers.Layer.Compumap>
 */
OpenLayers.Layer.Compumap.Compumap = OpenLayers.Class(OpenLayers.Layer.Compumap, {
    /**
     * Constructor: OpenLayers.Layer.Compumap.Mapnik
     *
     * Parameters:
     * name - {String}
     * options - {Object} Hashtable of extra options to tag onto the layer
     */
    initialize: function(name, options) {
        var url = [
            "http://192.168.10.11/mapas/tiles",
        ];
        options = OpenLayers.Util.extend({ numZoomLevels: 18 }, options);
        var newArguments = [name, url, options];
        OpenLayers.Layer.Compumap.prototype.initialize.apply(this, newArguments);
    },

    CLASS_NAME: "OpenLayers.Layer.Compumap.Compumap"
});
