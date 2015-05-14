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
    /// Operator activity data access class.
    /// </summary>
    public class OperatorActivityDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public OperatorActivityDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets operator activity information whitin the givenn range.
        /// </summary>
        /// <param name="initialTime"></param>
        /// <param name="endTime"></param>
        /// <param name="distrito"></param>
        /// <param name="planta"></param>
        /// <param name="operators"></param>
        /// <param name="km"></param>
        /// <returns></returns>
        public IEnumerable<OperatorActivity> GetOperatorActivitys(DateTime initialTime, DateTime endTime, Int32 distrito, Int32 planta, List<Int32> operators, Int32 km)
        {
            return DAOFactory.DatamartDAO.GetOperatorActivities(initialTime, endTime, distrito, planta, operators, km).ToList();
        }

        #endregion
    }
}
