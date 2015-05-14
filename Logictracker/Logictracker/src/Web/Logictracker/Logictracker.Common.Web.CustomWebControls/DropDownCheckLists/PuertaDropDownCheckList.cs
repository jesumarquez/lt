using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:PuertaDropDownCheckList ID=\"PuertaDropDownCheckList1\" runat=\"server\"></{0}:PuertaDropDownCheckList>")]
    public class PuertaDropDownCheckList : DropDownCheckListBase
    {
        public override Type Type { get { return typeof(PuertaAcceso); } }

        protected override void Bind() { BindingManager.BindPuertas(this); }

    }
}