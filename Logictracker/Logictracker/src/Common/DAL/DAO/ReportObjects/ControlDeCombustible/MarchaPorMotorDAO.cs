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
    public class MarchaPorMotorDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MarchaPorMotorDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        public IEnumerable<MarchaMotor> FindAllHsDeMarchaForMotor(int motor, DateTime startDate, DateTime endDate)
        {
            var list = new List<MarchaMotor>();
            var actualDate = startDate;

            while(actualDate <= endDate && actualDate <= DateTime.Now)
            {
                var date = actualDate;

                var movmentsForPeriod = DAOFactory.MovimientoDAO.FindConsumosByMotorAndDate(motor, date, date.AddDays(1)); ;

                var firstHorometro = movmentsForPeriod.Min(mov => mov.HsEnMarcha);
                var lastHorometro = movmentsForPeriod.Max(mov => mov.HsEnMarcha);

                list.Add(new MarchaMotor { Fecha = actualDate, HsEnMarcha = (int) (lastHorometro - firstHorometro) });

                actualDate = actualDate.AddDays(1);
            }

            return (from o in list where o.HsEnMarcha > 0 select o).Any() ? list.OrderBy(m => m.Fecha).ToList() : new List<MarchaMotor>();
        }

        #endregion
    }
}
