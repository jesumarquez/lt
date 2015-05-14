using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.ListBoxs
{
    public class DepartamentoListBox : ListBoxBase
    {
        public override Type Type { get { return typeof(Departamento); } }

        protected override void Bind() { BindingManager.BindDepartamento(this); }
    }
}