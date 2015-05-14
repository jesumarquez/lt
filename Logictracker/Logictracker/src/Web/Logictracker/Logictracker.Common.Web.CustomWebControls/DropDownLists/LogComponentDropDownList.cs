using System;
using System.Web.UI;
using Logictracker.DatabaseTracer.Enums;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:LogComponentDropDownList runat=\"server\"></{0}:LogComponentDropDownList>")]
    public class LogComponentDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(LogComponents); } }

        protected override void Bind() { BindingManager.BindLogComponents(this); }
    }
}
