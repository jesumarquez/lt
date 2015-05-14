using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoProveedorDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(TipoProveedor); } }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindTipoProveedor(this); }

        #endregion
    }
}