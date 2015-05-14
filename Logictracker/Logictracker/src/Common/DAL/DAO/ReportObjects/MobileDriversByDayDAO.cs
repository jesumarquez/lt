using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobileDriversByDayDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileDriversByDayDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets information about all the drivers that used the givenn mobile within the defined interval by day.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="iniDate"></param>
        /// <param name="finDate"></param>
        /// <returns></returns>
        public List<MobileDriversByDay> GetMobileDriversByDay(Int32 mobile, DateTime iniDate, DateTime finDate)
        {
            var datas = DAOFactory.DatamartDAO.GetBetweenDates(mobile, iniDate, finDate);

            foreach (var datamart in datas) datamart.Begin = datamart.Begin.ToDisplayDateTime();

            var results = datas.Select(data => data.Begin.Date).Distinct().ToDictionary(day => day, day => (from data in datas where data.Begin.Date.Equals(day) select data));

            var query = new List<MobileDriversByDay>();

            foreach (var day in results.Keys)
                query.AddRange(results[day]
                .Where(datamart => datamart.Employee != null)
                .GroupBy(datamart => new
                                         {
                                             datamart.Employee.Legajo,
                                             datamart.Employee.Entidad.Descripcion,
                                             Numero = datamart.Employee.Tarjeta != null ? datamart.Employee.Tarjeta.Numero : "-"
                                         })
                .Select(datamart => new MobileDriversByDay
                {
                    IdMovil = mobile,
                    Infracciones = datamart.Sum(data => data.Infractions),
                    DrivingTime = TimeSpan.FromHours(datamart.Sum(data => data.MovementHours)),
                    Kilometros = datamart.Sum(data => data.Kilometers),
                    Legajo = datamart.Key.Legajo,
                    Nombre = datamart.Key.Descripcion,
                    Tarjeta = datamart.Key.Numero,
                    Fecha = day.Date
                   }).ToList()
                .Union(results[day]
                    .Where(datamart => datamart.Employee == null)
                    .GroupBy(datamart => datamart.Vehicle.Id)
                    .Select(datamart => new MobileDriversByDay
                    {
                        IdMovil = mobile,
                        Infracciones = datamart.Sum(data => data.Infractions),
                        DrivingTime = TimeSpan.FromHours(datamart.Sum(data => data.MovementHours)),
                        Kilometros = datamart.Sum(data => data.Kilometers),
                        Legajo = "-",
                        Nombre = "Sin Chofer Identificado",
                        Tarjeta = "-",
                        Fecha = day.Date
                    })).ToList());

            return query.Where(result => result.Kilometros > 0).OrderBy(result => result.Fecha).ThenBy(result => result.Nombre).ToList();
        }

        #endregion
    }
}
