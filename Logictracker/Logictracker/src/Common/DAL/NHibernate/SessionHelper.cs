using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Logictracker.DAL.NHibernate
{
    public static class SessionHelper
    {
        /// <summary>
        /// Retrieve the current ISession.
        /// </summary>
        public static ISession Current
        {
            get
            {
                return NHibernateHelper.GetCurrentSession();
            }
        }

        /// <summary>
        /// Create an ISession.
        /// </summary>
        public static void CreateSession()
        {
            NHibernateHelper.CreateSession();
        }

        /// <summary>
        /// Clear an ISession.
        /// </summary>
        public static void ClearSession()
        {
            Current.Clear();
        }

        /// <summary>
        /// Open an ISession.
        /// </summary>
        public static ISession OpenSession()
        {
            return NHibernateHelper.OpenSession();
        }

        /// <summary>
        /// Close an ISession.
        /// </summary>
        public static void CloseSession()
        {
            NHibernateHelper.CloseSession();
        }
    }
}
