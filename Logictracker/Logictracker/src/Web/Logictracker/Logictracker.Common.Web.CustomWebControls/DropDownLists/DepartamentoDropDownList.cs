using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class DepartamentoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Departamento); } }

        protected override void Bind() { BindingManager.BindDepartamento(this); }

    }
}