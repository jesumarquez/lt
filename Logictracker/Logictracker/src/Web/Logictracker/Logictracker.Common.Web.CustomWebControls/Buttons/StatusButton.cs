using System;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using System.Web.UI;
using Logictracker.Web.CustomWebControls.Helpers;

namespace Logictracker.Web.CustomWebControls.Buttons
{
    public class StatusButton: ImageButton
    {
        private const string ConnectedImage = "Logictracker.Web.CustomWebControls.Buttons.connected.png";
        private const string DisconnectedImage = "Logictracker.Web.CustomWebControls.Buttons.disconnected.png";

        protected override void  OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);
            if(Page.IsPostBack) return;

            var scriptHelper = new ScriptHelper(this);

            var connectedText = CultureManager.GetLabel("ONLINE_CONNECTED");
            var disconnectedText = CultureManager.GetLabel("ONLINE_DISCONNECTED");
            var connectedUrl = scriptHelper.GetWebResourceUrl(ConnectedImage);
            var disconnectedUrl = scriptHelper.GetWebResourceUrl(DisconnectedImage);

            ToolTip = connectedText;
            ImageUrl = connectedUrl;
            OnClientClick = "return !check_status();";


            var script = string.Format(
                @"var lastUpdate = new Date(); 
setInterval(check_status, 10000);
function check_status(){{
    var secs = Math.floor(new Date().getTime() - lastUpdate.getTime())/1000;
    if (secs > 20){{
        $get('{0}').src = '{1}';
        $get('{0}').title = '{3}';
        if(secs < 60) {5};
        return false;
    }} else {{
        $get('{0}').src = '{2}';
        $get('{0}').title = '{4}'; 
        return true;
    }}
}}",
                ClientID,
                disconnectedUrl,
                connectedUrl,
                disconnectedText,
                connectedText,
                Page.ClientScript.GetPostBackEventReference(this, "")
                );
            this.RegisterStartupJScript("check_status", script);

            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.PreLoad += Page_PreLoad;
        }

        void Page_PreLoad(object sender, EventArgs e)
        {
            var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional };
            updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ID });
            Parent.Controls.Add(updatePanel);
        }

        public void Update()
        {
            ScriptManager.RegisterStartupScript(Page, typeof(string), "check_status_update" + Guid.NewGuid(), GetUpdateScript(), true);
        }
        public string GetUpdateScript()
        {
            return "lastUpdate = new Date();";
        }
    }
}
