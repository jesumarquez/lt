using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class EstadoTicketMantenimientoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TicketMantenimiento.EstadosTicket); } }

        protected override void Bind() { BindingManager.BindEstadosTicketMantenimiento(this); }
    }
}