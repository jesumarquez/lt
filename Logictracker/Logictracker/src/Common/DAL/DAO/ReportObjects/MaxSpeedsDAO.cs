using System;
using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MaxSpeedsDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MaxSpeedsDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns mobile daily max speeds for the givenn interval.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public List<MaxSpeeds> GetMobileMaxSpeeds(int mobile, DateTime desde, DateTime hasta)
        {
            var results = new List<MaxSpeeds>();

            var start = desde;

            while (start <= hasta)
            {
                var data = DAOFactory.DatamartDAO.GetByVehicleMaxSpeed(mobile, start);

                if (data != null && data.MaxSpeed > 0)
                    results.Add(new MaxSpeeds
                                    {
                                        Dia = data.Begin.ToDisplayDateTime(),
                                        Velocidad = data.MaxSpeed,
                                        CometidoPor = data.Employee == null ? "Sin Identificar" : data.Employee.Legajo
                                    });

                start = start.AddDays(1);
            }

            return results;
        }

        /// <summary>
        /// Returns operator daily max speeds for the givenn interval.
        /// </summary>
        /// <param name="operador"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public List<MaxSpeeds> GetOperatorMaxSpeeds(int operador, DateTime desde, DateTime hasta)
        {
            var results = new List<MaxSpeeds>();

            var start = desde;

            while (start <= hasta)
            {
                var data = DAOFactory.DatamartDAO.GetByEmployeeMaxSpeed(operador, start);

                if (data != null && data.MaxSpeed > 0)
                    results.Add(new MaxSpeeds
                                    {
                                        Dia = data.Begin.ToDisplayDateTime(),
                                        Velocidad = data.MaxSpeed,
                                        CometidoPor = data.Vehicle == null ? "Sin Identificar" : data.Vehicle.Interno
                                    });

                start = start.AddDays(1);
            }

            return results;
        }

        #endregion
    }
}
