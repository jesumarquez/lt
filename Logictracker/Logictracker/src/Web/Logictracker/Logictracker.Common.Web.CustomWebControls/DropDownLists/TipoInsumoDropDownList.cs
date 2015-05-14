using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoInsumoDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(TipoInsumo); } }

        public override string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_TIPO_INSUMO"); }
        }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindTipoInsumo(this); }

        #endregion
    }
}