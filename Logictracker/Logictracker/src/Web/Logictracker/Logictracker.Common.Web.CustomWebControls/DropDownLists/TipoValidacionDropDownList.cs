#region Usings

using System;
using System.Web.UI;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:TipoValidacionDropDownList runat=\"server\"></{0}:TipoValidacionDropDownList>")]
    public class TipoValidacionDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T asociated to the control.
        /// </summary>
        public override Type Type { get { return typeof(Int16); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// User types binding.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindTipoValidacion(this);
        }

        #endregion
    }
}
