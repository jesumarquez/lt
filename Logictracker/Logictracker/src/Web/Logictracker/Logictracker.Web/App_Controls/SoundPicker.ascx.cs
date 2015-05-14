#region Usings

using System;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Web.BaseClasses.BaseControls;

#endregion

namespace Logictracker.App_Controls
{
    public partial class App_Controls_SoundPicker : BaseUserControl
    {
        public Unit Width
        {
            get { return panelSonido.Width; }
            set { panelSonido.Width = value; }
        }

        public int Selected
        {
            get
            {
                if (ViewState["MarcaId"] == null) Selected = cbSonido.Selected;
                return (int)ViewState["MarcaId"];
            }
            set { ViewState["MarcaId"] = value;
                cbSonido.SelectedValue = value.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sound initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbSonido_InitialBind(object sender, EventArgs e) { cbSonido.EditValue = Selected; }

        /// <summary>
        /// Event play sound T action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkSonido_Click(object sender, EventArgs e)
        {
            litSonido.Text = string.Empty;
            if (cbSonido.SelectedIndex > 0) PlaySound();
        }

        /// <summary>
        /// Plays a preview of the selected sound.
        /// </summary>
        private void PlaySound()
        {
            var sound = DAOFactory.SonidoDAO.FindById(Convert.ToInt32(cbSonido.SelectedValue));
            var path = string.Concat(Config.Directory.SoundsDir, sound.URL);

            litSonido.Text = @"<div style=""display: block""><embed src=" + path + @" autostart=""true"" width=""0"" height=""0"" id=""sound1"" enablejavascript=""true"" /></div>";
        }
    }
}
