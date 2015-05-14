#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:TipoUsuarioDropDownList runat=\"server\"></{0}:TipoUsuarioDropDownList>")]
    public class TipoUsuarioDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T asociated to the control.
        /// </summary>
        public override Type Type { get { return typeof(Usuario); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// User types binding.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindUserTypes(this);
        }

        #endregion
    }
}