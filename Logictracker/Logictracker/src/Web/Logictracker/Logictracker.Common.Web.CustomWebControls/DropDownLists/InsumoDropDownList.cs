using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class InsumoDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(Insumo); } }

        public bool DeCombustible { get; set; }

        public override string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_INSUMOS"); }
        }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindInsumo(this); }

        #endregion
    }
}