#region Usings

using Logictracker.Configuration;
using NHibernate;

#endregion

namespace Logictracker.Metrics.NHibernateManagers
{
    public static class NHibernateHelper
    {
        #region Private Properties

        /// <summary>
        /// NHIbernate session factory.
        /// </summary>
        private static readonly ISessionFactory SessionFactory;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Configurates and instanciates a Nhibernate session factory.
        /// </summary>
        static NHibernateHelper()
        {
            var configuration = new NHibernate.Cfg.Configuration();

            configuration.AddAssembly(Config.Metrics.MetricsNhibernateAssembly);

            SessionFactory = configuration.BuildSessionFactory();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a new NHibernate session from the current session factory.
        /// </summary>
        /// <returns></returns>
        public static ISession GetSession() { return SessionFactory.OpenSession(); }

        /// <summary>
        /// Gets a new NHibernate stateless session from the current session factory.
        /// </summary>
        /// <returns></returns>
        public static IStatelessSession GetStatelessSession() { return SessionFactory.OpenStatelessSession(); }

        #endregion
    }
}