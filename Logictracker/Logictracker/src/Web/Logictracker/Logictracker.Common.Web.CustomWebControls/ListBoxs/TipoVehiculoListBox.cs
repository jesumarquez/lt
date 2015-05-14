#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TipoVehiculoListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof (TipoCoche); } }

        /// <summary>
        /// Determines if the controls needs or not autobinding functionality.
        /// </summary>
        public bool AutoBind
        {
            get { return (bool) (ViewState["AutoBind"] ?? true); }
            set { ViewState["AutoBind"] = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Vehicle types data binding.
        /// </summary>
        protected override void Bind() { if (AutoBind) BindingManager.BindVehicleTypes(this); }

        #endregion
    }
}