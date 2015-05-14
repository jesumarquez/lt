#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:ClienteDropDownCheckList runat=\"server\"></{0}:ClienteDropDownCheckList>")]
    public class ClienteDropDownCheckList : DropDownCheckListBase
    {
        #region Public Properties

        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(Cliente); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindCliente(this); }

        #endregion
    }
}
