using System;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class NivelDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Nivel); } }

        protected override void Bind()
        {
            BindingManager.BindNivel(this);
        }
    }
}