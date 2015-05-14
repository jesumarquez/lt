using System;
using System.Web.UI;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:EquipoDropDownList ID=\"EquipoDropDownList1\" runat=\"server\"></{0}:EquipoDropDownList>")]
    [Serializable]
    public class EquipoDropDownList : DropDownListBase
    {
        #region Public Properties 
        
        /// <summary>
        /// Type associate to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof(Equipo); } }

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_NO_EQUIPMENT"); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind equipments.
        /// </summary>
        protected override void Bind() { BindingManager.BindEquipo(this); }

        #endregion
    }
}