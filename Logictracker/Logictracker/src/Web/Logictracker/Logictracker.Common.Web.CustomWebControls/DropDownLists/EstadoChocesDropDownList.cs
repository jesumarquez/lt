#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class EstadoChocesDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated T.
        /// </summary>
        public override Type Type { get { return typeof(Coche); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Vehicle states binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindEstadoCoches(this); }

        #endregion
    }
}