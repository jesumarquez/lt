using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists.Tickets
{
    [ToolboxData("<{0}:BocaDeCargaDropDownList ID=\"BocaDeCargaDropDownList1\" runat=\"server\" />")]
    [Serializable]
    public class BocaDeCargaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(BocaDeCarga); } }

        protected override void Bind() { BindingManager.BindBocaDeCarga(this); }

    }
}
