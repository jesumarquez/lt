using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Security;

namespace Logictracker.Web.CustomWebControls.Labels
{
    public class OnlineClock: Control, INamingContainer
    {
        private string labelID { get { return GetChildID("label"); } }
        private string GetChildID(string controlID) { return string.Concat(ClientID, "_", controlID); } 

        private Label label;
        protected override void CreateChildControls()
        {
            label = new Label {ID = labelID};
            label.Style.Add(HtmlTextWriterStyle.Width, "auto");
            label.Text = DateTime.UtcNow.ToDisplayDateTime().ToString("HH:mm");

            Controls.Add(label);
        }
        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();
            var now = DateTime.UtcNow.ToDisplayDateTime();
            var time = string.Format("new Date({0},{1},{2},{3},{4},{5});",
                                     now.Year,
                                     now.Month,
                                     now.Day,
                                     now.Hour,
                                     now.Minute,
                                     now.Second);

            var func = GetChildID("Tick");
            var script = @" var serverTime = " + time + @";
                            function padLeft(a,l){var s=a+'';while(s.length<l)s='0'+s;return s;}
                            function " + func + @"() {serverTime.setTime(serverTime.getTime() + 10000);
                                    $get('" + label.ClientID + @"').innerHTML = padLeft(serverTime.getHours(), 2, '0') + '<blink>:</blink>' + padLeft(serverTime.getMinutes(), 2, '0');}
                            setInterval(" + func + @", 10000);";
            
            Page.ClientScript.RegisterStartupScript(typeof (string), GetChildID("Functions"), script, true);

            base.OnLoad(e);
        }
    }
}
