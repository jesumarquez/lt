using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:BocaDeCargaDropDownCheckList runat=\"server\"></{0}:BocaDeCargaDropDownCheckList>")]
    public class BocaDeCargaDropDownCheckList : DropDownCheckListBase
    {
        public override Type Type { get { return typeof(CentroDeCostos); } }

        protected override void Bind() { BindingManager.BindBocaDeCarga(this); }

    }
}
