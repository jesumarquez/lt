#region Usings

using System;
using Urbetrack.Common.Web.CustomWebControls.BaseControls;
using Urbetrack.Common.Web.CustomWebControls.Helpers;
using Urbetrack.Types.BusinessObjects;

#endregion

namespace Urbetrack.Common.Web.CustomWebControls.ListBoxs
{
    public class TipoGeocercaListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated T of the list box.
        /// </summary>
        public override Type Type { get { return typeof (TipoGeocerca); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Geocerca types data binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoGeocerca(this); }

        #endregion
    }
}