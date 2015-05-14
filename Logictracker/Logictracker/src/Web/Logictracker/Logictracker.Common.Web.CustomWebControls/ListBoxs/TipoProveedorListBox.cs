using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TipoProveedorListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(TipoProveedor); } }

        protected override void Bind() { BindingManager.BindTipoProveedor(this); }
    }
}