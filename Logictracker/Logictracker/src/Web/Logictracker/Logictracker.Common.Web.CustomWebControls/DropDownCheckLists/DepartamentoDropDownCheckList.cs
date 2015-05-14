using System;
using System.Web.UI;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownCheckLists
{
    [ToolboxData("<{0}:DepartamentoDropDownCheckList ID=\"DepartamentoDropDownCheckList1\" runat=\"server\"></{0}:DepartamentoDropDownCheckList>")]
    public class DepartamentoDropDownCheckList : DropDownCheckListBase
    {
        public override Type Type { get { return typeof(Departamento); } }

        protected override void Bind() { BindingManager.BindDepartamentos(this); }

    }
}
