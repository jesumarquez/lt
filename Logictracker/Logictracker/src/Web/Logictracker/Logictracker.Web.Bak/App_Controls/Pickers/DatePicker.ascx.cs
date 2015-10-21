#region Usings

using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Web.BaseClasses.BaseControls;

#endregion

namespace Logictracker.App_Controls.Pickers
{
    /// <summary>
    /// Date Picker Ajax Control.
    /// </summary>
    public partial class App_Controls_DatePicker : BaseDatePickerControl
    {
        #region Public Properties

        /// <summary>
        /// The selected date.
        /// </summary>
        public DateTime SelectedDate
        {
            get
            {
                var date = string.IsNullOrEmpty(txtFecha.Text) ? DateTime.MinValue : Convert.ToDateTime(txtFecha.Text);

                if (DefaultTimeMode.Equals(TimeMode.Start)) return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

                return DefaultTimeMode.Equals(TimeMode.End) ? new DateTime(date.Year, date.Month, date.Day, 23, 59, 59) : date;
            }
            set { txtFecha.Text = value.ToShortDateString(); }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Picker associated TextBox.
        /// </summary>
        protected override TextBox FechaTextBox { get { return txtFecha; } }

        /// <summary>
        /// Picker associated MaskedEditValidator.
        /// </summary>
        protected override MaskedEditValidator FechaMaskedEditValidator { get { return mevFecha; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets javascript validation function name.
        /// </summary>
        /// <returns></returns>
        protected override string GetValidationFunction() { return "CheckDateRange"; }

        /// <summary>
        /// Sets the DatePicker initial value.
        /// </summary>
        protected override void SetInitialValue() { if (!Page.IsPostBack) SelectedDate = DateTime.Today; }

        #endregion
    }
}
