using Logictracker.Web.Monitor.Markers;

namespace Logictracker.Web.Monitor
{
    public static class MarkerFactory
    {
        public static Marker CreateMarker(string id, string imageUrl, double latitud, double longitud)
        {
            return CreateMarker(id, imageUrl, latitud, longitud, null);
        }
        public static Marker CreateMarker(string id, string imageUrl, double latitud, double longitud, string popupContentHtml)
        {
            return new Marker(id, imageUrl, latitud, longitud, popupContentHtml);
        }
        public static LabeledMarker CreateLabeledMarker(string id, string imageUrl, double latitud, double longitud, string label)
        {
            return CreateLabeledMarker(id, imageUrl, latitud, longitud, label, null);
        }
        public static LabeledMarker CreateLabeledMarker(string id, string imageUrl, double latitud, double longitud, string label, string labelStyle)
        {
            return CreateLabeledMarker(id, imageUrl, latitud, longitud, label, labelStyle, null);
        }
        public static LabeledMarker CreateLabeledMarker(string id, string imageUrl, double latitud, double longitud, string label, string labelStyle, string popupContentHtml)
        {
            return new LabeledMarker(id, imageUrl, latitud, longitud, label, labelStyle, popupContentHtml);
        }
    }
}