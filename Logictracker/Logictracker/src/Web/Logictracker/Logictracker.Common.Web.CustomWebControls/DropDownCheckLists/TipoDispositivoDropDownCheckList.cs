#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:TipoDispositivoDropDownCheckList runat=\"server\"></{0}:TipoDispositivoDropDownCheckList>")]
    public class TipoDispositivoDropDownCheckList : DropDownCheckListBase, ITipoDispositivoAutoBindeable
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