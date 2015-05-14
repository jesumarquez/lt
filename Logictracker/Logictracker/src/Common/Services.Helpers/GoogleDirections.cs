using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Globalization;
using Logictracker.DatabaseTracer.Core;

namespace Logictracker.Services.Helpers
{
    public static class GoogleDirections
    {
        public static class Modes
        {
            /// <summary>
            /// (predeterminado) proporciona indicaciones para llegar en coche estándar a través del sistema de carreteras.
            /// </summary>
            public const string Driving = "driving";

            /// <summary>
            /// solicita rutas para llegar a pie a través de rutas peatonales y aceras (en los lugares en los que estén disponibles).
            /// </summary>
            public const string Walking = "walking";

            /// <summary>
            /// solicita rutas para llegar en bicicleta a través de carriles bici y vías preferenciales para bicicletas (actualmente, solo disponible en EE.UU.).
            /// </summary>
            public const string Bicycling = "bicycling"; 

        }

        public static class Avoids
        {
            /// <summary>
            /// indica que la ruta calculada debe evitar los peajes de carretera y de puentes.
            /// </summary>
            public const string Tolls = "tolls";

            /// <summary>
            /// indica que la ruta calculada debe evitar las autopistas y las autovías.
            /// </summary>
            public const string Highways = "highways"; 

        }

        private const string DirectionsUrl = "http://maps.googleapis.com/maps/api/directions/xml?sensor=false&language=es&units=metric&origin={0}&destination={1}&mode={2}";

        public static Directions GetDirections(LatLon origen, LatLon destino, string mode, string avoid, params LatLon[] waypoints)
        {
            try
            {
                var baseUrl = string.Format(DirectionsUrl, origen, destino, mode);

                if (!string.IsNullOrEmpty(avoid))
                {
                    baseUrl += string.Concat("&avoid=", avoid);
                }

                if (waypoints != null && waypoints.Length > 0)
                {
                    baseUrl += "&waypoints=" + waypoints.Aggregate(string.Empty, (w, y) => string.Concat(w, "|", y));
                }

                var xml = new XmlDocument();
                xml.Load(baseUrl);

                var status = xml.GetXmlNode("status");                    
                if (status == null || status.InnerText != "OK")
                {
                    return null;
                }
                var route = xml.GetXmlNode("route");
                if (route == null)
                {
                    return null;
                }

                var ci = CultureInfo.InvariantCulture;

                var summary = route.GetInnerText("summary");

                var directions = new Directions
                                     {
                                         Descripcion = summary
                                     };

                var legs = route.GetChilds("leg");

                foreach(var leg in legs)
                {
                    var steps = leg.GetChilds("step");

                    var directionsLeg = new DirectionsLeg();
                    directions.Legs.Add(directionsLeg);

                    foreach(var step in steps)
                    {
                        var stepstartlat = step.GetChild("start_location").GetInnerText("lat");
                        var stepstartlon = step.GetChild("start_location").GetInnerText("lng");
                        var stependlat = step.GetChild("end_location").GetInnerText("lat");
                        var stependlon = step.GetChild("end_location").GetInnerText("lng");
                        var stepduration = step.GetChild("duration").GetInnerText("value");
                        var stepdistance = step.GetChild("distance").GetInnerText("value");
                        var steppolyline = step.GetChild("polyline").GetInnerText("points");
                        var stepinstructions = step.GetInnerText("html_instructions");

                        var directionsStep = new DirectionsStep
                                                 {
                                                     StartLocation =
                                                         new LatLon(Convert.ToDouble(stepstartlat, ci),
                                                                    Convert.ToDouble(stepstartlon, ci)),
                                                     EndLocation =
                                                         new LatLon(Convert.ToDouble(stependlat, ci),
                                                                    Convert.ToDouble(stependlon, ci)),
                                                     Duration = TimeSpan.FromSeconds(Convert.ToInt32(stepduration, ci)),
                                                     Distance = Convert.ToDouble(stepdistance, ci),
                                                     Points = DecodePolylinePoints(steppolyline),
                                                     Instructions = stepinstructions
                                                 };

                        directionsLeg.Steps.Add(directionsStep);
                    }
                }

                return directions;
            }
            catch (Exception ex)
            {
                STrace.Exception("GoogleDirections", ex);
                return null;
            }
        }

        private static List<LatLon> DecodePolylinePoints(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints)) return null;
            var poly = new List<LatLon>();
            var polylinechars = encodedPoints.ToCharArray();
            var index = 0;

            var currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    var p = new LatLon(Convert.ToDouble(currentLat) / 100000.0, 
                        Convert.ToDouble(currentLng) / 100000.0);
                    poly.Add(p);
                }
            }
            catch (Exception)
            {
                // logo it
            }
            return poly;
        }
    }
    
}
