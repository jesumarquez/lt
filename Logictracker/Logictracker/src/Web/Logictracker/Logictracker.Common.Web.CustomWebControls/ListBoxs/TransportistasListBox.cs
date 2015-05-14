using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class TransportistasListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(Transportista); } }

        public override string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_TRANSPORTISTA"); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindTransportista(this); }

        #endregion
    }
}