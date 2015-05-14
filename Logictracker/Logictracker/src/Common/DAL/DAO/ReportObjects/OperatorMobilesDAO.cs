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
    public class OperatorMobilesDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public OperatorMobilesDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets information about all the mobiles that has been used by the givenn operator within the defined interval.
        /// </summary>
        /// <param name="operador"></param>
        /// <param name="iniDate"></param>
        /// <param name="finDate"></param>
        /// <returns></returns>
        public IEnumerable<OperatorMobiles> GetOperatorMobiles(Int32 operador, DateTime iniDate, DateTime finDate)
        {
            return DAOFactory.DatamartDAO.GetForEmployee(operador, iniDate, finDate)
                .GroupBy(datamart => new { datamart.Vehicle.Interno, datamart.Vehicle.Patente })
                .Select(datamart => new OperatorMobiles
                {
                    Infracciones = datamart.Sum(data => data.Infractions),
                    HsDriving = datamart.Sum(data => data.MovementHours) + datamart.Sum(data => data.StoppedHours) + datamart.Sum(data => data.NoReportHours),
                    Kilometros = datamart.Sum(data => data.Kilometers),
                    Interno = datamart.Key.Interno,
                    Patente = datamart.Key.Patente,
                })
                .ToList();
        }

        #endregion
    }
}
