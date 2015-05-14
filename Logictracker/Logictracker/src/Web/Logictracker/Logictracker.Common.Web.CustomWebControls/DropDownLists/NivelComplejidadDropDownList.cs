using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class NivelComplejidadDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(TicketMantenimiento.NivelesComplejidad); } }

        protected override void Bind() { BindingManager.BindNivelComplejidad(this); }
    }
}