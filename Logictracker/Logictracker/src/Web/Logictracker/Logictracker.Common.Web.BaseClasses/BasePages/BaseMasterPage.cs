using System.Web.UI;
using Logictracker.Configuration;
using Logictracker.Security;
using Logictracker.Types.SecurityObjects;

namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Class that implements all the necesary logic for the master pages.
    /// </summary>
    public abstract class BaseMasterPage : MasterPage
    {
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
    }
}