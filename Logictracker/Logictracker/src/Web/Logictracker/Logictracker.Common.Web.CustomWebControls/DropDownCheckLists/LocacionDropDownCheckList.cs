#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:LocacionDropDownCheckList runat=\"server\"></{0}:LocacionDropDownList>")]
    public class LocacionDropDownCheckList : DropDownCheckListBase
    {
        #region Public Properties 
        
        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(Empresa); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindLocacion(this); }
     
        #endregion
    }
}