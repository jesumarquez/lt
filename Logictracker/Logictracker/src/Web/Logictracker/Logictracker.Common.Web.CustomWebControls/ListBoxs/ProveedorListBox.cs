using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class ProveedorListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(Proveedor); } }

        protected override void Bind() { BindingManager.BindProveedor(this); }
    }
}