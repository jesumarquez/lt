using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class EmpleadoDropDownList : DropDownListBase, IEmpleadoAutoBindeable
    {
        #region Public Properties

        /// <summary>
        /// Gets the associated T.
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

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_NO_EMPLOYEE"); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Drivers binding.
        /// </summary>
        protected override void Bind() { BindingManager.BindEmpleados(this); }

        #endregion
    }
}