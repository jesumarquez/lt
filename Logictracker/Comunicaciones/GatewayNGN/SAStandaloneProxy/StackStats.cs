#region Usings

using System;
using System.Windows.Forms;
using XbeeCore;

#endregion

namespace Urbetrack.GatewayMovil
{
    public partial class StackStats : Form
    {

        public StackStats()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var data = Stack.Update();
            stats.BeginUpdate();
            foreach (var k in data.Keys)
            {
                //Console.WriteLine("procesando {0}", k);
                var li = stats.FindItemWithText(k);
                if (li == null)
                {
                    li = new ListViewItem(k, 0);
                    li.SubItems.Add(data[k]);
                    stats.Items.Add(li);
                }
                else
                {
                    li.SubItems[1].Text = data[k];
                }
            }
            stats.EndUpdate();
        }

        private void StackStats_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}