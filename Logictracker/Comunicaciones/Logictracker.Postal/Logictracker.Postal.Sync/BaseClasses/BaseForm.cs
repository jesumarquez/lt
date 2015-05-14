#region Usings

using System;
using System.Text;
using System.Windows.Forms;
using Urbetrack.DAL.Factories;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.DatabaseTracer.Enums;
using Urbetrack.Postal.Sync.Forms;

#endregion

namespace Urbetrack.Postal.Sync.BaseClasses
{
    /// <summary>
    /// Class for gathering generic forms behaivour and properties.
    /// </summary>
    public class BaseForm : Form
    {
        #region Private Properties

        /// <summary>
        /// Database logger.
        /// </summary>
        private Tracer _tracer;
        
        /// <summary>
        /// Data access factory class.
        /// </summary>
        private DAOFactory _daoFactory;

        /// <summary>
        /// Current system status wait form.
        /// </summary>
        private readonly Wait _waitForm = new Wait { StartPosition = FormStartPosition.CenterParent };
       
        #endregion

        #region Protected Properties

        /// <summary>
        /// Database logger accessor.
        /// </summary>
        protected Tracer Tracer { get { return _tracer ?? (_tracer = new Tracer(LogModules.UrbetrackLaPostalSync, GetType().FullName)); } }

        /// <summary>
        /// Data access factory class accessor.
        /// </summary>
        protected DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Dispose all assigned resources.
        /// </summary>
        protected void DisposeResources()
        {
            if (_daoFactory != null) _daoFactory.Dispose();

            if (_tracer != null) _tracer.Dispose();
        }

        /// <summary>
        /// Saves into database the givenn exception and displays the specified error message.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        protected void InformsException(Exception ex, String message)
        {
            Tracer.TraceException(ex);

            var text = GetMessageText(ex, message);

            MessageBox.Show(text);
        }

        /// <summary>
        /// Opens a new status form to display the current wait action.
        /// </summary>
        /// <param name="status"></param>
        protected void ShowStatusForm(String status)
        {
            Enabled = false;

            _waitForm.DisplayStatus(status);

            _waitForm.Show(this);

            _waitForm.Refresh();
        }

        /// <summary>
        /// Closes the currently oppended wait status form.
        /// </summary>
        protected void CloseStatusForm()
        {
            Enabled = true;

            _waitForm.Hide();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the error message to be displayed.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static String GetMessageText(Exception ex, String message)
        {
            var builder = new StringBuilder();

            builder.AppendLine(message);
            builder.AppendLine();
            builder.AppendLine(ex.Message);

            return builder.ToString();
        }

        #endregion
    }
}