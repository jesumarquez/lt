#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class ConsistenciaStockGraphDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public ConsistenciaStockGraphDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        /// <summary>
        /// Stock Consistence between dates grouped by Day.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public IList GetByTanqueAndDate(int tank, DateTime startDate, DateTime endDate, int interval)
        {
            var intervalValue = new TimeSpan(0, interval, 0);
            var actualDate = startDate;
            var results = new List<ConsistenciaStockGraph>();

            while(actualDate <= endDate && actualDate <= DateTime.Now.AddMinutes(interval))
            {
                var stocks = DAOFactory.VolumenHistoricoDAO.GetVolumes(tank, actualDate, endDate);

                var realStocks = (from vol in stocks where  !vol.EsTeorico orderby vol.Fecha descending, vol.Id descending select vol.Volumen).FirstOrDefault();

                var teoricStocks = (from vol in stocks where vol.EsTeorico orderby vol.Fecha descending, vol.Id descending select vol.Volumen).FirstOrDefault();

                results.Add(new ConsistenciaStockGraph
                                {
                                    StockReal = realStocks,
                                    StockTeorico = teoricStocks,
                                    Fecha = actualDate <= endDate && actualDate <= DateTime.Now ? actualDate : endDate <= DateTime.Now ? endDate : DateTime.Now 
                                });

                actualDate = actualDate.Add(intervalValue);
            }

            return results;
        }
    }
}
