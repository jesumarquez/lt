#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

#endregion

namespace Logictracker.Web.Controls.BindableListControl
{
    public static class BindableListControlExtensions
    {
        public static IEnumerable<IBindableListControl> GetParents(this IBindableListControl control)
        {
            var names = (control.ParentControls ?? string.Empty).Split(',');
            var asControl = control as Control;
            return names.Select(id => asControl.GetControlOnPage(id.Trim()) as IBindableListControl).Where(b => b != null);
        }
    }
}
