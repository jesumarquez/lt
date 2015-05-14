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
    public class ConsumoCaudalMotorDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public ConsumoCaudalMotorDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

      #region Public Methods

        public List<ConsumoCaudalMotor> FindConsumoCaudalMotores(List<int> motores, DateTime desde, DateTime hasta, int intervalo)
        {
            if (!motores.Any()) return new List<ConsumoCaudalMotor>();

            var results = new List<ConsumoCaudalMotor>();

            var actualDate = desde.AddMinutes(-1*intervalo);

            while (actualDate <= hasta)
            {
                var movimiento = DAOFactory.MovimientoDAO.FindConsumosWhitinInterval(motores, actualDate, hasta, intervalo);

                results.Add(new ConsumoCaudalMotor
                                    {
                                        Fecha = actualDate.AddMinutes(intervalo) > DateTime.Now ? DateTime.Now : actualDate.AddMinutes(intervalo),
                                        Caudal = movimiento.Average(mov => mov.Caudal),
                                        Consumo = movimiento.Sum(mov => mov.Volumen)
                                    });


                actualDate = actualDate.AddMinutes(intervalo);

                if (actualDate >= DateTime.Now) break;
            }

            return results.Any(res => res.Consumo > 0 || res.Caudal > 0) ? results : null;
        }

        #endregion
    }
}
