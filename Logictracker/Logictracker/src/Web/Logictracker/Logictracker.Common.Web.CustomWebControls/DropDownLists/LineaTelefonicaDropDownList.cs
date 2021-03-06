using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class LineaTelefonicaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(LineaTelefonica); } }

        protected override void Bind()
        {
            BindingManager.BindLineaTelefonica(this);
        }
    }
}