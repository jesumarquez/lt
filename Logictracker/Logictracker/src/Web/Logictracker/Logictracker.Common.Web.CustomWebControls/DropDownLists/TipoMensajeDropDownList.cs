#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:TipoMensajeDropDownList runat=\"server\"></{0}:TipoMensajeDropDownList>")]
    public class TipoMensajeDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Type.
        /// </summary>
        public override Type Type { get { return typeof(TipoMensaje); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoMensaje(this); }

        #endregion
    }
}