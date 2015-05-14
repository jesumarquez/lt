using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ReportObjects.RankingDeOperadores;

namespace Logictracker.DAL.DAO.ReportObjects.RankingDeOperadores
{
    public class OperatorRankingDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public OperatorRankingDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Private Properties

        private ReportFactory _reportFactory;
        private ReportFactory ReportFactory { get { return _reportFactory ?? (_reportFactory = new ReportFactory(DAOFactory)); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the operators ranking data filtered by the givenn parameter values.
        /// </summary>
        /// <param name="tipos"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="distritos"></param>
        /// <param name="bases"></param>
        /// <param name="transportistas"></param>
        /// <returns></returns>
        public List<OperatorRanking> GetRanking(List<Int32> distritos, List<Int32> bases, List<Int32> transportistas, List<Int32> tipos, List<Int32> centros, List<Int32> departamentos, DateTime from, DateTime to)
        {
            var ranking = new List<OperatorRanking>();

            //var operatorsActivites = GetDriversActivity(distritos, bases, transportistas, tipos, from, to);

            var allEmployees = DAOFactory.EmpleadoDAO.GetList(distritos, bases, tipos, transportistas, centros, departamentos);

            var operatorsActivites = GetDriversActivity(from, to, allEmployees.Select(e => e.Id).ToList());

            var employees = operatorsActivites.Select(activity => activity.IdOperador).ToList();

            //var operatorsMessages = DAOFactory.LogMensajeDAO.GetInfractions(employees, from, to);
            //var conInfraccion = operatorsMessages.Select(m => m.Chofer).Distinct();
            var infracciones = DAOFactory.InfraccionDAO.GetByEmpleados(employees, from, to);

            foreach (var chofer in allEmployees)
            {
                var driver = chofer.Id;
                var choferRanking = operatorsActivites.FirstOrDefault(a => a.IdOperador == driver);
                var hasInfo = choferRanking != null;

                if (!hasInfo)
                    choferRanking = new OperatorRanking
                                        {
                                            IdOperador = driver,
                                            Legajo = chofer.Legajo
                                        };
                choferRanking.Operador = chofer.Entidad.Descripcion;

                var messages = hasInfo
                                   ? infracciones.Where(message => message.Empleado.Id.Equals(driver)).ToList()
                                   : new List<Infraccion>(0);

                foreach (var infraction in messages)
                {
                    var gravedad = GetGravedadInfraccion(infraction);

                    if (gravedad.Equals(0)) continue;

                    if (gravedad.Equals(1)) choferRanking.InfraccionesLeves++;
                    else if (gravedad.Equals(2)) choferRanking.InfraccionesMedias++;
                    else choferRanking.InfraccionesGraves++;

                    choferRanking.Puntaje += GetPonderacionInfraccion(infraction);
                }

                ranking.Add(choferRanking);
            }

            return ranking;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets datamart records for the specified driver and time span.
        /// </summary>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="choferes"></param>
        /// <returns></returns>
        private IEnumerable<OperatorRanking> GetDriversActivity(DateTime desde, DateTime hasta, List<int> choferes)
        {
            return DAOFactory.DatamartDAO.GetOperatorsRanking(desde, hasta, choferes);
        }

        /// <summary>
        /// Gets the ponderation associated to the current infraction.
        /// </summary>
        /// <param name="infraction"></param>
        /// <returns></returns>
        private double GetPonderacionInfraccion(Infraccion infraction)
        {
            return (ReportFactory.PuntajeExcesoVelocidadDAO.GetSpeedingPoints(infraction)*ReportFactory.PuntajeExcesoTiempoDAO.GetDurationPoints(infraction))/1000.0;
        }

        /// <summary>
        /// Gets the severity of the current infraction.
        /// </summary>
        /// <param name="infraction"></param>
        /// <returns></returns>
        private static int GetGravedadInfraccion(Infraccion infraction)
        {
            switch(infraction.CodigoInfraccion)
            {
                case Infraccion.Codigos.ExcesoVelocidad:
                case Infraccion.Codigos.ExcesoRpm:
                    {
                        var difference = infraction.Alcanzado - infraction.Permitido;
                        return difference <= 9 ? 1 : difference > 9 && difference <= 17 ? 2 : 3;
                    }
                default: return 0;
            }
        }

        #endregion
    }
}