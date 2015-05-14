#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TiposFuncionDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the control.
        /// </summary>
        public override Type Type { get { return typeof(Funcion); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Function types binding.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindFunctionTypes(this);
        }

        #endregion
    }
}