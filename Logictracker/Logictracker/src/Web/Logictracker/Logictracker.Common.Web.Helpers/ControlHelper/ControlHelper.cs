using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Logictracker.Web.Helpers.ControlHelper
{
    public class ControlHelper
    {
        public static List<int> GetSelectedValues(ListBox lb)
        {
            return lb.SelectedIndex < 0
                           ? new List<int>(1) { 0 }
                           : (from index in lb.GetSelectedIndices()
                              select lb.Items[index].Value.Contains("T-") || lb.Items[index].Value.Contains("V-")
                                    ? Convert.ToInt32(lb.Items[index].Value.Split('-')[1])
                                    : Convert.ToInt32(lb.Items[index].Value))
                             .ToList();
        }

        public static void ToogleItems(ListBox lb)
        {
            foreach (ListItem item in lb.Items) item.Selected = !item.Selected;
        }
    }
}