#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoLogisticoDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Associated T.
        /// </summary>
        public override Type Type { get { return typeof(Mensaje); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Types binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTiposLogisticos(this); }

        #endregion
    }
}