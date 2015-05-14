using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class InsumoListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(Insumo); } }

        protected override void Bind() { BindingManager.BindInsumo(this); }
    }
}