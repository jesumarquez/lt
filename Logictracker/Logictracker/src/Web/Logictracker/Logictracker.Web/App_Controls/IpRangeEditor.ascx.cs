#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.App_Controls
{
    public partial class App_Controls_IpRangeEditor : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void txtIpFrom_TextChanged(object sender, EventArgs e)
        {
            var ipFrom = txtIpFrom.Text.Trim();
            if (!string.IsNullOrEmpty(ipFrom) && IsIpValid(ipFrom)) txtIpTo.Text = txtIpFrom.Text;
        }
        protected void btAddIpRange_Click(object sender, EventArgs e)
        {
            var ipFrom = txtIpFrom.Text.Trim();
            var ipTo = txtIpTo.Text.Trim();
            if (!IsIpValid(ipFrom) || !IsIpValid(ipTo)) return;
            cbIpRanges.Items.Add(new ListItem(ipFrom + " - " + ipTo, ipFrom + ";" + ipTo));
            txtIpFrom.Text = txtIpTo.Text = string.Empty;
        }
        protected void btDelIpRange_Click(object sender, EventArgs e)
        {
            if (cbIpRanges.SelectedIndex < 0) return;
            cbIpRanges.Items.RemoveAt(cbIpRanges.SelectedIndex);
        }
        private static bool IsIpValid(string ip)
        {
            var regEx = new Regex(@"^(([01]?\d\d?|2[0-4]\d|25[0-5])\.){3}([01]?\d\d?|25[0-5]|2[0-4]\d)$");
            return regEx.IsMatch(ip);
        }

        public void SetIpRanges(IEnumerable ranges)
        {
            cbIpRanges.Items.Clear();
            foreach(IpRange range in ranges)
            {
                cbIpRanges.Items.Add(new ListItem(range.IpFrom + " - " + range.IpTo, range.IpFrom + ";" + range.IpTo));
            }
        }

        public List<IpRange> GetIpRanges()
        {
            return (from ListItem li in cbIpRanges.Items
                    select li.Value.Split(';')
                    into ips select new IpRange {IpFrom = ips[0], IpTo = ips[1]}).ToList();
        }
    }
}
