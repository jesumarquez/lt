using System;
using System.Web;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using NHibernate;
using NHibernate.Context;

namespace Logictracker.DAL.NHibernate
{
    public static class NHibernateHelper
    {
        private static readonly global::NHibernate.Cfg.Configuration _configuration;
        private static ISessionFactory _sessionFactory;

        static NHibernateHelper()
        {
            _configuration = new global::NHibernate.Cfg.Configuration();
            _configuration.AddAssembly(Config.NhibernateAssembly);
            _sessionFactory = _configuration.BuildSessionFactory();
        }
        
        /// <summary>
        ///     In case there is an already instantiated NHibernate ISessionFactory,
        ///     retrieve it, otherwise instantiate it.
        /// </summary>
        public static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }    

        public static string getConnectionString()
        {
            return _configuration.GetProperty("connection.connection_string");
        }

        /// <summary>
        ///     Open an ISession based on the built SessionFactory.
        /// </summary>
        /// <returns>Opened ISession.</returns>
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        /// <summary>
        ///     Create an ISession and bind it to the current tNHibernate Context.
        /// </summary>
        public static void CreateSession()
        {
            CurrentSessionContext.Bind(OpenSession());
        }

        /// <summary>
        ///     Close an ISession and unbind it from the current
        ///     NHibernate Context.
        /// </summary>
        public static void CloseSession()
        {
            if (CurrentSessionContext.HasBind(SessionFactory))
            {
                var session = CurrentSessionContext.Unbind(SessionFactory);
                if (session == null) return;
                if (session.Transaction != null && session.Transaction.IsActive)
                {
                    STrace.Error(typeof(NHibernateHelper).FullName, String.Format("A non closed transaction is Active at CloseSession()!: {0}", HttpContext.Current != null ?HttpContext.Current.Request.CurrentExecutionFilePath : "No context?"));
                    try
                    {
                        session.Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(typeof(NHibernateHelper).FullName, ex, "CloseSession();");
                        try
                        {
                            session.Transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            STrace.Exception(typeof(NHibernateHelper).FullName, ex2, "CloseSession(); doing rollback");
                        }
                    }
                }
                session.Close();
                session.Dispose();
            }
        }

        /// <summary>
        ///     Retrieve the current binded NHibernate ISession, in case there
        ///     is any. Otherwise, open a new ISession.
        /// </summary>
        /// <returns>The current binded NHibernate ISession.</returns>
        public static ISession GetCurrentSession()
        {
             if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                CurrentSessionContext.Bind(OpenSession());
            }
            return SessionFactory.GetCurrentSession();
        }
    }
}