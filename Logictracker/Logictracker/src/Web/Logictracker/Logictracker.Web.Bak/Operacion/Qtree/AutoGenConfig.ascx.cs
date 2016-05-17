#region Usings

using System;
using System.Collections.Generic;
using System.Web.UI;
using Logictracker.Qtree.AutoGen;

#endregion

namespace Logictracker.Operacion.Qtree
{
    public partial class Operacion_Qtree_AutoGenConfig : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitLevels();
            }
        }

        private void InitLevels()
        {
            lvlAutopista.SelectedLevel = 13;
            lvlRuta.SelectedLevel = 12;
            lvlAvenida.SelectedLevel = 6;
            lvlCalle.SelectedLevel = 4;
        }

        public List<NivelAutoGen> GetNiveles()
        {
            var nivelAutopista = new NivelAutoGen(0, 1000, lvlAutopista.SelectedLevel, Convert.ToInt32(txtSliderAutopista.Text));
            var nivelRutaNac = new NivelAutoGen(1, 2200, lvlRuta.SelectedLevel, Convert.ToInt32(txtSliderRuta.Text));
            var nivelRutaProv = new NivelAutoGen(1, 2300, lvlRuta.SelectedLevel, Convert.ToInt32(txtSliderRuta.Text));
            var nivelAvenida = new NivelAutoGen(2, 1100, lvlAvenida.SelectedLevel, Convert.ToInt32(txtSliderAvenida.Text));
            var nivelPuente = nivelAvenida;
            var nivelTunel = nivelAvenida;
            var nivelCalle = new NivelAutoGen(3, 1200, lvlCalle.SelectedLevel, Convert.ToInt32(txtSliderCalle.Text));

            return new List<NivelAutoGen>{nivelAutopista, nivelRutaNac, nivelRutaProv, nivelAvenida, nivelCalle};
        }
    }
}
