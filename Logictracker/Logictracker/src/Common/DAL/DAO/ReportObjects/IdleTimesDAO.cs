using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class IdleTimesDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public IdleTimesDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        public IEnumerable<IdleTimes> GetAllMovilesStoppedInPlanta(int planta, DateTime start, DateTime end, bool showUndefined)
        {
            var results = new List<IdleTimes>();

            var Base = DAOFactory.LineaDAO.FindById(planta);

            if (Base.ReferenciaGeografica == null) return results;

            var plantaGeoreff = Base.ReferenciaGeografica.Id;
            var actualDate = start;

            var datam = DAOFactory.DatamartDAO.GetVehiclesStoppedInBase(start, end, plantaGeoreff, showUndefined).Select( d=> new { d.Begin, IdVehiculo = d.Vehicle.Id});

            while (actualDate <= end)
            {
                results.Add(new IdleTimes
                        {
                            Date = actualDate.ToDisplayDateTime(),
                            TotalVehicles = datam.Where(d => d.Begin >= actualDate && d.Begin <= actualDate.AddMinutes(60)).Select(d=> d.IdVehiculo).ToList().Distinct().Count()
                        });

                actualDate = actualDate.AddMinutes(60);
            }

            return results;
        }

        #endregion
    }
}
