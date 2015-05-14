#region Usings

using System;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

#endregion

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class MensajesDropDownList : DropDownListBase, IMensajeAutoBindeable
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(Mensaje); } }

        /// <summary>
        /// Determines wither to show only maintenance messages.
        /// </summary>
        public bool SoloMantenimiento
        {
            get { return (bool)(ViewState["SoloMantenimiento"] ?? false); }
            set { ViewState["SoloMantenimiento"] = value; }
        }

        public bool SoloCombustible
        {
            get { return (bool)(ViewState["SoloCombustible"] ?? false); }
            set { ViewState["SoloCombustible"] = value; }
        }

        public bool AddSinMensaje
        {
            get { return ViewState["AddSinMensaje"] != null && Convert.ToBoolean(ViewState["AddSinMensaje"]); }
            set { ViewState["AddSinMensaje"] = value; }
        }

        public bool SoloAtencion
        {
            get { return (bool)(ViewState["SoloAtencion"] ?? false); }
            set { ViewState["SoloAtencion"] = value; }
        }

        public bool BindIds
        {
            get { return (bool)(ViewState["BindIds"] ?? false); }
            set { ViewState["BindIds"] = value; }
        }

        #endregion

        #region Protected Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { BindingManager.BindMensajes(this); }

        #endregion
    }
}
