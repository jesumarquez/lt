namespace Logictracker.Web.Monitor
{
    public static class LayerFactory
    {
        public static Layer GetGoogleSatellite(string layerName, int minZoomLevel)
        {
            return new Layer(layerName, string.Format("new OpenLayers.Layer.Google('{0}', {{type: google.maps.MapTypeId.SATELLITE, minZoomLevel: {1}, 'sphericalMercator': true}})", layerName, minZoomLevel), true);
        }
        public static Layer GetGoogleHybrid(string layerName, int minZoomLevel)
        {
            return new Layer(layerName, string.Format("new OpenLayers.Layer.Google('{0}', {{type: google.maps.MapTypeId.HYBRID, minZoomLevel: {1}, 'sphericalMercator': true}})", layerName, minZoomLevel), true);
        }
        public static Layer GetGooglePhysical(string layerName, int minZoomLevel)
        {
            return new Layer(layerName, string.Format("new OpenLayers.Layer.Google('{0}', {{type: google.maps.MapTypeId.TERRAIN, minZoomLevel: {1}, 'sphericalMercator': true}})", layerName, minZoomLevel), true);
        }
        public static Layer GetGoogleStreet(string layerName, int minZoomLevel, string mapTypeId, string stylez)
        {
            var l = new Layer(layerName, string.Format("new OpenLayers.Layer.Google('{0}', {{type: '{2}', minZoomLevel: {1}, 'sphericalMercator': true}})", layerName, minZoomLevel, mapTypeId), true);
            string postCode = "{0}.getLayer('{1}').mapObject.mapTypes.set('" + mapTypeId + "', new google.maps.StyledMapType(" + stylez + ", {{ name: '" + mapTypeId + "' }}));";
            postCode = postCode + "{0}.getLayer('{1}').mapObject.mapTypeId='" + mapTypeId + "';";
            l.PostCode = postCode;
            return l;
        }
        
        public static Layer GetGoogleStreet(string layerName, int minZoomLevel)
        {
            return new Layer(layerName, string.Format("new OpenLayers.Layer.Google('{0}', {{minZoomLevel: {1}, 'sphericalMercator': true}})", layerName, minZoomLevel), true);
        }
        public static Layer GetGML(string layerName, string fileName, bool visibility)
        {
            return new Layer(layerName, string.Format(@"new OpenLayers.Layer.GML('{0}', '{1}', {{projection: {2},
                visibility: {3}, format: OpenLayers.Format.KML, formatOptions: {{extractStyles: true, extractAttributes:
                true}}, 'sphericalMercator': true}})", layerName, fileName, ProjectionFactory.GetGoogle(),
                                                      visibility ? "true" : "false"), true);
        }
/*        public static Layer GetCompumap(string layerName, string imageHandler, int minZoomLevel)
        {
            return new Layer(layerName, string.Format("new OpenLayers.Layer.Compumap('{0}', '{1}',{{ 'minZoomLevel': {2} }})", layerName, imageHandler, minZoomLevel), false);
        }
 */
        public static Layer GetOpenStreetMap(string layerName)
        {
            return new Layer(layerName, string.Format("new OpenLayers.Layer.OSM('{0}')", layerName), false);
        }
        public static Layer GetMarkers(string layerName, bool visibility)
        {
            return new Layer(layerName, string.Format(@"new OpenLayers.Layer.Markers('{0}',{{'maxResolution': 111115.4,
                'minResolution': 0.00001, 'units': 'm', visibility: {1} }})", layerName, visibility ? "true" : "false"));
        }
        public static Layer GetVector(string layerName, bool visibility)
        {
            //return new Layer(LayerName, string.Format("new OpenLayers.Layer.Vector('{0}', {{ visibility: {1}, renderers: ['Canvas', 'VML'] }})",
            //                                          LayerName, visibility ? "true" : "false"));
            return new Layer(layerName, string.Format("new OpenLayers.Layer.Vector('{0}', {{ visibility: {1} }})", layerName, visibility ? "true" : "false"));
        }
        public static Layer GetVector(string layerName, bool visibility, string style)
        {
            //return new Layer(LayerName, string.Format("new OpenLayers.Layer.Vector('{0}', {{ visibility: {1}, style: {2}, renderers: ['Canvas', 'VML'] }})",
            //                                          LayerName, visibility ? "true" : "false", style));
            return new Layer(layerName, string.Format("new OpenLayers.Layer.Vector('{0}', {{ visibility: {1}, style: {2} }})", layerName, visibility ? "true" : "false", style));
        }
    }
}