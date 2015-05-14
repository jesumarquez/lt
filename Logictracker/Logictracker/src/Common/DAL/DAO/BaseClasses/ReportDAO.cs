#region Usings

using Logictracker.DAL.Factories;

#endregion

namespace Logictracker.DAL.DAO.BaseClasses
{
    /// <summary>
    /// Base report data access class.
    /// </summary>
    public abstract class ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report access class using the givenn daoFactory.
        /// </summary>
        /// <param name="daoFactory"></param>
        protected ReportDAO(DAOFactory daoFactory) { DAOFactory = daoFactory; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Bussiness objects data access class.
        /// </summary>
        protected readonly DAOFactory DAOFactory;

        #endregion
    }
}
