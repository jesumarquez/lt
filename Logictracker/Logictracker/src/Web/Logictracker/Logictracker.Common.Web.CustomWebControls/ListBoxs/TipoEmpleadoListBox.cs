using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TipoEmpleadoListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof(TipoEmpleado); } }

        public override string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_TIPOEMPLEADO"); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Employees types binding.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindTiposEmpleado(this);
        }

        #endregion
    }
}
