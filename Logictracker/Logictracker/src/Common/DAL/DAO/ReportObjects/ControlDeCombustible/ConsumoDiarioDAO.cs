#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class ConsumoDiarioDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public ConsumoDiarioDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        public IEnumerable<ConsumoDiario> FindConsumosByDate(int tanque, DateTime desde, DateTime hasta)
        {
            var results = new List<ConsumoDiario>();

            var actualDate = desde;

            while (actualDate <= hasta && actualDate <= DateTime.Now)
            {
                var consumos = DAOFactory.MovimientoDAO.FindTotalMovement(tanque, actualDate, actualDate.AddDays(1));

                var ingresos = DAOFactory.MovimientoDAO.FindTotalIncomeByTank(tanque, actualDate, actualDate.AddDays(1));

                var ingresosConciliacion = DAOFactory.MovimientoDAO.FindTotalConciliationIncome(tanque, actualDate, actualDate.AddDays(1));

                var egresosConciliacion = DAOFactory.MovimientoDAO.FindTotalConciliationOutcome(tanque, actualDate, actualDate.AddDays(1));

                results.Add(new ConsumoDiario
                                {
                                    Fecha = actualDate,
                                    VolumenConsumido = consumos,
                                    EgresosPorConciliacion = egresosConciliacion,
                                    Ingresos = ingresos,
                                    IngresosPorConciliacion = ingresosConciliacion
                                });

                actualDate = actualDate.AddDays(1);
            }

            return results.Any(result => result.VolumenConsumido > 0) ? results : new List<ConsumoDiario>();
        }


        /// <summary>
        /// Devuelve las diferencias de niveles en el periodo seleccionado particionado en intervalos de longitud "intervalo"
        /// </summary>
        /// <param name="tanque"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="intervalo"></param>
        /// <returns></returns>
        public List<ConsumoDiario> FindVariacionDeNivelTanque(int tanque, DateTime desde, DateTime hasta, int intervalo)
        {
            var list = new List<ConsumoDiario>();

            if (tanque <= 0) return list;

            var interval = TimeSpan.FromMinutes(intervalo);

            var lastVolume = DAOFactory.VolumenHistoricoDAO.FindLastRealVolume(tanque, desde, hasta).Volumen;
            
            var actualDate = desde.Subtract(interval);

            while (actualDate <= hasta && actualDate <= DateTime.Now)
            {
                var date = actualDate;

                var actualVolume = DAOFactory.VolumenHistoricoDAO.FindActualVolume(tanque, date, hasta, interval).Volumen;

                actualDate = actualDate.Add(interval);

                list.Add(new ConsumoDiario
                {
                    Fecha = (actualDate > hasta && actualDate >= DateTime.Now) ? hasta <= DateTime.Now ? hasta : DateTime.Now : actualDate,
                    VolumenConsumido = actualVolume - lastVolume, 
                });

                lastVolume = actualVolume;
            }

            return list.OrderBy(m => m.Fecha).ToList();
        }

        /// <summary>
        /// Devuelve los consumos e ingresos en el periodo seleccionado particionado en intervalos de longitud "intervalo"
        /// </summary>
        /// <param name="tanque"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="intervalo"></param>
        /// <returns></returns>
        public List<ConsumoDiario> FindVariacionDeNivelMotores(int tanque, DateTime desde, DateTime hasta, int intervalo)
        {
            var list = new List<ConsumoDiario>();
            if (tanque <= 0) return list;

            var interval = TimeSpan.FromMinutes(intervalo);
            var actualDate = desde.Subtract(interval);
         
            while (actualDate <= hasta && actualDate <= DateTime.Now)
            {
                var date = actualDate;

                var consumos = DAOFactory.MovimientoDAO.FindTotalMovementWithinInterval(tanque, date, hasta, interval);

                var ingresos = DAOFactory.MovimientoDAO.FindTotalIncomeWithinInterval(tanque, date, hasta, interval);

                list.Add(new ConsumoDiario
                {
                    Fecha = (actualDate > hasta && actualDate >= DateTime.Now) ? hasta <= DateTime.Now ? hasta : DateTime.Now : actualDate,
                    VolumenConsumido = consumos - ingresos,
                });

                actualDate = actualDate.Add(interval);
            }

            return list.OrderBy(m => m.Fecha).ToList();
        }

        #endregion
    }
}
