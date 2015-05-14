using System;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TicketListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(Ticket); } }

        protected override void Bind() { BindingManager.BindTickets(this); }
    }
}