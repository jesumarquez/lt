#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TipoParametroDispositivoListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(TipoParametroDispositivo); } }

        #endregion

        #region Protected Methods
        /// <summary>
        /// TipoParametroDispositivo binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoParametroDispositivo(this); }

        #endregion
    }
}