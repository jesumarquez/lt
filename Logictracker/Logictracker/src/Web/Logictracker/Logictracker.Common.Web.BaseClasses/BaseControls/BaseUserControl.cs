using System;
using System.Web.UI;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.SecurityObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.BaseClasses.BaseControls
{
    /// <summary>
    /// Adds configuration values and utility methods to a plain user control.
    /// </summary>
    public abstract class BaseUserControl : UserControl
    {
        #region Private Properties

        /// <summary>
        /// Data access factory.
        /// </summary>
        private DAOFactory _daof;

        /// <summary>
        /// Report objects data access factory.
        /// </summary>
        private ReportFactory _reportf;

        #endregion

        #region Public Properties

        /// <summary>
        /// Data access factory singleton.
        /// </summary>
        public DAOFactory DAOFactory
        {
            get
            {
                if (_daof == null)
                {
                    var page = Page as BasePage;

                    _daof = page == null ? new DAOFactory() : page.DAOFactory;
                }

                return _daof;
            }
        }

        /// <summary>
        /// Report objects data access factory singleton.
        /// </summary>
        public ReportFactory ReportFactory
        {
            get
            {
                if (_reportf == null)
                {
                    var page = Page as BasePage;

                    _reportf = page == null ? new ReportFactory(DAOFactory) : page.ReportFactory;
                }

                return _reportf;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The logged in user.
        /// </summary>
        protected static UserSessionData Usuario { get { return WebSecurity.AuthenticatedUser; } }

        /// <summary>
        /// Application root.
        /// </summary>
        protected static string ApplicationPath { get { return Config.ApplicationPath; } }

        /// <summary>
        /// Images directory.
        /// </summary>
        protected static string ImagesDir { get { return Config.Directory.ImagesDir; } }

        /// <summary>
        /// Icons directory.
        /// </summary>
        protected static string IconDir { get { return Config.Directory.IconDir; } }

        /// <summary>
        /// Application temporary directory.
        /// </summary>
        protected static string TmpDir { get { return Config.Directory.TmpDir; } }

        /// <summary>
        /// Gets the application title.
        /// </summary>
        protected static string ApplicationTitle { get { return Config.ApplicationTitle; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Findes the control with the indicated id in the givenn control collection or in its child controls.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected static Control FindControl(ControlCollection controls, string id)
        {
            foreach (Control control in controls)
            {
                if (control.ID == id) return control;

                var auxControl = FindControl(control.Controls, id);

                if (auxControl != null) return auxControl;
            }

            return null;
        }

        #endregion

        protected void ThrowError(string errorName, params object[] args)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError(errorName), args));
        }

        protected void ThrowMustEnter(string resource, string variable)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError("MUST_ENTER_VALUE"), CultureManager.GetString(resource, variable)));
        }

        protected void ThrowMustEnter(string labelVariable) { ThrowMustEnter("Labels", labelVariable); }

        protected void ThrowDuplicated(string resource, string variable)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError("DUPLICATED"), CultureManager.GetString(resource, variable)));
        }

        protected void ThrowDuplicated(string labelVariable) { ThrowDuplicated("Labels", labelVariable); }

        protected void ThrowInvalidValue(string resource, string variable)
        {
            throw new ApplicationException(string.Format(CultureManager.GetError("INVALID_VALUE"), CultureManager.GetString(resource, variable)));
        }

        protected void ThrowInvalidValue(string labelVariable) { ThrowInvalidValue("Labels", labelVariable); }

    }
}