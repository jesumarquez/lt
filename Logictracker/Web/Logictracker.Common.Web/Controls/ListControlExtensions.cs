#region Usings

using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Web.Controls
{
    public static class ListControlExtensions
    {
        public static void TrySelect(this ListControl control, string value)
        {
            var li = control.Items.FindByValue(value);
            if (li != null) li.Selected = true;
        }
    }
}
