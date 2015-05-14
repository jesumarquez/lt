namespace Logictracker.Web.Monitor
{
    public static class ProjectionFactory
    {
        public static string GetGoogle()
        {
            return GetEPSG4326();
        }
        public static string GetCompumap()
        {
            return GetEPSG900913();
        }
        public static string GetEPSG4326()
        {
            return "new OpenLayers.Projection('EPSG:4326')";
        }
        public static string GetEPSG900913()
        {
            return "new OpenLayers.Projection('EPSG:900913')"; 
        }
    }
}