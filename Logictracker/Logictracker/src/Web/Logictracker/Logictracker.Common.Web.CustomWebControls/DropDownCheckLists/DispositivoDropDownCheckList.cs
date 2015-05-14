#region Usings

using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:DispositivoDropDownList runat=\"server\"></{0}:DispositivoDropDownList>")]
    public class DispositivoDropDownCheckList : DropDownCheckListBase, IDispositivoAutoBindeable
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated T.
        /// </summary>
        public override Type Type { get { return typeof(Dispositivo); } }

        /// <summary>
        /// Tha associated vehicle id.
        /// </summary>
        public int Coche
        {
            get { return ViewState["Coche"] != null ? Convert.ToInt32(ViewState["Coche"]) : 0; }
            set { ViewState["Coche"] = value; }
        }

        public string Padre
        {
            get { return (string)ViewState["Padre"]; }
            set { ViewState["Padre"] = value; }
        }

        public bool HideAssigned
        {
            get { return ViewState["HideAssigned"] != null && (bool)ViewState["HideAssigned"]; }
            set { ViewState["HideAssigned"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Devices binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindDispositivos(this); }

        #endregion
    }
}