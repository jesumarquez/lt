using System;
using System.ComponentModel;
using System.Web.UI;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    [ToolboxData("<{0}:MovilDropDownList runat=\"server\"></{0}:MovilDropDownList>")]
    [Serializable]
    public class MovilDropDownList : DropDownListBase, IMovilAutoBindeable
    {
        #region Public Properties 
        
        public override Type Type { get { return typeof(Coche); } }

        public bool ShowDriverName
        {
            get { return (bool)(ViewState["ShowDriverName"] ?? false); }
            set { ViewState["ShowDriverName"] = value; }
        }

        public bool ShowOnlyAccessControl
        {
            get { return (bool)(ViewState["ShowOnlyAccessControl"] ?? false); }
            set { ViewState["ShowOnlyAccessControl"] = value; }
        }
        public bool HideWithNoDevice
        {
            get { return (bool)(ViewState["HideWithNoDevice"] ?? false); }
            set { ViewState["HideWithNoDevice"] = value; }
        }
        public bool HideInactive
        {
            get { return (bool)(ViewState["HideInactive"] ?? false); }
            set { ViewState["HideInactive"] = value; }
        }

        [Category("Custom Properties")]
        public MovilOptionGroupValue OptionGroupProperty
        {
            get { return (MovilOptionGroupValue)(ViewState["MovilOptionGroupValue"] ?? MovilOptionGroupValue.TipoVehiculo); }
            set { ViewState["MovilOptionGroupValue"] = value; }
        }

        public int Coche
        {
            get { return ViewState["Coche"] != null ? Convert.ToInt32(ViewState["Coche"]) : 0; }
            set { ViewState["Coche"] = value; }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind()
        {
            BindingManager.BindMovil(this);
        }

        #endregion
    }
}