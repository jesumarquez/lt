#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible
{
    [ToolboxData("<{0}:TipoMovimientoDropDownList runat=\"server\"></{0}:TipoMovimientoDropDownList>")]
    public class TipoMovimientoDropDownList: DropDownListBase
    {
        public override Type Type {get { return typeof (TipoMovimiento); } }

        protected override void Bind() {BindingManager.BindTipoMovimiento(this); }
    }
}
