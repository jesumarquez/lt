#region Usings

using System;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoReferenciaGeograficaDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the type associated to the drop down list.
        /// </summary>
        public override Type Type { get { return typeof(TipoReferenciaGeografica); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Vehicle types binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoReferenciaGeografica(this); }

        #endregion
    }
}