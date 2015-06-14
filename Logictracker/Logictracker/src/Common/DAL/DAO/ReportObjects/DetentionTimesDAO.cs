using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class DetentionTimesDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public DetentionTimesDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets detention's times
        /// </summary>
        /// <param name="vehiculo"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public List<DetentionTimes> GetDetentionTimes(int vehiculo, DateTime desde, DateTime hasta)
        {
            var datas = DAOFactory.DatamartDAO.GetBetweenDates(vehiculo, desde, hasta);
            
            if (!datas.Any()) return new List<DetentionTimes>();

            var actualDate = desde;

            var results = new List<DetentionTimes>();

            while (actualDate < hasta)
            {
                results.Add(new DetentionTimes
                                 {
                                     Fecha = actualDate.ToDisplayDateTime(),
                                     HsTurnoOn = datas.Where(
                                         d =>
                                         d.Begin >= actualDate && d.Begin < actualDate.AddDays(1) && d.Shift != null &&
                                         d.EngineStatus.Equals("Encendido"))
                                         .GroupBy(d => d.Vehicle.Id).Select(d => d.Sum(data => data.StoppedHours)).
                                         FirstOrDefault(),
                                     HsTurnoOff = datas.Where(
                                         d =>
                                         d.Begin >= actualDate && d.Begin < actualDate.AddDays(1) && d.Shift != null &&
                                         d.EngineStatus.Equals("Apagado"))
                                         .GroupBy(d => d.Vehicle.Id).Select(d => d.Sum(data => data.StoppedHours)).
                                         FirstOrDefault(),
                                     HsOn = datas.Where(
                                         d =>
                                         d.Begin >= actualDate && d.Begin < actualDate.AddDays(1) && d.Shift == null &&
                                         d.EngineStatus.Equals("Encendido"))
                                         .GroupBy(d => d.Vehicle.Id).Select(d => d.Sum(data => data.StoppedHours)).
                                         FirstOrDefault(),
                                     HsOff = datas.Where(
                                         d =>
                                         d.Begin >= actualDate && d.Begin < actualDate.AddDays(1) && d.Shift == null &&
                                         d.EngineStatus.Equals("Apagado"))
                                         .GroupBy(d => d.Vehicle.Id).Select(d => d.Sum(data => data.StoppedHours)).
                                         FirstOrDefault()
                                 });

                actualDate = actualDate.AddDays(1);
            }

            return results;
        }

        #endregion
    }
}
