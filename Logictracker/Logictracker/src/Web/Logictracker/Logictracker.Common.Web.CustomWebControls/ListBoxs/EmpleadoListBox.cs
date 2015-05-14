using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class EmpleadoListBox : ListBoxBase, IEmpleadoAutoBindeable
    {
        #region Public Properties

        /// <summary>
        /// Gets the T associated to the list box.
        /// </summary>
        public override Type Type { get { return typeof(Empleado); } }

        /// <summary>
        /// Determines wither to filter only employees in charge of other employees.
        /// </summary>
        public bool SoloResponsables
        {
            get { return ViewState["SoloResponsables"] != null ? Convert.ToBoolean(ViewState["SoloResponsables"]) : false; }
            set { ViewState["SoloResponsables"] = value; }
        }

        /// <summary>
        /// Determines if it will filter the results based only in district values.
        /// </summary>
        public bool AllowOnlyDistrictBinding
        {
            get { return ViewState["AllowOnlyDistrictBinding"] != null ? Convert.ToBoolean(ViewState["AllowOnlyDistrictBinding"]) : false; }
            set { ViewState["AllowOnlyDistrictBinding"] = value; }
        }

        /// <summary>
        /// Enables or disables auto binding.
        /// </summary>
        public bool AutoBind
        {
            get { return ViewState["AutoBind"] != null ? Convert.ToBoolean(ViewState["AutoBind"]) : true; }
            set { ViewState["AutoBind"] = value; }
        }

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_NO_EMPLOYEE"); } }

        #endregion

        #region Public Method

        /// <summary>
        /// Bind Locaciones.
        /// </summary>
        protected override void Bind() { if (AutoBind) BindingManager.BindEmpleados(this); }

        #endregion
    }
}