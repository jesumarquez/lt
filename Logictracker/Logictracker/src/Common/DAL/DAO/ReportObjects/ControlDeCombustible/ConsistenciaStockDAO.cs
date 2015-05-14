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
    public class ConsistenciaStockDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public ConsistenciaStockDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        /// <summary>
        /// Stock Consistence between dates grouped by Day.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IList GetByTanqueAndDateGroupedByDay(int tank, DateTime startDate, DateTime endDate)
        {
            var list = new List<ConsistenciaStock>();

            var results = DAOFactory.MovimientoDAO.FindByTanque(tank, startDate, endDate);
   
            var actualDate = startDate;
            
            while(actualDate <= endDate && actualDate <= DateTime.Now)
            {
                var totEgresos = (from m in results
                               where m.TipoMovimiento.Codigo.Equals("D")
                               && m.Fecha >= actualDate && m.Fecha < actualDate.AddDays(1) 
                               select m.Volumen).Sum();

                var totIngresos = (from m in results
                               where (m.TipoMovimiento.Codigo.Equals("R") || m.TipoMovimiento.Codigo.Equals("A"))
                               && m.Fecha >= actualDate && m.Fecha < actualDate.AddDays(1) 
                               select m.Volumen).Sum();

                var volumes = DAOFactory.VolumenHistoricoDAO.FindAll();

                var realStocks = (from m in volumes
                               where m.Tanque != null && m.Tanque.Id == tank && !m.EsTeorico
                                && m.Fecha >= actualDate && m.Fecha < actualDate.AddDays(1)
                                  orderby m.Fecha descending, m.Id descending
                               select m.Volumen);

                var stockFinal = (from m in volumes
                               where m.Tanque != null && m.Tanque.Id == tank && m.EsTeorico
                               && m.Fecha < actualDate.AddDays(1)
                                  orderby m.Fecha descending, m.Id descending
                               select m.Volumen).FirstOrDefault();

                var stockInicial = (from m in volumes
                               where m.Tanque != null && m.Tanque.Id == tank && m.EsTeorico
                                && m.Fecha < actualDate
                                    orderby m.Fecha descending, m.Id descending
                               select m.Volumen).FirstOrDefault();

                var stockSonda = realStocks.Any() ? realStocks.First() : 0;

                list.Add(new ConsistenciaStock
                             {
                                 Fecha = actualDate <= endDate && actualDate <= DateTime.Now ? actualDate 
                                                : endDate <= DateTime.Now ? endDate : DateTime.Now,
                                 Egresos = totEgresos,
                                 Ingresos = totIngresos,
                                 StockInicial = stockInicial,
                                 StockFinal = stockFinal,
                                 StockSonda = stockSonda,
                                 Diferencia = stockFinal - stockSonda
                             });
                actualDate = actualDate.AddDays(1);
            }
            return list.Any(m => !m.Egresos.Equals(0) && !m.Ingresos.Equals(0)) ? list : new List<ConsistenciaStock>();
        }
    }
}
