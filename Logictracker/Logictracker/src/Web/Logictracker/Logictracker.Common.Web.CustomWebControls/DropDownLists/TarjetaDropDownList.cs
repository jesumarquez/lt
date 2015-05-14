#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TarjetaDropDownList : DropDownListBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated T.
        /// </summary>
        public override Type Type { get { return typeof(Tarjeta); } }

        /// <summary>
        /// Tha associated driver id.
        /// </summary>
        public int Chofer
        {
            get { return ViewState["Chofer"] != null ? Convert.ToInt32(ViewState["Chofer"]) : 0; }
            set { ViewState["Chofer"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Devices binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTarjetas(this); }

        #endregion
    }
}