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
    /// Data access class for the mobile times chart.
    /// </summary>
    public class MonthlyTimesDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MonthlyTimesDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Get mobile times report.
        /// </summary>
        /// <param name="mobileId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<MobileTime> GetMobileTimes(int mobileId, DateTime startDate, DateTime endDate)
        {
            return DAOFactory.DatamartDAO.GetBetweenDates(mobileId, startDate, endDate)
                .GroupBy(datamart => datamart.Begin.ToDisplayDateTime().Date)
                .Select(data => new MobileTime(data.Key, data.Sum(datamart => datamart.MovementHours)))
                .Where(data => data.ElapsedTime >= 0.01)
                .OrderBy(data => data.Fecha)
                .ToList();
        }

        #endregion
    }
}
