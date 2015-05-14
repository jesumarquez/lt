namespace Logictracker.Web.Monitor
{
    public static class ControlFactory
    {
        public static Control GetPanZoomBar()
        {
            return GetPanZoomBar(true);
        }
        public static Control GetPanZoomBar(bool zoomWorldIcon)
        {
            var c = new Control("PanZoomBar", "new OpenLayers.Control.PanZoomBar({'zoomWorldIcon': " + (zoomWorldIcon?"true":"false") + "})");
            return c;
        }
        public static Control GetNavigation()
        {
            var c = new Control("Navigation", "new OpenLayers.Control.Navigation()");
            return c;
        }
        public static Control GetLayerSwitcher()
        {
            var c = new Control("LayerSwitcher", "new OpenLayers.Control.LayerSwitcher()");
            return c;
        }
        public static Control GetKeyboardDefaults()
        {
            var c = new Control("KeyboardDefaults", "new OpenLayers.Control.KeyboardDefaults()");
            return c;
        }
        public static ContextMenu.ContextMenu GetContextMenu()
        {
            var c = new ContextMenu.ContextMenu("ContextMenu", "olContextMenu");
            return c;
        }
        public static Control GetToolbar(bool drawPolygon, bool drawCircle, bool drawSquare, bool drawLine, bool selectPolygon, bool measurePath, bool measurePolygon)
        {
            return GetToolbar(drawPolygon, drawCircle, drawSquare, drawLine, selectPolygon, false, measurePath, measurePolygon, "");
        }
        public static Control GetToolbar(bool drawPolygon, bool drawCircle, bool drawSquare, bool drawLine, bool selectPolygon, bool editElements, bool measurePath, bool measurePolygon, string layerName)
        {
            var c = new Control("Toolbar", string.Format("{{map}}.createToolbar({0},{1},{2},{3},{4},{5},{6},{7},'{8}')",
                drawPolygon ? "true" : "false",
                drawCircle ? "true" : "false",
                drawSquare ? "true" : "false",
                drawLine ? "true" : "false",
                selectPolygon ? "true" : "false",
                editElements ? "true" : "false",
                measurePath ? "true" : "false",
                measurePolygon ? "true" : "false",
                layerName));
            return c;
        }
    }
}