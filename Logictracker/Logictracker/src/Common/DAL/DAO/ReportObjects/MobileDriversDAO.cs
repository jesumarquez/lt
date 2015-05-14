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
    public class MobileDriversDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileDriversDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets information about all the drivers that used the givenn mobile within the defined interval.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="iniDate"></param>
        /// <param name="finDate"></param>
        /// <returns></returns>
        public IEnumerable<MobileDrivers> GetMobileDrivers(int mobile, DateTime iniDate, DateTime finDate)
        {
            var lista = new List<MobileDrivers>();

            var withoutEmployee = DAOFactory.DatamartDAO.GetBetweenDatesWithoutEmployee(mobile, iniDate, finDate);

            if (withoutEmployee.Any())
                lista.AddRange(withoutEmployee
                    .GroupBy(datamart => new {datamart.Vehicle.Id})
                    .Select(datamart => new MobileDrivers
                                        {
                                            Infracciones = datamart.Sum(data => data.Infractions),
                                            DrivingT = datamart.Sum(data => data.MovementHours) + datamart.Sum(data => data.StoppedHours) + datamart.Sum(data => data.NoReportHours),
                                            Kilometros = datamart.Sum(data => data.Kilometers),
                                            Legajo = "-",
                                            Nombre = "Sin Chofer Identificado",
                                            Tarjeta = "-"
                                        })
                    .ToList());

            var withEmployee = DAOFactory.DatamartDAO.GetBetweenDatesWithEmployee(mobile, iniDate, finDate);

            if (withEmployee.Any())
                lista.AddRange(withEmployee
                                   .GroupBy(datamart => new
                                                            {
                                                                datamart.Employee.Legajo,
                                                                datamart.Employee.Entidad.Descripcion,
                                                                Numero = datamart.Employee.Tarjeta != null ? datamart.Employee.Tarjeta.Numero : "-"
                                                            })
                                   .Select(datamart => new MobileDrivers
                                                           {
                                                               Infracciones = datamart.Sum(data => data.Infractions),
                                                               DrivingT = datamart.Sum(data => data.MovementHours) + datamart.Sum(data => data.StoppedHours) + 
                                                                    datamart.Sum(data => data.NoReportHours),
                                                               Kilometros = datamart.Sum(data => data.Kilometers),
                                                               Legajo = datamart.Key.Legajo,
                                                               Nombre = datamart.Key.Descripcion,
                                                               Tarjeta = datamart.Key.Numero
                                                           })
                                   .ToList());

            return lista;
        }

        #endregion
    }
}
