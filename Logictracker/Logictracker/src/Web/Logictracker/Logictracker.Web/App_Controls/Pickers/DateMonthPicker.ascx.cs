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
    public partial class App_Controls_DateMonthPicker : BaseDatePickerControl
    {
        #region Public Properties

        /// <summary>
        /// The selected date.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return Convert.ToDateTime(txtFecha.Text); }
            set { txtFecha.Text = value.ToString("MM/yyyy"); }
        }
    
        #endregion

        #region Protected Properties

        /// <summary>
        /// Picker associated TextBox control.
        /// </summary>
        protected override TextBox FechaTextBox { get { return txtFecha; } }

        /// <summary>
        /// Picker associated MaskedEditValidatorControl.
        /// </summary>
        protected override MaskedEditValidator FechaMaskedEditValidator { get { return mevFecha; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the javascript validation function name.
        /// </summary>
        /// <returns></returns>
        protected override string GetValidationFunction() { return "CheckMonthRange"; }

        /// <summary>
        /// Sets control initial value.
        /// </summary>
        protected override void SetInitialValue() { if (!Page.IsPostBack) SelectedDate = DateTime.Today; }

        #endregion
    }
}
