#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.RankingDeOperadores
{
    /// <summary>
    /// Speeding ponderation data access class.
    /// </summary>
    public class PuntajeExcesoVelocidadDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public PuntajeExcesoVelocidadDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Private Properties

        /// <summary>
        /// Auxiliar list for aviding database.
        /// </summary>
        private List<PuntajeExcesoVelocidad> _puntajes;
        private IEnumerable<PuntajeExcesoVelocidad> Puntajes { get { return _puntajes ?? (_puntajes = DAOFactory.PuntajeExcesoVelocidadDAO.GetPoints()); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the list of points associated to the speed ponderation of the infraction.
        /// </summary>
        /// <param name="infraction"></param>
        /// <returns></returns>
        public int GetSpeedingPoints(Infraccion infraction)
        {
            switch (infraction.CodigoInfraccion)
            {
                case Infraccion.Codigos.ExcesoVelocidad:
                case Infraccion.Codigos.ExcesoRpm:
                    {
                        var porcentaje = ((infraction.Alcanzado - infraction.Permitido) * 100) / infraction.Permitido;
                        return Puntajes.Where(speed => speed.Porcentaje <= porcentaje).OrderByDescending(speed => speed.Porcentaje).Select(speed => speed.Puntaje).FirstOrDefault();
                    }
                default: return 0;
            }
        }

        #endregion
    }
}
