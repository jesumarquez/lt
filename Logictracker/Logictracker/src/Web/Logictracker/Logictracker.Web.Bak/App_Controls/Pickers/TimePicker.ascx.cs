#region Usings

using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Web.BaseClasses.BaseControls;

#endregion

namespace Logictracker.App_Controls.Pickers
{
    /// <summary>
    /// Time Picker Ajax Control.
    /// </summary>
    public partial class App_Controls_TimePicker : BaseDatePickerControl
    {
        #region Public Properties

        /// <summary>
        /// The selected time.
        /// </summary>
        public TimeSpan SelectedTime
        {
            get { return new TimeSpan(Convert.ToInt32(txtTime.Text.Split(':')[0]), Convert.ToInt32(txtTime.Text.Split(':')[1]), 0); }
            set { txtTime.Text = string.Format("{0}:{1}", value.Hours, value.Minutes); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Picker associated TextBox control.
        /// </summary>
        protected override TextBox FechaTextBox { get { return txtTime; } }

        /// <summary>
        /// Picker associated MaskedEditValidator.
        /// </summary>
        protected override MaskedEditValidator FechaMaskedEditValidator { get { return mevTime; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets the initial value according to the defualt time mode.
        /// </summary>
        protected override void SetInitialValue()
        {
            switch (DefaultTimeMode)
            {
                case TimeMode.Start: txtTime.Text = "00:00"; break;
                case TimeMode.Now: txtTime.Text = DateTime.Now.ToString("hh:mm"); break;
                case TimeMode.End: txtTime.Text = "23:59"; break;
            }
        }

        /// <summary>
        /// Gets the javascript validation function name.
        /// </summary>
        /// <returns></returns>
        protected override string GetValidationFunction() { return "CheckTimeRange"; }

        #endregion
    }
}
