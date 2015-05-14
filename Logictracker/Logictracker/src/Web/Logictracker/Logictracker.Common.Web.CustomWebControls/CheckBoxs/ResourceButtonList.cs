using System;
using System.Web.UI.WebControls;
using Logictracker.Culture;

namespace Logictracker.Web.CustomWebControls.CheckBoxs
{
    public class ResourceButtonList: RadioButtonList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AutoPostBack = true;
            
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            for (int i = 0; i < Items.Count; i++)
            {
                var res = Items[i].Attributes["ResourceName"];
                var val = Items[i].Attributes["VariableName"];
                Items[i].Text = CultureManager.GetString(string.IsNullOrEmpty(res) ? "Labels" : res, val);
                Items[i].Attributes.Remove("ResourceName");
                Items[i].Attributes.Remove("VariableName");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            foreach(ListItem item in Items)
            {
                item.Attributes["class"] = item.Selected ? "ResourceButtonListSelected" : "ResourceButtonList";
            }
            base.OnPreRender(e);
        }
    }
}
