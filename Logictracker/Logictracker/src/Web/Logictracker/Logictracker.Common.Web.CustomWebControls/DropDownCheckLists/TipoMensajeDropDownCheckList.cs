using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:TipoMensajeDropDownCheckList runat=\"server\"></{0}:TipoMensajeDropDownCheckList>")]
    public class TipoMensajeDropDownCheckList : DropDownCheckListBase
    {
        #region Overrides of DropDownCheckListBase

        public override Type Type { get { return typeof(TipoMensaje); } }

        protected override void Bind()
        {
            BindingManager.BindTipoMensaje(this);
        }

        #endregion
    }
}
