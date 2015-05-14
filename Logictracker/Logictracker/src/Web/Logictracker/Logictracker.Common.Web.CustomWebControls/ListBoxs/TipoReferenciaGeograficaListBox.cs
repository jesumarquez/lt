#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    /// <summary>
    /// Address types list box.
    /// </summary>
    public class TipoReferenciaGeograficaListBox : ListBoxBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the type associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(TipoReferenciaGeografica); } }

        public bool Monitor { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Address type data binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoDomicilio(this); }

        #endregion
    }
}