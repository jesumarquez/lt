#region Usings

using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.Panels;
using Logictracker.Web.CustomWebControls.ToolBar;

#endregion

namespace Logictracker.MasterPages
{
    public partial class AbmPage : BaseAbmMasterPage
    {
        #region Protected Properties

        public override ToolBar ToolBar { get { return ToolBar1; } }

        public override InfoLabel LblInfo { get { return infoLabel1; } }

        public override HtmlGenericControl LblId { get { return lblId; } } 

        #endregion

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            litPostback.Text = string.Format("$get('{0}').value = command; {1};", hidTabCommand.ClientID, Page.ClientScript.GetPostBackEventReference(btChangeTab, String.Empty));
        }

        protected void btChangeTab_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            var commandName = hidTabCommand.Value;
            var tab = FindTabPanel(Controls, commandName);
            if(tab != null) tab.InvokeSelected();
        }

        protected AbmTabPanel FindTabPanel(ControlCollection controls, string command)
        {
            foreach (System.Web.UI.Control control in controls)
            {
                if(control.GetType() == typeof(AbmTabPanel))
                {
                    var tab = control as AbmTabPanel;
                    if(tab.ClientID == command) return tab;
                }
                else
                {
                    var tab = FindTabPanel(control.Controls, command);
                    if(tab != null) return tab;
                }
            }
            return null;
        }

        public override void SetTabIndex(int index)
        {
            var scriptHelper = new ScriptHelper(this);
            scriptHelper.RegisterClientOnLoad("setTabIndex", string.Format("setTabIndex({0});",index));
        
        }
    }
}
