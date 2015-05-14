using System;
using System.Web.UI;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces.BussinessObjects;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:EmpleadoDropDownCheckList ID=\"EmpleadoDropDownCheckList1\" runat=\"server\"></{0}:EmpleadoDropDownCheckList>")]
    public class EmpleadoDropDownCheckList : DropDownCheckListBase, IEmpleadoAutoBindeable
    {
        public override Type Type { get { return typeof(Empleado); } }

        protected override void Bind() { BindingManager.BindEmpleados(this); }

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

    }
}