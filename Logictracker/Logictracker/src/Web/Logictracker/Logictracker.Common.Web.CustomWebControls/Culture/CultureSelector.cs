using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Logictracker.Configuration;

namespace Logictracker.Web.CustomWebControls.Culture
{
    /// <summary>
    /// Custom action toolbar control.
    /// </summary>
    [ToolboxData("<{0}:CultureSelector ID=\"CultureSelector1\" runat=\"server\"></{0}:CultureSelector>")]
    public class CultureSelector : Panel, INamingContainer
    {
        private static string[] _countries;
        private static string[] Countries
        {
            get
            {
                return _countries ?? (_countries = new[]
                                                       {
                                                           "ar|Argentina|es-AR",
                                                           "co|Colombia|es-CO",
                                                           "ve|Venezuela|es-VE"/*,
                                                           "us|Estados Unidos|en-US",
                                                           "br|Brasil|pt-BR"*/
                                                       });
            }
            set
            {
                _countries = value;
            }
        }

        #region Public Properties

        /// <summary>
        /// Defines if the nice little toolbar will do postaback Sync or Async.
        /// </summary>
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        [Localizable(true)]
        public bool ShowVertical
        {
            get { return (bool)(ViewState["ShowVertical"] ?? false); }
            set { ViewState["ShowVertical"] = value; }
        }

        /// <summary>
        /// Defines if the nice little toolbar will do postaback Sync or Async.
        /// </summary>
        [Bindable(true)]
        [DefaultValue("")]
        [Localizable(true)]
        public Color SelectedColor
        {
            get { return (Color)(ViewState["SelectedColor"] ?? Color.Red); }
            set { ViewState["SelectedColor"] = value; }
        }

        private int SelectedIndex
        {
            get { return (int)(ViewState["SelectedIndex"] ?? 0); }
            set { ViewState["SelectedIndex"] = value; }
        }
        #endregion

        #region Public Events

        public event EventHandler SelectedCountryChanged;

        #endregion

        #region Public Methods

        public string SelectedValue
        {
            get
            {
                var row = Countries[SelectedIndex].Split('|');
                return row[row.Length - 1];
            }
            set
            {
                SelectedIndex = GetCultureIndex(value);
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadData();

            foreach (var country in Countries)
            {
                var parts = country.Split('|');
                if(parts.Length != 3) continue;
                var ctl = new ImageButton { ID = parts[0], ToolTip = parts[1], ImageUrl = GetFlagUrl(parts[0]) };
                Controls.Add(ctl);
            }
        }

        private static void LoadData()
        {
            var file = Config.AvailableCulturesFile;
            if(string.IsNullOrEmpty(file) || !File.Exists(file)) return;

            Countries = File.ReadAllLines(file);
        }

        /// <summary>
        /// Overrides rendering process.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            RenderContents(writer);
        }
        /// <summary>
        /// Render the content of all associated controls.
        /// </summary>
        /// <param name="output"></param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            if(!string.IsNullOrEmpty(CssClass)) output.AddAttribute("class", CssClass);
            if (!string.IsNullOrEmpty(Style.Value)) output.AddAttribute("style", Style.Value);
            output.RenderBeginTag("span");

            output.Write("<table>");
            if (!ShowVertical) output.Write("<tr>");

            foreach (Control control in Controls)
            {
                if (ShowVertical) output.Write("<tr>");
                output.Write("<td>");

                if(Countries[SelectedIndex].StartsWith(control.ID + "|"))
                {
                    var ctl =control as ImageButton;
                    if (ctl != null)
                    {
                        ctl.BorderColor = SelectedColor;
                        ctl.BorderWidth = Unit.Pixel(2);
                    }
                }
                control.RenderControl(output);

                output.Write("</td>");
                if (ShowVertical) output.Write("</tr>");
            }
            if (!ShowVertical) output.Write("</tr>");
            output.Write("</table>");
            output.RenderEndTag();
        }

        /// <summary>
        /// Triggers the item command event handler if any is associated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnSelectedCountryChanged(object sender, EventArgs e) { if (SelectedCountryChanged != null) SelectedCountryChanged(sender, e); }

        protected override void AddedControl(Control control, int index)
        {
            base.AddedControl(control, index);
            SetCommandEvent(control);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the button command event as the handler of the comand event.
        /// </summary>
        /// <param name="control"></param>
        private void SetCommandEvent(Control control)
        {
            var button = control as IButtonControl;

            if (button == null) return;

            button.Command += Button_Command;
        }

        /// <summary>
        /// Triggers the on item comand event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Command(object sender, CommandEventArgs e)
        {
            var ctl = sender as ImageButton;
            if(ctl == null) return;
            var newIndex = GetIndex(ctl.ID);
            if (newIndex == SelectedIndex) return;
            SelectedIndex = newIndex;
            OnSelectedCountryChanged(sender, e);
        }

        private string GetFlagUrl(string file)
        {
            const string res = "Logictracker.Web.CustomWebControls.Culture.flags.{0}.gif";
            return Page.ClientScript.GetWebResourceUrl(GetType(), string.Format(res, file));
        }

        private static int GetIndex(string id)
        {
            for (var i = 0; i < Countries.Length; i++)
                if (Countries[i].StartsWith(id + "|")) return i;

            return -1;
        }
        private static int GetCultureIndex(string culture)
        {
            for (var i = 0; i < Countries.Length; i++)
                if (Countries[i].EndsWith("|" + culture)) return i;

            return 0;
        }
        #endregion
    }
}