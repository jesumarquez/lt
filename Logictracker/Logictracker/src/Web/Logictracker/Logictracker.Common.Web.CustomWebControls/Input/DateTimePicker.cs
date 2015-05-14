#region Usings

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Logictracker.Security;
using Logictracker.Web.CustomWebControls.Helpers;

#endregion

namespace Logictracker.Web.CustomWebControls.Input
{
    public enum DateTimePickerMode { DateTime, Date, Time, Month }
    public enum DateTimePickerTimeMode { None, Start, End, Now }

    [Designer("Logictracker.Web.CustomWebControls.Input.DateTimePickerDesigner")]
    public class DateTimePicker : Control, INamingContainer, IPostBackDataHandler
    {
        public event EventHandler DateChanged;      


        #region Protected Properties

        protected ScriptHelper ScriptHelper;

        // ID Properties
        protected string HiddenTextBoxId { get { return ScriptHelper.PrefixKey("hiddenTextBox"); } }
        protected string TextBoxId { get { return ScriptHelper.PrefixKey("textBox"); } }
        protected string CalendarButtonId { get { return ScriptHelper.PrefixKey("calendarButton"); } }
        protected string CalendarExtenderId { get { return ScriptHelper.PrefixKey("calendarExtender"); } }
        protected string MaskedEditExtenderId { get { return ScriptHelper.PrefixKey("maskedEditExtender"); } }
        protected string MaskEditValidatorId { get { return ScriptHelper.PrefixKey("maskEditValidator"); } }
        protected string JsObjectId { get { return string.Concat("DateTimePicker_", ClientID); } }

        // Control Properties
        protected TextBox TextBox;
        protected TextBox HiddenTextBox;
        protected Image CalendarButton;
        protected CalendarExtender CalendarExtender;
        protected MaskedEditExtender MaskedEditExtender;
        protected MaskedEditValidator MaskEditValidator;

        #endregion

        #region Internal Properties

        internal string CalendarImageUrl { get { return ScriptHelper.GetWebResourceUrl("Logictracker.Web.CustomWebControls.Input.calendar.gif"); } }
        internal ModeValues Values { get { return ModeValues.FromMode(Mode, TimeMode); } }

        private bool Loaded
        {
            get { return (bool)(ViewState["Loaded"] ?? false); }
            set { ViewState["Loaded"] = value; }
        }

        #endregion

        #region Public Properties

        public bool HideCalendarButton
        {
            get
            {
                EnsureChildControls(); return !CalendarButton.Visible;
            }
            set
            {
                EnsureChildControls();
                CalendarButton.Visible = !value;
            }
        }

        public bool AutoPostBack
        {
            get { return (bool) (ViewState["AutoPostBack"] ?? false);}
            set { ViewState["AutoPostBack"] = value; }
        }

        public Unit Width
        {
            get { return (Unit)(ViewState["Width"] ?? Values.Width); }
            set { ViewState["Width"] = value; if (TextBox != null) TextBox.Width = value; }
        }

        public DateTime? SelectedDate
        {
            get
            {
                EnsureChildControls(); return Values.ParseDateTime(TextBox.Text);
            }
            set
            {
                EnsureChildControls();
                CalendarExtender.SelectedDate = value;
                TextBox.Text = value.HasValue ? value.Value.ToString(Values.Format) : string.Empty;
            }
        }

        public TimeSpan? SelectedTime
        {
            get
            {
                EnsureChildControls(); return Values.ParseTimeSpan(TextBox.Text);
            }
            set
            {
                EnsureChildControls();
                if (Mode == DateTimePickerMode.Date) return;
                if (!value.HasValue) { TextBox.Text = string.Empty; }
                else if (Mode == DateTimePickerMode.Time && value.Value < TimeSpan.FromHours(24))
                {
                    var now = DateTime.Now.Date.Add(value.Value);
                    TextBox.Text = now.ToString(Values.Format);
                }
            }
        }

        public CalendarPosition PopupPosition
        {
            get { EnsureChildControls(); return CalendarExtender.PopupPosition; }
            set { EnsureChildControls(); CalendarExtender.PopupPosition = value; }
        }

        public bool Enabled
        {
            get { return (bool)(ViewState["Enabled"] ?? true); }
            set { ViewState["Enabled"] = value; }
        }
        public DateTimePickerMode Mode
        {
            get { return (DateTimePickerMode)(ViewState["Mode"] ?? DateTimePickerMode.DateTime); }
            set
            {
                ViewState["Mode"] = value;
                EnsureChildControls();
                CalendarExtender.Format = Values.Format;
                MaskedEditExtender.Mask = Values.Mask;
                MaskedEditExtender.MaskType = Values.MaskType;
                TextBox.Width = Width;

                if (Mode == DateTimePickerMode.Time)
                {
                    if (Controls.Contains(HiddenTextBox)) Controls.Remove(HiddenTextBox);
                    if (Controls.Contains(CalendarButton)) Controls.Remove(CalendarButton);
                    if (Controls.Contains(CalendarExtender)) Controls.Remove(CalendarExtender);
                }
            }
        }
        public DateTimePickerTimeMode TimeMode
        {
            get { return (DateTimePickerTimeMode)(ViewState["TimeMode"] ?? DateTimePickerTimeMode.None); }
            set { ViewState["TimeMode"] = value; }
        }
        public bool IsValidEmpty
        {
            get { return (bool)(ViewState["IsValidEmpty"] ?? true); }
            set
            {
                ViewState["IsValidEmpty"] = value;
                EnsureChildControls();
                MaskEditValidator.IsValidEmpty = value;
            }
        }
        /// <summary>
        /// Secures the control
        /// </summary>
        [Category("Custom Resources")]
        public string SecureRefference
        {
            get { return ViewState["SecureRefference"] != null ? ViewState["SecureRefference"].ToString() : string.Empty; }
            set { ViewState["SecureRefference"] = value; }
        }
        #endregion

        #region Contructors

        public DateTimePicker() { ScriptHelper = new ScriptHelper(this); } 
        #endregion

        #region Overriden Methods
        protected override void CreateChildControls()
        {
            HiddenTextBox = new TextBox { ID = HiddenTextBoxId, Width = new Unit("16px") };
            HiddenTextBox.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
            HiddenTextBox.Style.Add(HtmlTextWriterStyle.Position, "absolute");
            HiddenTextBox.Style.Add(HtmlTextWriterStyle.ZIndex, "0");

            TextBox = new TextBox { ID = TextBoxId, Width = Width };
            TextBox.Attributes.Add("AutoComplete", "off");
            TextBox.Style.Add(HtmlTextWriterStyle.Padding, "1px 0px 1px 0px");

            CalendarButton = new Image { ID = CalendarButtonId };
            CalendarButton.Style.Add(HtmlTextWriterStyle.Position, "relative");
            CalendarButton.Style.Add(HtmlTextWriterStyle.MarginBottom, "-6px;");
            CalendarButton.Style.Add("right", "2px");

            CalendarExtender = new CalendarExtender
                                       {
                                           ID = CalendarExtenderId,
                                           PopupButtonID = CalendarButton.ID,
                                           TargetControlID = HiddenTextBox.ID,
                                           Format = Values.Format,
                                           PopupPosition = CalendarPosition.TopRight
                                       };

            MaskedEditExtender = new MaskedEditExtender
                                         {
                                             ID = MaskedEditExtenderId,
                                             BehaviorID = MaskedEditExtenderId +"_bhv",
                                             TargetControlID = TextBox.ID,
                                             MaskType = Values.MaskType,
                                             Mask = Values.Mask,
                                             ClearMaskOnLostFocus = false,
                                             CultureName = "es-AR",
                                             OnFocusCssClass = "MaskedEditFocus",
                                             AutoComplete = false,
                                             OnInvalidCssClass = "MaskedInvalid"
                                         };

            MaskEditValidator = new MaskedEditValidator
                                        {
                                            ID = MaskEditValidatorId,
                                            ControlToValidate = TextBox.ID,
                                            ControlExtender = MaskedEditExtender.ID,
                                            IsValidEmpty = IsValidEmpty
                                        };

            Controls.Add(TextBox);
            if (Mode != DateTimePickerMode.Time)
            {
                Controls.Add(CalendarButton);
                Controls.Add(HiddenTextBox);
                Controls.Add(CalendarExtender);
            }
            Controls.Add(MaskedEditExtender);
            Controls.Add(MaskEditValidator);
        }

        protected override void OnLoad(EventArgs e)
        {
            Page.RegisterRequiresPostBack(this);

            var scriptManager = ScriptManager.GetCurrent(Page);

            if (scriptManager != null) scriptManager.RegisterAsyncPostBackControl(this);

            RegisterScripts();
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            RegisterScripts();

            base.OnPreRender(e);

            Visible = WebSecurity.IsSecuredAllowed(SecureRefference);
        }

        #endregion

        #region Protected Methods

        protected void RegisterScripts()
        {
            EnsureChildControls();
            CalendarButton.ImageUrl = CalendarImageUrl;
            CalendarExtender.OnClientDateSelectionChanged = ScriptHelper.PrefixKey("Changed");
            CalendarExtender.OnClientShowing = ScriptHelper.PrefixKey("Showing");
            CalendarExtender.OnClientHidden = ScriptHelper.PrefixKey("Hidden");
            CalendarExtender.OnClientShown = ScriptHelper.PrefixKey("Shown");

            if (!Page.IsPostBack) ScriptHelper.RegisterJsResource("Logictracker.Web.CustomWebControls.Input.DateTimePicker.js");

            string script;
            if (!Page.IsPostBack || !Loaded)
            {
                script = string.Format(@"var {0};
                function {1}(sender, args) {{ {0}.CalendarChanged(sender, args); }}
                function {2}(sender, args) {{ {0}.CalendarShowing(sender, args); }}
                function {3}(sender, args) {{ {0}.CalendarHidden(sender, args); }}
                function {4}(sender, args) {{ {0}.CalendarShown(sender, args); }}",
                    JsObjectId,
                    CalendarExtender.OnClientDateSelectionChanged,
                    CalendarExtender.OnClientShowing,
                    CalendarExtender.OnClientHidden,
                    CalendarExtender.OnClientShown);

                ScriptHelper.RegisterStartupScript(ScriptHelper.PrefixKey("Functions"), script);
            }
            script = string.Format(@"if(!{0}) {0} = new DateTimePicker('{1}', '{2}', '{3}','{4}'); else {0}.init('{1}', '{2}', '{3}','{4}');",
                    JsObjectId, TextBox.ClientID, HiddenTextBox.ClientID, Mode, MaskedEditExtender.BehaviorID);
            
            if (AutoPostBack)
            {
                script += string.Format(@"{0}.onchange = function(){{ {1} }};",
                                       JsObjectId, Page.ClientScript.GetPostBackEventReference(this,"change"));
            }

            script += string.Format("{0}.setEnabled({1});", JsObjectId, Enabled ? "true" : "false");

            ScriptHelper.RegisterClientOnLoad(ScriptHelper.PrefixKey("Init"), script);
            Loaded = true;
        } 

        #endregion

        #region Internal Classes
        internal class ModeValues
        {
            public string Format { get; private set; }
            public string Mask { get; private set; }
            public MaskedEditType MaskType { get; private set; }
            public Unit Width { get; private set; }
            public DateTimePickerMode Mode { get; private set; }
            public DateTimePickerTimeMode TimeMode { get; private set; }

            private ModeValues() { }

            public static ModeValues FromMode(DateTimePickerMode mode, DateTimePickerTimeMode timeMode)
            {
                switch (mode)
                {
                    case DateTimePickerMode.Date:
                        return new ModeValues { Mode = mode, TimeMode = timeMode, Format = "dd/MM/yyyy", Mask = "99/99/9999", MaskType = MaskedEditType.Date, Width = new Unit("70px") };
                    case DateTimePickerMode.DateTime:
                        return new ModeValues { Mode = mode, TimeMode = timeMode, Format = "dd/MM/yyyy HH:mm", Mask = "99/99/9999 99:99", MaskType = MaskedEditType.DateTime, Width = new Unit("103px") };
                    case DateTimePickerMode.Month:
                        return new ModeValues { Mode = mode, TimeMode = timeMode, Format = "MM/yyyy", Mask = "99/9999", MaskType = MaskedEditType.None, Width = new Unit("53px") };
                    case DateTimePickerMode.Time:
                        return new ModeValues { Mode = mode, TimeMode = timeMode, Format = "HH:mm", Mask = "99:99", MaskType = MaskedEditType.Time, Width = new Unit("38px") };
                }
                return null;
            }

            public DateTime? ParseDateTime(string text)
            {
                var toparse = text;
                switch (Mode)
                {
                    case DateTimePickerMode.Month: toparse = string.Concat("01/", text); break;
                    case DateTimePickerMode.Time: toparse = string.Concat(DateTime.Now.ToString("dd/MM/yyyy"), " ", text); break;
                    case DateTimePickerMode.Date:
                    case DateTimePickerMode.DateTime: break;
                }

                DateTime dt;
                if (DateTime.TryParse(toparse, new CultureInfo("es-AR"), DateTimeStyles.None, out dt))
                {
                    if (Mode != DateTimePickerMode.Date) return dt;
                    if (TimeMode == DateTimePickerTimeMode.Start) return dt.Date;
                    if (TimeMode == DateTimePickerTimeMode.End) return dt.Date.AddDays(1).AddMilliseconds(-1);
                    return dt;
                }
                return null;
            }
            public TimeSpan? ParseTimeSpan(string text)
            {
                if (Mode == DateTimePickerMode.Time)
                {
                    TimeSpan ts;
                    if (TimeSpan.TryParse(text, out ts)) return ts;
                }
                else
                {
                    var date = ParseDateTime(text);
                    if (date.HasValue) return date.Value.TimeOfDay;
                }
                return null;
            }
        } 
        #endregion

        #region Implementation of IPostBackDataHandler

        private string PostedValue = string.Empty;
        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (postCollection["__EVENTTARGET"] != UniqueID) return false;

            PostedValue = postCollection["__EVENTARGUMENT"];
            return true;
        }

        public void RaisePostDataChangedEvent()
        {
            if (PostedValue == "change" && DateChanged != null) DateChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the date according to the configured dafault Value.
        /// </summary>
        public void SetDate()
        {
            switch (TimeMode)
            {
                case DateTimePickerTimeMode.Start: { SelectedDate = DateTime.Now.Date; break; }
                case DateTimePickerTimeMode.End: { SelectedDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59); break; }
                case DateTimePickerTimeMode.Now: { SelectedDate = DateTime.Now; break; }
                default: break;
            }
        }

        #endregion
    }

    public class DateTimePickerDesigner : ControlDesigner 
    {
        public override string GetDesignTimeHtml()
        {
            var ctl = (DateTimePicker)Component;

            var sw = new StringWriter();
            var hw = new HtmlTextWriter(sw);

            var w = new Unit(ctl.Width.Value+5, UnitType.Pixel);
            var textBox = new TextBox { Width = w };
            textBox.Attributes.Add("AutoComplete", "off");
            textBox.Text = DateTime.Now.ToString(ctl.Values.Format);

            Image calendarButton = null;
            if (ctl.Mode != DateTimePickerMode.Time)
            {
                calendarButton = new Image {ImageUrl = ctl.CalendarImageUrl};
                calendarButton.Style.Add(HtmlTextWriterStyle.Position, "relative");
                calendarButton.Style.Add("right", "3px");
            }

            textBox.RenderControl(hw);
            if (ctl.Mode != DateTimePickerMode.Time) if (calendarButton != null) calendarButton.RenderControl(hw);

            return sw.ToString();

        }
    }
}
