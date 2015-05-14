#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:PlantaDropDownCheckList runat=\"server\"></{0}:PlantaDropDownCheckList>")]
    public class PlantaDropDownCheckList : DropDownCheckListBase
    {
        #region Public Properties
        
        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(Linea); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindPlanta(this); }

        #endregion
    }
}