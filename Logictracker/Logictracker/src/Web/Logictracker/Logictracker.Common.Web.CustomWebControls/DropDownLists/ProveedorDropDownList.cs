using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ProveedorDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(Proveedor); } }

        public override string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_PROVEEDOR"); }
        }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindProveedor(this); }

        #endregion
    }
}