using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Web.CustomWebControls.Helpers;

namespace Logictracker.Web.CustomWebControls.Labels.Popup
{
    public class BaloonTip: Panel
    {
        protected ScriptHelper ScriptHelper;

        public string ResourceName
        {
            get { return (string)ViewState["ResourceName"] ?? "SystemMessages"; }
            set { ViewState["ResourceName"] = value; }
        }
        public string VariableName
        {
            get { return (string)ViewState["VariableName"] ?? string.Empty; }
            set { ViewState["VariableName"] = value; }
        }
        public string Text
        { 
            get
            {
                if (!string.IsNullOrEmpty(VariableName))
                {
                    return CultureManager.GetString(ResourceName, VariableName);
                }
                return (string)ViewState["Text"] ?? string.Empty;
            }
            set { ViewState["Text"] = value; }
        }
        public string Url
        {
            get { return (string)ViewState["Url"] ?? string.Empty; }
            set { ViewState["Url"] = value; }
        }
        public string Css
        {
            get { return (string)ViewState["Css"] ?? string.Empty; }
            set { ViewState["Css"] = value; }
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ScriptHelper = new ScriptHelper(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptHelper.RegisterJsResource("Logictracker.Web.CustomWebControls.Labels.Popup.baloontip.js");
            var css = new HtmlLink();
            css.Href = ScriptHelper.GetWebResourceUrl("Logictracker.Web.CustomWebControls.Labels.Popup.baloontip.css");
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            css.Attributes["media"] = "all";

            Page.Header.Controls.Add(css);

        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            
            

            Style.Add("display", "inline");

            var img = new Image
            {
                ID = ID + "_image",
                ImageUrl = Url != string.Empty ? Url : ScriptHelper.GetWebResourceUrl("Logictracker.Web.CustomWebControls.Labels.Popup.help.png")
            };
            Controls.Add(img);

            var panel = new Panel { ID = ID + "_Baloon", CssClass = "balloonstyle" };

            if (Css == "monitor")
                panel.CssClass = "balloonmonitor";
            
            var lit = new Literal {Text = Text};
            panel.Controls.Add(lit);
            Controls.Add(panel);

            img.Attributes.Add("rel", panel.ClientID);

            ScriptHelper.RegisterStartupScript("tooltip_" + ID, "initalizetooltip('" + img.ClientID + "', '" + Css + "');");
            
        }
    }
}
