using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Data access class for mobile kilometers chart.
    /// </summary>
    public class MonthlyKilometersDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MonthlyKilometersDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        public List<MobileKilometer> GetMobileKilometers(Int32 mobileId, DateTime startDate, DateTime endDate)
        {
            return GetMobileKilometers(mobileId, startDate, endDate, false);
        }

        /// <summary>
        /// Returns a list of mobile kilometers.
        /// </summary>
        /// <param name="mobileId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="soloEnRuta"></param>
        /// <returns></returns>
        public List<MobileKilometer> GetMobileKilometers(Int32 mobileId, DateTime startDate, DateTime endDate, bool soloEnRuta)
        {
            return DAOFactory.DatamartDAO.GetBetweenDates(mobileId, startDate, endDate)
                .Where(datamart => !soloEnRuta || datamart.IdCiclo > 0)
                .GroupBy(datamart => datamart.Begin.ToDisplayDateTime().Date)
                .Select(data => new MobileKilometer(data.Key, data.Sum(datamart => datamart.Kilometers)))
                .Where(data => data.Kilometers > 0.01)
                .OrderBy(data => data.Fecha)
                .ToList();
        }

        #endregion
    }
}
