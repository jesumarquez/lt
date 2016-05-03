using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class DepartamentoDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Departamento); } }

        public bool FiltraPorUsuario
        {
            get { return (bool)(ViewState["FiltraPorUsuario"] ?? false); }
            set { ViewState["FiltraPorUsuario"] = value; }
        }

        protected override void Bind() { BindingManager.BindDepartamento(this); }

    }
}