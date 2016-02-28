using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Positions;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <summary>
    /// Class for implementing all the data access logic needed for the route positions.
    /// </summary>
    public class RoutePositionsDAO : GenericDAO<RoutePosition>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public RoutePositionsDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        public List<RoutePosition> GetPositions(int coche, DateTime inicio, DateTime fin, int maxMonths)
        {
            var minDesde = DateTime.UtcNow.AddMonths(-maxMonths);
            inicio = inicio > minDesde ? inicio : minDesde;

            var results = Session.Query<LogPosicion>()
                .Where(posicion => posicion.Coche.Id == coche 
                    && posicion.FechaMensaje >= inicio 
                    && posicion.FechaMensaje < fin)
                .OrderBy(p=>p.FechaMensaje)
                .Timeout(0)
                .ToList()
                .Select(posicion => GetRoutePosition(posicion, false, -1)).ToList();
            
            return results;
        }

        public IEnumerable<RoutePosition> GetPositionsDescartadas(int coche, DateTime inicio, DateTime fin, int maxMonths)
        {
            var minDesde = DateTime.UtcNow.AddMonths(-maxMonths);
            inicio = inicio > minDesde ? inicio : minDesde;

            var results = Session.Query<LogPosicionDescartada>()
                .Where(posicion => posicion.Coche.Id == coche 
                                && posicion.FechaMensaje >= inicio 
                                && posicion.FechaMensaje < fin)
                .ToList()
                .Select(posicion => GetRoutePosition(posicion, true, posicion.MotivoDescarte))
                .OrderBy(pos => pos.Date)
                .ToList();

            //foreach (var r in results)
            //{
            //    r.Recieved = r.Recieved.ToDisplayDateTime();
            //    r.Date = r.Date.ToDisplayDateTime();
            //}

            return results;
        }


        /// <summary>
        /// Instanciates a new route position.
        /// </summary>
        /// <param name="posicion"></param>
        /// <param name="historical"></param>
        /// <param name="motivoDescarte"></param>
        /// <returns></returns>
        private static RoutePosition GetRoutePosition(LogPosicionBase posicion, Boolean historical, int motivoDescarte) { return new RoutePosition(posicion, historical, motivoDescarte); }

        public List<List<RoutePosition>> GetPositionsByRoute(int mobileId, DateTime initialDate, DateTime finalDate, TimeSpan stoppedTime, int maxMonths)
        {
            var stoppedSince = DateTime.Now;
            var stopped = false;
            var route = 0;

            var positions = new List<List<RoutePosition>> {new List<RoutePosition>()};

            foreach (var position in GetPositions(mobileId, initialDate, finalDate, maxMonths))
            {
                if (position.Speed > 0)
                {
                    if (stopped && position.Date.Subtract(stoppedSince) >= stoppedTime)
                    {
                        route++;

                        positions.Add(new List<RoutePosition>());
                    }

                    stopped = false;
                }
                else if (!stopped)
                {
                    stopped = true;

                    stoppedSince = position.Date;
                }
                
                positions[route].Add(position);
            }

            for (var i = 0; i < positions.Count; i++)
            {
                if (!positions[i].Count.Equals(1) || positions.Count <= 1) continue;

                positions.RemoveAt(i);

                i--;
            }

            return positions;
        }

        #endregion
    }
}