#region Usings

using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

#endregion

namespace Logictracker.Web.BaseClasses.BaseControls
{
    /// <summary>
    /// Date picker base user control for common functionality.
    /// </summary>
    public abstract class BaseDatePickerControl : BaseUserControl
    {
        #region Public Enumerators

        /// <summary>
        /// Time default value mode.
        /// </summary>
        public enum TimeMode { NotSet, Start, Now, End }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Picker associated date TextBox.
        /// </summary>
        protected abstract TextBox FechaTextBox { get; }

        /// <summary>
        /// Picker associated MaskedEditValidator.
        /// </summary>
        protected abstract MaskedEditValidator FechaMaskedEditValidator { get; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Textbox width
        /// </summary>
        public int Width { set { if (FechaTextBox != null) FechaTextBox.Width = value; } }

        /// <summary>
        /// Determines wither if the empty value is or not allowed.
        /// </summary>
        public bool IsValidEmpty { set { if (FechaMaskedEditValidator != null) FechaMaskedEditValidator.IsValidEmpty = value; } }

        /// <summary>
        /// Related DatePicker control ID.
        /// </summary>
        public string TargetControlID
        {
            get { return ViewState["TargetControlID"] != null ? ViewState["TargetControlID"].ToString() : string.Empty; }
            set { ViewState["TargetControlID"] = value; }
        }

        /// <summary>
        /// Maximum posible interval.
        /// </summary>
        public int MaxInterval
        {
            get { return ViewState["MaxInterval"] != null ? Convert.ToInt32(ViewState["MaxInterval"]) : -1; }
            set { ViewState["MaxInterval"] = value; }
        }

        /// <summary>
        /// Gets the ClientID of the TextBox associated to the DatePicker.
        /// </summary>
        public string TextBoxControlID { get { return FechaTextBox.ClientID; } }

        /// <summary>
        /// Search button control ID.
        /// </summary>
        public string SearchButtonID
        {
            get { return ViewState["SearchButtonID"] != null ? ViewState["SearchButtonID"].ToString() : string.Empty; }
            set { ViewState["SearchButtonID"] = value; }
        }

        /// <summary>
        /// Default time mode.
        /// </summary>
        public TimeMode DefaultTimeMode
        {
            get { return ViewState["TimeMode"] != null ? (TimeMode)ViewState["TimeMode"] : TimeMode.NotSet; }
            set { ViewState["TimeMode"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets the initial date value.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetInitialValue();
        }

        /// <summary>
        /// Adds control values validation.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AddValidationScripts();
        }

        /// <summary>
        /// Adds the range and interval validation script to the search button.
        /// </summary>
        /// <param name="searchButton"></param>
        /// <param name="iniDateID"></param>
        /// <param name="finDateID"></param>
        /// <param name="interval"></param>
        protected void RegisterValidationScript(Button searchButton, string iniDateID, string finDateID, int interval)
        {
            searchButton.OnClientClick = string.Format("return {0}('{1}', '{2}', {3}, '{4}');", GetValidationFunction(), iniDateID, finDateID, interval, searchButton.OnClientClick);
        }

        /// <summary>
        /// Gets javascript validation function name.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetValidationFunction();

        /// <summary>
        /// Sets the DatePicker initial value.
        /// </summary>
        protected abstract void SetInitialValue();

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds data validation scripts.
        /// </summary>
        private void AddValidationScripts()
        {
            if (Page.IsPostBack) return;

            var searchButton = GetSearchButtonControl();

            if (searchButton == null) return;

            var targetControl = GetTargetControl();

            if (targetControl == null) return;

            RegisterValidationScript(searchButton, targetControl.TextBoxControlID, TextBoxControlID, MaxInterval);
        }

        /// <summary>
        /// Gets the associated DatePicker control.
        /// </summary>
        /// <returns></returns>
        private BaseDatePickerControl GetTargetControl()
        {
            if (string.IsNullOrEmpty(TargetControlID)) return null;

            return FindControl(Page.Form.Controls, TargetControlID) as BaseDatePickerControl;
        }

        /// <summary>
        /// Gets the associated search Button control.
        /// </summary>
        /// <returns></returns>
        private Button GetSearchButtonControl()
        {
            if (string.IsNullOrEmpty(SearchButtonID)) return null;

            return FindControl(Page.Form.Controls, SearchButtonID) as Button;
        }

        #endregion
    }
}