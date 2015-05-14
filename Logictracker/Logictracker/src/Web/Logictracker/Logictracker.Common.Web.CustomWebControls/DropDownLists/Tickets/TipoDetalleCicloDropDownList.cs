#region Usings

using System;
using System.Web.UI;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists.Tickets
{
    [ToolboxData("<{0}:TipoDetalleCicloDropDownList ID=\"TipoDetalleCicloDropDownList1\" runat=\"server\" />")]
    [Serializable]
    public class TipoDetalleCicloDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Associated T.
        /// </summary>
        public override Type Type { get { return typeof(Int16); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Access leves binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoDetalleCiclo(this); }

        #endregion
    }
}
