#region Usings

using System;
using System.Web.UI;
using Logictracker.Web.CustomWebControls.Helpers;

#endregion

namespace Logictracker.Web.CustomWebControls.Input
{
    public class DateTimeRangeValidator: Control
    {
        protected ScriptHelper ScriptHelper;

        public string StartControlID 
        {
            get { return (string)ViewState["StartControlID"]; }
            set { ViewState["StartControlID"] = value; }
        }
        public string EndControlID
        {
            get { return (string)ViewState["EndControlID"]; }
            set { ViewState["EndControlID"] = value; }
        }
        protected DateTimePicker StartControl
        {
            get { return FindControlInPage(StartControlID) as DateTimePicker; }
        }
        protected DateTimePicker EndControl
        {
            get { return FindControlInPage(EndControlID) as DateTimePicker; }
        }
        protected Control FindControlInPage(string id)
        {
            if (string.IsNullOrEmpty(id.Trim())) return null;
            return FindControlInPage(id, Page.Controls);
        }
        protected static Control FindControlInPage(string id, ControlCollection controls)
        {
            if (string.IsNullOrEmpty(id.Trim())) return null;
            foreach (Control control in controls)
            {
                if (control.ID == id) return control;
                var result = FindControlInPage(id, control.Controls);
                if (result != null) return result;
            }
            return null;
        }

        public TimeSpan MaxRange
        {
            get { return (TimeSpan)(ViewState["MaxRange"] ?? TimeSpan.MaxValue); }
            set { ViewState["MaxRange"] = value; }
        }
        public TimeSpan MinRange
        {
            get { return (TimeSpan)(ViewState["MinRange"] ?? TimeSpan.Zero); }
            set { ViewState["MinRange"] = value; }
        }

        public DateTimeRangeValidator() { ScriptHelper = new ScriptHelper(this); }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptHelper.RegisterJsResource("Logictracker.Web.CustomWebControls.Input.DateTimeRangeValidator.js");

            if(!Page.IsPostBack)
            {
                ScriptHelper.RegisterStartupScript(ClientID + "_Def", string.Format("var DateTimeRangeValidator_{0};", ClientID));
            }
            var max = MaxRange == TimeSpan.MaxValue ? -1 : MaxRange.TotalMinutes;

            ScriptHelper.RegisterClientOnLoad(ClientID + "_Create", 
                string.Format(@"DateTimeRangeValidator_{0} = new DateTimeRangeValidator(DateTimePicker_{1}, DateTimePicker_{2}, {3}, {4});",
                              ClientID,
                              StartControl.ClientID,
                              EndControl.ClientID,
                              MinRange.TotalMinutes, max));
        }
    }
}
