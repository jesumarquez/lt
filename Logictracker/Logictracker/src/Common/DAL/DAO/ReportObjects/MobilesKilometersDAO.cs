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
    /// Class that represents distance traveled by each vehicle during the specified time span.
    /// </summary>
    public class MobilesKilometersDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobilesKilometersDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Data Access

        /// <summary>
        /// Gets a list of kilometers traveled by each of the givenn mobiles during the specified timespan.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="ids"></param>
        /// <param name="enCiclo"> </param>
        /// <returns></returns>
        public List<MobilesKilometers> GetMobilesKilometers(DateTime desde, DateTime hasta, List<Int32> ids, bool enCiclo)
        {
            return (from kilometer in DAOFactory.DatamartDAO.GetMobilesKilometers(desde, hasta, ids, enCiclo) where kilometer.Kilometers > 0.0 select kilometer).ToList();
        }

        #endregion
    }
}
