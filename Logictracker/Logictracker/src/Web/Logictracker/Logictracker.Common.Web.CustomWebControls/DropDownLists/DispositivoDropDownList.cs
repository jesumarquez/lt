#region Usings

using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class DispositivoDropDownList : DropDownListBase, IDispositivoAutoBindeable
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

        public int Empleado
        {
            get { return ViewState["Empleado"] != null ? Convert.ToInt32(ViewState["Empleado"]) : 0; }
            set { ViewState["Empleado"] = value; }
        }

        public string Padre
        {
            get { return (string) ViewState["Padre"]; }
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