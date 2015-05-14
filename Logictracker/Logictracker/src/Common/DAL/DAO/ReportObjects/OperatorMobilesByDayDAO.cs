using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class OperatorMobilesByDayDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public OperatorMobilesByDayDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets information about all the drivers that used the givenn mobile within the defined interval by day.
        /// </summary>
        /// <param name="empleado"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public List<OperatorMobilesByDay> GetOperatorMobilesByDay(Int32 empleado, DateTime desde, DateTime hasta)
        {
            var datas = DAOFactory.DatamartDAO.GetForEmployee(empleado, desde, hasta);

            foreach (var datamart in datas) datamart.Begin = datamart.Begin.ToDisplayDateTime();

            var query = (datas
                .GroupBy(data => new { data.Vehicle, data.Begin.Date })
                .Select(data => new OperatorMobilesByDay
                                    {
                    HorasActivo = TimeSpan.FromHours(data.Sum(datamart => datamart.MovementHours)),
                    Infracciones = data.Sum(datamart => datamart.Infractions),
                    Recorrido = data.Sum(datamart => datamart.Kilometers),
                    VelocidadMaxima = data.Max(datamart => datamart.MaxSpeed),
                    VelocidadPromedio = (int)data.Average(datamart => datamart.AverageSpeed),
                    Fecha = data.Key.Date,
                    TipoVehiculo = data.Key.Vehicle.TipoCoche.Descripcion,
                    Movil = data.Key.Vehicle.Interno,
                    Marca = data.Key.Vehicle.Marca.Descripcion,
                    Responsable = data.Key.Vehicle.Chofer != null ? data.Key.Vehicle.Chofer.Entidad.Descripcion : "Sin Responsable",
                    IdMovil = data.Key.Vehicle.Id
                }).ToList());

            return query.Where(result => result.Recorrido > 0).OrderBy(result => result.Fecha).ThenBy(result => result.Movil).ToList();
        }

        #endregion
    }
}
