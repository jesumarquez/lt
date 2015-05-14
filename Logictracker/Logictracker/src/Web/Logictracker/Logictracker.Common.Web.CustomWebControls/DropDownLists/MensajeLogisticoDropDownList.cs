#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class MensajeLogisticoDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Associated T.
        /// </summary>
        public override Type Type { get { return typeof(Mensaje); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Messages binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindMensajesEstadosLogisticos(this); }

        #endregion
    }
}