#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible
{
    [ToolboxData("<{0}:MotivoConciliacionDropDownList runat=\"server\"></{0}:MotivoConciliacionDropDownList>")]
    public class MotivoConciliacionDropDownList : DropDownListBase
    {
        public override Type Type {get { return typeof (MotivoConciliacion); } }

        protected override void Bind() {BindingManager.BindMotivoConciliacion(this); }
    }
}
