#region Usings

using System;
using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobileActivityDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileActivityDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets mobile activitties report objects.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="ids"></param>
        /// <param name="km"></param>
        /// <returns></returns>
        public List<MobileActivity> GetMobileActivitys(DateTime desde, DateTime hasta, Int32 empresa, Int32 linea, List<Int32> ids, Int32 km)
        {
            return DAOFactory.DatamartDAO.GetMobileActivities(desde, hasta, empresa, linea, ids, km);
        }

        #endregion
    }
}
