using System;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class EmpresaTelefonicaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(int); } }

        protected override void Bind()
        {
            BindingManager.BindEmpresaTelefonica(this);
        }
    }
}