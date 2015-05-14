#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:TipoDispositivoDropDownCheckList runat=\"server\"></{0}:TipoDispositivoDropDownCheckList>")]
    public class TipoDeVehiculoDropDownCheckList : DropDownCheckListBase
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