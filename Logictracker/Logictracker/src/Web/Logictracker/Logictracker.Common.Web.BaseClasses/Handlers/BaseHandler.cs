using System;
using System.Web;
using Logictracker.Configuration;

namespace Logictracker.Web.BaseClasses.Handlers
{
    public abstract class BaseHandler : IHttpHandler
    {
        #region Protected Properties

        /// <summary>
        /// Application root.
        /// </summary>
        protected static string ApplicationPath { get { return Config.ApplicationPath; } }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines wither if the handler is reusable or not. Override this property to make the handler reusable.
        /// </summary>
        public virtual bool IsReusable { get { return false; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs the handler main tasks.
        /// </summary>
        /// <param name="context"></param>
        protected abstract void DoIt(HttpContext context);

        #endregion

        #region Public Methods

        /// <summary>
        /// Process the handler tasks and performs error handling.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest (HttpContext context)
        {
            try { DoIt(context); }
            catch (Exception ex)
            {
                context.Session.Add("Error", ex);

                context.Server.Transfer(string.Concat(ApplicationPath, "Error.aspx"));
            }
        }

        #endregion
    }
}