#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class TipoDispositivoDropDownList : DropDownListBase, ITipoDispositivoAutoBindeable
    {
        #region Public Properties

        /// <summary>
        /// Pulblicates the assosiated T.
        /// </summary>
        public override Type Type { get { return typeof(TipoDispositivo); } }

        /// <summary>
        /// Determines wither to display or not the no assigment value.
        /// </summary>
        public Boolean AddSinAsignar
        {
            get { return ViewState["AddSinAsignar"] != null ? Convert.ToBoolean(ViewState["AddSinAsignar"]) : false; }
            set { ViewState["AddSinAsignar"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Device types data binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindTipoDispositivo(this); }

        #endregion
    }
}