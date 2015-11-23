#region Usings

using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Web.BaseClasses.BaseControls;

#endregion

namespace Logictracker.App_Controls.Pickers
{
    public partial class App_Controls_DateTimePicker : BaseDatePickerControl
    {
        #region Public Enums

        /// <summary>
        /// Defines interval validation types.
        /// </summary>
        public enum IntervalValidationType { DAYS, HOURS }
     
        #endregion

        #region Public Properties

        /// <summary>
        /// The selected date.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return Convert.ToDateTime(txtFecha.Text); }
            set { txtFechaHidden.Text = txtFecha.Text = value.ToString("dd/MM/yyyy HH:mm"); }
        }

        /// <summary>
        /// Sets the validation T.
        /// </summary>
        public IntervalValidationType ValidationType
        {
            get
            {
                return ViewState["IntervalValidationType"] != null ? (IntervalValidationType)ViewState["IntervalValidationType"]
                           : IntervalValidationType.DAYS;
            }
            set { ViewState["IntervalValidationType"] = value; }
        }

        /// <summary>
        /// Enables or disables the control content.
        /// </summary>
        public bool Enabled
        {
            get { return txtFecha.Enabled; }
            set
            {
                txtFecha.Enabled = value;
                imgFecha.Visible = value;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Picker associated TextBoxControl.
        /// </summary>
        protected override TextBox FechaTextBox { get { return txtFecha; } }

        /// <summary>
        /// Picker associated MaskedEditValidator.
        /// </summary>
        protected override MaskedEditValidator FechaMaskedEditValidator { get { return mevFecha; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets the initial date value.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            txtFecha.Attributes.Add("onchange", string.Format("DateToHidden('{0}','{1}','{2}');", txtFecha.ClientID,
                                                              txtFechaHidden.ClientID, ceFecha.ClientID));

            txtFechaHidden.Attributes.Add("onchange", string.Format("DateToVisible('{0}','{1}');", txtFechaHidden.ClientID,
                                                                    txtFecha.ClientID));
        }

        /// <summary>
        /// Sets the initial value according to the defualt time mode.
        /// </summary>
        protected override void SetInitialValue()
        {
            switch (DefaultTimeMode)
            {
                case TimeMode.Start:
                    txtFechaHidden.Text = txtFecha.Text = string.Format("{0} 00:00", DateTime.Today.ToShortDateString()); break;
                case TimeMode.Now:
                    txtFechaHidden.Text = txtFecha.Text = string.Format("{0} {1}", DateTime.Today.ToShortDateString(),
                                                                        DateTime.Now.ToString("HH:mm")); break;
                case TimeMode.End:
                    txtFechaHidden.Text = txtFecha.Text = string.Format("{0} 23:59", DateTime.Today.ToShortDateString()); break;
            }
        }

        /// <summary>
        /// Gets the javascript validation function name.
        /// </summary>
        /// <returns></returns>
        protected override string GetValidationFunction()
        {
            return ValidationType.Equals(IntervalValidationType.DAYS) ? "CheckDateTimeRange" : "CheckDateTimeHoursRange";
        }

        #endregion
    }
}
