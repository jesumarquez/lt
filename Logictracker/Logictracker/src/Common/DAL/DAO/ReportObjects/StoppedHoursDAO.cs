using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class StoppedHoursDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public StoppedHoursDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public List<StoppedHours> GetVehicleStoppedTimeGroupedByInterval(int coche, DateTime iniDate, DateTime finDate, bool enableUndefined)
        {
            var results = new List<StoppedHours>();

            var datamartConTurnos = DAOFactory.DatamartDAO.GetBetweenDatesWithShift(coche, iniDate, finDate, enableUndefined);

            var datamartSinTurnos = DAOFactory.DatamartDAO.GetBetweenDatesWithoutShift(coche, iniDate, finDate, enableUndefined);

            var actualDate = iniDate;

            while (actualDate <= finDate)
            {
                var shiftData = datamartConTurnos.Where(d => d.Begin >= actualDate && d.Begin < actualDate.AddMinutes(60));
                var outOfShiftData = datamartSinTurnos.Where(d => d.Begin >= actualDate && d.Begin < actualDate.AddMinutes(60));

                results.Add(new StoppedHours
                        {
                            Date = actualDate.ToDisplayDateTime(),
                            HoursInShift = shiftData.Sum(d => d.StoppedHours) + shiftData.Sum(d => d.NoReportHours),
                            HoursOutOfShift = outOfShiftData.Sum(d => d.StoppedHours) + outOfShiftData.Sum(d => d.NoReportHours)
                        });

                actualDate = actualDate.AddMinutes(60);
            }

            return results;
        }
    }
}
