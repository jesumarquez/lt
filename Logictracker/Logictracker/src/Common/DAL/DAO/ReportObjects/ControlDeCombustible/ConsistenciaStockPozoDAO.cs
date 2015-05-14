#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class ConsistenciaStockPozoDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public ConsistenciaStockPozoDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public IList FindConsistenciaBetweenDates(int tanque, DateTime desde, DateTime hasta)
        {
            var results = new List<ConsistenciaStockPozo>();

            var cauDeIngreso = DAOFactory.CaudalimetroDAO.FindCaudalimetroDeEntradaByTanque(tanque);

            var caudalimetroDeIngreso = cauDeIngreso != null ? cauDeIngreso.Id : 0;

            var date = desde;
           
            while (date <= hasta && date <= DateTime.Now)
            {
                var dateFin = date.AddDays(1);

                /*Gets the first Volume of the Tank Medidor for the period*/
                var iniStock = DAOFactory.VolumenHistoricoDAO.FindInitialTheoricVolume(tanque, date);

                var initialStock = iniStock != null ? iniStock.Volumen : 0;

                /*consumos del día para todos los motores*/
                var totalMovement = DAOFactory.MovimientoDAO.FindTotalMovement(tanque, date, dateFin);

		        /*total de Ingresos*/
                var totalIngreso = DAOFactory.MovimientoDAO.FindTotalIncome(caudalimetroDeIngreso, date, dateFin);

                /*Gets the last Volume of the Tank Medidor for the period*/
                var lastTankSonda = DAOFactory.VolumenHistoricoDAO.FindLastRealVolume(tanque, date, dateFin);

                var lastTankSondaVolume = lastTankSonda != null ? lastTankSonda.Volumen : 0;

                var conciliaciones = DAOFactory.MovimientoDAO.FindConciliaciones(tanque, date, dateFin);

                var egresosConciliacion = (from Movimiento m in conciliaciones where m.TipoMovimiento.Codigo.Equals("E") select m.Volumen).Sum();

                var ingresosConciliacion = (from Movimiento m in conciliaciones where m.TipoMovimiento.Codigo.Equals("C") select m.Volumen).Sum();

        		/*Gets the first Volume of the Tank Medidor for the period*/

                var finalTeoricStock = DAOFactory.VolumenHistoricoDAO.FindInitialTheoricVolume(tanque, dateFin);

                var finalTeoricStockVolume = finalTeoricStock != null ? finalTeoricStock.Volumen : 0;

                var diferencia = lastTankSondaVolume - finalTeoricStockVolume;

                results.Add( new ConsistenciaStockPozo
                                    {
                                        Fecha = date,
                                        StockInicial = initialStock,
                                        Egresos = totalMovement,
                                        Ingresos = totalIngreso,
                                        EgresosPorConciliacion = egresosConciliacion,
                                        IngresosPorConciliacion = ingresosConciliacion,
                                        StockFinal = finalTeoricStockVolume,
                                        StockSonda = lastTankSondaVolume,
                                        DiferenciaDeStock = diferencia
                                    });

                date = date.AddDays(1);
            }

            return results.Any(res=> res.Ingresos >0 || res.Egresos >0) ? results : null;
        }
    }
}
