#region Usings

using System;
using System.Web.UI;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:EstadoTicketDropDownList ID=\"EstadoTicketDropDownList1\" runat=\"server\"></{0}:EstadoTicketDropDownList>")]
    public class EstadoTicketDropDownList : DropDownListBase
    {
        public override Type Type
        {
            get { return typeof (string); }
        }

        protected override void Bind()
        {
            BindingManager.BindEstadoTicket(this);
        }
    }
}
