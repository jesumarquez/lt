#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoDeVehiculoDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof(TipoCoche); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Vehicle types binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindVehicleTypes(this); }

        #endregion
    }
}