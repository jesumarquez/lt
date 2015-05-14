namespace Logictracker.Web
{
    public static class UrlMaker
    {
        public static class MonitorLogistico
        {
            public static string GetUrlDistribucion(int id)
            {
                return GetUrlDistribucion(id, false);
            }
            public static string GetUrlDistribucion(int id, bool calcularRecorrido)
            {
                return GetUrl("D", id, calcularRecorrido);
            }
            public static string GetUrlHormigon(int id)
            {
                return GetUrlHormigon(id, false);
            }
            public static string GetUrlHormigon(int id, bool calcularRecorrido)
            {
                return GetUrl("T", id, calcularRecorrido);
            }
            private static string GetUrl(string type, int id, bool calcular)
            {
                return string.Format("~/CicloLogistico/Monitor.aspx?t={0}&i={1}&c={2}", type, id, calcular?1:0);
            }
        }
    }
}
