using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.BaseControls;

namespace Logictracker.Web.CustomWebControls.DropDownLists
{
    public class ZonaDropDownList : DropDownListBase
    {
        public override Type Type { get { return typeof(Zona); } }

        protected override void Bind()
        {
            BindingManager.BindZona(this);
        }
    }
}