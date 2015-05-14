using System;
using System.Web.UI;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:TipoEmpleadoDropDownCheckList ID=\"TipoEmpleadoDropDownCheckList1\" runat=\"server\"></{0}:TipoEmpleadoDropDownCheckList>")]
    public class TipoEmpleadoDropDownCheckList : DropDownCheckListBase
    {
        public override Type Type { get { return typeof(TipoEmpleado); } }

        public override string NoneItemsName { get { return CultureManager.GetControl("DDL_NO_TIPOEMPLEADO"); } }

        protected override void Bind() { BindingManager.BindTiposEmpleado(this); }

    }
}