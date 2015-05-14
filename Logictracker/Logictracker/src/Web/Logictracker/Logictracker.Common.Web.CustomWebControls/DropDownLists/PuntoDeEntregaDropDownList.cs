#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    /// <summary>
    /// Custom dropdownlist for handling delivery points.
    /// </summary>
    public class PuntoDeEntregaDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the type associated to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof (PuntoEntrega); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Delivery points data binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindPuntoDeEntrega(this); }

        #endregion
    }
}
