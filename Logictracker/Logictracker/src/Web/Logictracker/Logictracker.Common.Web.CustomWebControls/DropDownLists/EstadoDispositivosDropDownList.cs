#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class EstadoDispositivosDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated T.
        /// </summary>
        public override Type Type { get { return typeof(Dispositivo); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Device states binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindEstadoDispositivos(this); }

        #endregion
    }
}