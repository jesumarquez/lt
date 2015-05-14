using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Web.CustomWebControls.Helpers;

namespace Logictracker.Web.CustomWebControls.Ajax
{
    public class SelectAllExtender: WebControl
    {
        public string TargetControlId { get; set; }
        public string ListControlId { get; set; }
        public bool AutoPostBack { get; set; }
        public ScriptHelper ScriptHelper;


        public Control ListControl
        {
            get
            {
                return Page.GetControl(ListControlId);
            }
        }

        public WebControl TargetControl
        {
            get
            {
                return Page.GetControl(TargetControlId) as WebControl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ScriptHelper  = new ScriptHelper(this);    
        }
       

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            const string script = "function SelectAllExtender_SelectAll(c){{ var cb = document.getElementById(c); var st = cb.selectedIndex < 0; for(var i = 0; i < cb.options.length; i++) {{cb.options[i].selected = st;}}}}";
            ScriptHelper.RegisterStartupScript("SelectAllExtender_SelectAll", script, true);

            var listControl = ListControl;
            var targetControl = TargetControl;

            targetControl.Attributes.Add("onclick", string.Format("SelectAllExtender_SelectAll('{0}');{1}", listControl.ClientID, AutoPostBack ? Page.ClientScript.GetPostBackEventReference(listControl, string.Empty) : string.Empty));
            targetControl.Style.Add("cursor", "pointer");
        }
    }
}
