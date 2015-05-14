using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoVehiculoDropDownList : DropDownListBase
    {
        #region Public Properties

        public override Type Type { get { return typeof(TipoCoche); } }

        #endregion

        #region Protected Methods

        protected override void Bind() { BindingManager.BindVehicleTypes(this); }

        #endregion
    }
}