#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Represents mobile activities during a specified timespan for vehicles assigned to transport companies.
    /// </summary>
    public class TransportActivityDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public TransportActivityDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets mobile activity within the specified timespan for vehicles associated to transport companies.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="ids"></param>
        /// <param name="km"></param>
        /// <returns></returns>
        public IEnumerable<TransportActivity> GetTransportActivities(DateTime desde, DateTime hasta, List<Int32> ids, int km)
        {
            if (!ids.Any()) return new List<TransportActivity>();

            var results = DAOFactory.DatamartDAO.GetTransportActivities(desde, hasta, ids);

            return (from result in results where result.Recorrido >= km select GetResultWithTickets(result, result.IdVehiculo, desde, hasta)).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the amount of tickets assigned to the mobile in the current period.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vehicleId"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        private TransportActivity GetResultWithTickets(TransportActivity result, int vehicleId, DateTime desde, DateTime hasta)
        {
            result.CantidadViajes = DAOFactory.TicketDAO.FindTotalTickets(vehicleId, desde, hasta);

            return result;
        }

        #endregion
    }
}
