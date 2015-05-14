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
    /// PuntajeExcesoTiempo data access class.
    /// </summary>
    public class PuntajeExcesoTiempoDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public PuntajeExcesoTiempoDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Private Properties

        /// <summary>
        /// Auxiliar list for aviding database.
        /// </summary>
        private List<PuntajeExcesoTiempo> _puntajes;
        private IEnumerable<PuntajeExcesoTiempo> Puntajes { get { return _puntajes ?? (_puntajes = DAOFactory.PuntajeExcesoTiempoDAO.GetPoints()); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the list of points associated to the duration of the infraction.
        /// </summary>
        /// <returns></returns>
        public int GetDurationPoints(Infraccion infraction)
        {
            switch (infraction.CodigoInfraccion)
            {
                case Infraccion.Codigos.ExcesoVelocidad:
                case Infraccion.Codigos.ExcesoRpm:
                    {
                        var duration = infraction.Alcanzado - infraction.Permitido;
                        return Puntajes.Where(time => time.Segundos <= duration).OrderByDescending(time => time.Segundos).Select(time => time.Puntaje).FirstOrDefault();
                    }
                default: return 0;
            }
        }

        #endregion
    }
}
