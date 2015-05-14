#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    /// <summary>
    /// Points of interest list box
    /// </summary>
    public class POIsListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(ReferenciaGeografica); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Points of interes data binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindReferenciaGeografica(this); }

        #endregion
    }
}