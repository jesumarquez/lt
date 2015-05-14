#region Usings

using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Qtree;

#endregion

namespace Logictracker.Operacion.Qtree
{
    public partial class Operacion_Qtree_LevelSelector : UserControl
    {
        public int SelectedLevel 
        { 
            get
            {
                return Convert.ToInt32(btSelected.CommandArgument);
            } 
            set
            {
                btSelected.Text = value.ToString();
                btSelected.BackColor = GetColorForLevel(value);
                btSelected.CommandArgument = value.ToString();
                updLevelSelector.Update();
            }
        }

        public Unit BoxSize
        {
            set 
            {
                btSelected.Width = value;
                btSelected.Height = value;
                panNivel.Width = new Unit(value.Value + 20, value.Type);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                SetColors();
                if (SelectedLevel == -1) SelectedLevel = 1;
            }
        }

        private void SetColors()
        {
            for (var i = 0; i < 16; i++)
                (FindControl("btNivel" + i) as Button).BackColor = GetColorForLevel(i);
        }
    
        public Color GetColorForLevel(int level)
        {
            return BaseQtree.GetColorForLevel(level);
        }

        protected void btNivel_Command(object sender, CommandEventArgs e)
        {
            SelectedLevel = Convert.ToInt32(e.CommandArgument);
        }

    }
}
