#region Usings

using System;
using System.Windows.Forms;

#endregion

namespace Urbetrack.Postal.Sync.Forms
{
    /// <summary>
    /// Form that shows the current status of the program.
    /// </summary>
    public partial class Wait : Form
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new wait status form with the givenn status text.
        /// </summary>
        public Wait() { InitializeComponent(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Dislays the givenn text status.
        /// </summary>
        /// <param name="text"></param>
        public void DisplayStatus(String text) { lblStatus.Text = text; }

        #endregion

        private void Wait_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }
    }
}
