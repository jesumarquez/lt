using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class EstadoPresupuestoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TicketMantenimiento.EstadosPresupuesto); } }

        protected override void Bind() { BindingManager.BindEstadosPresupuesto(this); }
    }
}