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
    public class MobilesTimeDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobilesTimeDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the acumulated movement time for the givenn mobiles within the indicated time span.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<MobilesTime> GetMobilesTime(DateTime desde, DateTime hasta, List<Int32> ids)
        {
            return (from time in DAOFactory.DatamartDAO.GetMobilesTimes(desde, hasta, ids) where time.ElapsedTime > 0.0 select time).ToList();
        }

        #endregion
    }
}