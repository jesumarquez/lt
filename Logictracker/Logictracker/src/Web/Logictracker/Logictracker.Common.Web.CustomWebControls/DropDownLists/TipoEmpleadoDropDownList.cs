#region Usings

using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    /// <summary>
    /// Custom employee type drop down list.
    /// </summary>
    public class TipoEmpleadoDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof(TipoEmpleado); } }

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_NO_TIPOEMPLEADO"); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Employees types binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTiposEmpleado(this); }

        #endregion
    }
}
