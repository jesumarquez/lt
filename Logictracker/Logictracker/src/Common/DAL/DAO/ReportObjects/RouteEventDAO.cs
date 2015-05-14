using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.ReportObjects;
using Logictracker.Utils;

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Data access class for the route events.
    /// </summary>
    public class RouteEventDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public RouteEventDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Const Private Properties

        /// <summary>
        /// Valor constante de un radian.
        /// </summary>
        private const double Radians = Math.PI / 180;

        #endregion

        #region Public Methods

        /// <summary>
        /// Method for getting a list of route events for a mobile
        /// within a givenn date span.
        /// </summary>
        /// <param name="mobileId">The mobile id.</param>
        /// <param name="initialDate">The initial date.</param>
        /// <param name="finalDate">The final date.</param>
        /// <param name="noReportTime">Time to be considered as no report interval. In minutes.</param>
        /// <returns>A list of route events.</returns>
        public List<RouteEvent> GetRouteEvents(int mobileId, DateTime initialDate, DateTime finalDate, int noReportTime)
        {
            var results = new List<RouteEvent>();

            var vehicle = DAOFactory.CocheDAO.FindById(mobileId);
            var maxMonths = vehicle != null && vehicle.Empresa != null ? vehicle.Empresa.MesesConsultaPosiciones : 3;

            var posiciones = DAOFactory.LogPosicionDAO.GetPositionsBetweenDates(mobileId, initialDate, finalDate, maxMonths).OfType<LogPosicionBase>().ToList();

            //if (posiciones.Count.Equals(0))
            //    posiciones = DAOFactory.LogPosicionHistoricaDAO.GetPositionsBetweenDates(mobileId, initialDate, finalDate).OfType<LogPosicionBase>().ToList();

            if (posiciones.Count.Equals(0)) return results;

            posiciones.OrderBy(pos => pos.FechaMensaje);

            if (posiciones.Count <= 1) return results;

            var iniDate = posiciones.First().FechaMensaje;
            var finDate = posiciones.First().FechaMensaje;
            var finLat = posiciones.First().Latitud;
            var finLon = posiciones.First().Longitud;
            var iniLat = finLat;
            var iniLon = finLon;
            var distance = 0.0;
            var direction = CardinalDirection(iniLat, iniLon, finLat, finLon);
            var directionDescription = string.Empty;
            var velocidades = new List<int> {posiciones.First().Velocidad};
            var inDetention = false;

            posiciones.Remove(posiciones.First());

            foreach (var pos in posiciones)
            {
                var actDirection = CardinalDirection(finLat, finLon, pos.Latitud, pos.Longitud);

                directionDescription = CardinalDirectionDescription(direction);

                var actDirectionDescription = CardinalDirectionDescription(actDirection);

                var noReport = (pos.FechaMensaje.Subtract(finDate).TotalMinutes > noReportTime);

                if (noReport 
                    || (velocidades.Max() > 0 && (pos.Velocidad == 0 || (directionDescription != string.Empty
                        && actDirectionDescription != string.Empty && directionDescription != actDirectionDescription))) 
                    || (velocidades.Max().Equals(0) && (pos.Velocidad > 0 && (iniDate != finDate || pos.FechaMensaje != finDate))))
                {
                    if (pos.Velocidad == 0)
                    {
                        distance = distance + Distancias.Loxodromica(finLat, finLon, pos.Latitud, pos.Longitud) / 1000;

                        finLat = pos.Latitud;
                        finLon = pos.Longitud;
                        finDate = pos.FechaMensaje;
                        inDetention = true;
                    }
                    else
                    {
                        if (inDetention) 
                            finDate = pos.FechaMensaje;

                        inDetention = false;
                    }

                    results.Add(new RouteEvent
                                    {
                                        InitialDate = iniDate,
                                        FinalDate = finDate,
                                        MinimumSpeed = velocidades.Min(),
                                        MaximumSpeed = velocidades.Max(),
                                        AverageSpeed = velocidades.Average(),
                                        Direction = direction,
                                        State = velocidades.Max() > 0 ? "EN MOVIMIENTO - " + directionDescription : "DETENIDO",
                                        Distance = distance,
                                        InitialLatitude = iniLat,
                                        InitialLongitude = iniLon,
                                        FinalLatitude = finLat,
                                        FinalLongitude = finLon
                                    });

                    velocidades.Clear();

                    distance = 0.0;
                    iniLat = noReport ? pos.Latitud : finLat;
                    iniLon = noReport ? pos.Longitud : finLon;
                    iniDate = noReport ? pos.FechaMensaje : finDate;
                }

                distance = distance + Distancias.Loxodromica(finLat, finLon, pos.Latitud, pos.Longitud)/1000;

                finDate = pos.FechaMensaje;
                finLat = pos.Latitud;
                finLon = pos.Longitud;

                direction = CardinalDirection(iniLat, iniLon, finLat, finLon);

                velocidades.Add(pos.Velocidad);
            }
            
            results.Add(new RouteEvent
                            {
                                InitialDate = iniDate,
                                FinalDate = posiciones.Last().FechaMensaje,
                                MinimumSpeed = velocidades.Min(),
                                MaximumSpeed = velocidades.Max(),
                                AverageSpeed = velocidades.Average(),
                                Direction = direction,
                                State = velocidades.Max() > 0 ? "EN MOVIMIENTO - " + directionDescription : "DETENIDO",
                                Distance = distance,
                                InitialLatitude = iniLat,
                                InitialLongitude = iniLon,
                                FinalLatitude = posiciones.Last().Latitud,
                                FinalLongitude = posiciones.Last().Longitud
                            });

            return results;
        }

        public List<RouteEvent> GetRouteEvents(List<int> moviles, DateTime initialDate, DateTime finalDate, int noReportTime)
        {
            var rutas = new List<RouteEvent>();

            foreach (var movil in moviles)
            {
                var routes = GetRouteEvents(movil, initialDate, finalDate, noReportTime);
                if (routes.Count > 0)  rutas.AddRange(routes);
            }
            return rutas;
        }

        #endregion

        #region Private Methods

        private static double CardinalDirection(double initialLatitude, double initialLongitude, double finalLatitude, double finalLongitude)
        {
            var inilat = initialLatitude*Radians;
            var finLat = finalLatitude*Radians;
            var lonDif = (finalLongitude - initialLongitude) * Radians;

            if (lonDif != 0)
            {
                var y = Math.Sin(lonDif)*Math.Cos(finLat);
                var x = Math.Cos(inilat)*Math.Sin(finLat) - Math.Sin(inilat)*Math.Cos(finLat)*Math.Cos(lonDif);

                return FloatModulus(Math.Atan2(y,x)/Radians + 360.0, 360.0);
            }

            return 0.0;
        }

        private static double FloatModulus(double a, double b) { return a >= b ? FloatModulus(a - b, b) : a; }

        private static string CardinalDirectionDescription(double degrees)
        {
            if ((0.0 <= degrees && degrees < 22.5) || (337.5 <= degrees && @degrees < 360.1)) return "NORTE";
            if (22.5 <= degrees && degrees < 67.5) return "NOR-ESTE";
            if (67.5 <= degrees && degrees < 112.5) return "ESTE";
            if (112.5 <= degrees && degrees < 157.5) return "SUR-ESTE";
            if (157.5 <= degrees && degrees < 202.5) return "SUR";
            if (202.5 <= degrees && degrees < 247.5) return "SUR-OESTE";
            if (247.5<= degrees && degrees < 292.5) return "OESTE";
            if (292.5 <= degrees && degrees < 337.5) return "NOR-OESTE";

            return "";
        }

        #endregion
    }
}
