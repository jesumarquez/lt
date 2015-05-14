using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TransportistaDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Associated T.
        /// </summary>
        public override Type Type { get { return typeof(Transportista); } }

        public override string NoneItemsName
        {
            get { return CultureManager.GetControl("DDL_NO_TRANSPORTISTA"); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///Transportistas binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTransportista(this); }

        #endregion
    }
}