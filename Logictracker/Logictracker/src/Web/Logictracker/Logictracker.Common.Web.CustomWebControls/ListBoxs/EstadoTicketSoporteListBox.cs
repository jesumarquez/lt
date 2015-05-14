using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class EstadoTicketSoporteListBox : ListBoxBase
    {
        public override Type Type { get { return typeof (Cliente); } }

        protected override void Bind() { BindingManager.BindEstadoTicketSoporte(this); }
    }
}
